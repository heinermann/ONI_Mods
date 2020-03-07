using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Heinermann.ShadowMinion
{
  [SerializationConfig(MemberSerialization.OptIn)]
  public class NullIdentity : KMonoBehaviour, IAssignableIdentity
  {
    [Serialize]
    public Ref<MinionAssignablesProxy> assignableProxy;

    [Serialize]
    public KCompBuilder.BodyData bodyData;

    public List<Ownables> GetOwners()
    {
      return assignableProxy.Get().ownables;
    }

    public string GetProperName()
    {
      return gameObject.GetProperName();
    }

    public Ownables GetSoleOwner()
    {
      return assignableProxy.Get().GetComponent<Ownables>();
    }

    public bool IsNull()
    {
      return this == null;
    }

    private static readonly EventSystem.IntraObjectHandler<NullIdentity> OnDiedDelegate = new EventSystem.IntraObjectHandler<NullIdentity>(delegate (NullIdentity component, object data)
    {
      component.OnDied(data);
    });

    protected override void OnPrefabInit()
    {
      KAnimControllerBase component = GetComponent<KAnimControllerBase>();
      if (component != null)
      {
        KAnimControllerBase kAnimControllerBase = component;
        kAnimControllerBase.OnUpdateBounds = (Action<Bounds>)Delegate.Combine(kAnimControllerBase.OnUpdateBounds, new Action<Bounds>(OnUpdateBounds));
      }
      Subscribe((int)GameHashes.Died, OnDiedDelegate);
    }

    protected override void OnSpawn()
    {
      ValidateProxy();
      CleanupLimboMinions();

      PathProber component = GetComponent<PathProber>();
      if (component != null)
      {
        component.SetGroupProber(MinionGroupProber.Get());
      }
      SymbolOverrideController component2 = GetComponent<SymbolOverrideController>();
      if (component2 != null)
      {
        Accessorizer component3 = base.gameObject.GetComponent<Accessorizer>();
        if (component3 != null)
        {
          bodyData = default(KCompBuilder.BodyData);
          component3.GetBodySlots(ref bodyData);
          string text = HashCache.Get().Get(component3.GetAccessory(Db.Get().AccessorySlots.HeadShape).symbol.hash);
          string str = text.Replace("headshape", "cheek");
          component2.AddSymbolOverride("snapto_cheek", Assets.GetAnim("head_swap_kanim").GetData().build.GetSymbol(str), 1);
          component2.AddSymbolOverride(Db.Get().AccessorySlots.HairAlways.targetSymbolId, component3.GetAccessory(Db.Get().AccessorySlots.Hair).symbol, 1);
          component2.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component3.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
        }
      }
      Prioritizable component4 = GetComponent<Prioritizable>();
      if (component4 != null)
      {
        component4.showIcon = false;
      }
      Pickupable component5 = GetComponent<Pickupable>();
      if (component5 != null)
      {
        component5.carryAnimOverride = Assets.GetAnim("anim_incapacitated_carrier_kanim");
      }
    }

    protected override void OnCleanUp()
    {
      if (assignableProxy != null)
      {
        MinionAssignablesProxy minionAssignablesProxy = assignableProxy.Get();
        if (minionAssignablesProxy != null && minionAssignablesProxy.target == this)
        {
          Util.KDestroyGameObject(minionAssignablesProxy.gameObject);
        }
      }
    }

    private void OnUpdateBounds(Bounds bounds)
    {
      KBoxCollider2D component = GetComponent<KBoxCollider2D>();
      component.offset = bounds.center;
      component.size = bounds.extents;
    }

    private void OnDied(object data)
    {
      GetSoleOwner().UnassignAll();
      //GetEquipment().UnequipAll();
    }

    public void ValidateProxy()
    {
      assignableProxy = MinionAssignablesProxy.InitAssignableProxy(assignableProxy, this);
    }

    private void CleanupLimboMinions()
    {
      KPrefabID component = GetComponent<KPrefabID>();
      if (component.InstanceID == -1)
      {
        DebugUtil.LogWarningArgs("Minion with an invalid kpid! Attempting to recover...", name);
        if (KPrefabIDTracker.Get().GetInstance(component.InstanceID) != null)
        {
          KPrefabIDTracker.Get().Unregister(component);
        }
        component.InstanceID = KPrefabID.GetUniqueID();
        KPrefabIDTracker.Get().Register(component);
        DebugUtil.LogWarningArgs("Restored as:", component.InstanceID);
      }
      if (component.conflicted)
      {
        DebugUtil.LogWarningArgs("Minion with a conflicted kpid! Attempting to recover... ", component.InstanceID, name);
        if (KPrefabIDTracker.Get().GetInstance(component.InstanceID) != null)
        {
          KPrefabIDTracker.Get().Unregister(component);
        }
        component.InstanceID = KPrefabID.GetUniqueID();
        KPrefabIDTracker.Get().Register(component);
        DebugUtil.LogWarningArgs("Restored as:", component.InstanceID);
      }
      assignableProxy.Get().SetTarget(this, base.gameObject);
    }

  }
}
