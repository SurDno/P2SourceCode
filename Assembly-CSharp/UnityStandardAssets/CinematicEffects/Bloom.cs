using System;

namespace UnityStandardAssets.CinematicEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Cinematic/Bloom")]
  [ImageEffectAllowedInSceneView]
  public class Bloom : MonoBehaviour
  {
    [SerializeField]
    public Settings settings = Settings.defaultSettings;
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
        if ((UnityEngine.Object) m_Shader == (UnityEngine.Object) null)
          m_Shader = Shader.Find("Hidden/Image Effects/Cinematic/Bloom");
        return m_Shader;
      }
    }

    public Material material
    {
      get
      {
        if ((UnityEngine.Object) m_Material == (UnityEngine.Object) null)
          m_Material = ImageEffectHelper.CheckShaderAndCreateMaterial(shader);
        return m_Material;
      }
    }

    private void OnEnable()
    {
      if (ImageEffectHelper.IsSupported(shader, true, false, (MonoBehaviour) this))
        return;
      this.enabled = false;
    }

    private void OnDisable()
    {
      if ((UnityEngine.Object) m_Material != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) m_Material);
      m_Material = (Material) null;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      bool isMobilePlatform = Application.isMobilePlatform;
      int width = source.width;
      int height = source.height;
      if (!settings.highQuality)
      {
        width /= 2;
        height /= 2;
      }
      RenderTextureFormat format = isMobilePlatform ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
      float num1 = (float) ((double) Mathf.Log((float) height, 2f) + settings.radius - 8.0);
      int num2 = (int) num1;
      int num3 = Mathf.Clamp(num2, 1, 16);
      float thresholdLinear = settings.thresholdLinear;
      material.SetFloat("_Threshold", thresholdLinear);
      float num4 = (float) (thresholdLinear * (double) settings.softKnee + 9.9999997473787516E-06);
      material.SetVector("_Curve", (Vector4) new Vector3(thresholdLinear - num4, num4 * 2f, 0.25f / num4));
      material.SetFloat("_PrefilterOffs", !settings.highQuality && settings.antiFlicker ? -0.5f : 0.0f);
      material.SetFloat("_SampleScale", 0.5f + num1 - num2);
      material.SetFloat("_Intensity", Mathf.Max(0.0f, settings.intensity));
      RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, format);
      Graphics.Blit((Texture) source, temporary, material, settings.antiFlicker ? 1 : 0);
      RenderTexture source1 = temporary;
      for (int index = 0; index < num3; ++index)
      {
        m_blurBuffer1[index] = RenderTexture.GetTemporary(source1.width / 2, source1.height / 2, 0, format);
        Graphics.Blit((Texture) source1, m_blurBuffer1[index], material, index == 0 ? (settings.antiFlicker ? 3 : 2) : 4);
        source1 = m_blurBuffer1[index];
      }
      for (int index = num3 - 2; index >= 0; --index)
      {
        RenderTexture renderTexture = m_blurBuffer1[index];
        material.SetTexture("_BaseTex", (Texture) renderTexture);
        m_blurBuffer2[index] = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0, format);
        Graphics.Blit((Texture) source1, m_blurBuffer2[index], material, settings.highQuality ? 6 : 5);
        source1 = m_blurBuffer2[index];
      }
      material.SetTexture("_BaseTex", (Texture) source);
      Graphics.Blit((Texture) source1, destination, material, settings.highQuality ? 8 : 7);
      for (int index = 0; index < 16; ++index)
      {
        if ((UnityEngine.Object) m_blurBuffer1[index] != (UnityEngine.Object) null)
          RenderTexture.ReleaseTemporary(m_blurBuffer1[index]);
        if ((UnityEngine.Object) m_blurBuffer2[index] != (UnityEngine.Object) null)
          RenderTexture.ReleaseTemporary(m_blurBuffer2[index]);
        m_blurBuffer1[index] = (RenderTexture) null;
        m_blurBuffer2[index] = (RenderTexture) null;
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
        set => threshold = value;
        get => Mathf.Max(0.0f, threshold);
      }

      public float thresholdLinear
      {
        set => threshold = Mathf.LinearToGammaSpace(value);
        get => Mathf.GammaToLinearSpace(thresholdGamma);
      }

      public static Settings defaultSettings
      {
        get
        {
          return new Settings {
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
