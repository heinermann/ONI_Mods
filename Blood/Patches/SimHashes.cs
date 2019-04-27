using Harmony;
using System;

namespace Heinermann.Blood.Patches
{
  [HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
  class SimHashes_ToString
  {
    static bool Prefix(ref Enum __instance, ref string __result)
    {
      if (!(__instance is SimHashes)) return true;
      return !BloodElement.SimHashNameLookup.TryGetValue((SimHashes)__instance, out __result);
    }
  }
}
