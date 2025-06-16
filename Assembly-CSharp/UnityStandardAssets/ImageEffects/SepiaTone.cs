namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Color Adjustments/Sepia Tone")]
  public class SepiaTone : ImageEffectBase
  {
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      Graphics.Blit((Texture) source, destination, material);
    }
  }
}
