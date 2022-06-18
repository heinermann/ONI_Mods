using System;

namespace ClusterioLibSharp.Slave
{
  public class SlaveConfig
  {
    public SlaveGroup slave { get; set; } = new SlaveGroup();
  }

  public class SlaveGroup
  {
    public string name { get; set; } = "New Slave";
    public int id { get; set; } = (int)(new Random().NextDouble() * int.MaxValue);
    public string factorio_directory { get; set; } = "factorio";
    public string instances_directory { get; set; } = "instances";
    public string master_url { get; set; } = "http://localhost:8080/";
    public string master_token { get; set; } = "enter token here";
    public string tls_ca { get; set; }
    public string public_address { get; set; } = "localhost";
    public int max_reconnect_delay { get; set; } = 60;
  }

}
