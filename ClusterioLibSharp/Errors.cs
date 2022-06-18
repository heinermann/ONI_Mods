using System;

namespace ClusterioLibSharp
{
  public class CommandError : Exception { }
  public class RequestError : Exception
  {
    public RequestError(string message) : base(message) { }
  }

  public class PermissionError : RequestError {
    public PermissionError(string message) : base(message) { }
  }
  
  public class InvalidMessage : Exception
  {
    public InvalidMessage(string msg) : base(msg) { }
    public InvalidMessage(string msg, object errors) : base(msg)
    {
      //TODO errors?
    }
  }
  
  public class SessionLost : Exception
  {
    public SessionLost(string message) : base(message) { }
  }

  public class AuthenticationFailed : Exception
  {
    public AuthenticationFailed(string message) : base(message) { }
  }

  public class StartupError : Exception { }
  public class EnvironmentError : Exception { }
  
  public class PluginError : Exception
  {
    public PluginError(string pluginname, Exception original) : base($"PluginError: {original.Message}")
    {
      // TODO: pluginName
    }
  }
}
