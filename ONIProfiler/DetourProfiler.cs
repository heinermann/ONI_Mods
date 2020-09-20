
using Harmony;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace Heinermann.ONIProfiler
{
  public class DetourProfiler : ProfilerBase
  {
    class ProfileStats
    {
      public ulong calls = 0;
      public ulong totalTime = 0;
    }

    struct Totals
    {
      public double totalMs;
      public ulong totalCalls;
    }

    static ThreadLocal<Dictionary<string, ProfileStats>> profileData = new ThreadLocal<Dictionary<string, ProfileStats>>(() => new Dictionary<string, ProfileStats>());

    static ThreadLocal<Stopwatch> dumpTimer = new ThreadLocal<Stopwatch>(() => Stopwatch.StartNew());

    public override void DoOnLoad()
    {
    }

    public override void DoPrePatch(HarmonyInstance harmony)
    {
      Stopwatch timer = Stopwatch.StartNew();

      HarmonyMethod prefix = new HarmonyMethod(typeof(DetourProfiler), nameof(DetourPrefix));
      HarmonyMethod postfix = new HarmonyMethod(typeof(DetourProfiler), nameof(DetourPostfix));

      List<MethodBase> targetMethods = ProfileUtils.GetTargetMethods()
        .Where(method => !method.DeclaringType.Name.Equals("Expectations"))
        .ToList();

      //PatchProcessor processor = new PatchProcessor(harmony, targetMethods, prefix, postfix);
      //processor.Patch();

      foreach (MethodBase method in targetMethods)
      {
        DynamicMethod patched = harmony.Patch(method, prefix, postfix);

        // We assume this is unique, if not it will throw
        //profileData.Add(method.FullDescription(), new ProfileStats());
      }

      Debug.Log($"[ONIProfiler] PrePatch took {timer.ElapsedMilliseconds}ms. Patched {targetMethods.Count} methods.");
    }

    static void DetourPrefix(out long __state)
    {
      __state = Stopwatch.GetTimestamp();
    }

    static void DetourPostfix(MethodBase __originalMethod, long __state)
    {
      long nanoTime = ProfileUtils.ticksToNanoTime(Stopwatch.GetTimestamp() - __state);

      string decoratedName = __originalMethod.FullDescription();

      ProfileStats stats;
      if (!profileData.Value.TryGetValue(decoratedName, out stats)) {
        profileData.Value[decoratedName] = stats = new ProfileStats();
      }

      stats.calls++;
      stats.totalTime += Convert.ToUInt64(nanoTime);

      if (dumpTimer.Value.ElapsedMilliseconds > 30000)
      {
        dumpTimer.Value.Restart();
        DumpData();
      }
    }

    static Totals GetDumpTotals()
    {
      Totals result = new Totals();
      result.totalMs = 0.0;
      result.totalCalls = 0;
      foreach (ProfileStats stats in profileData.Value.Values)
      {
        result.totalMs += stats.totalTime / 1000000.0;
        result.totalCalls += stats.calls;
      }

      return result;
    }

    static string CreateDumpCsv()
    {
      Totals totals = GetDumpTotals();

      StringBuilder csv = new StringBuilder(profileData.Value.Count * 256);
      csv.AppendLine("name,totalMs,numCalls,avgMsPerCall,time%,call%");

      foreach (KeyValuePair<string, ProfileStats> item in profileData.Value)
      {
        ProfileStats stats = item.Value;
        if (stats.calls == 0) continue;

        double ms = stats.totalTime / 1000000.0;
        csv.AppendLine($"{item.Key.Replace(',', ';')},{ms},{stats.calls},{ms / stats.calls},{100.0 * ms / totals.totalMs},{100.0 * stats.calls / totals.totalCalls}");
      }
      return csv.ToString();
    }

    static void DumpData()
    {
      Stopwatch timer = Stopwatch.StartNew();
      
      string dumpPath = getDumpPath();
      Debug.Log($"[ONIProfiler] Dumping data to {dumpPath}");

      try   // This can fail with a Sharing violation if the CSV is opened in another app while profiling is still happening
      {
        File.WriteAllText(dumpPath, CreateDumpCsv().ToString());
      } catch
      {
        Debug.LogError($"[ONIProfiler] Failed to write dump: {dumpPath}");
      }

      Debug.Log($"[ONIProfiler] Dump took {timer.ElapsedMilliseconds}ms");
    }
  }
}
