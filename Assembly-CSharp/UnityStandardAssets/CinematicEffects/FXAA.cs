using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
  [Serializable]
  public class FXAA : IAntiAliasing
  {
    private Shader m_Shader;
    private Material m_Material;
    [SerializeField]
    [HideInInspector]
    public FXAA.Preset preset = FXAA.Preset.defaultPreset;
    public static FXAA.Preset[] availablePresets = new FXAA.Preset[5]
    {
      FXAA.Preset.extremePerformancePreset,
      FXAA.Preset.performancePreset,
      FXAA.Preset.defaultPreset,
      FXAA.Preset.qualityPreset,
      FXAA.Preset.extremeQualityPreset
    };

    private Shader shader
    {
      get
      {
        if ((UnityEngine.Object) this.m_Shader == (UnityEngine.Object) null)
          this.m_Shader = Shader.Find("Hidden/Fast Approximate Anti-aliasing");
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

    public bool validSourceFormat { get; private set; }

    public void OnEnable(AntiAliasing owner)
    {
      if (ImageEffectHelper.IsSupported(this.shader, true, false, (MonoBehaviour) owner))
        return;
      owner.enabled = false;
    }

    public void OnDisable()
    {
      if (!((UnityEngine.Object) this.m_Material != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_Material);
    }

    public void OnPreCull(Camera camera)
    {
    }

    public void OnPostRender(Camera camera)
    {
    }

    public void OnRenderImage(Camera camera, RenderTexture source, RenderTexture destination)
    {
      this.material.SetVector("_QualitySettings", (Vector4) new Vector3(this.preset.qualitySettings.subpixelAliasingRemovalAmount, this.preset.qualitySettings.edgeDetectionThreshold, this.preset.qualitySettings.minimumRequiredLuminance));
      this.material.SetVector("_ConsoleSettings", new Vector4(this.preset.consoleSettings.subpixelSpreadAmount, this.preset.consoleSettings.edgeSharpnessAmount, this.preset.consoleSettings.edgeDetectionThreshold, this.preset.consoleSettings.minimumRequiredLuminance));
      Graphics.Blit((Texture) source, destination, this.material, 0);
    }

    [Serializable]
    public struct QualitySettings
    {
      [Tooltip("The amount of desired sub-pixel aliasing removal. Effects the sharpeness of the output.")]
      [Range(0.0f, 1f)]
      public float subpixelAliasingRemovalAmount;
      [Tooltip("The minimum amount of local contrast required to qualify a region as containing an edge.")]
      [Range(0.063f, 0.333f)]
      public float edgeDetectionThreshold;
      [Tooltip("Local contrast adaptation value to disallow the algorithm from executing on the darker regions.")]
      [Range(0.0f, 0.0833f)]
      public float minimumRequiredLuminance;
    }

    [Serializable]
    public struct ConsoleSettings
    {
      [Tooltip("The amount of spread applied to the sampling coordinates while sampling for subpixel information.")]
      [Range(0.33f, 0.5f)]
      public float subpixelSpreadAmount;
      [Tooltip("This value dictates how sharp the edges in the image are kept; a higher value implies sharper edges.")]
      [Range(2f, 8f)]
      public float edgeSharpnessAmount;
      [Tooltip("The minimum amount of local contrast required to qualify a region as containing an edge.")]
      [Range(0.125f, 0.25f)]
      public float edgeDetectionThreshold;
      [Tooltip("Local contrast adaptation value to disallow the algorithm from executing on the darker regions.")]
      [Range(0.04f, 0.06f)]
      public float minimumRequiredLuminance;
    }

    [Serializable]
    public struct Preset
    {
      [FXAA.Preset.Layout]
      public FXAA.QualitySettings qualitySettings;
      [FXAA.Preset.Layout]
      public FXAA.ConsoleSettings consoleSettings;
      private static readonly FXAA.Preset s_ExtremePerformance = new FXAA.Preset()
      {
        qualitySettings = new FXAA.QualitySettings()
        {
          subpixelAliasingRemovalAmount = 0.0f,
          edgeDetectionThreshold = 0.333f,
          minimumRequiredLuminance = 0.0833f
        },
        consoleSettings = new FXAA.ConsoleSettings()
        {
          subpixelSpreadAmount = 0.33f,
          edgeSharpnessAmount = 8f,
          edgeDetectionThreshold = 0.25f,
          minimumRequiredLuminance = 0.06f
        }
      };
      private static readonly FXAA.Preset s_Performance = new FXAA.Preset()
      {
        qualitySettings = new FXAA.QualitySettings()
        {
          subpixelAliasingRemovalAmount = 0.25f,
          edgeDetectionThreshold = 0.25f,
          minimumRequiredLuminance = 0.0833f
        },
        consoleSettings = new FXAA.ConsoleSettings()
        {
          subpixelSpreadAmount = 0.33f,
          edgeSharpnessAmount = 8f,
          edgeDetectionThreshold = 0.125f,
          minimumRequiredLuminance = 0.06f
        }
      };
      private static readonly FXAA.Preset s_Default = new FXAA.Preset()
      {
        qualitySettings = new FXAA.QualitySettings()
        {
          subpixelAliasingRemovalAmount = 0.75f,
          edgeDetectionThreshold = 0.166f,
          minimumRequiredLuminance = 0.0833f
        },
        consoleSettings = new FXAA.ConsoleSettings()
        {
          subpixelSpreadAmount = 0.5f,
          edgeSharpnessAmount = 8f,
          edgeDetectionThreshold = 0.125f,
          minimumRequiredLuminance = 0.05f
        }
      };
      private static readonly FXAA.Preset s_Quality = new FXAA.Preset()
      {
        qualitySettings = new FXAA.QualitySettings()
        {
          subpixelAliasingRemovalAmount = 1f,
          edgeDetectionThreshold = 0.125f,
          minimumRequiredLuminance = 1f / 16f
        },
        consoleSettings = new FXAA.ConsoleSettings()
        {
          subpixelSpreadAmount = 0.5f,
          edgeSharpnessAmount = 4f,
          edgeDetectionThreshold = 0.125f,
          minimumRequiredLuminance = 0.04f
        }
      };
      private static readonly FXAA.Preset s_ExtremeQuality = new FXAA.Preset()
      {
        qualitySettings = new FXAA.QualitySettings()
        {
          subpixelAliasingRemovalAmount = 1f,
          edgeDetectionThreshold = 0.063f,
          minimumRequiredLuminance = 0.0312f
        },
        consoleSettings = new FXAA.ConsoleSettings()
        {
          subpixelSpreadAmount = 0.5f,
          edgeSharpnessAmount = 2f,
          edgeDetectionThreshold = 0.125f,
          minimumRequiredLuminance = 0.04f
        }
      };

      public static FXAA.Preset extremePerformancePreset => FXAA.Preset.s_ExtremePerformance;

      public static FXAA.Preset performancePreset => FXAA.Preset.s_Performance;

      public static FXAA.Preset defaultPreset => FXAA.Preset.s_Default;

      public static FXAA.Preset qualityPreset => FXAA.Preset.s_Quality;

      public static FXAA.Preset extremeQualityPreset => FXAA.Preset.s_ExtremeQuality;

      [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
      public class LayoutAttribute : PropertyAttribute
      {
      }
    }
  }
}
