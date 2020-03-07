namespace Heinermann.ShadowMinion
{
  public class NullGasBreather : OxygenBreather.IGasProvider
  {
    public bool ConsumeGas(OxygenBreather oxygen_breather, float amount)
    {
      return true;
    }

    public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
    {
    }

    public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
    {
    }

    public bool ShouldEmitCO2()
    {
      return true;
    }

    public bool ShouldStoreCO2()
    {
      return false;
    }
  }
}
