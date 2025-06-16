using UnityEngine;

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
