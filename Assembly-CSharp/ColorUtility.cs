// Decompiled with JetBrains decompiler
// Type: ColorUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
