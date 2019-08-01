using Harmony;
using UnityEngine;

namespace Heinermann.ShadowMinion.Patches
{
  [HarmonyPatch(typeof(SandboxSpawnerTool), "SpawnMinion")]
  class SandboxSpawnerTool_SpawnMinion
  {
    static bool Prefix(int ___currentCell)
    {
      Debug.LogWarning("SHADOW");
      GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(ShadowMinionConfig.ID));
      gameObject.name = Assets.GetPrefab(ShadowMinionConfig.ID).name;

      Vector3 position = Grid.CellToPosCBC(___currentCell, Grid.SceneLayer.Move);
      gameObject.transform.SetLocalPosition(position);
      gameObject.SetActive(true);

      new ShadowMinionStartingStats().Apply(gameObject);

      return false;
    }
  }
}
