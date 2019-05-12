namespace Heinermann.CritterTraits.Traits
{
  public static class Huge
  {
    public const string ID = "CritterHuge";
    public const string NAME = "Huge";
    public const string DESCRIPTION = "Is 50% larger than average.";

    public static void Init()
    {
      TraitHelpers.CreateScaleTrait(ID, NAME, DESCRIPTION, 1.5f);
    }

  }
}
