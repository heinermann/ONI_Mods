﻿using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Heinermann.ShadowMinion
{
  public class ShadowMinionStartingStats
  {
    public string Name;

    public string NameStringKey;

    public string GenderStringKey;

    public List<Trait> Traits = new List<Trait>();

    public Trait stressTrait;

    public int voiceIdx;

    public Dictionary<string, int> StartingLevels = new Dictionary<string, int>();

    public Personality personality;

    public List<Accessory> accessories = new List<Accessory>();

    public Dictionary<SkillGroup, float> skillAptitudes = new Dictionary<SkillGroup, float>();

    public ShadowMinionStartingStats()
    {
      int idx2 = UnityEngine.Random.Range(0, 35);
      personality = new Personality("Shadow", "Shadow", "NB", "Sweet", "Aggressive", "None", 1, 1, -1, 1, 1, 1, "");

      voiceIdx = UnityEngine.Random.Range(0, 4);
      Name = personality.Name;
      NameStringKey = personality.nameStringKey;
      GenderStringKey = personality.genderStringKey;
      Traits.Add(Db.Get().traits.Get(ShadowMinionConfig.MINION_BASE_TRAIT_ID));

      GenerateAptitudes();
      int pointsDelta = GenerateTraits();
      GenerateAttributes(pointsDelta);
      KCompBuilder.BodyData bodyData = CreateBodyData(personality);
      foreach (AccessorySlot resource in Db.Get().AccessorySlots.resources)
      {
        if (resource.accessories.Count != 0)
        {
          Accessory accessory = null;
          if (resource == Db.Get().AccessorySlots.HeadShape)
          {
            accessory = resource.Lookup(bodyData.headShape);
            if (accessory == null)
            {
              personality.headShape = 0;
            }
          }
          else if (resource == Db.Get().AccessorySlots.Mouth)
          {
            accessory = resource.Lookup(bodyData.mouth);
            if (accessory == null)
            {
              personality.mouth = 0;
            }
          }
          else if (resource == Db.Get().AccessorySlots.Eyes)
          {
            accessory = resource.Lookup(bodyData.eyes);
            if (accessory == null)
            {
              personality.eyes = 0;
            }
          }
          else if (resource == Db.Get().AccessorySlots.Hair)
          {
            accessory = resource.Lookup(bodyData.hair);
            if (accessory == null)
            {
              personality.hair = 0;
            }
          }
          else if (resource != Db.Get().AccessorySlots.HatHair)
          {
            if (resource == Db.Get().AccessorySlots.Body)
            {
              accessory = resource.Lookup(bodyData.body);
              if (accessory == null)
              {
                personality.body = 0;
              }
            }
            else if (resource == Db.Get().AccessorySlots.Arm)
            {
              accessory = resource.Lookup(bodyData.arms);
            }
          }
          if (accessory == null)
          {
            accessory = resource.accessories[0];
          }
          accessories.Add(accessory);
        }
      }
    }

    private int GenerateTraits()
    {
      int statDelta = 0;
      List<string> selectedTraits = new List<string>();
      System.Random randSeed = new System.Random();
      Trait trait = stressTrait = Db.Get().traits.Get(personality.stresstrait);
      Func<List<DUPLICANTSTATS.TraitVal>, bool> func = delegate (List<DUPLICANTSTATS.TraitVal> traitPossibilities)
      {
        if (Traits.Count > DUPLICANTSTATS.MAX_TRAITS)
        {
          return false;
        }
        float num2 = Util.GaussianRandom(0f, 1f);
        List<DUPLICANTSTATS.TraitVal> list = new List<DUPLICANTSTATS.TraitVal>(traitPossibilities);
        list.ShuffleSeeded(randSeed);
        list.Sort((DUPLICANTSTATS.TraitVal t1, DUPLICANTSTATS.TraitVal t2) => -t1.probability.CompareTo(t2.probability));
        foreach (DUPLICANTSTATS.TraitVal item in list)
        {
          DUPLICANTSTATS.TraitVal current = item;
          if (!selectedTraits.Contains(current.id))
          {
            if (current.requiredNonPositiveAptitudes != null)
            {
              bool flag2 = false;
              foreach (KeyValuePair<SkillGroup, float> skillAptitude in skillAptitudes)
              {
                if (flag2)
                {
                  break;
                }
                foreach (HashedString requiredNonPositiveAptitude in current.requiredNonPositiveAptitudes)
                {
                  if (requiredNonPositiveAptitude == skillAptitude.Key.IdHash && skillAptitude.Value > 0f)
                  {
                    flag2 = true;
                    break;
                  }
                }
              }
              if (flag2)
              {
                continue;
              }
            }
            if (current.mutuallyExclusiveTraits != null)
            {
              bool flag3 = false;
              foreach (string item2 in selectedTraits)
              {
                flag3 = current.mutuallyExclusiveTraits.Contains(item2);
                if (flag3)
                {
                  break;
                }
              }
              if (flag3)
              {
                continue;
              }
            }
            if (num2 > current.probability)
            {
              Trait trait3 = Db.Get().traits.TryGet(current.id);
              if (trait3 == null)
              {
                Debug.LogWarning("Trying to add nonexistent trait: " + current.id);
              }
              else
              {
                selectedTraits.Add(current.id);
                statDelta += current.statBonus;
                Traits.Add(trait3);
                return true;
              }
            }
          }
        }
        return false;
      };
      int num = 3;
      bool flag = false;
      while (!flag)
      {
        for (int i = 0; i < num; i++)
        {
          flag = (func(DUPLICANTSTATS.BADTRAITS) || flag);
        }
      }
      flag = false;
      while (!flag)
      {
        for (int j = 0; j < num; j++)
        {
          flag = (func(DUPLICANTSTATS.GOODTRAITS) || flag);
        }
      }
      return statDelta;
    }

    private void GenerateAptitudes()
    {
      int num = UnityEngine.Random.Range(1, 4);
      List<SkillGroup> list = new List<SkillGroup>(Db.Get().SkillGroups.resources);
      list.Shuffle();
      for (int i = 0; i < num; i++)
      {
        skillAptitudes.Add(list[i], DUPLICANTSTATS.APTITUDE_BONUS);
      }
    }

    private void GenerateAttributes(int pointsDelta)
    {
      float f = Util.GaussianRandom(0f, 1f) * ((float)DUPLICANTSTATS.MAX_STAT_POINTS - (float)DUPLICANTSTATS.MIN_STAT_POINTS) / 2f + (float)DUPLICANTSTATS.MIN_STAT_POINTS;
      int num = Mathf.RoundToInt(f);
      List<string> list = new List<string>(DUPLICANTSTATS.ALL_ATTRIBUTES);
      int[] randomDistribution = DUPLICANTSTATS.DISTRIBUTIONS.GetRandomDistribution();
      for (int i = 0; i < list.Count; i++)
      {
        if (!StartingLevels.ContainsKey(list[i]))
        {
          StartingLevels[list[i]] = 0;
        }
      }
      foreach (KeyValuePair<SkillGroup, float> skillAptitude in skillAptitudes)
      {
        if (skillAptitude.Key.relevantAttributes.Count > 0)
        {
          for (int j = 0; j < skillAptitude.Key.relevantAttributes.Count; j++)
          {
            Dictionary<string, int> startingLevels;
            string id;
            (startingLevels = StartingLevels)[id = skillAptitude.Key.relevantAttributes[j].Id] = startingLevels[id] + DUPLICANTSTATS.APTITUDE_ATTRIBUTE_BONUSES[skillAptitudes.Count - 1];
          }
        }
      }
      list.Shuffle();
      for (int k = 0; k < list.Count; k++)
      {
        string text = list[k];
        int b = randomDistribution[Mathf.Min(k, randomDistribution.Length - 1)];
        int num2 = Mathf.Min(num, b);
        if (!StartingLevels.ContainsKey(text))
        {
          StartingLevels[text] = 0;
        }
        Dictionary<string, int> startingLevels;
        string key;
        (startingLevels = StartingLevels)[key = text] = startingLevels[key] + num2;
        num -= num2;
      }
    }

    public void Apply(GameObject go)
    {
      MinionIdentity component = go.GetComponent<MinionIdentity>();
      component.SetName(Name);
      component.nameStringKey = NameStringKey;
      component.genderStringKey = GenderStringKey;
      ApplyTraits(go);
      ApplyRace(go);
      ApplyAptitudes(go);
      ApplyAccessories(go);
      ApplyExperience(go);
    }

    public void ApplyExperience(GameObject go)
    {
      foreach (KeyValuePair<string, int> startingLevel in StartingLevels)
      {
        go.GetComponent<AttributeLevels>().SetLevel(startingLevel.Key, startingLevel.Value);
      }
    }

    public void ApplyAccessories(GameObject go)
    {
      Accessorizer component = go.GetComponent<Accessorizer>();
      foreach (Accessory accessory in accessories)
      {
        component.AddAccessory(accessory);
      }
    }

    public void ApplyRace(GameObject go)
    {
      MinionIdentity component = go.GetComponent<MinionIdentity>();
      component.voiceIdx = voiceIdx;
    }

    public static KCompBuilder.BodyData CreateBodyData(Personality p)
    {
      KCompBuilder.BodyData result = default(KCompBuilder.BodyData);
      result.eyes = HashCache.Get().Add(string.Format("eyes_{0:000}", p.eyes));
      result.hair = HashCache.Get().Add(string.Format("hair_{0:000}", p.hair));
      result.headShape = HashCache.Get().Add(string.Format("headshape_{0:000}", p.headShape));
      result.mouth = HashCache.Get().Add(string.Format("mouth_{0:000}", p.mouth));
      result.neck = HashCache.Get().Add(string.Format("neck_{0:000}", p.neck));
      result.arms = HashCache.Get().Add(string.Format("arm_{0:000}", p.body));
      result.body = HashCache.Get().Add(string.Format("body_{0:000}", p.body));
      result.hat = HashedString.Invalid;
      return result;
    }

    public void ApplyAptitudes(GameObject go)
    {
      MinionResume component = go.GetComponent<MinionResume>();
      foreach (KeyValuePair<SkillGroup, float> skillAptitude in skillAptitudes)
      {
        component.SetAptitude(skillAptitude.Key.Id, skillAptitude.Value);
      }
    }

    public void ApplyTraits(GameObject go)
    {
      Traits component = go.GetComponent<Traits>();
      component.Clear();
      foreach (Trait trait in Traits)
      {
        component.Add(trait);
      }
      component.Add(stressTrait);
      go.GetComponent<MinionIdentity>().SetName(Name);
      go.GetComponent<MinionIdentity>().SetGender(GenderStringKey);
    }

    public GameObject Deliver(Vector3 location)
    {
      GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID));
      gameObject.SetActive(true);
      gameObject.transform.SetLocalPosition(location);
      Apply(gameObject);
      Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
      ChoreProvider component = gameObject.GetComponent<ChoreProvider>();
      new EmoteChore(component, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", Telepad.PortalBirthAnim, null);
      return gameObject;
    }
  }
}
