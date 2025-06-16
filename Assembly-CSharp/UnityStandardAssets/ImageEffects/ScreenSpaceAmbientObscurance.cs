// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.ImageEffects.ScreenSpaceAmbientObscurance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Rendering/Screen Space Ambient Obscurance")]
  internal class ScreenSpaceAmbientObscurance : PostEffectsBase
  {
    [Range(0.0f, 3f)]
    public float intensity = 0.5f;
    [Range(0.1f, 3f)]
    public float radius = 0.2f;
    [Range(0.0f, 3f)]
    public int blurIterations = 1;
    [Range(0.0f, 5f)]
    public float blurFilterDistance = 1.25f;
    [Range(0.0f, 1f)]
    public int downsample = 0;
    public Texture2D rand = (Texture2D) null;
    public Shader aoShader = (Shader) null;
    private Material aoMaterial = (Material) null;

    public override bool CheckResources()
    {
      this.CheckSupport(true);
      this.aoMaterial = this.CheckShaderAndCreateMaterial(this.aoShader, this.aoMaterial);
      if (!this.isSupported)
        this.ReportAutoDisable();
      return this.isSupported;
    }

    private void OnDisable()
    {
      if ((bool) (Object) this.aoMaterial)
        Object.DestroyImmediate((Object) this.aoMaterial);
      this.aoMaterial = (Material) null;
    }

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!this.CheckResources())
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        Matrix4x4 projectionMatrix = this.GetComponent<Camera>().projectionMatrix;
        Matrix4x4 inverse = projectionMatrix.inverse;
        this.aoMaterial.SetVector("_ProjInfo", new Vector4((float) (-2.0 / ((double) Screen.width * (double) projectionMatrix[0])), (float) (-2.0 / ((double) Screen.height * (double) projectionMatrix[5])), (1f - projectionMatrix[2]) / projectionMatrix[0], (1f + projectionMatrix[6]) / projectionMatrix[5]));
        this.aoMaterial.SetMatrix("_ProjectionInv", inverse);
        this.aoMaterial.SetTexture("_Rand", (Texture) this.rand);
        this.aoMaterial.SetFloat("_Radius", this.radius);
        this.aoMaterial.SetFloat("_Radius2", this.radius * this.radius);
        this.aoMaterial.SetFloat("_Intensity", this.intensity);
        this.aoMaterial.SetFloat("_BlurFilterDistance", this.blurFilterDistance);
        int width = source.width;
        int height = source.height;
        RenderTexture renderTexture = RenderTexture.GetTemporary(width >> this.downsample, height >> this.downsample);
        Graphics.Blit((Texture) source, renderTexture, this.aoMaterial, 0);
        if (this.downsample > 0)
        {
          RenderTexture temporary = RenderTexture.GetTemporary(width, height);
          Graphics.Blit((Texture) renderTexture, temporary, this.aoMaterial, 4);
          RenderTexture.ReleaseTemporary(renderTexture);
          renderTexture = temporary;
        }
        for (int index = 0; index < this.blurIterations; ++index)
        {
          this.aoMaterial.SetVector("_Axis", (Vector4) new Vector2(1f, 0.0f));
          RenderTexture temporary = RenderTexture.GetTemporary(width, height);
          Graphics.Blit((Texture) renderTexture, temporary, this.aoMaterial, 1);
          RenderTexture.ReleaseTemporary(renderTexture);
          this.aoMaterial.SetVector("_Axis", (Vector4) new Vector2(0.0f, 1f));
          renderTexture = RenderTexture.GetTemporary(width, height);
          Graphics.Blit((Texture) temporary, renderTexture, this.aoMaterial, 1);
          RenderTexture.ReleaseTemporary(temporary);
        }
        this.aoMaterial.SetTexture("_AOTex", (Texture) renderTexture);
        Graphics.Blit((Texture) source, destination, this.aoMaterial, 2);
        RenderTexture.ReleaseTemporary(renderTexture);
      }
    }
  }
}
