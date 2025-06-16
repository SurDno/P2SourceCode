using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Edge Detection/Crease Shading")]
  public class CreaseShading : PostEffectsBase
  {
    public float intensity = 0.5f;
    public int softness = 1;
    public float spread = 1f;
    public Shader blurShader = (Shader) null;
    private Material blurMaterial = (Material) null;
    public Shader depthFetchShader = (Shader) null;
    private Material depthFetchMaterial = (Material) null;
    public Shader creaseApplyShader = (Shader) null;
    private Material creaseApplyMaterial = (Material) null;

    public override bool CheckResources()
    {
      this.CheckSupport(true);
      this.blurMaterial = this.CheckShaderAndCreateMaterial(this.blurShader, this.blurMaterial);
      this.depthFetchMaterial = this.CheckShaderAndCreateMaterial(this.depthFetchShader, this.depthFetchMaterial);
      this.creaseApplyMaterial = this.CheckShaderAndCreateMaterial(this.creaseApplyShader, this.creaseApplyMaterial);
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
        int width = source.width;
        int height = source.height;
        float num1 = (float) (1.0 * (double) width / (1.0 * (double) height));
        float num2 = 1f / 512f;
        RenderTexture temporary1 = RenderTexture.GetTemporary(width, height, 0);
        RenderTexture renderTexture1 = RenderTexture.GetTemporary(width / 2, height / 2, 0);
        Graphics.Blit((Texture) source, temporary1, this.depthFetchMaterial);
        Graphics.Blit((Texture) temporary1, renderTexture1);
        for (int index = 0; index < this.softness; ++index)
        {
          RenderTexture temporary2 = RenderTexture.GetTemporary(width / 2, height / 2, 0);
          this.blurMaterial.SetVector("offsets", new Vector4(0.0f, this.spread * num2, 0.0f, 0.0f));
          Graphics.Blit((Texture) renderTexture1, temporary2, this.blurMaterial);
          RenderTexture.ReleaseTemporary(renderTexture1);
          RenderTexture renderTexture2 = temporary2;
          RenderTexture temporary3 = RenderTexture.GetTemporary(width / 2, height / 2, 0);
          this.blurMaterial.SetVector("offsets", new Vector4(this.spread * num2 / num1, 0.0f, 0.0f, 0.0f));
          Graphics.Blit((Texture) renderTexture2, temporary3, this.blurMaterial);
          RenderTexture.ReleaseTemporary(renderTexture2);
          renderTexture1 = temporary3;
        }
        this.creaseApplyMaterial.SetTexture("_HrDepthTex", (Texture) temporary1);
        this.creaseApplyMaterial.SetTexture("_LrDepthTex", (Texture) renderTexture1);
        this.creaseApplyMaterial.SetFloat("intensity", this.intensity);
        Graphics.Blit((Texture) source, destination, this.creaseApplyMaterial);
        RenderTexture.ReleaseTemporary(temporary1);
        RenderTexture.ReleaseTemporary(renderTexture1);
      }
    }
  }
}
