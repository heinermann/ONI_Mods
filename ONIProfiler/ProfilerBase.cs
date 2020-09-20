using Harmony;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Heinermann.ONIProfiler
{
  public abstract class ProfilerBase
  {
    protected static readonly string dumpDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    protected static readonly string dumpTimePrefix = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm");

    public static string getDumpPath()
    {
      Thread thread = Thread.CurrentThread;
      string threadName = thread.Name ?? $"thread{thread.ManagedThreadId}";
      return $"{dumpDirectory}/{dumpTimePrefix}_{threadName}_profile.csv";
    }

    public abstract void DoOnLoad();
    public abstract void DoPrePatch(HarmonyInstance harmony);
  }
}
