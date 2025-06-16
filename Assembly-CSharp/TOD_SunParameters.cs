// Decompiled with JetBrains decompiler
// Type: TOD_SunParameters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class TOD_SunParameters
{
  [Tooltip("Diameter of the sun in degrees.\nThe diameter as seen from earth is 0.5 degrees.")]
  [TOD_Min(0.0f)]
  public float MeshSize = 1f;
  [Tooltip("Brightness of the sun.")]
  [TOD_Min(0.0f)]
  public float MeshBrightness = 2f;
  [Tooltip("Contrast of the sun.")]
  [TOD_Min(0.0f)]
  public float MeshContrast = 1f;
}
