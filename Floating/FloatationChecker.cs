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
