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

    /**
     * Gets the element of a cell at the given position.
     */
    public static Element GetElement(Vector2 pos)
    {
      int cell = PosToCell(pos);
      return Grid.IsValidCell(cell) ? Grid.Element[cell] : null;
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
      return GetMass(pos) / 1000f > positionInCell;
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

    private static readonly Tag[] nonFloatableTags = new Tag[]{
      GameTags.Stored,
      GameTags.StoredPrivate,
      GameTags.Creature,
      GameTags.SwimmingCreature,
      GameTags.Egg,
      GameTags.IncubatableEgg,
      GameTags.Entombed,
      GameTags.Sealed,
      GameTags.Equipped
    };

    /**
     * Checks if the object meets the tag requirements for floatation.
     */
    public static bool HasFloatableTags(Transform obj)
    {
      KPrefabID prefab = obj?.GetComponent<KPrefabID>();
      if (prefab == null) return false;

      if (prefab.HasAnyTags(nonFloatableTags) || !prefab.HasTag(GameTags.Pickupable)) return false;

      if (prefab.HasTag(GameTags.BaseMinion) && !prefab.HasTag(GameTags.Corpse)) return false;

      return true;
    }

    public static float GetYExtent(GravityComponent component)
    {
      return Mathf.Max(0.2f, component.extents.y);
    }

    /**
     * Checks if this object should float, based on whether it is in liquid and the
     * element's mass max is lower than the liquid's mass max.
     */
    public static bool ShouldFloat(Transform transform)
    {
      // Must have valid tags
      if (transform == null || !HasFloatableTags(transform)) return false;

      // Must be in liquid
      if (!IsVisiblyInLiquid(transform.GetPosition())) return false;

      // Bottled liquids and gasses always float
      Element floaterElem = transform.GetComponent<PrimaryElement>()?.Element;
      if (floaterElem == null) return false;
      if (floaterElem.IsLiquid || floaterElem.IsGas) return true;

      // Must have less MaxMass than the liquid's MaxMass
      Element liquidElem = GetElement(transform.GetPosition());
      float floaterMaxMass = Mathf.Max(floaterElem.defaultValues.mass, floaterElem.maxMass);
      if (liquidElem == null || floaterMaxMass >= liquidElem.maxMass) return false;

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
