using Klei.AI;
using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  static class ShortLived
  {
    public const string ID = "CritterShortLived";
    public const string NAME = "Short-lived";
    public const string DESCRIPTION = "Has a 20% shorter lifespan.";

    public static void Init()
    {
      TraitHelpers.CreateTrait(ID, NAME, DESCRIPTION,
        on_add: delegate (GameObject go)
        {
          var modifiers = go.GetComponent<Modifiers>();
          if (modifiers != null)
          {
            modifiers.attributes.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, -0.20f, DESCRIPTION, is_multiplier: true));
          }
        },
        positiveTrait: false
      );
    }
  }
}
