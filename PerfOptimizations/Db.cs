using HarmonyLib;
using UnityEngine;

namespace Heinermann.PerfOptimizations
{
  [HarmonyPatch(typeof(Db), "Get")]
  class Db_Get
  {
    static bool Prefix(ref Db __result, ref Db ____Instance)
    {
      if (____Instance is null)
      {
        ____Instance = Resources.Load<Db>("Db");
        ____Instance.Initialize();
      }
      __result = ____Instance;
      return false;
    }
  }
}
