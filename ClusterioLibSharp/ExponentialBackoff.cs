using System;

namespace ClusterioLibSharp
{
  public class ExponentialBackoff
  {
    public int Base { get; set; } = 1;
    public int Max { get; set; } = 60;
    public int? Reset { get; set; } = null;

    double exp = 0;
    DateTime lastInvocationTime = DateTime.UtcNow;
    Random rand = new Random();

    public int Delay()
    {
      if (!Reset.HasValue)
      {
        Reset = 2 * Max;
      }

      DateTime invocationTime = DateTime.UtcNow;
      int interval = (invocationTime - lastInvocationTime).Milliseconds / 1000;
      lastInvocationTime = invocationTime;

      if (interval > Reset)
      {
        exp = 0;
      }

      exp = Math.Min(exp + 1, Math.Log(Max, 2.0));
      return (int)(Math.Pow(rand.NextDouble() * Base * 2, exp) * 1000);
    }
  }
}
