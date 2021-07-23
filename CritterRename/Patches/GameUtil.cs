using HarmonyLib;
using System;
using UnityEngine;

namespace Heinermann.CritterRename.Patches
{
  // Fixes the "CritterName x 1" issue
  [HarmonyPatch(typeof(GameUtil), "GetUnitFormattedName", new Type[] { typeof(GameObject), typeof(bool) })]
  static class GameUtil_GetUnitFormattedName
  {
    static bool Prefix(GameObject go, bool upperName, ref string __result)
    {
      KPrefabID prefab = go.GetComponent<KPrefabID>();
      CritterName name = go.GetComponent<CritterName>();
      if (prefab != null && name != null && prefab.HasTag(GameTags.Creature) && name.HasName())
      {
        __result = !upperName ? go.GetProperName() : StringFormatter.ToUpper(go.GetProperName());
        return false;
      }
      return true;
    }
  }
}
