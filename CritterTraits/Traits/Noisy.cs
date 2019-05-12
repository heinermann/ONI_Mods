using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  class Noisy
  {
    public const string ID = "CritterNoisy";
    public const string NAME = "Noisy";
    public const string DESCRIPTION = "Makes a lot of noise when it moves.";

    public static void Init()
    {
      TraitHelpers.CreateTrait(ID, NAME, DESCRIPTION,
        on_add: delegate (GameObject go)
        {
          go.AddOrGet<Components.Noisy>();
        },
        positiveTrait: false
      );
    }
  }
}
