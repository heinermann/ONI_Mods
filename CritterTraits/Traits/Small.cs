namespace Heinermann.CritterTraits.Traits
{
  public static class Small
  {
    public const string ID = "CritterSmall";
    public const string NAME = "Small";
    public const string DESCRIPTION = "Is 20% smaller than average.";

    public static void Init()
    {
      TraitHelpers.CreateScaleTrait(ID, NAME, DESCRIPTION, 0.8f);
    }
  }
}
