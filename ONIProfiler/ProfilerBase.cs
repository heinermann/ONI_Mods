using Harmony;
using System.IO;
using System.Reflection;

namespace Heinermann.ONIProfiler
{
  public abstract class ProfilerBase
  {
    protected static readonly string dumpDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    protected static readonly string dumpFile = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + "_profile.csv";
    protected static readonly string dumpPath = $"{dumpDirectory}/{dumpFile}";

    public abstract void DoOnLoad();
    public abstract void DoPrePatch(HarmonyInstance harmony);
  }
}
