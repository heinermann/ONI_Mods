using Harmony;

namespace Heinermann.Floating.Patches
{
  /*
  [HarmonyPatch(typeof(Pickupable), "OnLanded")]
  class Pickupable_OnLanded
  {
    static bool Prefix(Pickupable __instance, object data)
    {
      return !Helpers.ShouldFloatLite(__instance.transform);
    }
  }*/
}
