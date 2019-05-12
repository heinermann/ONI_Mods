using Klei.AI;
using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  static class Enduring
  {
    public const string ID = "CritterEnduring";
    public const string NAME = "Enduring";
    public const string DESCRIPTION = "Lives 25% longer than usual.";

    public static void Init()
    {
      TraitHelpers.CreateTrait(ID, NAME, DESCRIPTION,
        on_add: delegate (GameObject go)
        {
          var modifiers = go.GetComponent<Modifiers>();
          if (modifiers != null)
          {
            modifiers.attributes.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 0.25f, DESCRIPTION, is_multiplier: true));
          }
        },
        positiveTrait: true
      );
    }
  }
}
