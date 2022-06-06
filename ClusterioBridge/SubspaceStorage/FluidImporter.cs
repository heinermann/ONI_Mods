namespace Heinermann.ClusterioBridge.SubspaceStorage
{
  public class FluidImporter : KMonoBehaviour, ISim1000ms
  {
    public void Sim1000ms(float dt)
    {
      // TODO

      var storage = GetComponent<Storage>();
      //storage.AddLiquid(element, mass, temperature, disease_id, disease_count);
    }
  }
}
