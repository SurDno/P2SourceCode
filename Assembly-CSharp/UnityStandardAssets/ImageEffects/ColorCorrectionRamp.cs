using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Color Adjustments/Color Correction (Ramp)")]
  public class ColorCorrectionRamp : ImageEffectBase
  {
    public Texture textureRamp;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      this.material.SetTexture("_RampTex", this.textureRamp);
      Graphics.Blit((Texture) source, destination, this.material);
    }
  }
}
