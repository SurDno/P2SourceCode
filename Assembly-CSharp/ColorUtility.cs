using UnityEngine;

public static class ColorUtility
{
  public static string ToRGBHex(this Color c)
  {
    return string.Format("#{0:X2}{1:X2}{2:X2}", (object) ColorUtility.ToByte(c.r), (object) ColorUtility.ToByte(c.g), (object) ColorUtility.ToByte(c.b));
  }

  private static byte ToByte(float f)
  {
    f = Mathf.Clamp01(f);
    return (byte) ((double) f * (double) byte.MaxValue);
  }
}
