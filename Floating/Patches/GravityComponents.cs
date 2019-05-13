using Harmony;
using UnityEngine;

namespace Heinermann.Floating.Patches
{
  [HarmonyPatch(typeof(GravityComponents), "FixedUpdate")]
  class GravityComponents_FixedUpdate
  {
    private static float randomXVelocity(float sign = 0f)
    {
      if (sign == 0f)
      {
        sign = Mathf.Round(Random.Range(0f, 1f)) * 2 - 1;
      }
      return Random.Range(3f, 6f) * sign;
    }

    // TODO: Check for potential bug with 1-tile width water
    // TODO: Use the width from KCollider2D instead of gravity's radius
    static void Prefix(ref GravityComponents __instance, float dt)
    {
      GravityComponents.Tuning tuning = TuningData<GravityComponents.Tuning>.Get();
      float maxSqMagnitude = tuning.maxVelocity * tuning.maxVelocity;

      var data = __instance.GetDataList();
      for (int i = 0; i < data.Count; i++)
      {
        GravityComponent grav = data[i];

        // Requirements
        if (!Helpers.ShouldFloat(grav.transform)) continue;

        Vector2 position = grav.transform.GetPosition();

        // Calculating the new velocities (compensate for the fact that additional gravity will be applied)
        float reverseGravityConstant = Helpers.IsSurfaceLiquid(position) && grav.velocity.y > 0f ? 0f : 9.8f / 2f;
        Vector2 reversedChange = new Vector2(grav.velocity.x, grav.velocity.y + 9.8f * dt);
        Vector2 newChange = new Vector2(reversedChange.x, reversedChange.y + reverseGravityConstant * dt);

        // X-velocity roaming
        if (Mathf.Abs(grav.velocity.x) < 0.2f * dt)
        {
          newChange.x += randomXVelocity() * dt;
        }
        else
        {
          float direction = grav.velocity.x / Mathf.Abs(grav.velocity.x);
          Vector2 expectedPosition = position + newChange * dt;

          Vector2 wallCheckPositionA = expectedPosition;
          wallCheckPositionA.x += grav.radius * 1.5f * direction;
          wallCheckPositionA.y = expectedPosition.y + grav.radius;

          Vector2 wallCheckPositionB = wallCheckPositionA;
          wallCheckPositionB.y = expectedPosition.y - grav.radius;

          if (Helpers.IsSolidCell(wallCheckPositionA) || Helpers.IsSolidCell(wallCheckPositionB))
          {
            newChange.x = randomXVelocity(-direction) * dt;
          }
        }

        // Limit stuff, copied from Game's implementation
        float targetSqMagnitude = (newChange - reversedChange).sqrMagnitude;
        if (targetSqMagnitude > maxSqMagnitude)
        {
          newChange = reversedChange + (newChange - reversedChange) * tuning.maxVelocity / Mathf.Sqrt(targetSqMagnitude);
        }

        grav.elapsedTime = dt; // Prevent sticking to the floor
        grav.velocity = newChange;

        data[i] = grav;
      }
    }
  }
}
