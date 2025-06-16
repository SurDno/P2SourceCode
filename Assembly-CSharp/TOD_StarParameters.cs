// Decompiled with JetBrains decompiler
// Type: TOD_StarParameters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class TOD_StarParameters
{
  [Tooltip("Size of the stars.")]
  [TOD_Min(0.0f)]
  public float Size = 1f;
  [Tooltip("Brightness of the stars.")]
  [TOD_Min(0.0f)]
  public float Brightness = 1f;
  [Tooltip("Type of the stars position calculation.")]
  public TOD_StarsPositionType Position = TOD_StarsPositionType.Rotating;
}
