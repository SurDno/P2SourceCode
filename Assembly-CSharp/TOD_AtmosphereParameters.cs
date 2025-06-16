// Decompiled with JetBrains decompiler
// Type: TOD_AtmosphereParameters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class TOD_AtmosphereParameters
{
  [Tooltip("Intensity of the atmospheric Rayleigh scattering.")]
  [TOD_Min(0.0f)]
  public float RayleighMultiplier = 1f;
  [Tooltip("Intensity of the atmospheric Mie scattering.")]
  [TOD_Min(0.0f)]
  public float MieMultiplier = 1f;
  [Tooltip("Overall brightness of the atmosphere.")]
  [TOD_Min(0.0f)]
  public float Brightness = 1.5f;
  [Tooltip("Overall contrast of the atmosphere.")]
  [TOD_Min(0.0f)]
  public float Contrast = 1.5f;
  [Tooltip("Directionality factor that determines the size of the glow around the sun.")]
  [TOD_Range(0.0f, 1f)]
  public float Directionality = 0.7f;
  [Tooltip("Density of the fog covering the sky.")]
  [TOD_Range(0.0f, 1f)]
  public float Fogginess = 0.0f;
}
