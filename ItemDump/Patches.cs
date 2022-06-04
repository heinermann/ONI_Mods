using HarmonyLib;
using Sky.Data.Csv;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static EntityTemplates;

namespace ItemDump
{
  public class ItemInfo
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
  }

  public class Patches
  {
    public static List<ItemInfo> items = new List<ItemInfo>();

    [HarmonyPatch(typeof(EntityTemplates), "CreateLooseEntity")]
    public class EntityTemplates_CreateLooseEntity_Patch
    {
      public static void Prefix(string id, string name, string desc, float mass, bool unitMass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, CollisionShape collisionShape, float width, float height, bool isPickupable, int sortOrder, SimHashes element, List<Tag> additionalTags)
      {
        items.Add(new ItemInfo()
        {
          Id = id,
          Name = name,
          Desc = desc
        });
      }
    }

    [HarmonyPatch(typeof(Sim), "Start")]
    public class Sim_Start_Patch
    {
      private static string stripLinks(string s)
      {
        if (s == null) return null;
        return Regex.Replace(s, @"\<[^>]+\>", "");
      }

      public static void Prefix()
      {
        File.Delete("./oni_items.csv");
        using (var csv = CsvWriter.Create("./oni_items.csv"))
        {
          csv.WriteRow("id", "name", "description");
          foreach (ItemInfo itm in items)
          {
            csv.WriteRow(stripLinks(itm.Id), stripLinks(itm.Name), stripLinks(itm.Desc));
          }
        }
      }
    }
  }
}
