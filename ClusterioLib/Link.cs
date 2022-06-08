using Events;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp;

namespace ClusterioLib
{
  public class Link
  {
    protected readonly Logger logger = new Logger(LogLevel.Debug);

    string source;
    string target;
    WebSocketBaseConnector connector;

    Dictionary<string, List<object>> waiters = new Dictionary<string, List<object>>(); // str, promise
    Dictionary<string, object> handlers = new Dictionary<string, object>(); // str, fn
    Dictionary<string, object> validators = new Dictionary<string, object>(); // str, fn



    public Link(string source, string target, WebSocketBaseConnector connector)
    {
      this.source = source;
      this.target = target;
      this.connector = connector;

      connector.On("message", OnMessageHandler);
      connector.On("invalidate", OnInvalidateHandler);
      connector.On("close", OnCloseHandler);
    }

    private void OnMessageHandler(object sender, EventEmitterEventArgs args)
    {
      var payload = args.Arguments.First() as Message;
      try
      {
        processMessage(payload);
      }
      catch (InvalidMessage err)
      {
        logger.Error($"Invalid message on {source}-{target} link: ${err.Message}");
        //if (err.errors)
        //{
        //  logger.Error(JSON.stringify(err.errors, null, 4));
        //}

        if (payload.type.EndsWith("_request") && payload.seq.HasValue)
        {
          string baseType = payload.type.Substring(0, payload.type.Length - 8);
          connector.send($"{baseType}_response", new { seq = payload.seq, error = err.Message });
        }
      }
      catch (Exception err)
      {
        connector.Emit("error", err);
      }
    }

    private void OnInvalidateHandler(object sender, EventEmitterEventArgs args)
    {
      foreach (var waiterType in waiters.Values)
      {
        foreach (var waiter in waiterType)
        {
          // TODO
          //waiter.reject(new SessionLost("Session Lost"));
        }
      }
      waiters.Clear();
    }

    private void OnCloseHandler(object sender, EventEmitterEventArgs args)
    {
      foreach (var waiterType in waiters.Values)
      {
        foreach (var waiter in waiterType)
        {
          // TODO
          //waiter.reject(new SessionLost("Session Closed"));
        }
      }
      waiters.Clear();
    }

    public void processMessage(Message message)
    {
      // TODO move schema check to Connectors.cs
      //if (!libSchema.message(message))
      //{
      //  throw new libErrors.InvalidMessage("Malformed", libSchema.message.errors);
      //}

      validators.TryGetValue(message.type, out object validator);
      if (validator == null)
      {
        throw new InvalidMessage($"No validator for {message.type} on {source}-{target}");
      }

      // TODO
      //if (!validator(message))
      //{
      //throw new InvalidMessage($"Validation failed for {message.type}", validator.errors);
      //}

      bool hadHandlers = processHandler(message);
      bool hadWaiters = processWaiters(message);
      if (!hadHandlers && !hadWaiters)
      {
        throw new InvalidMessage($"Unhandled message {message.type}");
      }
    }

    protected bool processHandler(Message message)
    {
      handlers.TryGetValue(message.type, out object handler);
      if (handler != null)
      {
        //handler(message); // TODO
        return true;
      }
      return false;
    }

    protected bool processWaiters(Message message)
    {
      this.waiters.TryGetValue(message.type, out List<object> waiters);
      if (waiters == null || waiters.Count == 0) return false;

      // TODO finish this
      return false;
    }

    // TODO finish
  }
}
