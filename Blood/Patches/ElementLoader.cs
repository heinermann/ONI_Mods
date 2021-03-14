using Harmony;
using Klei;
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
      
      var elementCollection = YamlIO.Parse<ElementLoader.ElementEntryCollection>(BloodElement.CONFIG, new FileHandle());
      __result.AddRange(elementCollection.elements);
    }
  }

  [HarmonyPatch(typeof(Assets), "SubstanceListHookup")]
  class Assets_SubstanceListHookup
  {
    static void Prefix(Assets __instance)
    {
      SubstanceTable table = __instance.substanceTable;
      var water = table.GetSubstance(SimHashes.Water);
      var ice = table.GetSubstance(SimHashes.Ice);
      
      table.GetList().Add(BloodElement.CreateBloodSubstance(water));
      table.GetList().Add(BloodElement.CreateFrozenBloodSubstance(ice.material, water.anim));
    }
  }

  /*
  [HarmonyPatch(typeof(ElementLoader), "Load")]
  class ElementLoader_Load
  {
    static void Prefix(ref Hashtable substanceList, Dictionary<string, SubstanceTable> substanceTable)
    {
      var water = substanceTable[""].GetSubstance(SimHashes.Water);
      var ice = substanceTable[""].GetSubstance(SimHashes.Ice);
      substanceList[BloodElement.BloodSimHash] = BloodElement.CreateBloodSubstance(water);
      substanceList[BloodElement.FrozenBloodSimHash] = BloodElement.CreateFrozenBloodSubstance(ice.material, water.anim);
    }
  }*/
}
