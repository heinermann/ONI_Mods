using HarmonyLib;
using System.Collections.Generic;

namespace Heinermann.ONIProfiler
{
  public class ONIProfiler : KMod.UserMod2
  {
    public static Harmony harmony = new Harmony("mod.heinermann.oniprofiler");
    static ProfilerBase profiler = new DetourProfiler();

    public override void OnAllModsLoaded(Harmony _harmony, IReadOnlyList<KMod.Mod> mods)
    {
      harmony.PatchAll();
      profiler.DoOnLoad(harmony);
    }
  }
}
