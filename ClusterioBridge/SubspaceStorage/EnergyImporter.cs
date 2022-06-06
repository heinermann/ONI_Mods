namespace Heinermann.ClusterioBridge.SubspaceStorage
{
  public class EnergyImporter : KMonoBehaviour, ISim1000ms
  {
    public void Sim1000ms(float dt)
    {
      var battery = GetComponent<BatterySmart>();

      // battery.capacity // Joules

      // TODO
      float joulesAdded = 0;

      battery.AddEnergy(joulesAdded);
    }
  }
}
