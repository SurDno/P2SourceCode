namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Other/Screen Overlay")]
  public class ScreenOverlay : PostEffectsBase
  {
    public OverlayBlendMode blendMode = OverlayBlendMode.Overlay;
    public float intensity = 1f;
    public Texture2D texture = (Texture2D) null;
    public Shader overlayShader = (Shader) null;
    private Material overlayMaterial = (Material) null;

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
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        overlayMaterial.SetVector("_UV_Transform", new Vector4(1f, 0.0f, 0.0f, 1f));
        overlayMaterial.SetFloat("_Intensity", intensity);
        overlayMaterial.SetTexture("_Overlay", (Texture) texture);
        Graphics.Blit((Texture) source, destination, overlayMaterial, (int) blendMode);
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
