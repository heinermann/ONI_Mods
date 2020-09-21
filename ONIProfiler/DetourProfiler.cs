
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
  using ProfileDict = Dictionary<string, ulong>;

  public class DetourProfiler : ProfilerBase
  {
    static ThreadLocal<ProfileDict> profileData = new ThreadLocal<ProfileDict>(() => new ProfileDict());
    static ThreadLocal<Stopwatch> traceTimer = new ThreadLocal<Stopwatch>(() => Stopwatch.StartNew());
    static ThreadLocal<Stopwatch> dumpTimer = new ThreadLocal<Stopwatch>(() => Stopwatch.StartNew());

    static readonly long nanosecondsPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;

    static readonly MethodInfo GetStackTraces = typeof(Thread).GetMethod("Mono_GetStackTraces", BindingFlags.NonPublic | BindingFlags.Static);

    static readonly long NANOSECONDS_PER_TRACE = 200000;
    static readonly long MILLISECONDS_PER_DUMP = 30000;


    public override void DoOnLoad()
    {
    }

    public override void DoPrePatch(HarmonyInstance harmony)
    {
      Stopwatch timer = Stopwatch.StartNew();

      HarmonyMethod postfix = new HarmonyMethod(typeof(DetourProfiler), nameof(DetourPostfix));

      // TODO: Move these to ProfileUtils, to the Type filter (duh!)
      List<MethodBase> targetMethods = ProfileUtils.GetTargetMethods()
        .Where(method => 
          !method.DeclaringType.Name.Equals("Expectations") &&
          !method.DeclaringType.Name.Equals("KleiAccount") &&
          !method.DeclaringType.Name.Equals("KleiMetrics") &&
          !method.DeclaringType.Name.Equals("ThreadedHttps`1[T]") &&
          !method.DeclaringType.FullName.Equals("System.Enum") &&
          !method.DeclaringType.FullName.Equals("System.Number") &&
          !method.DeclaringType.FullName.Equals("System.SharedStatics") &&
          !method.DeclaringType.FullName.StartsWith("LibNoiseDotNet.") &&
          !method.DeclaringType.FullName.StartsWith("Mono.") &&
          !method.DeclaringType.FullName.StartsWith("System.Reflection.") &&
          !method.DeclaringType.FullName.StartsWith("System.IO.IsolatedStorage.") &&
          !method.DeclaringType.FullName.StartsWith("System.Threading.") &&
          !method.DeclaringType.FullName.StartsWith("System.Security.") &&
          !method.DeclaringType.FullName.StartsWith("System.Runtime.") &&
          !method.DeclaringType.FullName.StartsWith("System.Diagnostics.") &&
          !method.DeclaringType.FullName.StartsWith("System.Configuration.") &&
          !method.DeclaringType.FullName.StartsWith("System.IO.Ports.") &&
          !method.DeclaringType.FullName.StartsWith("System.IO.Compression.") &&
          !method.DeclaringType.FullName.StartsWith("System.Net.") &&
          !method.DeclaringType.FullName.StartsWith("System.CodeDom.") &&
          !method.DeclaringType.FullName.StartsWith("Microsoft.") &&
          !method.DeclaringType.FullName.Equals("System.IO.WindowsWatcher") &&
          !method.DeclaringType.FullName.Equals("System.Buffers.Binary.BinaryPrimitives") &&
          !method.DeclaringType.FullName.Equals("System.Environment") &&
          !method.Name.Equals("ReadUInt64")
        )
        .ToList();

      PatchProcessor processor = new PatchProcessor(harmony, targetMethods, null, postfix);
      processor.Patch();

      /*
      foreach (MethodBase method in targetMethods)
      {
        try
        {
          harmony.Patch(method, null, postfix);
        }
        catch (Exception e)
        {
          Debug.LogError(e.ToString());
        }
      }*/

      Debug.Log($"[ONIProfiler] PrePatch took {timer.ElapsedMilliseconds}ms. Patched {targetMethods.Count} methods.");
    }

    static void DetourPostfix()
    {
      if (traceTimer.Value.ElapsedTicks / nanosecondsPerTick > NANOSECONDS_PER_TRACE)
      {
        traceTimer.Value.Restart();

        string traceStr = GetFlatStackTrace();

        profileData.Value.TryGetValue(traceStr, out ulong count);
        profileData.Value[traceStr] = count + 1;

        if (dumpTimer.Value.ElapsedMilliseconds > MILLISECONDS_PER_DUMP)
        {
          dumpTimer.Value.Restart();
          DumpData();
        }
      }
    }

    static StackTrace GetStackTrace()
    {
      Dictionary<Thread, StackTrace> result = GetStackTraces.Invoke(null, new object[] { }) as Dictionary<Thread, StackTrace>;
      return result[Thread.CurrentThread];
    }

    static string GetFlatStackTrace()
    {
      List<string> result = new List<string>();
      foreach (StackFrame frame in GetStackTrace().GetFrames().Skip(11))
      {
        string methodDescription = frame.GetMethod()?.FullDescription()?.Replace("_Patch1", "");
        if (methodDescription == null)
        {
          methodDescription = $"<{frame.GetILOffset():X}:{frame.GetNativeOffset():X}> <Unknown method>";
        }

        result.Insert(0, methodDescription);
      }
      
      return string.Join(";", result);
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
