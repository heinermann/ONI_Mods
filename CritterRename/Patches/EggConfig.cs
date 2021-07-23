using HarmonyLib;
using UnityEngine;

namespace Heinermann.CritterRename.Patches
{
  [HarmonyPatch(typeof(EggConfig), "CreateEgg")]
  class EggConfig_CreateEgg
  {
    static void Postfix(ref GameObject __result,
                        string id,
                        string name,
                        string desc,
                        Tag creature_id,
                        string anim,
                        float mass,
                        int egg_sort_order,
                        float base_incubation_rate)
    {
      __result.AddOrGet<CritterName>();
    }
  }
}
