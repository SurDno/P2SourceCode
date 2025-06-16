// Decompiled with JetBrains decompiler
// Type: TOD_MoonParameters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class TOD_MoonParameters
{
  [Tooltip("Diameter of the moon in degrees.\nThe diameter as seen from earth is 0.5 degrees.")]
  [TOD_Min(0.0f)]
  public float MeshSize = 1f;
  [Tooltip("Brightness of the moon.")]
  [TOD_Min(0.0f)]
  public float MeshBrightness = 2f;
  [Tooltip("Contrast of the moon.")]
  [TOD_Min(0.0f)]
  public float MeshContrast = 1f;
  [Tooltip("Size of the moon halo.")]
  [TOD_Min(0.0f)]
  public float HaloSize = 0.1f;
  [Tooltip("Brightness of the moon halo.")]
  [TOD_Min(0.0f)]
  public float HaloBrightness = 1f;
  [Tooltip("Type of the moon position calculation.")]
  public TOD_MoonPositionType Position = TOD_MoonPositionType.Realistic;
}
