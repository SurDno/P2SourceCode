﻿namespace UnityStandardAssets.CinematicEffects
{
  public static class ImageEffectHelper
  {
    public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
    {
      if ((Object) s == (Object) null || !s.isSupported)
      {
        Debug.LogWarningFormat("Missing shader for image effect {0}", (object) effect);
        return false;
      }
      if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
      {
        Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", (object) effect);
        return false;
      }
      if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
      {
        Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", (object) effect);
        return false;
      }
      if (!needHdr || SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
        return true;
      Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", (object) effect);
      return false;
    }

    public static Material CheckShaderAndCreateMaterial(Shader s)
    {
      if ((Object) s == (Object) null || !s.isSupported)
        return (Material) null;
      Material material = new Material(s);
      material.hideFlags = HideFlags.DontSave;
      return material;
    }

    public static bool supportsDX11
    {
      get => SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
    }
  }
}
