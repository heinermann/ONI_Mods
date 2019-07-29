using Harmony;

namespace Heinermann.CritterRename.Patches
{
  [HarmonyPatch(typeof(FertilityMonitor.Instance), "LayEgg")]
  class FertilityMonitor_LayEgg
  {
    static void Postfix(FertilityMonitor.Instance __instance)
    {
      Util_KInstantiate.spawner = __instance.gameObject;
    }
  }
}
