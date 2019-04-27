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
      Strings.Add("STRINGS.ELEMENTS.BLOOD.NAME", STRINGS.UI.FormatAsLink("Blood", "BLOOD"));
      Strings.Add("STRINGS.ELEMENTS.BLOOD.DESC", "Bathe in the blood of your enemies! Contains some iron.");
      Strings.Add("STRINGS.ELEMENTS.FROZENBLOOD.NAME", STRINGS.UI.FormatAsLink("Frozen Blood", "FROZENBLOOD"));
      Strings.Add("STRINGS.ELEMENTS.FROZENBLOOD.DESC", "Blood that has been frozen. Contains some iron.");

      var elementCollection = YamlIO<ElementLoader.ElementEntryCollection>.Parse(BloodElement.CONFIG);
      __result.AddRange(elementCollection.elements);
    }
  }

  [HarmonyPatch(typeof(ElementLoader), "Load")]
  class ElementLoader_Load
  {
    static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
    {
      substanceList[BloodElement.BloodSimHash] = BloodElement.CreateBloodSubstance(substanceTable.GetSubstance(SimHashes.Magma));
      substanceList[BloodElement.FrozenBloodSimHash] = BloodElement.CreateFrozenBloodSubstance(substanceTable.GetSubstance(SimHashes.Ice));
    }
  }
}
