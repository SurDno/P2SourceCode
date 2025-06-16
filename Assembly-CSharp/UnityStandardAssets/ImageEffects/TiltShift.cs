using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Camera/Tilt Shift (Lens Blur)")]
  internal class TiltShift : PostEffectsBase
  {
    public TiltShift.TiltShiftMode mode = TiltShift.TiltShiftMode.TiltShiftMode;
    public TiltShift.TiltShiftQuality quality = TiltShift.TiltShiftQuality.Normal;
    [Range(0.0f, 15f)]
    public float blurArea = 1f;
    [Range(0.0f, 25f)]
    public float maxBlurSize = 5f;
    [Range(0.0f, 1f)]
    public int downsample = 0;
    public Shader tiltShiftShader = (Shader) null;
    private Material tiltShiftMaterial = (Material) null;

    public override bool CheckResources()
    {
      this.CheckSupport(true);
      this.tiltShiftMaterial = this.CheckShaderAndCreateMaterial(this.tiltShiftShader, this.tiltShiftMaterial);
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
        this.tiltShiftMaterial.SetFloat("_BlurSize", (double) this.maxBlurSize < 0.0 ? 0.0f : this.maxBlurSize);
        this.tiltShiftMaterial.SetFloat("_BlurArea", this.blurArea);
        source.filterMode = FilterMode.Bilinear;
        RenderTexture renderTexture = destination;
        if ((double) this.downsample > 0.0)
        {
          renderTexture = RenderTexture.GetTemporary(source.width >> this.downsample, source.height >> this.downsample, 0, source.format);
          renderTexture.filterMode = FilterMode.Bilinear;
        }
        int num = (int) this.quality * 2;
        Graphics.Blit((Texture) source, renderTexture, this.tiltShiftMaterial, this.mode == TiltShift.TiltShiftMode.TiltShiftMode ? num : num + 1);
        if (this.downsample > 0)
        {
          this.tiltShiftMaterial.SetTexture("_Blurred", (Texture) renderTexture);
          Graphics.Blit((Texture) source, destination, this.tiltShiftMaterial, 6);
        }
        if (!((Object) renderTexture != (Object) destination))
          return;
        RenderTexture.ReleaseTemporary(renderTexture);
      }
    }

    public enum TiltShiftMode
    {
      TiltShiftMode,
      IrisMode,
    }

    public enum TiltShiftQuality
    {
      Preview,
      Normal,
      High,
    }
  }
}
