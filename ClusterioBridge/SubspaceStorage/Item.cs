using UnityEngine;

namespace Heinermann.ClusterioBridge.SubspaceStorage
{
  public class Item
  {
    public string Name { get; set; }
    public int Count { get; set; }

    public static Item FromONI(GameObject obj)
    {
      var elem = obj.GetComponent<PrimaryElement>();
      var prefabId = obj.GetComponent<KPrefabID>();

      string itemName;
      if (prefabId != null)
      {
        itemName = prefabId.PrefabTag.Name;
      }
      else
      {
        itemName = elem.Element.name;
      }

      return new Item()
      {
        Name = itemName,
        Count = (int)elem.Units
      };
    }
  }
}
