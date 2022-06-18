using ClusterioLibSharp.Link;
using Events;
using SharpPromise;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClusterioLibSharp.Slave
{
  public class Slave : Link.Link
  {
    object pluginInfos;
    SlaveConfig config;
    object tlsCa;

    Dictionary<string, object> instanceConnections = new Dictionary<string, object>();
    Dictionary<string, object> discoveredInstanceInfos = new Dictionary<string, object>();
    Dictionary<string, object> instanceInfos = new Dictionary<string, object>();

    HashSet<string> adminlist = new HashSet<string>();
    Dictionary<string, string> banlist = new Dictionary<string, string>();
    HashSet<string> whitelist = new HashSet<string>();

    string serverVersion;
    Dictionary<string, string> serverPlugins;

    bool startup = true;
    bool disconnecting = false;
    bool shuttingDown = false;

    public Slave(WebSocketClientConnector connector, SlaveConfig slaveConfig, object tlsCa, object pluginInfos) : base("slave", "master", connector)
    {
      //libLink.attachAllMessages(this) // TODO

      this.pluginInfos = pluginInfos;
      // TODO: attach plugin messages

      this.config = slaveConfig;
      this.tlsCa = tlsCa;
      this.connector = connector;

      connector.On("hello", OnHello);
      connector.On("connect", OnConnect);
      connector.On("close", OnClose);

      foreach (var eventName in new[] { "connect", "drop", "resume", "close" })
      {
        connector.On(eventName, OnConnectionChanged);
      }
    }

    private void OnHello(object sender, EventEmitterEventArgs args)
    {
      dynamic data = args.Arguments.First();
      this.serverVersion = data.version;
      this.serverPlugins = data.plugins;
    }

    private void OnConnect(object sender, EventEmitterEventArgs args)
    {
      if (shuttingDown) return;

      //updateInstances() // TODO
    }

    private void OnClose(object sender, EventEmitterEventArgs args)
    {
      if (shuttingDown) return;

      if (disconnecting)
      {
        disconnecting = false;
        // TODO investigate if this is actually calling connect() or not
        connector.connect().AsPromise().Catch(err =>
        {
          logger.Fatal($"Unexpected error reconnecting to master:\n{err}");
          shutdown();
        });
      }
      else
      {
        logger.Fatal("Master connection was unexpectedly closed");
        shutdown();
      }
    }

    private void OnConnectionChanged(object sender, EventEmitterEventArgs args)
    {
      foreach (var instanceConnection in instanceConnections.Values)
      {
        //libLink.messages.masterConnectionEvent.send(instanceConnection, { event }); // TODO
      }
    }

    public async void shutdown()
    {
      if (shuttingDown) return;
      shuttingDown = true;

      try
      {
        // await messages.prepareDisconnect.send(this);
      } catch (Exception err)
      {
        if (!(err is SessionLost))
        {
          logger.Error($"Unexpected error preparing disconnect:\n{err}");
        }
      }

      // TODO instances
    }
  }
}
