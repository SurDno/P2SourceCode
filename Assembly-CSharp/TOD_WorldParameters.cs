// Decompiled with JetBrains decompiler
// Type: TOD_WorldParameters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class TOD_WorldParameters
{
  [Tooltip("Latitude of the current location in degrees.")]
  [Range(-90f, 90f)]
  public float Latitude = 0.0f;
  [Tooltip("Longitude of the current location in degrees.")]
  [Range(-180f, 180f)]
  public float Longitude = 0.0f;
  [Tooltip("UTC/GMT time zone of the current location in hours.")]
  [Range(-14f, 14f)]
  public float UTC = 0.0f;
}
