using Harmony;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Heinermann.ONIProfiler
{
  public class StackTraceProfiler : ProfilerBase
  {
    struct ProfileStats
    {
      public ulong profiles;
    }

    public override void DoOnLoad()
    {
      Thread mainThread = Thread.CurrentThread;
      Thread worker = new Thread(() => WorkerThread(mainThread));
      worker.Start();
    }

    public override void DoPrePatch(HarmonyInstance harmony)
    {
    }

    public void WorkerThread(Thread mainThread)
    {
      Dictionary<string, ProfileStats> profileData = new Dictionary<string, ProfileStats>();
      ulong totalProfiles = 0;
      Stopwatch dumpTimer = Stopwatch.StartNew();

      while (true)
      {
        Thread.Sleep(5);
        mainThread.Suspend();
        // TODO: THIS DOES NOT WORK - CONSTRUCTOR NOT IMPLEMENTED
        StackTrace trace = new StackTrace(mainThread, false);
        foreach (StackFrame frame in trace.GetFrames())
        {
          string functionName = $"{frame.GetMethod().DeclaringType.Name}.{frame.GetMethod().Name}";
          ProfileStats stats = profileData[functionName];
          stats.profiles++;
          profileData[functionName] = stats;
          totalProfiles++;
        }
        mainThread.Resume();

        if (dumpTimer.ElapsedMilliseconds > 20000)
        {
          StringBuilder csv = new StringBuilder(profileData.Count * 256);
          csv.AppendLine("name,timesProfiled,percentage");
          foreach (KeyValuePair<string, ProfileStats> item in profileData)
          {
            ulong numProfiles = item.Value.profiles;
            csv.AppendLine($"{item.Key},{numProfiles},{100.0 * numProfiles / totalProfiles}");
          }
          File.WriteAllText($"{dumpDirectory}/profile.csv", csv.ToString());

          dumpTimer.Restart();
        }
      }
    }

  }
}
