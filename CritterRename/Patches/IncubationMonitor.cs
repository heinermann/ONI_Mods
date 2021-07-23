using HarmonyLib;
using Klei.AI;
using System.Diagnostics;
using UnityEngine;

namespace Heinermann.CritterRename.Patches
{
  [HarmonyPatch(typeof(Amount), "Copy")]
  static class Amount_Copy
  {
    static void Prefix(GameObject to, GameObject from)
    {
      string callingMethod = new StackFrame(2).GetMethod().Name;
      if (callingMethod == "SpawnBaby")
      {
        from.GetComponent<CritterName>()?.TransferTo(to.AddOrGet<CritterName>());
      }
    }
  }
}
