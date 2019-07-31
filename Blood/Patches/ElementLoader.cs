using Harmony;
using Klei;
using System.Collections;
using System.Collections.Generic;

namespace Heinermann.Blood.Patches
{
  [HarmonyPatch(typeof(ElementLoader), "CollectElementsFromYAML")]
  class ElementLoader_CollectElementsFromYAML
  {
    static void Postfix(ref List<ElementLoader.ElementEntry> __result)
    {
      var containsIron = $"Contains traces of {STRINGS.UI.FormatAsLink("iron", "IRON")}.";
      Strings.Add("STRINGS.ELEMENTS.BLOOD.NAME", STRINGS.UI.FormatAsLink("Blood", "BLOOD"));
      Strings.Add("STRINGS.ELEMENTS.BLOOD.DESC", $"Bathe in the blood of your enemies! {containsIron}");
      Strings.Add("STRINGS.ELEMENTS.FROZENBLOOD.NAME", STRINGS.UI.FormatAsLink("Frozen Blood", "FROZENBLOOD"));
      Strings.Add("STRINGS.ELEMENTS.FROZENBLOOD.DESC", $"Blood that has been frozen. {containsIron}");

      var elementCollection = YamlIO.Parse<ElementLoader.ElementEntryCollection>(BloodElement.CONFIG, "ElementLoader.cs");
      __result.AddRange(elementCollection.elements);
    }
  }

  [HarmonyPatch(typeof(ElementLoader), "Load")]
  class ElementLoader_Load
  {
    static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
    {
      var water = substanceTable.GetSubstance(SimHashes.Water);
      var ice = substanceTable.GetSubstance(SimHashes.Ice);
      substanceList[BloodElement.BloodSimHash] = BloodElement.CreateBloodSubstance(water);
      substanceList[BloodElement.FrozenBloodSimHash] = BloodElement.CreateFrozenBloodSubstance(ice.material, water.anim);
    }
  }
}
