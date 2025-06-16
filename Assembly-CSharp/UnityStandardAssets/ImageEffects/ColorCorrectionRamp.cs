namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Color Adjustments/Color Correction (Ramp)")]
  public class ColorCorrectionRamp : ImageEffectBase
  {
    public Texture textureRamp;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      material.SetTexture("_RampTex", textureRamp);
      Graphics.Blit((Texture) source, destination, material);
    }
  }
}
