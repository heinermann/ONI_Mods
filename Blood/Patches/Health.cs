using Harmony;
using Klei;

namespace Heinermann.Blood.Patches
{
  [HarmonyPatch(typeof(Health), "OnHealthChanged")]
  class Health_OnHealthChanged
  {
    const float BODY_TEMP = 310; // Kelvin, 37C

    // Based on duplicant weight (30kg) * average blood volume per kg of weight (70mL)
    // converted to mass, divided by duplicant health (100), rounded up to nearest gram
    const float MASS_MULTIPLIER = 0.024f; //kg

    static void Prefix(ref Health __instance, float delta)
    {
      if (delta >= 0) return;

      var component = __instance.GetComponent<KPrefabID>();
      var spawnCell = Grid.PosToCell(component);

      var noDisease = SimUtil.DiseaseInfo.Invalid;

      SimMessages.AddRemoveSubstance(
        gameCell: spawnCell,
        new_element: BloodElement.BloodSimHash,
        ev: null,
        mass: MASS_MULTIPLIER * delta * -1,
        temperature: BODY_TEMP,
        disease_idx: noDisease.idx,
        disease_count: noDisease.count
      );
    }
  }
}
