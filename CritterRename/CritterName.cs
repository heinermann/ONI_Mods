using KSerialization;
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

    protected override void OnSpawn()
    {
      if (!Util.IsNullOrWhitespace(critterName))
      {
        ApplyName();
      }
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
      if (Util.IsNullOrWhitespace(newName))
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

    public void TransferTo(CritterName other)
    {
      if (Util.IsNullOrWhitespace(critterName)) return;

      other.critterName = critterName;
      other.generation = generation;

      if (other.IsEgg())
      {
        other.generation += 1;
      }

      other.ApplyName();
    }

    public void ResetToPrefabName()
    {
      KPrefabID prefab = GetComponent<KPrefabID>();
      if (prefab != null)
      {
        critterName = "";
        setGameObjectName(TagManager.GetProperName(prefab.PrefabTag));
      }
    }
  }
}
