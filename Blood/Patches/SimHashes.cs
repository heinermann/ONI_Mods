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

  [HarmonyPatch(typeof(Enum), "Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) })]
  class Enum_Parse
  {
    static bool Prefix(ref object __result, Type enumType, string value, bool ignoreCase)
    {
      if (!enumType.Equals(typeof(SimHashes))) return true;

      if (String.Compare(value, "Blood", ignoreCase) == 0)
      {
        __result = BloodElement.BloodSimHash;
        return false;
      }
      else if (String.Compare(value, "FrozenBlood", ignoreCase) == 0)
      {
        __result = BloodElement.FrozenBloodSimHash;
        return false;
      }
      return true;
    }
  }
}
