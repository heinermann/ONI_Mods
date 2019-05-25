using UnityEngine;

namespace Heinermann.Floating
{
  public static class Helpers
  {
    /**
     * Gets the mass of the cell at the given position.
     */
    public static float GetMass(Vector2 pos)
    {
      int cell = PosToCell(pos);
      return Grid.IsValidCell(cell) ? Grid.Mass[cell] : float.PositiveInfinity;
    }

    public static bool IsSurfaceLiquid(Vector2 pos)
    {
      return IsVisiblyInLiquid(pos) && !IsVisiblyInLiquid(pos + Vector2.up);
    }

    /**
     * Copy of original with bug fix
     */
    public static bool IsVisiblyInLiquid(Vector2 pos)
    {
      if (!IsValidLiquidCell(pos)) return false;

      if (IsValidLiquidCell(pos + Vector2.up))
      {
        return true;
      }

      float positionInCell = pos.y - Mathf.Floor(pos.y);
      if (GetMass(pos) / 1000f > positionInCell)
      {
        return true;
      }
      return false;
    }

    public static int PosToCell(Vector2 pos)
    {
      pos.y -= 0.05f; // Compensation for PosToCell adding 0.05f
      return Grid.PosToCell(pos);
    }

    /**
     * Checks if the cell at the position is solid.
     */
    public static bool IsSolidCell(Vector2 pos)
    {
      int cell = PosToCell(pos);
      return Grid.IsSolidCell(cell);
    }
    
    /**
     * Checks if the object meets the tag requirements for floatation.
     */
    public static bool HasFloatableTags(Transform obj)
    {
      if (obj?.GetComponent<KPrefabID>() == null) return false;

      if (!obj.HasTag(GameTags.Pickupable)) return false;

      if (obj.HasTag(GameTags.Minion) && !obj.HasTag(GameTags.Corpse)) return false;

      if (obj.HasTag(GameTags.Creature)) return false;

      return true;
    }

    public static float GetYExtent(GravityComponent component)
    {
      return Mathf.Max(0.05f, component.radius);
    }

    /**
     * Checks if this object should float, based on whether it is in liquid and the
     * element mass is low enough.
     */
    public static bool ShouldFloat(GravityComponent component)
    {
      if (!ShouldFloatLite(component.transform)) return false;

      Vector2 position = (Vector2)component.transform.GetPosition() + Vector2.up * GetYExtent(component);
      if (!IsVisiblyInLiquid(position)) return false;

      return true;
    }

    public static bool ShouldFloatLite(Transform transform)
    {
      if (transform == null || !HasFloatableTags(transform)) return false;

      PrimaryElement element = transform.GetComponent<PrimaryElement>();
      if (element == null || element.Mass >= 250f) return false;

      if (!IsVisiblyInLiquid(transform.GetPosition())) return false;

      return true;
    }

    public static bool IsValidLiquidCell(int cell)
    {
      return Grid.IsValidCell(cell) && Grid.IsLiquid(cell);
    }
    public static bool IsValidLiquidCell(Vector2 pos)
    {
      return IsValidLiquidCell(PosToCell(pos));
    }

    /**
     * Gets the nearest liquid surface Y position from the given position.
     */
    public static float GetLiquidSurfaceDistanceAbove(Vector2 initialPos)
    {
      if (!IsValidLiquidCell(initialPos)) return float.PositiveInfinity;

      Vector2 pos = initialPos;
      while (IsValidLiquidCell(pos + Vector2.up))
      {
        pos += Vector2.up;
      }

      pos.y += (GetMass(pos) / 1000f) - (pos.y - Mathf.Floor(pos.y));
      return pos.y - initialPos.y;
    }
  }
}
