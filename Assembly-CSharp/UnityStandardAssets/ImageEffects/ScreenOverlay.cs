// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.ImageEffects.ScreenOverlay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Other/Screen Overlay")]
  public class ScreenOverlay : PostEffectsBase
  {
    public ScreenOverlay.OverlayBlendMode blendMode = ScreenOverlay.OverlayBlendMode.Overlay;
    public float intensity = 1f;
    public Texture2D texture = (Texture2D) null;
    public Shader overlayShader = (Shader) null;
    private Material overlayMaterial = (Material) null;

    public override bool CheckResources()
    {
      this.CheckSupport(false);
      this.overlayMaterial = this.CheckShaderAndCreateMaterial(this.overlayShader, this.overlayMaterial);
      if (!this.isSupported)
        this.ReportAutoDisable();
      return this.isSupported;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!this.CheckResources())
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        this.overlayMaterial.SetVector("_UV_Transform", new Vector4(1f, 0.0f, 0.0f, 1f));
        this.overlayMaterial.SetFloat("_Intensity", this.intensity);
        this.overlayMaterial.SetTexture("_Overlay", (Texture) this.texture);
        Graphics.Blit((Texture) source, destination, this.overlayMaterial, (int) this.blendMode);
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
