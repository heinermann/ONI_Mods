using Harmony;
using System;

namespace Heinermann.CritterTraits.Patches
{
  [HarmonyPatch(typeof(Debug), "Break")]
  class Debug_Break
  {
    static void Prefix()
    {
      Console.WriteLine(Environment.StackTrace);
    }
  }

  [HarmonyPatch(typeof(Debug), "Assert", new Type[] { typeof(bool) })]
  class Debug_Assert
  {
    static void Prefix(bool condition)
    {
      if (!condition)
      {
        Console.WriteLine(Environment.StackTrace);
      }
    }
  }
}
