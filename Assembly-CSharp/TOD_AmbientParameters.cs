// Decompiled with JetBrains decompiler
// Type: TOD_AmbientParameters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class TOD_AmbientParameters
{
  [Tooltip("Ambient light mode.")]
  public TOD_AmbientType Mode = TOD_AmbientType.Color;
  [Tooltip("Saturation of the ambient light.")]
  [TOD_Min(0.0f)]
  public float Saturation = 1f;
  [Tooltip("Refresh interval of the ambient light probe in seconds.")]
  [TOD_Min(0.0f)]
  public float UpdateInterval = 1f;
}
