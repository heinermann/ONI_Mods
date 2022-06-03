using HarmonyLib;

namespace Heinermann.ResetBuildPriority.Patches
{
  public static class PlanScreenState
  {
    private static bool ignoreCategoryClose = false;

    [HarmonyPatch(typeof(PlanScreen), "CloseCategoryPanel")]
    public static class PlanScreen_CloseCategoryPanel
    {
      static void Prefix(PlanScreen __instance, bool playSound)
      {
        if (!ignoreCategoryClose || playSound)
        {
          __instance.ProductInfoScreen?.materialSelectionPanel?.PriorityScreen?.ResetPriority();
        }
      }
    }

    [HarmonyPatch(typeof(PlanScreen), "OnClickCategory")]
    public static class PlanScreen_OnClickCategory
    {
      static void Prefix()
      {
        ignoreCategoryClose = true;
      }
      static void Postfix()
      {
        ignoreCategoryClose = false;
      }
    }
  }
}
