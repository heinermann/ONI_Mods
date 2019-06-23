using Harmony;

namespace Heinermann.Floating.Patches
{
  [HarmonyPatch(typeof(Pickupable), "OnSpawn")]
  class Pickupable_OnSpawn
  {
    static void Postfix(Pickupable __instance)
    {
      __instance.gameObject.AddOrGet<FloatationChecker>();
    }
  }
}
