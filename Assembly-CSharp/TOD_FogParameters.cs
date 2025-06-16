// Decompiled with JetBrains decompiler
// Type: TOD_FogParameters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class TOD_FogParameters
{
  [Tooltip("Fog color mode.")]
  public TOD_FogType Mode = TOD_FogType.Color;
  [Tooltip("Fog color sampling height.\n = 0 fog is atmosphere color at horizon.\n = 1 fog is atmosphere color at zenith.")]
  [TOD_Range(0.0f, 1f)]
  public float HeightBias = 0.0f;
  public float StartDistance = 0.0f;
  public float GlobalDensity = 1f / 1000f;
  public float TransparentDensityMultiplier = 5f;
}
