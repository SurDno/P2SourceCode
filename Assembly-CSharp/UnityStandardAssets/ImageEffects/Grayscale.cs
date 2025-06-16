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
      material.SetTexture("_RampTex", textureRamp);
      material.SetFloat("_RampOffset", rampOffset);
      Graphics.Blit(source, destination, material);
    }
  }
}
