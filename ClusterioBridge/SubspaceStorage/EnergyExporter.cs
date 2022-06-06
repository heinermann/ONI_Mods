namespace Heinermann.ClusterioBridge.SubspaceStorage
{
  public class EnergyExporter : KMonoBehaviour, ISim1000ms
  {
    public void Sim1000ms(float dt)
    {
      var battery = GetComponent<BatterySmart>();
      
      // TODO
      float joulesUploaded = battery.JoulesAvailable;

      // TODO

      battery.ConsumeEnergy(joulesUploaded);
    }
  }
}
