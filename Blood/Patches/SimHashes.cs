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
  /*
  TODO: include this
  [HarmonyPatch(typeof(Enum), nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) })]
  class SimHashes_Parse
  {
      static bool Prefix(Type enumType, string value, ref object __result)
      {
          if (!enumType.Equals(typeof(SimHashes))) return true;
          if (SimHashReverseTable.ContainsKey(value))
          {
              __result = SimHashReverseTable[value];
              return false;
          }
          return true;
      }
  }
  */
}
