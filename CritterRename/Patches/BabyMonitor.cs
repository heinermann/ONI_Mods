using Harmony;

namespace Heinermann.CritterRename.Patches
{
  [HarmonyPatch(typeof(BabyMonitor.Instance), "SpawnAdult")]
  static class BabyMonitor_SpawnAdult
  {
    static void Prefix(BabyMonitor.Instance __instance)
    {
      Util_KInstantiate.spawner = __instance.gameObject;
    }
  }
}
