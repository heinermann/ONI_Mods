using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  static class Glowing
  {
    public const string ID = "CritterGlowing";
    public const string NAME = "Bioluminescent";
    public const string DESCRIPTION = "Gives off a faint, steady glow.";

    public static void Init()
    {
      TraitHelpers.CreateTrait(ID, NAME, DESCRIPTION,
        on_add: delegate (GameObject go)
        {
          CritterUtil.AddObjectLight(go, Random.ColorHSV(0f, 1f, 0f, 1f, 0.5f, 0.8f), 2f, 600);
          //go.AddOrGetDef<CreatureLightToggleController.Def>();
        },
        positiveTrait: true
      );
    }
  }
}
