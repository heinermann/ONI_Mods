
using HarmonyLib;

namespace Heinermann.PerfOptimizations
{
  /*
  [HarmonyPatch(typeof(Sensors), "UpdateSensors")]
  class Sensors_UpdateSensors
  {
    static bool Prefix(Sensors __instance)
    {
      //Debug.LogWarning("============================================================");
      foreach (Sensor sensor in __instance.sensors)
      {
        //Debug.LogWarning(sensor.Name);
        sensor.Update();
      }
      return false;
    }
  }*/
}
