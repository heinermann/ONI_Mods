using Klei.AI;
using KSerialization;
using UnityEngine;

namespace Heinermann.CritterTraits.Components
{
  /**
   * This class applies the necessary traits on object spawn, only if they haven't
   * been applied before. The state of whether the traits were applied or not is
   * saved via this class.
   */
  public class CritterTraits : KMonoBehaviour, ISaveLoadable
  {
    [SerializeField]
    [Serialize]
    private bool appliedCritterTraits = false;

    protected override void OnPrefabInit()
    {
      TraitHelpers.InitCritterTraits();
    }

    protected override void OnSpawn()
    {
      if (!appliedCritterTraits)
      {
        var modifiers = gameObject.AddOrGet<Modifiers>();
        modifiers.InitializeComponent();

        var traits = gameObject.AddOrGet<Klei.AI.Traits>();
        TraitHelpers.ChooseTraits(gameObject).ForEach(trait => traits.Add(Db.Get().traits.Get(trait)));

        appliedCritterTraits = true;
      }
    }
  }
}
