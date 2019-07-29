using Harmony;

namespace Heinermann.DisableSplashScreen.Patches
{
  [HarmonyPatch(typeof(SplashMessageScreen), "OnSpawn")]
  class SplashMessageScreen_OnSpawn
  {
    static bool Prefix(ref SplashMessageScreen __instance)
    {
      __instance.gameObject.SetActive(false);
      return false;
    }
  }
}
