// Decompiled with JetBrains decompiler
// Type: TOD_CloudParameters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class TOD_CloudParameters
{
  [Tooltip("Size of the clouds.")]
  [TOD_Min(1f)]
  public float Size = 2f;
  [Tooltip("Opacity of the clouds.")]
  [TOD_Range(0.0f, 1f)]
  public float Opacity = 1f;
  [Tooltip("How much sky is covered by clouds.")]
  [TOD_Range(0.0f, 1f)]
  public float Coverage = 0.3f;
  [Tooltip("Sharpness of the cloud to sky transition.")]
  [TOD_Range(0.0f, 1f)]
  public float Sharpness = 0.5f;
  [Tooltip("Amount of skylight that is blocked.")]
  [TOD_Range(0.0f, 1f)]
  public float Attenuation = 0.5f;
  [Tooltip("Amount of sunlight that is blocked.\nOnly affects the highest cloud quality setting.")]
  [TOD_Range(0.0f, 1f)]
  public float Saturation = 0.5f;
  [Tooltip("Intensity of the cloud translucency glow.\nOnly affects the highest cloud quality setting.")]
  [TOD_Min(0.0f)]
  public float Scattering = 1f;
  [Tooltip("Brightness of the clouds.")]
  [TOD_Min(0.0f)]
  public float Brightness = 1.5f;
}
