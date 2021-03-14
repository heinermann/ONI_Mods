using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Heinermann.Blood
{
  public static class BloodElement
  {
    public static readonly Color32 BLOOD_RED = new Color32(77, 7, 7, 255);

    public static readonly SimHashes BloodSimHash = (SimHashes)Hash.SDBMLower("Blood");
    public static readonly SimHashes FrozenBloodSimHash = (SimHashes)Hash.SDBMLower("FrozenBlood");

    public static readonly Dictionary<SimHashes, string> SimHashNameLookup = new Dictionary<SimHashes, string>
    {
      { BloodSimHash, "Blood" },
      { FrozenBloodSimHash, "FrozenBlood" }
    };

    public static readonly Dictionary<string, object> ReverseSimHashNameLookup =
      SimHashNameLookup.ToDictionary(x => x.Value, x => x.Key as object);

    public static Substance CreateBloodSubstance(Substance source)
    {
      return ModUtil.CreateSubstance(
        name: "Blood",
        state: Element.State.Liquid,
        kanim: source.anim,
        material: source.material,
        colour: BLOOD_RED,
        ui_colour: BLOOD_RED,
        conduit_colour: BLOOD_RED
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

    public static Substance CreateFrozenBloodSubstance(Material sourceMaterial, KAnimFile sourceAnim)
    {
      return ModUtil.CreateSubstance(
        name: "FrozenBlood",
        state: Element.State.Solid,
        kanim: sourceAnim,
        material: CreateFrozenBloodMaterial(sourceMaterial),
        colour: BLOOD_RED,
        ui_colour: BLOOD_RED,
        conduit_colour: BLOOD_RED
      );
    }
  }

}
