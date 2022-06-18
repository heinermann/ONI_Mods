using Newtonsoft.Json;
using WebSocketSharp;

namespace ClusterioLibSharp.Link
{
  public class Message
  {
    public ulong? seq { get; set; }
    public string type { get; set; }
    public object data { get; set; }
    
    public Message() {}

    public Message(ulong? seq, string type, object data)
    {
      this.seq = seq;
      this.type = type;
      this.data = data;
    }

    public void Send(WebSocket socket)
    {
      socket.Send(JsonConvert.SerializeObject(this));
    }

    private string _plugin;
    public string plugin {
      get {
        if (_plugin == null)
        {
          int index = type.IndexOf(':');
          _plugin = index == -1 ? null : type.Substring(0, index);
        }
        return _plugin;
      }
    }
  }
}
