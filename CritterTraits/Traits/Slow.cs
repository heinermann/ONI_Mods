using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  static class Slow
  {
    public const string ID = "CritterSlow";
    public const string NAME = "Slow";
    public const string DESCRIPTION = "Moves at half the speed.";

    public static void Init()
    {
      TraitHelpers.CreateTrait(ID, NAME, DESCRIPTION,
        on_add: delegate (GameObject go)
        {
          var navigator = go.GetComponent<Navigator>();
          if (navigator != null)
          {
            navigator.defaultSpeed /= 2f;
          }
        },
        positiveTrait: false
      );
    }
  }
}
