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
    public Shader blurShader;
    private Material blurMaterial;
    public Shader depthFetchShader;
    private Material depthFetchMaterial;
    public Shader creaseApplyShader;
    private Material creaseApplyMaterial;

    public override bool CheckResources()
    {
      CheckSupport(true);
      blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
      depthFetchMaterial = CheckShaderAndCreateMaterial(depthFetchShader, depthFetchMaterial);
      creaseApplyMaterial = CheckShaderAndCreateMaterial(creaseApplyShader, creaseApplyMaterial);
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit(source, destination);
      }
      else
      {
        int width = source.width;
        int height = source.height;
        float num1 = (float) (1.0 * width / (1.0 * height));
        float num2 = 1f / 512f;
        RenderTexture temporary1 = RenderTexture.GetTemporary(width, height, 0);
        RenderTexture renderTexture1 = RenderTexture.GetTemporary(width / 2, height / 2, 0);
        Graphics.Blit(source, temporary1, depthFetchMaterial);
        Graphics.Blit(temporary1, renderTexture1);
        for (int index = 0; index < softness; ++index)
        {
          RenderTexture temporary2 = RenderTexture.GetTemporary(width / 2, height / 2, 0);
          blurMaterial.SetVector("offsets", new Vector4(0.0f, spread * num2, 0.0f, 0.0f));
          Graphics.Blit(renderTexture1, temporary2, blurMaterial);
          RenderTexture.ReleaseTemporary(renderTexture1);
          RenderTexture renderTexture2 = temporary2;
          RenderTexture temporary3 = RenderTexture.GetTemporary(width / 2, height / 2, 0);
          blurMaterial.SetVector("offsets", new Vector4(spread * num2 / num1, 0.0f, 0.0f, 0.0f));
          Graphics.Blit(renderTexture2, temporary3, blurMaterial);
          RenderTexture.ReleaseTemporary(renderTexture2);
          renderTexture1 = temporary3;
        }
        creaseApplyMaterial.SetTexture("_HrDepthTex", temporary1);
        creaseApplyMaterial.SetTexture("_LrDepthTex", renderTexture1);
        creaseApplyMaterial.SetFloat("intensity", intensity);
        Graphics.Blit(source, destination, creaseApplyMaterial);
        RenderTexture.ReleaseTemporary(temporary1);
        RenderTexture.ReleaseTemporary(renderTexture1);
      }
    }
  }
}
