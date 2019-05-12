namespace Heinermann.CritterTraits.Traits
{
  static class Large
  {
    public const string ID = "CritterLarge";
    public const string NAME = "Large";
    public const string DESCRIPTION = "Is 25% larger than average.";

    public static void Init()
    {
      TraitHelpers.CreateScaleTrait(ID, NAME, DESCRIPTION, 1.25f);
    }
  }
}
