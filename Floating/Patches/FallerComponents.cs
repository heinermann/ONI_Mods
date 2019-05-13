using Harmony;
using UnityEngine;

namespace Heinermann.Floating.Patches
{
  [HarmonyPatch(typeof(FallerComponents), "OnLanded")]
  class FallerComponents_OnLanded
  {
    static bool Prefix(Transform transform)
    {
      return !Helpers.ShouldFloat(transform);
    }
  }
}
