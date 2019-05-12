using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  static class Fast
  {
    public const string ID = "CritterFast";
    public const string NAME = "Fast";
    public const string DESCRIPTION = "Is twice as fast as its peers.";

    public static void Init()
    {
      TraitHelpers.CreateTrait(ID, NAME, DESCRIPTION,
        on_add: delegate (GameObject go)
        {
          var navigator = go.GetComponent<Navigator>();
          if (navigator != null)
          {
            navigator.defaultSpeed *= 2f;
          }
        },
        positiveTrait: true
      );
    }
  }
}
