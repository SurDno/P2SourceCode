// Decompiled with JetBrains decompiler
// Type: TOD_LightParameters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class TOD_LightParameters
{
  [Tooltip("Refresh interval of the light source position in seconds.")]
  [TOD_Min(0.0f)]
  public float UpdateInterval = 0.0f;
  [Tooltip("Controls how low the light source is allowed to go.\n = -1 light source can go as low as it wants.\n = 0 light source will never go below the horizon.\n = +1 light source will never leave zenith.")]
  [TOD_Range(-1f, 1f)]
  public float MinimumHeight = 0.0f;
}
