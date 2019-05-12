using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  class Stinky
  {
    public const string ID = "CritterStinky";
    public const string NAME = "Stinky";
    public const string DESCRIPTION = "Gives off a funny smell.";

    public static void Init()
    {
      TraitHelpers.CreateTrait(ID, NAME, DESCRIPTION,
        on_add: delegate (GameObject go)
        {
          go.FindOrAddUnityComponent<Components.Stinky>();
        },
        positiveTrait: false
      );
    }
  }
}
