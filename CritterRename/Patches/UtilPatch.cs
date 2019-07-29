using Harmony;
using System;
using UnityEngine;

namespace Heinermann.CritterRename.Patches
{
  [HarmonyPatch(typeof(global::Util), "KInstantiate", new Type[] { typeof(GameObject), typeof(Vector3) })]
  class Util_KInstantiate
  {
    public static GameObject spawner = null;

    static void Postfix(GameObject __result, GameObject original, Vector3 position)
    {
      if (spawner == null) return;

      KPrefabID prefab = __result?.GetComponent<KPrefabID>();
      if (prefab != null && (prefab.HasTag(GameTags.Creature) || prefab.HasTag(GameTags.Egg)))
      {
        CritterName destination = __result.AddOrGet<CritterName>();
        CritterName source = spawner.GetComponent<CritterName>();
        source?.TransferTo(destination);
      }
      spawner = null;
    }
  }
}
