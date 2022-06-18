using Json.Schema;

namespace ClusterioLibSharp
{
  public static class Schema
  {
    public static readonly JsonSchema messageSchema = JsonSchema.FromFile("schemas/message.json");
    public static readonly JsonSchema heartbeat = JsonSchema.FromFile("schemas/heartbeat.json");
    public static readonly JsonSchema serverHandshake = JsonSchema.FromFile("schemas/serverHandshake.json");
    public static readonly JsonSchema clientHandshake = JsonSchema.FromFile("schemas/clientHandshake.json");
  }
}
