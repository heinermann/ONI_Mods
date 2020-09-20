using Harmony;

namespace Heinermann.ONIProfiler
{
  public class ONIProfiler
  {
    public static class Mod_OnLoad
    {
      static ProfilerBase profiler = new DetourProfiler();

      public static void PrePatch(HarmonyInstance harmony)
      {
        profiler.DoPrePatch(harmony);
      }

      public static void OnLoad()
      {
        profiler.DoOnLoad();
      }
    }
  }
}
