// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.CinematicEffects.Bloom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace UnityStandardAssets.CinematicEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Cinematic/Bloom")]
  [ImageEffectAllowedInSceneView]
  public class Bloom : MonoBehaviour
  {
    [SerializeField]
    public Bloom.Settings settings = Bloom.Settings.defaultSettings;
    [SerializeField]
    [HideInInspector]
    private Shader m_Shader;
    private Material m_Material;
    private const int kMaxIterations = 16;
    private RenderTexture[] m_blurBuffer1 = new RenderTexture[16];
    private RenderTexture[] m_blurBuffer2 = new RenderTexture[16];

    public Shader shader
    {
      get
      {
        if ((UnityEngine.Object) this.m_Shader == (UnityEngine.Object) null)
          this.m_Shader = Shader.Find("Hidden/Image Effects/Cinematic/Bloom");
        return this.m_Shader;
      }
    }

    public Material material
    {
      get
      {
        if ((UnityEngine.Object) this.m_Material == (UnityEngine.Object) null)
          this.m_Material = ImageEffectHelper.CheckShaderAndCreateMaterial(this.shader);
        return this.m_Material;
      }
    }

    private void OnEnable()
    {
      if (ImageEffectHelper.IsSupported(this.shader, true, false, (MonoBehaviour) this))
        return;
      this.enabled = false;
    }

    private void OnDisable()
    {
      if ((UnityEngine.Object) this.m_Material != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_Material);
      this.m_Material = (Material) null;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      bool isMobilePlatform = Application.isMobilePlatform;
      int width = source.width;
      int height = source.height;
      if (!this.settings.highQuality)
      {
        width /= 2;
        height /= 2;
      }
      RenderTextureFormat format = isMobilePlatform ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
      float num1 = (float) ((double) Mathf.Log((float) height, 2f) + (double) this.settings.radius - 8.0);
      int num2 = (int) num1;
      int num3 = Mathf.Clamp(num2, 1, 16);
      float thresholdLinear = this.settings.thresholdLinear;
      this.material.SetFloat("_Threshold", thresholdLinear);
      float num4 = (float) ((double) thresholdLinear * (double) this.settings.softKnee + 9.9999997473787516E-06);
      this.material.SetVector("_Curve", (Vector4) new Vector3(thresholdLinear - num4, num4 * 2f, 0.25f / num4));
      this.material.SetFloat("_PrefilterOffs", !this.settings.highQuality && this.settings.antiFlicker ? -0.5f : 0.0f);
      this.material.SetFloat("_SampleScale", 0.5f + num1 - (float) num2);
      this.material.SetFloat("_Intensity", Mathf.Max(0.0f, this.settings.intensity));
      RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, format);
      Graphics.Blit((Texture) source, temporary, this.material, this.settings.antiFlicker ? 1 : 0);
      RenderTexture source1 = temporary;
      for (int index = 0; index < num3; ++index)
      {
        this.m_blurBuffer1[index] = RenderTexture.GetTemporary(source1.width / 2, source1.height / 2, 0, format);
        Graphics.Blit((Texture) source1, this.m_blurBuffer1[index], this.material, index == 0 ? (this.settings.antiFlicker ? 3 : 2) : 4);
        source1 = this.m_blurBuffer1[index];
      }
      for (int index = num3 - 2; index >= 0; --index)
      {
        RenderTexture renderTexture = this.m_blurBuffer1[index];
        this.material.SetTexture("_BaseTex", (Texture) renderTexture);
        this.m_blurBuffer2[index] = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0, format);
        Graphics.Blit((Texture) source1, this.m_blurBuffer2[index], this.material, this.settings.highQuality ? 6 : 5);
        source1 = this.m_blurBuffer2[index];
      }
      this.material.SetTexture("_BaseTex", (Texture) source);
      Graphics.Blit((Texture) source1, destination, this.material, this.settings.highQuality ? 8 : 7);
      for (int index = 0; index < 16; ++index)
      {
        if ((UnityEngine.Object) this.m_blurBuffer1[index] != (UnityEngine.Object) null)
          RenderTexture.ReleaseTemporary(this.m_blurBuffer1[index]);
        if ((UnityEngine.Object) this.m_blurBuffer2[index] != (UnityEngine.Object) null)
          RenderTexture.ReleaseTemporary(this.m_blurBuffer2[index]);
        this.m_blurBuffer1[index] = (RenderTexture) null;
        this.m_blurBuffer2[index] = (RenderTexture) null;
      }
      RenderTexture.ReleaseTemporary(temporary);
    }

    [Serializable]
    public struct Settings
    {
      [SerializeField]
      [Tooltip("Filters out pixels under this level of brightness.")]
      public float threshold;
      [SerializeField]
      [Range(0.0f, 1f)]
      [Tooltip("Makes transition between under/over-threshold gradual.")]
      public float softKnee;
      [SerializeField]
      [Range(1f, 7f)]
      [Tooltip("Changes extent of veiling effects in a screen resolution-independent fashion.")]
      public float radius;
      [SerializeField]
      [Tooltip("Blend factor of the result image.")]
      public float intensity;
      [SerializeField]
      [Tooltip("Controls filter quality and buffer resolution.")]
      public bool highQuality;
      [SerializeField]
      [Tooltip("Reduces flashing noise with an additional filter.")]
      public bool antiFlicker;

      public float thresholdGamma
      {
        set => this.threshold = value;
        get => Mathf.Max(0.0f, this.threshold);
      }

      public float thresholdLinear
      {
        set => this.threshold = Mathf.LinearToGammaSpace(value);
        get => Mathf.GammaToLinearSpace(this.thresholdGamma);
      }

      public static Bloom.Settings defaultSettings
      {
        get
        {
          return new Bloom.Settings()
          {
            threshold = 0.9f,
            softKnee = 0.5f,
            radius = 2f,
            intensity = 0.7f,
            highQuality = true,
            antiFlicker = false
          };
        }
      }
    }
  }
}
