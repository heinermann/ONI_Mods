using UnityEngine;

namespace Heinermann.Floating
{
  public static class Helpers
  {
    public static float GetMass(Vector2 pos)
    {
      int cell = Grid.PosToCell(pos);
      if (!Grid.IsValidCell(cell)) return 0f;

      return Grid.Mass[cell];
    }

    public static bool IsSurfaceLiquid(Vector2 pos)
    {
      int cellAbove = Grid.CellAbove(Grid.PosToCell(pos));
      return Grid.IsVisiblyInLiquid(pos) && Grid.IsValidCell(cellAbove) && !Grid.IsVisiblyInLiquid(Grid.CellToPos2D(cellAbove));
    }

    public static bool IsSolidCell(Vector2 pos)
    {
      int cell = Grid.PosToCell(pos);
      return Grid.IsSolidCell(cell);
    }

    public static bool HasFloatableTags(Transform obj)
    {
      if (obj?.GetComponent<KPrefabID>() == null) return false;

      if (!obj.HasTag(GameTags.Pickupable)) return false;

      if (obj.HasTag(GameTags.Minion) && !obj.HasTag(GameTags.Corpse)) return false;

      if (obj.HasTag(GameTags.Creature)) return false;

      return true;
    }

    public static bool ShouldFloat(Transform obj)
    {
      if (obj == null) return false;

      if (!HasFloatableTags(obj)) return false;

      PrimaryElement element = obj.GetComponent<PrimaryElement>();

      if (element == null) return false;

      Vector2 position = obj.GetPosition();
      if (!Grid.IsVisiblyInLiquid(position) || GetMass(position) / 4f < element.Mass)
      {
        return false;
      }

      return true;
    }
  }
}
