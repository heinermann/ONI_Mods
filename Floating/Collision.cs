using UnityEngine;

namespace Heinermann.Floating
{
  public static class Collision
  {
    public static void ApplyBoxColliderRestrictionsX(ref GravityComponent grav, ref Vector2 newPosition)
    {
      KBoxCollider2D collider = grav.transform.GetComponent<KBoxCollider2D>();

      Vector2 oldPosition = grav.transform.position;
      Vector2 diffPosition = newPosition - oldPosition;

      float xDirection = Mathf.Sign(diffPosition.x);

      Bounds bounds = collider.bounds;
      bounds.center += (Vector3)diffPosition;

      Vector2 min = bounds.min;
      Vector2 max = bounds.max;

      float extentX = collider.bounds.extents.x + 0.01f;
      for (
        Vector2 iterator = new Vector2(xDirection < 0 ? min.x : max.x, Mathf.Floor(min.y));
        iterator.y <= max.y;
        iterator.y += 1f)
      {
        if (Helpers.IsSolidCell(iterator))
        {
          grav.velocity.x = 0f;

          float offset = xDirection < 0 ? 1f + extentX : -extentX;
          newPosition.x = Mathf.Floor(iterator.x) + offset;
          break;
        }
      }
    }

    public static void ApplyBoxColliderRestrictionsY(ref GravityComponent grav, ref Vector2 newPosition)
    {
      KBoxCollider2D collider = grav.transform.GetComponent<KBoxCollider2D>();

      Vector2 oldPosition = grav.transform.position;
      Vector2 diffPosition = newPosition - oldPosition;

      float yDirection = Mathf.Sign(diffPosition.y);

      Bounds bounds = collider.bounds;
      bounds.center += (Vector3)diffPosition;

      float extentY = Helpers.GetYExtent(grav);
      float minY = bounds.center.y - extentY;
      float maxY = bounds.center.y + extentY;

      for (
        Vector2 iterator = new Vector2(Mathf.Floor(bounds.min.x), yDirection < 0 ? minY : maxY);
        iterator.x <= bounds.max.x;
        iterator.x += 1f)
      {
        if (Helpers.IsSolidCell(iterator))
        {
          grav.velocity.y = 0f;

          float offset = yDirection < 0 ? 1f + extentY : -extentY;
          newPosition.y = Mathf.Floor(iterator.y) + offset;
          break;
        }
      }
    }

    public static void ApplyBoxColliderRestrictions(ref GravityComponent grav, ref Vector2 newPosition)
    {
      Vector2 proposalA = newPosition;
      ApplyBoxColliderRestrictionsX(ref grav, ref proposalA);
      ApplyBoxColliderRestrictionsY(ref grav, ref proposalA);

      Vector2 proposalB = newPosition;
      ApplyBoxColliderRestrictionsY(ref grav, ref proposalB);
      ApplyBoxColliderRestrictionsX(ref grav, ref proposalB);

      if ((proposalA - newPosition).sqrMagnitude < (proposalB - newPosition).sqrMagnitude)
      {
        newPosition = proposalA;
      }
      else
      {
        newPosition = proposalB;
      }
    }

    public static void ApplyRadiusRestrictionsX(ref GravityComponent grav, ref Vector2 newPosition)
    {
      float radius = Helpers.GetYExtent(grav) + 0.001f;

      Vector2 left = newPosition + Vector2.left * radius;
      Vector2 right = newPosition + Vector2.right * radius;
      if (Helpers.IsSolidCell(left))
      {
        newPosition.x = Mathf.Floor(left.x) + 1f + radius;
        grav.velocity.x = 0f;
      }
      else if (Helpers.IsSolidCell(right))
      {
        newPosition.x = Mathf.Floor(right.x) - radius;
        grav.velocity.x = 0f;
      }
    }

    public static void ApplyRadiusRestrictionsY(ref GravityComponent grav, ref Vector2 newPosition)
    {
      float radius = Helpers.GetYExtent(grav) + 0.001f;

      Vector2 up = newPosition + Vector2.up * radius;
      Vector2 down = newPosition + Vector2.down * radius;
      if (Helpers.IsSolidCell(up))
      {
        Vector2 sourcePos = newPosition;
        newPosition.y = Mathf.Floor(up.y) - radius;
        grav.velocity.y = 0f;
      }
      else if (Helpers.IsSolidCell(down))
      {
        newPosition.y = Mathf.Floor(down.y) + 1f + radius;
        grav.velocity.y = 0f;
      }
    }

    public static void ApplyRadiusRestrictions(ref GravityComponent grav, ref Vector2 newPosition)
    {
      Vector2 proposalA = newPosition;
      ApplyRadiusRestrictionsX(ref grav, ref proposalA);
      ApplyRadiusRestrictionsY(ref grav, ref proposalA);

      Vector2 proposalB = newPosition;
      ApplyRadiusRestrictionsY(ref grav, ref proposalB);
      ApplyRadiusRestrictionsX(ref grav, ref proposalB);

      if ((proposalA - newPosition).sqrMagnitude < (proposalB - newPosition).sqrMagnitude)
      {
        newPosition = proposalA;
      }
      else
      {
        newPosition = proposalB;
      }
    }

    // TODO: Better resolution, since bad axis can trigger and shift things weirdly
    public static void ApplyGuardRails(ref GravityComponent grav, ref Vector2 newPosition)
    {
      if (grav.transform.GetComponent<KBoxCollider2D>() != null)
      {
        ApplyBoxColliderRestrictions(ref grav, ref newPosition);
      }
      else
      {
        ApplyRadiusRestrictions(ref grav, ref newPosition);
      }
    }
  }
}
