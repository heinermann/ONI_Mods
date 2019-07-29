using Harmony;

namespace Heinermann.CritterRename.Patches
{
  [HarmonyPatch(typeof(IncubationMonitor), "SpawnBaby")]
  static class IncubationMonitor_SpawnBaby
  {
    static void Prefix(IncubationMonitor.Instance smi)
    {
      Util_KInstantiate.spawner = smi.gameObject;
    }
  }
}
