using Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp;

namespace ClusterioLib.Link
{
  public class Waiter
  {
    public Task promise { get; set; }
    public ResolveCB resolve { get; set; }
    public RejectCB reject { get; set; }

    public delegate void ResolveCB(Message message);
    public delegate void RejectCB(Exception exception);
    //public delegate void PromiseCB(ResolveCB resolve, RejectCB reject);
  }

  public class Link
  {
    string source;
    string target;
    WebSocketBaseConnector connector;

    public delegate void HandlerCB(Message message);
    public delegate bool ValidatorCB(Message message);

    Dictionary<string, List<Waiter>> waiters = new Dictionary<string, List<Waiter>>();
    Dictionary<string, HandlerCB> handlers = new Dictionary<string, HandlerCB>();
    Dictionary<string, ValidatorCB> validators = new Dictionary<string, ValidatorCB>();

    protected readonly Logger logger = new Logger(LogLevel.Debug);

    public Link(string source, string target, WebSocketBaseConnector connector)
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
  }
}
