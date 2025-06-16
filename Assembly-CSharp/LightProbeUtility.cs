// Decompiled with JetBrains decompiler
// Type: LightProbeUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
public static class LightProbeUtility
{
  private static int[] _idSHA = new int[3]
  {
    Shader.PropertyToID("unity_SHAr"),
    Shader.PropertyToID("unity_SHAg"),
    Shader.PropertyToID("unity_SHAb")
  };
  private static int[] _idSHB = new int[3]
  {
    Shader.PropertyToID("unity_SHBr"),
    Shader.PropertyToID("unity_SHBg"),
    Shader.PropertyToID("unity_SHBb")
  };
  private static int _idSHC = Shader.PropertyToID("unity_SHC");

  public static void SetSHCoefficients(
    Vector3 position,
    MaterialPropertyBlock properties,
    Renderer renderer = null)
  {
    SphericalHarmonicsL2 probe;
    LightProbes.GetInterpolatedProbe(position, renderer, out probe);
    for (int rgb = 0; rgb < 3; ++rgb)
      properties.SetVector(LightProbeUtility._idSHA[rgb], new Vector4(probe[rgb, 3], probe[rgb, 1], probe[rgb, 2], probe[rgb, 0] - probe[rgb, 6]));
    for (int rgb = 0; rgb < 3; ++rgb)
      properties.SetVector(LightProbeUtility._idSHB[rgb], new Vector4(probe[rgb, 4], probe[rgb, 6], probe[rgb, 5] * 3f, probe[rgb, 7]));
    properties.SetVector(LightProbeUtility._idSHC, new Vector4(probe[0, 8], probe[2, 8], probe[1, 8], 1f));
  }

  public static void SetSHCoefficients(Vector3 position, Material material, Renderer renderer = null)
  {
    SphericalHarmonicsL2 probe;
    LightProbes.GetInterpolatedProbe(position, renderer, out probe);
    for (int rgb = 0; rgb < 3; ++rgb)
      material.SetVector(LightProbeUtility._idSHA[rgb], new Vector4(probe[rgb, 3], probe[rgb, 1], probe[rgb, 2], probe[rgb, 0] - probe[rgb, 6]));
    for (int rgb = 0; rgb < 3; ++rgb)
      material.SetVector(LightProbeUtility._idSHB[rgb], new Vector4(probe[rgb, 4], probe[rgb, 6], probe[rgb, 5] * 3f, probe[rgb, 7]));
    material.SetVector(LightProbeUtility._idSHC, new Vector4(probe[0, 8], probe[2, 8], probe[1, 8], 1f));
  }
}
