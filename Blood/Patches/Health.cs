using HarmonyLib;
using System;
using static Klei.SimUtil;

namespace Heinermann.Blood.Patches
{
  [HarmonyPatch(typeof(Health), "OnHealthChanged")]
  class Health_OnHealthChanged
  {
    private const float BODY_TEMP = 310; // Kelvin, 37C

    // Based on duplicant weight (30kg) * average blood volume per kg of weight (70mL)
    // converted to mass, divided by duplicant health (100), rounded up to nearest gram
    private const float MASS_MULTIPLIER = 0.024f; //kg

    static void Prefix(ref Health __instance, float delta)
    {
      var healthSubtracted = Math.Min(-delta, __instance.maxHitPoints);
      if (healthSubtracted <= 0) return;

      SimMessages.AddRemoveSubstance(
        gameCell: Grid.PosToCell(__instance),
        new_element: BloodElement.BloodSimHash,
        ev: null,
        mass: MASS_MULTIPLIER * healthSubtracted,
        temperature: BODY_TEMP,
        disease_idx: DiseaseInfo.Invalid.idx,
        disease_count: DiseaseInfo.Invalid.count
      );
    }
  }
}
