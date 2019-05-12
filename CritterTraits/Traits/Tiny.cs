namespace Heinermann.CritterTraits.Traits
{
  public static class Tiny
  {
    public const string ID = "CritterTiny";
    public const string NAME = "Tiny";
    public const string DESCRIPTION = "Is 40% smaller than average.";

    public static void Init()
    {
      TraitHelpers.CreateScaleTrait(ID, NAME, DESCRIPTION, 0.6f);
    }
  }
}
