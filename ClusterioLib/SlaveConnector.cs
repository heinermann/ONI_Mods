using ClusterioLib.Link;

namespace ClusterioLib
{
  public class SlaveConnector : WebSocketClientConnector
  {
    SlaveConfig slaveConfig;
    object pluginInfos;

    public SlaveConnector(SlaveConfig slaveConfig, object tlsCa, object pluginInfos)
      : base(slaveConfig.slave.master_url, slaveConfig.slave.max_reconnect_delay, tlsCa)
    {
      this.slaveConfig = slaveConfig;
      this.pluginInfos = pluginInfos;
    }

    public override void register()
    {
      logger.Info("Connector | registering slave");

      // TODO: Plugins shenanigans for C# land.

      sendHandshake("register_slave", new
      {
        token = slaveConfig.slave.master_token,
        agent = "Clusterio C# Slave",
        version = "1.0", // ??
        id = slaveConfig.slave.id,
        name = slaveConfig.slave.name,
        public_address = slaveConfig.slave.public_address,
        plugins = new { } // TODO
      });
    }
  }
}
