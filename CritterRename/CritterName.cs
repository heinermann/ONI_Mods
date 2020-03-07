using KSerialization;
using STRINGS;
using UnityEngine;

namespace Heinermann.CritterRename
{
  public class CritterName : KMonoBehaviour, ISaveLoadable
  {
    [SerializeField]
    [Serialize]
    private string critterName = "";

    [SerializeField]
    [Serialize]
    private uint generation = 1;

    private static readonly EventSystem.IntraObjectHandler<CritterName> OnSpawnedFromDelegate =
      new EventSystem.IntraObjectHandler<CritterName>(OnSpawnedFrom);

    private static readonly EventSystem.IntraObjectHandler<CritterName> OnLayEggDelegate =
      new EventSystem.IntraObjectHandler<CritterName>(OnLayEgg);

    protected override void OnPrefabInit()
    {
      Subscribe((int)GameHashes.SpawnedFrom, OnSpawnedFromDelegate);
      Subscribe((int)GameHashes.LayEgg, OnLayEggDelegate);
    }

    protected override void OnSpawn()
    {
      if (!Util.IsNullOrWhitespace(critterName))
      {
        ApplyName();
      }
    }

    private static void OnSpawnedFrom(CritterName component, object data)
    {
      (data as GameObject).GetComponent<CritterName>()?.TransferTo(component);
    }

    private static void OnLayEgg(CritterName component, object data)
    {
      component.TransferTo((data as GameObject).GetComponent<CritterName>());
    }

    private bool IsCritter()
    {
      return this.HasTag(GameTags.Creature);
    }

    private bool IsEgg()
    {
      return this.HasTag(GameTags.Egg);
    }

    public void SetName(string newName)
    {
      generation = 1;
      if (Util.IsNullOrWhitespace(newName) || newName.ToLower() == UI.StripLinkFormatting(GetPrefabName()).ToLower())
      {
        ResetToPrefabName();
        return;
      }

      critterName = newName;
      ApplyName();
    }

    private void setGameObjectName(string newName)
    {
      KSelectable selectable = GetComponent<KSelectable>();

      name = newName;
      selectable?.SetName(newName);
      gameObject.name = newName;
    }

    public void ApplyName()
    {
      if (!IsCritter()) return;

      string newName = critterName;
      if (generation == 2)
      {
        newName += " Jr.";
      }
      else if (generation > 2)
      {
        newName += " " + Util.ToRoman(generation);
      }
      setGameObjectName(newName);
    }

    public bool HasName()
    {
      return !Util.IsNullOrWhitespace(critterName);
    }

    public void TransferTo(CritterName other)
    {
      if (other == null || !HasName()) return;

      other.critterName = critterName;
      other.generation = generation;

      if (other.IsEgg())
      {
        other.generation += 1;
      }

      other.ApplyName();
    }

    public string GetPrefabName()
    {
      KPrefabID prefab = GetComponent<KPrefabID>();
      if (prefab != null)
      {
        return TagManager.GetProperName(prefab.PrefabTag);
      }
      return null;
    }

    public void ResetToPrefabName()
    {
      string prefabName = GetPrefabName();
      if (prefabName != null)
      {
        critterName = "";
        setGameObjectName(prefabName);
      }
    }
  }
}
