﻿using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Other/Screen Overlay")]
  public class ScreenOverlay : PostEffectsBase
  {
    public OverlayBlendMode blendMode = OverlayBlendMode.Overlay;
    public float intensity = 1f;
    public Texture2D texture;
    public Shader overlayShader;
    private Material overlayMaterial;

    public override bool CheckResources()
    {
      CheckSupport(false);
      overlayMaterial = CheckShaderAndCreateMaterial(overlayShader, overlayMaterial);
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit(source, destination);
      }
      else
      {
        overlayMaterial.SetVector("_UV_Transform", new Vector4(1f, 0.0f, 0.0f, 1f));
        overlayMaterial.SetFloat("_Intensity", intensity);
        overlayMaterial.SetTexture("_Overlay", texture);
        Graphics.Blit(source, destination, overlayMaterial, (int) blendMode);
      }
    }

    public enum OverlayBlendMode
    {
      Additive,
      ScreenBlend,
      Multiply,
      Overlay,
      AlphaBlend,
    }
  }
}
