using System.Collections.Generic;
using UnityEngine;

namespace Heinermann.Blood
{
  public static class ElementUtil
  {
    public static void RegisterElementStrings(string elementId, string name, string description)
    {
      string upperElemId = elementId.ToUpper();
      Strings.Add($"STRINGS.ELEMENTS.{upperElemId}.NAME", STRINGS.UI.FormatAsLink(name, upperElemId));
      Strings.Add($"STRINGS.ELEMENTS.{upperElemId}.DESC", description);
    }

    public static void AddSubstance(Substance substance)
    {
      Assets.instance.substanceTable.GetList().Add(substance);
    }

    public static Substance CreateSubstance(string name, Element.State state, KAnimFile kanim, Material material, Color32 colour)
    {
      return ModUtil.CreateSubstance(name, state, kanim, material, colour, colour, colour);
    }

    public static Substance CreateRegisteredSubstance(string name, Element.State state, KAnimFile kanim, Material material, Color32 colour)
    {
      Substance result = CreateSubstance(name, state, kanim, material, colour);
      SimHashUtil.RegisterSimHash(name);
      AddSubstance(result);
      ElementLoader.FindElementByHash(result.elementID).substance = result;
      return result;
    }
  }
}
