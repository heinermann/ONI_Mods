using System.Reflection;
using UnityEngine;

namespace Heinermann.Floating
{
  public class FloatationChecker : KMonoBehaviour, ISim4000ms
  {
    public void Sim4000ms(float dt)
    {
      if (Helpers.ShouldFloat(transform))
      {
        AddFaller();
        TryMergeWithOthers();
      }
    }

    void AddFaller()
    {
      if (!GameComps.Fallers.Has(gameObject))
      {
        GameComps.Fallers.Add(gameObject, Vector2.zero);
      }

      MethodInfo addGravity = typeof(FallerComponents).GetMethod("AddGravity", BindingFlags.Static | BindingFlags.NonPublic);
      addGravity.Invoke(null, new object[] { transform, Vector2.zero });
    }

    private void TryMergeWithOthers()
    {
      Pickupable pickupable = GetComponent<Pickupable>();
      if (!pickupable.absorbable) return;

      Vector3 thisPosition = transform.GetPosition();

      int cell = Helpers.PosToCell(transform.position);
      pickupable.objectLayerListItem.Update(cell);
      for (ObjectLayerListItem i = pickupable.objectLayerListItem.nextItem; i != null; i = i.nextItem)
      {
        Pickupable target = i.gameObject.GetComponent<Pickupable>();
        if (target?.TryAbsorb(pickupable, false) == true)
        {
          // Offset the position to make it look less like one of the objects are vanishing
          target.transform.SetPosition((thisPosition + target.transform.GetPosition()) / 2);
          break;
        }
      }
    }
  }
}
