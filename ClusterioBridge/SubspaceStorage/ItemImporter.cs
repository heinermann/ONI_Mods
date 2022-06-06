namespace Heinermann.ClusterioBridge.SubspaceStorage
{
  public class ItemImporter : KMonoBehaviour, ISim1000ms
  {
    public void Sim1000ms(float dt)
    {
      // TODO

      var storage = GetComponent<Storage>();
      
      // if incoming item is an ore
      //    storage.AddOre(element, mass, temp, disease, disease_count)
      // else
      //    create new object from prefab
      //    storage.Store(item);  // Note: consider also the arguments for this
    }
  }
}
