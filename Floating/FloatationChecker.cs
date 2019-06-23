using System.Reflection;
using UnityEngine;

namespace Heinermann.Floating
{
  public class FloatationChecker : KMonoBehaviour, ISim4000ms
  {
    void AddFaller()
    {
      if (!GameComps.Fallers.Has(gameObject))
      {
        GameComps.Fallers.Add(gameObject, Vector2.zero);
      }

      MethodInfo addGravity = typeof(FallerComponents).GetMethod("AddGravity", BindingFlags.Static | BindingFlags.NonPublic);
      addGravity.Invoke(null, new object[] { transform, Vector2.zero });
    }

    public void Sim4000ms(float dt)
    {
      if (Helpers.ShouldFloat(transform))
      {
        AddFaller();
      }
    }
  }
}
