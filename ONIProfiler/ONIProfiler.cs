using HarmonyLib;

namespace Heinermann.ONIProfiler
{
  public class ONIProfiler : KMod.UserMod2
  {
    static ProfilerBase profiler = new DetourProfiler();

    public override void OnLoad(Harmony harmony)
    {
      profiler.DoOnLoad(harmony);
    }
  }
}
