namespace Heinermann.ClusterioBridge.SubspaceStorage
{
  public class GasImporter : KMonoBehaviour, ISim1000ms
  {
    public void Sim1000ms(float dt)
    {
      // TODO

      var storage = GetComponent<Storage>();
      //storage.AddGasChunk(element, mass, temperature, disease_id, disease_count, false);
    }
  }
}
