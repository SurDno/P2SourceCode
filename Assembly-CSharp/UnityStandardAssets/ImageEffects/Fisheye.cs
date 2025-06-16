using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Displacement/Fisheye")]
  public class Fisheye : PostEffectsBase
  {
    [Range(0.0f, 1.5f)]
    public float strengthX = 0.05f;
    [Range(0.0f, 1.5f)]
    public float strengthY = 0.05f;
    public Shader fishEyeShader = (Shader) null;
    private Material fisheyeMaterial = (Material) null;

    public override bool CheckResources()
    {
      this.CheckSupport(false);
      this.fisheyeMaterial = this.CheckShaderAndCreateMaterial(this.fishEyeShader, this.fisheyeMaterial);
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
        float num1 = 5f / 32f;
        float num2 = (float) ((double) source.width * 1.0 / ((double) source.height * 1.0));
        this.fisheyeMaterial.SetVector("intensity", new Vector4(this.strengthX * num2 * num1, this.strengthY * num1, this.strengthX * num2 * num1, this.strengthY * num1));
        Graphics.Blit((Texture) source, destination, this.fisheyeMaterial);
      }
    }
  }
}
