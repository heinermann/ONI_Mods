using ClusterioLibSharp.NodeLibs;
using Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using WebSocketSharp;

namespace ClusterioLibSharp.Link
{
  public enum ConnectionState
  {
    CLOSED,
    CONNECTING,
    CONNECTED,
    RESUMING
  }

  public abstract class WebSocketBaseConnector : EventEmitterEx
  {
    protected int? sessionTimeout;
    protected ConnectionState state;
    protected bool closing;
    protected WebSocket socket;
    protected ulong seq;
    protected DateTime? lastHeartbeat;
    protected Timer heartbeatId;
    protected int heartbeatInterval;
    protected ulong? lastReceivedSeq;
    protected readonly List<Message> sendBuffer = new List<Message>();

    protected readonly Logger logger = new Logger(LogLevel.Debug);

    public WebSocketBaseConnector(int? sessionTimeout)
    {
      reset();
      this.sessionTimeout = sessionTimeout;
    }

    protected void reset()
    {
      state = ConnectionState.CLOSED;
      closing = false;
      
      socket?.Close();
      socket = null;

      seq = 1;

      stopHeartbeat();

      heartbeatInterval = 1;
      lastReceivedSeq = null;
      sendBuffer.Clear();
    }

    protected void check(params ConnectionState[] expectedStates)
    {
      if (!expectedStates.Contains(state))
      {
        throw new Exception($"Expected state {expectedStates} but state is {state}");
      }
    }

    protected void dropSendBufferSeq(ulong seq)
    {
      sendBuffer.RemoveAll(msg => msg.seq <= seq);
    }

    protected void doHeartbeat(object obj, ElapsedEventArgs e)
    {
      check(ConnectionState.CONNECTED);
      if (DateTime.UtcNow - lastHeartbeat > TimeSpan.FromMilliseconds(2000 * heartbeatInterval))
      {
        logger.Debug("Connector | closing after heartbeat timed out");
        socket.Close(4008, "Heartbeat timeout");
      }
      else
      {
        var msg = new Message(null, "heartbeat", new { seq = lastReceivedSeq });
        msg.Send(socket);
      }
    }

    protected void processHeartbeat(Message message)
    {
      // TODO Schema validation
      lastHeartbeat = DateTime.UtcNow;
      dropSendBufferSeq(message.data.seq);
    }

    public void startHeartbeat()
    {
      if (heartbeatId != null)
      {
        throw new Exception("heartbeat is already running");
      }
      lastHeartbeat = DateTime.UtcNow;
      
      heartbeatId = new Timer(heartbeatInterval * 1000);
      heartbeatId.Elapsed += doHeartbeat;
      heartbeatId.AutoReset = true;
      heartbeatId.Enabled = true;
    }

    public void stopHeartbeat()
    {
      heartbeatId?.Close();
      heartbeatId = null;
      lastHeartbeat = null;
    }

    public ulong send(string type, dynamic data)
    {
      if (state != ConnectionState.CONNECTED && state != ConnectionState.RESUMING)
      {
        throw new SessionLost("No session");
      }
      ulong thisSeq = seq;
      seq++;

      Message msg = new Message(thisSeq, type, data);
      sendBuffer.Add(msg);

      if (state == ConnectionState.CONNECTED)
      {
        msg.Send(socket);
      }
      return thisSeq;
    }

    public abstract void setClosing();

    public bool Connected
    {
      get
      {
        return state == ConnectionState.CONNECTED;
      }
    }

    public bool Closing
    {
      get
      {
        return closing || state == ConnectionState.CLOSED;
      }
    }

    public bool HasSession
    {
      get
      {
        return state == ConnectionState.CONNECTED || state == ConnectionState.RESUMING;
      }
    }
  }

  public abstract class WebSocketClientConnector : WebSocketBaseConnector
  {
    Timer reconnectId;
    DateTime? startedResuming;
    string url;
    ExponentialBackoff backoff;
    object tlsca; // TODO
    string sessionToken;

    public WebSocketClientConnector(string url, int maxReconnectDelay, object tlsca = null) : base(null)
    {
      this.url = url;
      this.backoff = new ExponentialBackoff() { Max = maxReconnectDelay };
      this.tlsca = tlsca;

      reset();
    }

    protected new void reset()
    {
      reconnectId?.Close();
      reconnectId = null;
      sessionToken = null;
      startedResuming = null;
      base.reset();
    }

    public void sendHandshake(string type, dynamic data)
    {
      check(ConnectionState.CONNECTING, ConnectionState.RESUMING);
      new Message(null, type, data).Send(socket);
    }

    public async void close(ushort code, string reason)
    {
      if (state == ConnectionState.CLOSED) return;
      
      closing = true;
      if (state == ConnectionState.CONNECTED || socket != null)
      {
        socket.Close(code, reason);
        await Once("close");
      }
      else
      {
        reset();
        Emit("close");
      }
    }

    public override void setClosing()
    {
      if (state == ConnectionState.CLOSED) return;
      
      closing = true;
      if (state == ConnectionState.CONNECTED) return;

      if (socket != null)
      {
        socket.Close(1000, "Connector closing");
      }
      else
      {
        reset();
        Emit("close");
      }
    }

    public async Task<EventEmitterEventArgs> connect()
    {
      check(ConnectionState.CLOSED);
      state = ConnectionState.CONNECTING;

      doConnect();

      return await Once("connect");
    }

    protected void doConnect()
    {
      var targetUrl = new Uri(new Uri(url), "api/socket");

      logger.Debug($"Connector | connecting to {targetUrl}");

      socket = new WebSocket(targetUrl.AbsoluteUri);
      
      // TODO: use tlsca

      attachSocketHandlers();
    }

    public void reconnect()
    {
      if (socket != null)
      {
        throw new Exception("Cannot reconnect while socket is open");
      }

      if (reconnectId != null)
      {
        logger.Error("Unexpected double call to reconnect");
      }

      int delay = backoff.Delay();
      if (state == ConnectionState.RESUMING &&
        startedResuming + TimeSpan.FromMilliseconds(sessionTimeout.Value * 1000) < DateTime.UtcNow + TimeSpan.FromMilliseconds(delay))
      {
        logger.Error("Connector | Session timed out trying to resume");
        reset();
        state = ConnectionState.CONNECTING;
        Emit("invalidate");
      }
      logger.Debug($"Connector | waiting {(Math.Round(delay / 10.0) / 100)} seconds for reconnect");
      reconnectId = new Timer(delay);
      reconnectId.Elapsed += (_1, _2) => {
        reconnectId = null;
        doConnect();
      };
      reconnectId.AutoReset = false;
      reconnectId.Enabled = true;
    }

    protected void attachSocketHandlers()
    {
      socket.OnClose += Socket_OnClose;
      socket.OnError += Socket_OnError;
      socket.OnOpen += Socket_OnOpen;
      socket.OnMessage += Socket_OnMessage;
    }

    private void Socket_OnMessage(object sender, MessageEventArgs e)
    {
      var message = JsonConvert.DeserializeObject<Message>(e.Data);
      switch (state)
      {
        case ConnectionState.CONNECTING:
        case ConnectionState.RESUMING:
          processHandshake(message);
          break;
        case ConnectionState.CONNECTED:
          if (message.seq != null)
          {
            lastReceivedSeq = message.seq;
          }

          if (message.type == "heartbeat")
          {
            processHeartbeat(message);
          }
          else
          {
            Emit("message", message);
          }

          break;
        default:
          throw new Exception($"Received message in unexpected state {state}");
      }
    }

    private void Socket_OnOpen(object sender, EventArgs e)
    {
      logger.Debug("Connector | Open");
    }

    private void Socket_OnError(object sender, ErrorEventArgs e)
    {
      string code = e.Exception == null ? "" : $", code: ${e.Exception}";
      string message = $"Connector | Socket error: ${e.Message ?? "unknown error"}${code}";
      if (state == ConnectionState.CONNECTED)
      {
        logger.Error(message);
      }
      else
      {
        logger.Debug(message);
      }

      // TODO determine if certificate validation failed
    }

    private void Socket_OnClose(object sender, CloseEventArgs e)
    {
      ConnectionState previousState = state;

      // Authentication failed
      if (e.Code == 4003)
      {
        Emit("error", new AuthenticationFailed(e.Reason));
        closing = true;
      }

      socket = null;
      if (state == ConnectionState.CONNECTED)
      {
        stopHeartbeat();
        if (closing)
        {
          reset();
          Emit("close");
        }
        else
        {
          state = ConnectionState.RESUMING;
          startedResuming = DateTime.UtcNow;
          reconnect();
          Emit("drop");
        }
      }
      else
      {
        if (closing)
        {
          reset();
          Emit("close");
        }
        else
        {
          reconnect();
        }
      }

      string message = $"Connector | Close (code: {e.Code}, reason: {e.Reason})";
      if (previousState == ConnectionState.CONNECTED && e.Code != (ushort)CloseStatusCode.Normal)
      {
        logger.Info(message);
      }
      else
      {
        logger.Debug(message);
      }
    }

    private void SendMessage(Message message)
    {
      message.Send(socket);
    }

    protected void processHandshake(Message message)
    {
      // TODO schema validation

      switch(message.type)
      {
        case "hello":
          logger.Debug($"Connector | received hello from master version {message.data.version}");
          Emit("hello", message.data);
          if (sessionToken != null)
          {
            logger.Debug("Connector | Attempting resume");
            sendHandshake("resume", new
            {
              session_token = sessionToken,
              last_seq = lastReceivedSeq
            });
          }
          break;
        case "ready":
          logger.Debug("Connector | received ready from master");
          state = ConnectionState.CONNECTED;
          sessionToken = message.data.session_token;
          sessionTimeout = message.data.session_timeout;
          heartbeatInterval = message.data.heartbeat_interval;
          startHeartbeat();
          sendBuffer.ForEach(SendMessage);
          Emit("connect", message.data);
          break;
        case "continue":
          logger.Info("Connector | resuming existing session");
          state = ConnectionState.CONNECTED;
          heartbeatInterval = message.data.heartbeat_interval;
          sessionTimeout = message.data.session_timeout;
          startHeartbeat();
          dropSendBufferSeq(message.data.last_seq);
          sendBuffer.ForEach(SendMessage);
          startedResuming = null;
          Emit("resume");
          break;
        case "invalidate":
          logger.Warn("Connector | session invalidated by master");
          state = ConnectionState.CONNECTING;
          seq = 1;
          lastReceivedSeq = null;
          sessionToken = null;
          sessionTimeout = null;
          sendBuffer.Clear();
          startedResuming = null;
          Emit("invalidate");
          register();
          break;
      }
    }

    public abstract void register();
  }
}
