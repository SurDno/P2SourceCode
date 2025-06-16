using UnityEngine;

namespace SRF
{
  public static class SRFFloatExtensions
  {
    public static float Sqr(this float f) => f * f;

    public static float SqrRt(this float f) => Mathf.Sqrt(f);

    public static bool ApproxZero(this float f) => Mathf.Approximately(0.0f, f);

    public static bool Approx(this float f, float f2) => Mathf.Approximately(f, f2);
  }
}
