using Jering.Javascript.NodeJS;
using System;
using System.Reflection;

namespace ClusterioLib
{
  public static class NodeManager
  {
    public static void Start()
    {
      string assemblyDir = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
      Console.WriteLine($"NodeJS ClusterioLib run directory: {assemblyDir}");

      int num = StaticNodeJSService.InvokeFromStringAsync<int>("module.exports = (callback) => callback(null, 3);").Result;
      Console.WriteLine($"TEST NUM: {num}");
    }
  }
}
