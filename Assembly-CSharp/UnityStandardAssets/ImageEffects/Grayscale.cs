using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Color Adjustments/Grayscale")]
  public class Grayscale : ImageEffectBase
  {
    public Texture textureRamp;
    [Range(-1f, 1f)]
    public float rampOffset;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      this.material.SetTexture("_RampTex", this.textureRamp);
      this.material.SetFloat("_RampOffset", this.rampOffset);
      Graphics.Blit((Texture) source, destination, this.material);
    }
  }
}
