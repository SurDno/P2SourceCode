using System;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcessingStackOverride : MonoBehaviour
{
  [SerializeField]
  private PostProcessingStackOverride nestedOverride;
  [Space]
  [SerializeField]
  private AntialiasingOverride antialiasing;
  [SerializeField]
  private AmbientOcclusionOverride ambientOcclusion;
  [SerializeField]
  private ScreenSpaceReflectionOverride screenSpaceReflection;
  [SerializeField]
  private DepthOfFieldOverride depthOfField;
  [SerializeField]
  private MotionBlurOverride motionBlur;
  [SerializeField]
  private ColorGradingOverride colorGrading;
  [SerializeField]
  private ChromaticAberrationOverride chromaticAberration;
  [SerializeField]
  private VignetteOverride vignette;
  private AntialiasingModel antialiasingModel;
  private AmbientOcclusionModel ambientOcclusionModel;
  private ScreenSpaceReflectionModel screenSpaceReflectionModel;
  private DepthOfFieldModel depthOfFieldModel;
  private MotionBlurModel motionBlurModel;
  private ColorGradingModel colorGradingModel;
  private ChromaticAberrationModel chromaticAberrationModel;
  private VignetteModel vignetteModel;

  public PostProcessingStackOverride NestedOverride
  {
    get => nestedOverride;
    set => nestedOverride = value;
  }

  public AntialiasingOverride Antialiasing => antialiasing;

  public AmbientOcclusionOverride AmbientOcclusion
  {
    get => ambientOcclusion;
  }

  public ScreenSpaceReflectionOverride ScreenSpaceReflection
  {
    get => screenSpaceReflection;
  }

  public DepthOfFieldOverride DepthOfField => depthOfField;

  public MotionBlurOverride MotionBlur => motionBlur;

  public ColorGradingOverride ColorGrading => colorGrading;

  public ChromaticAberrationOverride ChromaticAberration
  {
    get => chromaticAberration;
  }

  public VignetteOverride Vignette => vignette;

  public void ApplyTo(PostProcessingProfile source, PostProcessingProfile target)
  {
    if (nestedOverride != null)
      nestedOverride.ApplyTo(source, target);
    antialiasing.Apply(source.antialiasing, ref target.antialiasing, ref antialiasingModel);
    ambientOcclusion.Apply(source.ambientOcclusion, ref target.ambientOcclusion, ref ambientOcclusionModel);
    screenSpaceReflection.Apply(source.screenSpaceReflection, ref target.screenSpaceReflection, ref screenSpaceReflectionModel);
    depthOfField.Apply(source.depthOfField, ref target.depthOfField, ref depthOfFieldModel);
    motionBlur.Apply(source.motionBlur, ref target.motionBlur, ref motionBlurModel);
    colorGrading.Apply(source.colorGrading, ref target.colorGrading, ref colorGradingModel);
    chromaticAberration.Apply(source.chromaticAberration, ref target.chromaticAberration, ref chromaticAberrationModel);
    vignette.Apply(source.vignette, ref target.vignette, ref vignetteModel);
  }

  [Serializable]
  public abstract class ModelOverride<T> where T : PostProcessingModel, new()
  {
    public bool Override;
    public bool Enabled;

    public void Apply(T source, ref T target, ref T local)
    {
      if (Override)
      {
        if (local == null)
          local = new T();
        if (target != local)
          target = local;
        local.enabled = Enabled;
        ApplySettings(source, local);
      }
      else
      {
        if (target != local)
          return;
        target = source;
      }
    }

    protected abstract void ApplySettings(T source, T target);
  }

  [Serializable]
  public class AntialiasingOverride : ModelOverride<AntialiasingModel>
  {
    protected override void ApplySettings(AntialiasingModel source, AntialiasingModel target)
    {
      target.settings = source.settings;
    }
  }

  [Serializable]
  public class AmbientOcclusionOverride : 
    ModelOverride<AmbientOcclusionModel>
  {
    protected override void ApplySettings(
      AmbientOcclusionModel source,
      AmbientOcclusionModel target)
    {
      target.settings = source.settings;
    }
  }

  [Serializable]
  public class ScreenSpaceReflectionOverride : 
    ModelOverride<ScreenSpaceReflectionModel>
  {
    protected override void ApplySettings(
      ScreenSpaceReflectionModel source,
      ScreenSpaceReflectionModel target)
    {
      target.settings = source.settings;
    }
  }

  [Serializable]
  public class DepthOfFieldOverride : ModelOverride<DepthOfFieldModel>
  {
    [UnityEngine.Min(0.1f)]
    [Tooltip("Distance to the point of focus.")]
    public float FocusDistance = DepthOfFieldModel.Settings.defaultSettings.focusDistance;
    [Range(0.05f, 32f)]
    [Tooltip("Ratio of aperture (known as f-stop or f-number). The smaller the value is, the shallower the depth of field is.")]
    public float Aperture = DepthOfFieldModel.Settings.defaultSettings.aperture;
    [Tooltip("Calculate the focal length automatically from the field-of-view value set on the camera. Using this setting isn't recommended.")]
    public bool UseCameraFov = DepthOfFieldModel.Settings.defaultSettings.useCameraFov;
    [Range(1f, 300f)]
    [Tooltip("Distance between the lens and the film. The larger the value is, the shallower the depth of field is.")]
    public float FocalLength = DepthOfFieldModel.Settings.defaultSettings.focalLength;

    protected override void ApplySettings(DepthOfFieldModel source, DepthOfFieldModel target)
    {
      DepthOfFieldModel.Settings settings = source.settings with
      {
        focusDistance = FocusDistance,
        aperture = Aperture,
        useCameraFov = UseCameraFov,
        focalLength = FocalLength
      };
      target.settings = settings;
    }
  }

  [Serializable]
  public class MotionBlurOverride : ModelOverride<MotionBlurModel>
  {
    [Range(0.0f, 360f)]
    [Tooltip("The angle of rotary shutter relative to 30 fps. Larger values give longer exposure.")]
    public float ShutterAngle = MotionBlurModel.Settings.defaultSettings.shutterAngle;

    protected override void ApplySettings(MotionBlurModel source, MotionBlurModel target)
    {
      MotionBlurModel.Settings settings = source.settings with
      {
        shutterAngle = ShutterAngle
      };
      target.settings = settings;
    }
  }

  [Serializable]
  public class ColorGradingOverride : ModelOverride<ColorGradingModel>
  {
    public ColorGradingModel.BasicSettings Basic = ColorGradingModel.Settings.defaultSettings.basic;
    public ColorGradingModel.ChannelMixerSettings ChannelMixer = ColorGradingModel.Settings.defaultSettings.channelMixer;

    protected override void ApplySettings(ColorGradingModel source, ColorGradingModel target)
    {
      ColorGradingModel.Settings settings = source.settings with
      {
        basic = Basic,
        channelMixer = ChannelMixer
      };
      target.settings = settings;
    }
  }

  [Serializable]
  public class ChromaticAberrationOverride : 
    ModelOverride<ChromaticAberrationModel>
  {
    [Range(0.0f, 1f)]
    [Tooltip("Amount of tangential distortion.")]
    public float Intensity = ChromaticAberrationModel.Settings.defaultSettings.intensity;

    protected override void ApplySettings(
      ChromaticAberrationModel source,
      ChromaticAberrationModel target)
    {
      ChromaticAberrationModel.Settings settings = source.settings with
      {
        intensity = Intensity
      };
      target.settings = settings;
    }
  }

  [Serializable]
  public class VignetteOverride : ModelOverride<VignetteModel>
  {
    [ColorUsage(false)]
    [Tooltip("Vignette color. Use the alpha channel for transparency.")]
    public Color Color = VignetteModel.Settings.defaultSettings.color;
    [Range(0.0f, 1f)]
    [Tooltip("Amount of vignetting on screen.")]
    public float Intensity = VignetteModel.Settings.defaultSettings.intensity;

    protected override void ApplySettings(VignetteModel source, VignetteModel target)
    {
      VignetteModel.Settings settings = source.settings with
      {
        color = Color,
        intensity = Intensity
      };
      target.settings = settings;
    }
  }
}
