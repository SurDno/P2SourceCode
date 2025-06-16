// Decompiled with JetBrains decompiler
// Type: SRF.SRFFloatExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
