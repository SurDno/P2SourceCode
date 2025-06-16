using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
public class WaveDistortion : ImageEffectBase
{
  [SerializeField]
  private float intensity;
  [SerializeField]
  private float noiseScale = 1f;
  [SerializeField]
  private float noiseSpeed = 1f;

  public float Intensity
  {
    get => intensity;
    set => intensity = value;
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (intensity == 0.0)
    {
      Graphics.Blit(source, destination);
    }
    else
    {
      material.SetFloat("_Intensity", intensity);
      material.SetFloat("_NoiseScale", noiseScale);
      material.SetFloat("_NoiseSpeed", noiseSpeed);
      Graphics.Blit(source, destination, material);
    }
  }
}
