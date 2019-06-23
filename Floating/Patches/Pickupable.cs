using Harmony;

namespace Heinermann.Floating.Patches
{
  [HarmonyPatch(typeof(Pickupable), "OnPrefabInit")]
  class Pickupable_OnPrefabInit
  {
    static void Postfix(Pickupable __instance)
    {
      __instance.gameObject.AddOrGet<FloatationChecker>();
    }
  }
}
