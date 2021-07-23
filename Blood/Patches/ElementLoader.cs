using HarmonyLib;

namespace Heinermann.Blood.Patches
{
  [HarmonyPatch(typeof(Assets), "SubstanceListHookup")]
  class Assets_SubstanceListHookup
  {
    static void Prefix()
    {
      string containsIron = $"Contains traces of {STRINGS.UI.FormatAsLink("iron", "IRON")}.";
      ElementUtil.RegisterElementStrings(BloodElement.BLOOD_ID, "Blood", $"Bathe in the blood of your enemies! {containsIron}");
      ElementUtil.RegisterElementStrings(BloodElement.FROZENBLOOD_ID, "Frozen Blood", $"Blood that has been frozen. {containsIron}");
    }

    static void Postfix()
    {
      BloodElement.RegisterBloodSubstance();
      BloodElement.RegisterFrozenBloodSubstance();
    }
  }
}
