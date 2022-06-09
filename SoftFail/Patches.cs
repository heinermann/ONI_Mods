using HarmonyLib;
using System;
using System.Reflection;

namespace SoftFail
{
  public class Patches
  {
    [HarmonyPatch(typeof(Assembly), "GetTypes", new Type[] { })]
    public class Db_Initialize_Patch
    {
      static void Finalizer(ref Exception __exception, ref Type[] __result)
      {
        if (__exception != null)
        {
          __result = new Type[] { };
        }
        __exception = null;
      }
    }
  }
}
