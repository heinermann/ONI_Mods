using UnityEngine;

namespace Heinermann.Blood
{
  public static class BloodElement
  {
    public static readonly Color32 BLOOD_RED = new Color32(77, 7, 7, 255);
    public const string BLOOD_ID = "Blood";
    public const string FROZENBLOOD_ID = "FrozenBlood";

    public static readonly SimHashes BloodSimHash = (SimHashes)Hash.SDBMLower(BLOOD_ID);
    public static readonly SimHashes FrozenBloodSimHash = (SimHashes)Hash.SDBMLower(FROZENBLOOD_ID);

    public static void RegisterBloodSubstance()
    {
      ElementUtil.CreateRegisteredSubstance(
        name: BLOOD_ID,
        state: Element.State.Liquid,
        kanim: ElementUtil.FindAnim("liquid_tank_kanim"),
        material: Assets.instance.substanceTable.liquidMaterial,
        colour: BLOOD_RED
      );
    }

    static Texture2D TintTextureBloodRed(Texture sourceTexture, string name)
    {
      Texture2D newTexture = Util.DuplicateTexture(sourceTexture as Texture2D);
      var pixels = newTexture.GetPixels32();
      for (int i = 0; i < pixels.Length; ++i)
      {
        var gray = ((Color)pixels[i]).grayscale * 1.5f;
        pixels[i] = (Color)BLOOD_RED * gray;
      }
      newTexture.SetPixels32(pixels);
      newTexture.Apply();
      newTexture.name = name;
      return newTexture;
    }

    static Material CreateFrozenBloodMaterial(Material source)
    {
      var frozenBloodMaterial = new Material(source);

      Texture2D newTexture = TintTextureBloodRed(frozenBloodMaterial.mainTexture, "frozenblood");

      frozenBloodMaterial.mainTexture = newTexture;
      frozenBloodMaterial.name = "matFrozenBlood";

      return frozenBloodMaterial;
    }

    public static void RegisterFrozenBloodSubstance()
    {
      Substance ice = Assets.instance.substanceTable.GetSubstance(SimHashes.Ice);

      ElementUtil.CreateRegisteredSubstance(
        name: FROZENBLOOD_ID,
        state: Element.State.Solid,
        kanim: ElementUtil.FindAnim("frozenblood_kanim"),
        material: CreateFrozenBloodMaterial(ice.material),
        colour: BLOOD_RED
      );
    }
  }

}
