using Events;
using SharpPromise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp;

namespace ClusterioLibSharp.Link
{
  public class Waiter
  {
    public Promise<Message> promise { get; set; }
    public Action<Message> resolve { get; set; }
    public Action<Exception> reject { get; set; }

    public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();
  }

  public class Link
  {
    public string source;
    public string target;
    public WebSocketClientConnector connector;

    public delegate void HandlerCB(Message message);
    public delegate bool ValidatorCB(Message message);

    Dictionary<string, List<Waiter>> waiters = new Dictionary<string, List<Waiter>>();
    Dictionary<string, HandlerCB> handlers = new Dictionary<string, HandlerCB>();
    Dictionary<string, ValidatorCB> validators = new Dictionary<string, ValidatorCB>();

    protected readonly Logger logger = new Logger(LogLevel.Debug);

    public Link(string source, string target, WebSocketClientConnector connector)
    {
      this.source = source;
      this.target = target;
      this.connector = connector;
    }

    private void OnMessage(object sender, EventEmitterEventArgs args)
    {
      Message payload = args.Arguments.First() as Message;
      try
      {
        processMessage(payload);
      }
      catch (InvalidMessage e)
      {
        logger.Error($"Invalid message on ${source}-${target} link: ${e.Message}");
        // TODO log errors

        if (payload.type != null && payload.type.EndsWith("_request") && payload.seq.HasValue)
        {
          string typeString = payload.type.Substring(0, payload.type.Length - 8);
          connector.send($"{typeString}_response", new { seq = payload.seq, error = e.Message });
        }
      }
      catch (Exception e)
      {
        connector.Emit("error", e);
      }
    }

    private void OnInvalidate(object sender, EventEmitterEventArgs args)
    {
      foreach(var waiterType in waiters.Values)
      {
        foreach (Waiter waiter in waiterType)
        {
          waiter.reject(new SessionLost("Session Lost"));
        }
      }
      waiters.Clear();
    }

    private void OnClose(object sender, EventEmitterEventArgs args)
    {
      foreach (var waiterType in waiters.Values)
      {
        foreach (Waiter waiter in waiterType)
        {
          waiter.reject(new SessionLost("Session Closed"));
        }
      }
      waiters.Clear();
    }

    public void processMessage(Message message)
    {
      // TODO schema validation

      validators.TryGetValue(message.type, out var validator);
      if (validator == null)
      {
        throw new InvalidMessage($"No validator for {message.type} on {source}-${target}");
      }

      if (!validator(message))
      {
        throw new InvalidMessage($"Validation failed for {message.type}"); // TODO: validator.errors
      }

      bool hadHandlers = processHandler(message);
      bool hadWaiters = processWaiters(message);

      if (!hadWaiters && !hadHandlers)
      {
        throw new InvalidMessage($"Unhandled message {message.type}");
      }
    }

    private bool processHandler(Message message)
    {
      bool hasHandler = handlers.TryGetValue(message.type, out HandlerCB handler);
      if (hasHandler) {
        handler(message);
      }
      return hasHandler;
    }

    private bool dataMatches(dynamic data, dynamic matches)
    {
      IDictionary<string, dynamic> compareData = (IDictionary<string, dynamic>)data;
      IDictionary<string, dynamic> matchData = (IDictionary<string, dynamic>)matches;
      // TODO: Possibly not accurate, will likely compare the instance addresses instead of deep compare
      // Consider https://github.com/GregFinzer/Compare-Net-Objects ?
      return matchData.All(compareData.Contains);
    }

    private bool processWaiters(Message message)
    {
      // TODO might have to lock this because of waitFor
      bool hasWaiters = this.waiters.TryGetValue(message.type, out List<Waiter> waiters);
      if (!hasWaiters || waiters.Count == 0) return false;

      var matched = new List<int>();
      for (int index = 0; index < waiters.Count; index++)
      {
        Waiter waiter = waiters[index];
        if (!dataMatches(message.data, waiter.data))
        {
          continue;
        }

        waiter.resolve(message);
        matched.Add(index);
      }

      matched.Reverse();
      matched.ForEach(waiters.RemoveAt);
      return matched.Count > 0;
    }

    public void setHandler(string type, HandlerCB handler, ValidatorCB validator)
    {
      if (handlers.ContainsKey(type)) throw new Exception($"{type} already has a handler");
      if (validator == null) throw new Exception("validator is required");

      setValidator(type, validator);
      handlers.Add(type, handler);
    }

    public void setValidator(string type, ValidatorCB validator)
    {
      if (validators.ContainsKey(type)) throw new Exception($"{type} already has a validator");
      validators.Add(type, validator);
    }

    public async Task<Message> waitFor(string type, dynamic data)
    {
      if (!validators.ContainsKey(type)) throw new Exception($"No validator for {type} on {source}-{target}");

      var waiter = new Waiter() { data = data };
      waiter.promise = new Promise<Message>((resolve, reject) => {
        // TODO: Consider locking because of processWaiters
        waiter.resolve = resolve;
        waiter.reject = reject;
        if (!waiters.ContainsKey(type))
        {
          waiters.Add(type, new List<Waiter> { waiter });
        }
        else
        {
          waiters[type].Add(waiter);
        }
      });
      return await waiter.promise.AsTask();
    }

    public async void prepareDisconnectRequestHandler()
    {
      var promises = new List<IPromise>();
      foreach (var waiterType in waiters.Values)
      {
        foreach (var waiter in waiterType)
        {
          promises.Add(waiter.promise.Then(() => { }, () => { }));
        }
      }
      await Promise.All(promises);
    }
  }
}
