using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Heinermann.ShadowMinion
{
  public class ShadowMinionStartingStats
  {
    public string Name;

    public string NameStringKey;

    public string GenderStringKey;

    public Personality personality;

    public List<Accessory> accessories = new List<Accessory>();

    public ShadowMinionStartingStats()
    {
      personality = new Personality("Shadow", "Shadow", "NB", "Sweet", "Aggressive", "None", 1, 1, -1, 1, 1, 1, "");

      Name = personality.Name;
      NameStringKey = personality.nameStringKey;
      GenderStringKey = personality.genderStringKey;

      KCompBuilder.BodyData bodyData = CreateBodyData(personality);
      foreach (AccessorySlot resource in Db.Get().AccessorySlots.resources)
      {
        if (resource.accessories.Count != 0)
        {
          Accessory accessory = null;
          if (resource == Db.Get().AccessorySlots.HeadShape)
          {
            accessory = resource.Lookup(bodyData.headShape);
            if (accessory == null) personality.headShape = 0;
          }
          else if (resource == Db.Get().AccessorySlots.Mouth)
          {
            accessory = resource.Lookup(bodyData.mouth);
            if (accessory == null) personality.mouth = 0;
          }
          else if (resource == Db.Get().AccessorySlots.Eyes)
          {
            accessory = resource.Lookup(bodyData.eyes);
            if (accessory == null) personality.eyes = 0;
          }
          else if (resource == Db.Get().AccessorySlots.Hair)
          {
            accessory = resource.Lookup(bodyData.hair);
            if (accessory == null) personality.hair = 0;
          }
          else if (resource == Db.Get().AccessorySlots.Body)
          {
            accessory = resource.Lookup(bodyData.body);
            if (accessory == null) personality.body = 0;
          }
          else if (resource == Db.Get().AccessorySlots.Arm)
          {
            accessory = resource.Lookup(bodyData.arms);
          }

          if (accessory == null)
          {
            accessory = resource.accessories[0];
          }
          accessories.Add(accessory);
        }
      }
    }

    public void Apply(GameObject go)
    {
      //MinionIdentity component = go.GetComponent<MinionIdentity>();
      //component.SetName(Name);
      //component.nameStringKey = NameStringKey;
      //component.genderStringKey = GenderStringKey;
      ApplyTraits(go);
      ApplyAccessories(go);
    }

    // Required
    public void ApplyAccessories(GameObject go)
    {
      Accessorizer component = go.GetComponent<Accessorizer>();
      foreach (Accessory accessory in accessories)
      {
        component.AddAccessory(accessory);
      }
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

    public void ApplyTraits(GameObject go)
    {
      Traits component = go.GetComponent<Traits>();
      component.Clear();

      component.Add(Db.Get().traits.Get(ShadowMinionConfig.MINION_BASE_TRAIT_ID));

      //go.GetComponent<MinionIdentity>().SetName(Name);
      //go.GetComponent<MinionIdentity>().SetGender(GenderStringKey);
    }
  }
}
