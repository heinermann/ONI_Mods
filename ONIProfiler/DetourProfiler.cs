
using Harmony;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Heinermann.ONIProfiler
{
  using ProfileDict = Dictionary<string, ulong>;

  public class DetourProfiler : ProfilerBase
  {
    static ThreadLocal<ProfileDict> profileData = new ThreadLocal<ProfileDict>(() => new ProfileDict());
    static ThreadLocal<Stopwatch> traceTimer = new ThreadLocal<Stopwatch>(() => Stopwatch.StartNew());
    static ThreadLocal<Stopwatch> dumpTimer = new ThreadLocal<Stopwatch>(() => Stopwatch.StartNew());

    static readonly long nanosecondsPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;

    public override void DoOnLoad()
    {
    }

    public override void DoPrePatch(HarmonyInstance harmony)
    {
      Stopwatch timer = Stopwatch.StartNew();

      HarmonyMethod postfix = new HarmonyMethod(typeof(DetourProfiler), nameof(DetourPostfix));

      List<MethodBase> targetMethods = ProfileUtils.GetTargetMethods()
        .Where(method => !method.DeclaringType.Name.Equals("Expectations"))
        .ToList();

      PatchProcessor processor = new PatchProcessor(harmony, targetMethods, null, postfix);
      processor.Patch();

      /*foreach (MethodBase method in targetMethods)
      {
        harmony.Patch(method, null, postfix);
      }*/

      Debug.Log($"[ONIProfiler] PrePatch took {timer.ElapsedMilliseconds}ms. Patched {targetMethods.Count} methods.");
    }

    static void DetourPostfix(MethodBase __originalMethod)
    {
      if (traceTimer.Value.ElapsedTicks / nanosecondsPerTick > 100000)
      {
        traceTimer.Value.Restart();

        StackTrace trace = new StackTrace(2, false);
        string traceStr = $"{FlattenStackTrace(trace)};{__originalMethod.FullDescription()}";

        ulong count;
        profileData.Value.TryGetValue(traceStr, out count);
        profileData.Value[traceStr] = count + 1;

        if (dumpTimer.Value.ElapsedMilliseconds > 30000)
        {
          dumpTimer.Value.Restart();
          DumpData();
        }
      }
    }

    static string FlattenStackTrace(StackTrace trace)
    {
      var stackStrings = trace.GetFrames().Select(frame => frame.GetMethod().FullDescription()).Reverse();
      return string.Join(";", stackStrings);
    }

    static string CreateFlatFile()
    {
      return profileData.Value.Join(pair => $"{pair.Key} {pair.Value}", "\n");
    }

    static void DumpData()
    {
      Stopwatch timer = Stopwatch.StartNew();
      
      string dumpPath = getDumpPath();
      Debug.Log($"[ONIProfiler] Dumping data to {dumpPath}");

      try   // This can fail with a Sharing violation if another app opens it
      {
        File.WriteAllText(dumpPath, CreateFlatFile());
      } catch
      {
        Debug.LogError($"[ONIProfiler] Failed to write dump: {dumpPath}");
      }

      Debug.Log($"[ONIProfiler] Dump took {timer.ElapsedMilliseconds}ms");
    }
  }
}
