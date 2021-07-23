using HarmonyLib;

namespace Heinermann.CritterRename.Patches
{
  // This is to fix two bugs:
  // 1. Blank string causes the UI to freeze/mess up
  // 2. Space sets a blank text when I really want it to just reset it
  [HarmonyPatch(typeof(EditableTitleBar), "OnEndEdit")]
  class EditableTitleBar_OnEndEdit
  {
    static void Prefix(ref string finalStr)
    {
      if (string.IsNullOrEmpty(finalStr))
      {
        // Coerce the control into running its proper OnEndEdit code
        finalStr = " ";
      }
    }

    static void Postfix(string finalStr)
    {
      if (finalStr.Trim().Length == 0)
      {
        // Set the title again, but not to `finalStr`
        DetailsScreen.Instance.SetTitle(0);
      }
    }
  }
}
