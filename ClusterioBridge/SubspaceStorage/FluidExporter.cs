namespace Heinermann.ClusterioBridge.SubspaceStorage
{
  public class FluidExporter : KMonoBehaviour, ISim1000ms
  {
    public void Sim1000ms(float dt)
    {
      StorageHelpers.ExportItems(gameObject);
    }
  }
}
