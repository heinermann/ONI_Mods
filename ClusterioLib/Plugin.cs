namespace ClusterioLib
{
  public class ParsedArgs
  {
    public string source { get; set; }
    public string format { get; set; }
    public string time { get; set; }
    public string type { get; set; }
    public string level { get; set; }
    public string file { get; set; }
    public string action { get; set; }
    public string message { get; set; }
    public string line { get; set; }
  }

  public class BaseInstancePlugin
  {
    object info; // TODO
    object instance; // TODO
    object slave; // TODO
    
    public BaseInstancePlugin(object info, object instance, object slave)
    {
      this.info = info;
      this.instance = instance;
      this.slave = slave;
      // TODO
    }

    // TODO
  }
}
