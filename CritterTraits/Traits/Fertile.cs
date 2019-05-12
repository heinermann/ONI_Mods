using Klei.AI;
using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  static class Fertile
  {
    public const string ID = "CritterFertile";
    public const string NAME = "Fertile";
    public const string DESCRIPTION = "Is very fertile, fertility is improved by 25%.";

    public static void Init()
    {
      TraitHelpers.CreateTrait(ID, NAME, DESCRIPTION,
        on_add: delegate (GameObject go)
        {
          var modifiers = go.GetComponent<Modifiers>();
          if (modifiers != null)
          {
            modifiers.attributes.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, 0.25f, DESCRIPTION, is_multiplier: true));
          }
        },
        positiveTrait: true
      );
    }
  }
}
