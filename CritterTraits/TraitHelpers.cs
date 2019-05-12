using Heinermann.CritterTraits.Traits;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Heinermann.CritterTraits
{
  public static class TraitHelpers
  {
    /**
     * Register all traits with the DB.
     */
    private static bool traitsInitialized = false;
    public static void InitCritterTraits()
    {
      if (traitsInitialized) return;

      Large.Init();
      Huge.Init();
      Small.Init();
      Tiny.Init();

      Glowing.Init();

      Fast.Init();
      Slow.Init();

      Enduring.Init();
      ShortLived.Init();

      Fertile.Init();

      Noisy.Init();

      Traits.Stinky.Init();

      traitsInitialized = true;
    }

    /**
     * Chooses a random set of traits between 0 and 4. (max is capped randomly between 2 and 4)
     * Only applied traits that are relevant, i.e. Glowing trait on Shine Bug doesn't make sense.
     */
    public static List<string> ChooseTraits(GameObject inst)
    {
      var result = new List<string>();
      int numTraitsToChoose = UnityEngine.Random.Range(2, 4);

      result.Add(ChooseTraitFrom(SIZE_GROUP, 0.4f));
      result.Add(ChooseTraitFrom(NOISE_GROUP, 0.05f));
      result.Add(ChooseTraitFrom(SMELL_GROUP, 0.05f));

      // Glowing (if it doesn't already glow)
      if (inst.GetComponent<Light2D>() == null)
      {
        result.Add(ChooseTraitFrom(GLOW_GROUP, 0.1f));
      }

      // Speed
      if (inst.GetComponent<Navigator>() != null)
      {
        result.Add(ChooseTraitFrom(SPEED_GROUP, 0.2f));
      }

      // Lifespan
      if (HasAmount(inst, Db.Get().Amounts.Age))
      {
        result.Add(ChooseTraitFrom(LIFESPAN_GROUP, 0.15f));
      }

      // Fertility
      if (HasAmount(inst, Db.Get().Amounts.Fertility))
      {
        result.Add(ChooseTraitFrom(FERTILITY_GROUP, 0.1f));
      }

      return result
        .Where(s => s != null)
        .OrderBy(x => Util.GaussianRandom())
        .Take(numTraitsToChoose)
        .ToList();
    }

    private static bool HasAmount(GameObject go, Amount amount)
    {
      var modifiers = go.GetComponent<Modifiers>();
      if (modifiers == null) return false;

      return modifiers.amounts.Has(amount);
    }

    /**
     * Chooses a trait from the given list with a probability. If the probability check fails it returns null.
     */
    private static string ChooseTraitFrom(List<string> traits, float desiredProbability)
    {
      float prob = UnityEngine.Random.Range(0f, 1f);
      if (prob <= desiredProbability)
      {
        return Util.GetRandom(traits);
      }
      return null;
    }

    private static readonly List<string> SIZE_GROUP = new List<string> { Large.ID, Huge.ID, Small.ID, Tiny.ID };
    private static readonly List<string> GLOW_GROUP = new List<string> { Glowing.ID };
    private static readonly List<string> SPEED_GROUP = new List<string> { Fast.ID, Slow.ID };
    private static readonly List<string> LIFESPAN_GROUP = new List<string> { Enduring.ID, ShortLived.ID };
    private static readonly List<string> FERTILITY_GROUP = new List<string> { Fertile.ID };
    private static readonly List<string> NOISE_GROUP = new List<string> { Noisy.ID };
    private static readonly List<string> SMELL_GROUP = new List<string> { Traits.Stinky.ID };

    public static void CreateTrait(string id, string name, string desc, Action<GameObject> on_add, bool positiveTrait)
    {
      Trait trait = Db.Get().CreateTrait(
        id: id,
        name: name,
        description: desc,
        group_name: null,
        should_save: true,
        disabled_chore_groups: null,
        positive_trait: positiveTrait,
        is_valid_starter_trait: false);

      trait.OnAddTrait = on_add;
    }

    public static void CreateComponentTrait<T>(string id, string name, string desc, bool positiveTrait) where T : KMonoBehaviour
    {
      Action<GameObject> onAdd = delegate (GameObject go)
      {
        go.FindOrAddUnityComponent<T>();
      };
      CreateTrait(id, name, desc, onAdd, positiveTrait);
    }

    /**
     * Creates a trait that modifies the object's scale (including max health and calories).
     */
    public static void CreateScaleTrait(string id, string name, string desc, float scale)
    {
      CreateTrait(id, name, desc,
        on_add: delegate (GameObject go)
        {
          CritterUtil.SetObjectScale(go, scale, desc);
        },
        positiveTrait: scale >= 1.0f
      );
    }
  }
}
