using System.Collections.Generic;
using UnityEngine;

namespace Heinermann.Blood
{
  public static class BloodElement
  {
    public static readonly Color32 BLOOD_RED = new Color32(77, 7, 7, 0);

    public static readonly SimHashes BloodSimHash = (SimHashes)Hash.SDBMLower("Blood");
    public static readonly SimHashes FrozenBloodSimHash = (SimHashes)Hash.SDBMLower("FrozenBlood");

    public static Dictionary<SimHashes, string> SimHashNameLookup = new Dictionary<SimHashes, string>
    {
      { BloodSimHash, "Blood" },
      { FrozenBloodSimHash, "FrozenBlood" }
    };

    // iron mass = 0.5 mg/mL blood (0.0005g/1.13g = 0.00044)
    public const string CONFIG = @"
---
elements:
  - elementId: Blood
    maxMass: 1000
    liquidCompression: 1.02
    speed: 100
    minHorizontalFlow: 0.1
    minVerticalFlow: 0.01
    specificHeatCapacity: 3.49
    thermalConductivity: 0.58
    solidSurfaceAreaMultiplier: 1
    liquidSurfaceAreaMultiplier: 25
    gasSurfaceAreaMultiplier: 1
    lowTemp: 272.5
    highTemp: 372.5
    lowTempTransitionTarget: FrozenBlood
    highTempTransitionTarget: Steam
    highTempTransitionOreId: Iron
    highTempTransitionOreMassConversion: 0.0005
    defaultTemperature: 310
    defaultMass: 1000
    molarMass: 30
    toxicity: 0.1
    lightAbsorptionFactor: 0.8
    tags:
    - Mixture
    - AnyWater
    isDisabled: false
    state: Liquid
    localizationID: STRINGS.ELEMENTS.BLOOD.NAME

  - elementId: FrozenBlood
    specificHeatCapacity: 3.05
    thermalConductivity: 1
    solidSurfaceAreaMultiplier: 1
    liquidSurfaceAreaMultiplier: 1
    gasSurfaceAreaMultiplier: 1
    strength: 1
    highTemp: 272.5
    highTempTransitionTarget: Blood
    defaultTemperature: 230
    defaultMass: 500
    maxMass: 800
    hardnessTier: 2
    hardness: 10
    molarMass: 35
    lightAbsorptionFactor: 0.8
    materialCategory: Liquifiable
    tags:
    - IceOre
    - Mixture
    - BuildableAny
    buildMenuSort: 5
    isDisabled: false
    state: Solid
    localizationID: STRINGS.ELEMENTS.FROZENBLOOD.NAME
";

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
        pixels[i].r = (byte)(pixels[i].r * (BLOOD_RED.r + 100) / 255);
        pixels[i].g = (byte)(pixels[i].g * BLOOD_RED.g / 255);
        pixels[i].b = (byte)(pixels[i].b * BLOOD_RED.b / 255);
      }
      newTexture.SetPixels32(pixels);
      newTexture.Apply();
      newTexture.name = name;
      return newTexture;
    }

    static Material CreateFrozenBloodMaterial(Material iceMaterial)
    {
      var frozenBloodMaterial = new Material(iceMaterial);

      Texture2D newTexture = TintTextureBloodRed(frozenBloodMaterial.mainTexture, "frozenblood");

      frozenBloodMaterial.mainTexture = newTexture;
      frozenBloodMaterial.name = "matFrozenBlood";

      return frozenBloodMaterial;
    }

    public static Substance CreateFrozenBloodSubstance(Substance source)
    {
      return ModUtil.CreateSubstance(
        name: "FrozenBlood",
        state: Element.State.Solid,
        kanim: source.anim,
        material: CreateFrozenBloodMaterial(source.material),
        colour: BLOOD_RED,
        ui_colour: BLOOD_RED,
        conduit_colour: BLOOD_RED
      );
    }
  }

}
