using Harmony;
using System.Collections.Generic;
using UnityEngine;

namespace Heinermann.Floating.Patches
{
  [HarmonyPatch(typeof(GravityComponents), "FixedUpdate")]
  class GravityComponents_FixedUpdate
  {
    private const float REVERSE_GRAVITY = 9.8f;

    private const float MIN_X_VELOCITY = 2.5f;
    private const float MAX_X_VELOCITY = 5f;

    private static readonly GravityComponent NULL_COMPONENT = new GravityComponent
    {
      elapsedTime = -1,
      landOnFakeFloors = false,
      onLanded = null,
      radius = 0,
      transform = null,
      velocity = Vector2.zero
    };

    private static float RandomXVelocity()
    {
      float sign = Mathf.Round(UnityEngine.Random.Range(0f, 1f)) * 2 - 1;
      return UnityEngine.Random.Range(MIN_X_VELOCITY, MAX_X_VELOCITY) * sign;
    }

    /**
     * X roaming
     */
     // TODO: Bias based on liquid mass in right/left cells
    private static void ApplyXVelocityChanges(ref GravityComponent grav, float dt)
    {
      Vector2 position = grav.transform.GetPosition();
      Vector2 newChange = new Vector2(grav.velocity.x, grav.velocity.y);

      if (Mathf.Abs(grav.velocity.x) < MIN_X_VELOCITY / 2 * dt)
      {
        newChange.x += RandomXVelocity() * dt;
      }
      grav.velocity = newChange;
    }

    /**
     * Reverse gravity application (floatation)
     */
    private static void ApplyYVelocityChanges(ref GravityComponent grav, float dt)
    {
      GravityComponents.Tuning tuning = TuningData<GravityComponents.Tuning>.Get();

      float yExtent = Helpers.GetYExtent(grav);
      Vector2 position = (Vector2)grav.transform.GetPosition() + Vector2.up * (yExtent + 0.01f);

      float distanceToSurface = Helpers.GetLiquidSurfaceDistanceAbove(position);
      if (distanceToSurface != float.PositiveInfinity && distanceToSurface > 2 * yExtent)
      {
        Vector2 target = position + Vector2.up * distanceToSurface;
        Mathf.SmoothDamp(position.y, target.y, ref grav.velocity.y, 1f, tuning.maxVelocityInLiquid, dt);
      }
      else if (grav.velocity.y > 0)
      {
        Mathf.SmoothDamp(position.y, position.y, ref grav.velocity.y, 5f, tuning.maxVelocityInLiquid, dt);
      }
    }

    // TODO: Check for potential bug with 1-tile width water
    static void Prefix(ref GravityComponents __instance, ref Dictionary<int, GravityComponent> __state, float dt)
    {
      __state = new Dictionary<int, GravityComponent>();

      var data = __instance.GetDataList();
      for (int i = 0; i < data.Count; i++)
      {
        GravityComponent grav = data[i];
        if (!Helpers.ShouldFloat(grav)) continue;

        ApplyXVelocityChanges(ref grav, dt);
        ApplyYVelocityChanges(ref grav, dt);

        Vector3 pos = grav.transform.GetPosition();
        Vector2 newPosition = (Vector2)pos + grav.velocity * dt;

        Collision.ApplyGuardRails(ref grav, ref newPosition);
        grav.transform.SetPosition(new Vector3(newPosition.x, newPosition.y, pos.z));

        grav.elapsedTime += dt;

        __state.Add(i, grav);
        data[i] = NULL_COMPONENT;
      }
    }

    static void Postfix(ref GravityComponents __instance, ref Dictionary<int, GravityComponent> __state)
    {
      var data = __instance.GetDataList();
      foreach (KeyValuePair<int, GravityComponent> newGrav in __state)
      {
        data[newGrav.Key] = newGrav.Value;
      }
    }
  }
}
