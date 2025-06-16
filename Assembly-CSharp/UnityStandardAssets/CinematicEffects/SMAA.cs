using System;

namespace UnityStandardAssets.CinematicEffects
{
  [Serializable]
  public class SMAA : IAntiAliasing
  {
    [TopLevelSettings]
    public GlobalSettings settings = GlobalSettings.defaultSettings;
    [SettingsGroup]
    public QualitySettings quality = QualitySettings.presetQualitySettings[2];
    [SettingsGroup]
    public PredicationSettings predication = PredicationSettings.defaultSettings;
    [SettingsGroup]
    [ExperimentalGroup]
    public TemporalSettings temporal = TemporalSettings.defaultSettings;
    private Matrix4x4 m_ProjectionMatrix;
    private Matrix4x4 m_PreviousViewProjectionMatrix;
    private float m_FlipFlop = 1f;
    private RenderTexture m_Accumulation;
    private Shader m_Shader;
    private Texture2D m_AreaTexture;
    private Texture2D m_SearchTexture;
    private Material m_Material;

    public Shader shader
    {
      get
      {
        if ((UnityEngine.Object) m_Shader == (UnityEngine.Object) null)
          m_Shader = Shader.Find("Hidden/Subpixel Morphological Anti-aliasing");
        return m_Shader;
      }
    }

    private Texture2D areaTexture
    {
      get
      {
        if ((UnityEngine.Object) m_AreaTexture == (UnityEngine.Object) null)
          m_AreaTexture = Resources.Load<Texture2D>("AreaTex");
        return m_AreaTexture;
      }
    }

    private Texture2D searchTexture
    {
      get
      {
        if ((UnityEngine.Object) m_SearchTexture == (UnityEngine.Object) null)
          m_SearchTexture = Resources.Load<Texture2D>("SearchTex");
        return m_SearchTexture;
      }
    }

    private Material material
    {
      get
      {
        if ((UnityEngine.Object) m_Material == (UnityEngine.Object) null)
          m_Material = ImageEffectHelper.CheckShaderAndCreateMaterial(shader);
        return m_Material;
      }
    }

    public void OnEnable(AntiAliasing owner)
    {
      if (ImageEffectHelper.IsSupported(shader, true, false, (MonoBehaviour) owner))
        return;
      owner.enabled = false;
    }

    public void OnDisable()
    {
      if ((UnityEngine.Object) m_Material != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) m_Material);
      if ((UnityEngine.Object) m_Accumulation != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) m_Accumulation);
      m_Material = (Material) null;
      m_Accumulation = (RenderTexture) null;
    }

    public void OnPreCull(Camera camera)
    {
      if (!temporal.UseTemporal())
        return;
      m_ProjectionMatrix = camera.projectionMatrix;
      m_FlipFlop -= 2f * m_FlipFlop;
      Matrix4x4 identity = Matrix4x4.identity with
      {
        m03 = 0.25f * m_FlipFlop * temporal.fuzzSize / (float) camera.pixelWidth,
        m13 = -0.25f * m_FlipFlop * temporal.fuzzSize / (float) camera.pixelHeight
      };
      camera.projectionMatrix = identity * camera.projectionMatrix;
    }

    public void OnPostRender(Camera camera)
    {
      if (!temporal.UseTemporal())
        return;
      camera.ResetProjectionMatrix();
    }

    public void OnRenderImage(Camera camera, RenderTexture source, RenderTexture destination)
    {
      int pixelWidth = camera.pixelWidth;
      int pixelHeight = camera.pixelHeight;
      bool flag = false;
      QualitySettings qualitySettings = quality;
      if (settings.quality != QualityPreset.Custom)
        qualitySettings = QualitySettings.presetQualitySettings[(int) settings.quality];
      int edgeDetectionMethod = (int) settings.edgeDetectionMethod;
      int pass1 = 4;
      int pass2 = 5;
      int pass3 = 6;
      Matrix4x4 m = GL.GetGPUProjectionMatrix(m_ProjectionMatrix, true) * camera.worldToCameraMatrix;
      material.SetTexture("_AreaTex", (Texture) areaTexture);
      material.SetTexture("_SearchTex", (Texture) searchTexture);
      material.SetVector("_Metrics", new Vector4(1f / pixelWidth, 1f / pixelHeight, (float) pixelWidth, (float) pixelHeight));
      material.SetVector("_Params1", new Vector4(qualitySettings.threshold, qualitySettings.depthThreshold, (float) qualitySettings.maxSearchSteps, (float) qualitySettings.maxDiagonalSearchSteps));
      material.SetVector("_Params2", (Vector4) new Vector2((float) qualitySettings.cornerRounding, qualitySettings.localContrastAdaptationFactor));
      material.SetMatrix("_ReprojectionMatrix", m_PreviousViewProjectionMatrix * Matrix4x4.Inverse(m));
      float num = m_FlipFlop < 0.0 ? 2f : 1f;
      material.SetVector("_SubsampleIndices", new Vector4(num, num, num, 0.0f));
      Shader.DisableKeyword("USE_PREDICATION");
      if (settings.edgeDetectionMethod == EdgeDetectionMethod.Depth)
        camera.depthTextureMode |= DepthTextureMode.Depth;
      else if (predication.enabled)
      {
        camera.depthTextureMode |= DepthTextureMode.Depth;
        Shader.EnableKeyword("USE_PREDICATION");
        material.SetVector("_Params3", (Vector4) new Vector3(predication.threshold, predication.scale, predication.strength));
      }
      Shader.DisableKeyword("USE_DIAG_SEARCH");
      Shader.DisableKeyword("USE_CORNER_DETECTION");
      if (qualitySettings.diagonalDetection)
        Shader.EnableKeyword("USE_DIAG_SEARCH");
      if (qualitySettings.cornerDetection)
        Shader.EnableKeyword("USE_CORNER_DETECTION");
      Shader.DisableKeyword("USE_UV_BASED_REPROJECTION");
      if (temporal.UseTemporal())
        Shader.EnableKeyword("USE_UV_BASED_REPROJECTION");
      if ((UnityEngine.Object) m_Accumulation == (UnityEngine.Object) null || m_Accumulation.width != pixelWidth || m_Accumulation.height != pixelHeight)
      {
        if ((bool) (UnityEngine.Object) m_Accumulation)
          RenderTexture.ReleaseTemporary(m_Accumulation);
        m_Accumulation = RenderTexture.GetTemporary(pixelWidth, pixelHeight, 0, source.format, RenderTextureReadWrite.Linear);
        m_Accumulation.hideFlags = HideFlags.HideAndDontSave;
        flag = true;
      }
      RenderTexture renderTexture1 = TempRT(pixelWidth, pixelHeight, source.format);
      Graphics.Blit((Texture) null, renderTexture1, material, 0);
      Graphics.Blit((Texture) source, renderTexture1, material, edgeDetectionMethod);
      if (settings.debugPass == DebugPass.Edges)
      {
        Graphics.Blit((Texture) renderTexture1, destination);
      }
      else
      {
        RenderTexture renderTexture2 = TempRT(pixelWidth, pixelHeight, source.format);
        Graphics.Blit((Texture) null, renderTexture2, material, 0);
        Graphics.Blit((Texture) renderTexture1, renderTexture2, material, pass1);
        if (settings.debugPass == DebugPass.Weights)
        {
          Graphics.Blit((Texture) renderTexture2, destination);
        }
        else
        {
          material.SetTexture("_BlendTex", (Texture) renderTexture2);
          if (temporal.UseTemporal())
          {
            Graphics.Blit((Texture) source, renderTexture1, material, pass2);
            if (settings.debugPass == DebugPass.Accumulation)
              Graphics.Blit((Texture) m_Accumulation, destination);
            else if (!flag)
            {
              material.SetTexture("_AccumulationTex", (Texture) m_Accumulation);
              Graphics.Blit((Texture) renderTexture1, destination, material, pass3);
            }
            else
              Graphics.Blit((Texture) renderTexture1, destination);
            Graphics.Blit((Texture) destination, m_Accumulation);
            RenderTexture.active = (RenderTexture) null;
          }
          else
            Graphics.Blit((Texture) source, destination, material, pass2);
        }
        RenderTexture.ReleaseTemporary(renderTexture2);
      }
      RenderTexture.ReleaseTemporary(renderTexture1);
      m_PreviousViewProjectionMatrix = m;
    }

    private RenderTexture TempRT(int width, int height, RenderTextureFormat format)
    {
      int depthBuffer = 0;
      return RenderTexture.GetTemporary(width, height, depthBuffer, format, RenderTextureReadWrite.Linear);
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingsGroup : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TopLevelSettings : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ExperimentalGroup : Attribute
    {
    }

    public enum DebugPass
    {
      Off,
      Edges,
      Weights,
      Accumulation,
    }

    public enum QualityPreset
    {
      Low,
      Medium,
      High,
      Ultra,
      Custom,
    }

    public enum EdgeDetectionMethod
    {
      Luma = 1,
      Color = 2,
      Depth = 3,
    }

    [Serializable]
    public struct GlobalSettings
    {
      [Tooltip("Use this to fine tune your settings when working in Custom quality mode. \"Accumulation\" only works when \"Temporal Filtering\" is enabled.")]
      public DebugPass debugPass;
      [Tooltip("Low: 60% of the quality.\nMedium: 80% of the quality.\nHigh: 95% of the quality.\nUltra: 99% of the quality (overkill).")]
      public QualityPreset quality;
      [Tooltip("You've three edge detection methods to choose from: luma, color or depth.\nThey represent different quality/performance and anti-aliasing/sharpness tradeoffs, so our recommendation is for you to choose the one that best suits your particular scenario:\n\n- Depth edge detection is usually the fastest but it may miss some edges.\n- Luma edge detection is usually more expensive than depth edge detection, but catches visible edges that depth edge detection can miss.\n- Color edge detection is usually the most expensive one but catches chroma-only edges.")]
      public EdgeDetectionMethod edgeDetectionMethod;

      public static GlobalSettings defaultSettings
      {
        get
        {
          return new GlobalSettings {
            debugPass = DebugPass.Off,
            quality = QualityPreset.High,
            edgeDetectionMethod = EdgeDetectionMethod.Color
          };
        }
      }
    }

    [Serializable]
    public struct QualitySettings
    {
      [Tooltip("Enables/Disables diagonal processing.")]
      public bool diagonalDetection;
      [Tooltip("Enables/Disables corner detection. Leave this on to avoid blurry corners.")]
      public bool cornerDetection;
      [Range(0.0f, 0.5f)]
      [Tooltip("Specifies the threshold or sensitivity to edges. Lowering this value you will be able to detect more edges at the expense of performance.\n0.1 is a reasonable value, and allows to catch most visible edges. 0.05 is a rather overkill value, that allows to catch 'em all.")]
      public float threshold;
      [Min(0.0001f)]
      [Tooltip("Specifies the threshold for depth edge detection. Lowering this value you will be able to detect more edges at the expense of performance.")]
      public float depthThreshold;
      [Range(0.0f, 112f)]
      [Tooltip("Specifies the maximum steps performed in the horizontal/vertical pattern searches, at each side of the pixel.\nIn number of pixels, it's actually the double. So the maximum line length perfectly handled by, for example 16, is 64 (by perfectly, we meant that longer lines won't look as good, but still antialiased).")]
      public int maxSearchSteps;
      [Range(0.0f, 20f)]
      [Tooltip("Specifies the maximum steps performed in the diagonal pattern searches, at each side of the pixel. In this case we jump one pixel at time, instead of two.\nOn high-end machines it is cheap (between a 0.8x and 0.9x slower for 16 steps), but it can have a significant impact on older machines.")]
      public int maxDiagonalSearchSteps;
      [Range(0.0f, 100f)]
      [Tooltip("Specifies how much sharp corners will be rounded.")]
      public int cornerRounding;
      [Min(0.0f)]
      [Tooltip("If there is an neighbor edge that has a local contrast factor times bigger contrast than current edge, current edge will be discarded.\nThis allows to eliminate spurious crossing edges, and is based on the fact that, if there is too much contrast in a direction, that will hide perceptually contrast in the other neighbors.")]
      public float localContrastAdaptationFactor;
      public static QualitySettings[] presetQualitySettings = new QualitySettings[4]
      {
        new QualitySettings {
          diagonalDetection = false,
          cornerDetection = false,
          threshold = 0.15f,
          depthThreshold = 0.01f,
          maxSearchSteps = 4,
          maxDiagonalSearchSteps = 8,
          cornerRounding = 25,
          localContrastAdaptationFactor = 2f
        },
        new QualitySettings {
          diagonalDetection = false,
          cornerDetection = false,
          threshold = 0.1f,
          depthThreshold = 0.01f,
          maxSearchSteps = 8,
          maxDiagonalSearchSteps = 8,
          cornerRounding = 25,
          localContrastAdaptationFactor = 2f
        },
        new QualitySettings {
          diagonalDetection = true,
          cornerDetection = true,
          threshold = 0.1f,
          depthThreshold = 0.01f,
          maxSearchSteps = 16,
          maxDiagonalSearchSteps = 8,
          cornerRounding = 25,
          localContrastAdaptationFactor = 2f
        },
        new QualitySettings {
          diagonalDetection = true,
          cornerDetection = true,
          threshold = 0.05f,
          depthThreshold = 0.01f,
          maxSearchSteps = 32,
          maxDiagonalSearchSteps = 16,
          cornerRounding = 25,
          localContrastAdaptationFactor = 2f
        }
      };
    }

    [Serializable]
    public struct TemporalSettings
    {
      [Tooltip("Temporal filtering makes it possible for the SMAA algorithm to benefit from minute subpixel information available that has been accumulated over many frames.")]
      public bool enabled;
      [Range(0.5f, 10f)]
      [Tooltip("The size of the fuzz-displacement (jitter) in pixels applied to the camera's perspective projection matrix.\nUsed for 2x temporal anti-aliasing.")]
      public float fuzzSize;

      public bool UseTemporal() => enabled;

      public static TemporalSettings defaultSettings
      {
        get
        {
          return new TemporalSettings {
            enabled = false,
            fuzzSize = 2f
          };
        }
      }
    }

    [Serializable]
    public struct PredicationSettings
    {
      [Tooltip("Predicated thresholding allows to better preserve texture details and to improve performance, by decreasing the number of detected edges using an additional buffer (the detph buffer).\nIt locally decreases the luma or color threshold if an edge is found in an additional buffer (so the global threshold can be higher).")]
      public bool enabled;
      [Min(0.0001f)]
      [Tooltip("Threshold to be used in the additional predication buffer.")]
      public float threshold;
      [Range(1f, 5f)]
      [Tooltip("How much to scale the global threshold used for luma or color edge detection when using predication.")]
      public float scale;
      [Range(0.0f, 1f)]
      [Tooltip("How much to locally decrease the threshold.")]
      public float strength;

      public static PredicationSettings defaultSettings
      {
        get
        {
          return new PredicationSettings {
            enabled = false,
            threshold = 0.01f,
            scale = 2f,
            strength = 0.4f
          };
        }
      }
    }
  }
}
