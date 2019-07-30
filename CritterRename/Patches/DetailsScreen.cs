using Harmony;
using STRINGS;
using System;
using UnityEngine;

namespace Heinermann.CritterRename.Patches
{
  [HarmonyPatch(typeof(DetailsScreen), "SetTitle", new Type[] { typeof(int) })]
  class DetailsScreen_SetTitle
  {
    static void Postfix(int selectedTabIndex, DetailsScreen __instance, EditableTitleBar ___TabTitle)
    {
      GameObject target = __instance.target;
      KPrefabID prefab = target?.GetComponent<KPrefabID>();
      if (prefab != null && prefab.HasTag(GameTags.Creature))
      {
        ___TabTitle.SetUserEditable(true);

        ___TabTitle.SetTitle(target.GetProperName());
        ___TabTitle.SetSubText("");

        string properName = UI.StripLinkFormatting(target.GetProperName());
        string originalProperName = UI.StripLinkFormatting(TagManager.GetProperName(prefab.PrefabTag));
        if (properName != originalProperName)
        {
          ___TabTitle.SetSubText(originalProperName);
        }
      }
    }
  }

  [HarmonyPatch(typeof(DetailsScreen), "OnNameChanged")]
  class DetailsScreen_OnNameChanged
  {
    static void Postfix(string newName, DetailsScreen __instance)
    {
      GameObject target = __instance.target;
      KPrefabID prefab = target?.GetComponent<KPrefabID>();
      if (prefab != null && prefab.HasTag(GameTags.Creature))
      {
        target.AddOrGet<CritterName>().SetName(newName);
        __instance.SetTitle(0);
      }
    }
  }

}
