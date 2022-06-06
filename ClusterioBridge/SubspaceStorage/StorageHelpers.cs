using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Heinermann.ClusterioBridge.SubspaceStorage
{
  public static class StorageHelpers
  {
    public static bool IsSufficientMass(GameObject obj)
    {
      var elem = obj.GetComponent<PrimaryElement>();
      if (elem == null) return false;

      return elem.Units >= 1f;
    }

    public static List<GameObject> GetItemsForExport(Storage storage)
    {
      var result = new List<GameObject>();
      foreach (GameObject itm in storage.items)
      {
        if (IsSufficientMass(itm))
        {
          result.Add(itm);
        }
      }
      return result;
    }

    // TODO: Ignoring disease in prototype
    public static void ExportItems(GameObject storageObj)
    {
      var storage = storageObj.GetComponent<Storage>();
      List<GameObject> toExport = GetItemsForExport(storage);

      List<Item> prepItems = toExport.Select(Item.FromONI).ToList();

      // TODO

      // if success
    }
  }
}
