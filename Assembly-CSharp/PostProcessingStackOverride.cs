// Decompiled with JetBrains decompiler
// Type: PostProcessingStackOverride
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.PostProcessing;

#nullable disable
public class PostProcessingStackOverride : MonoBehaviour
{
  [SerializeField]
  private PostProcessingStackOverride nestedOverride;
  [Space]
  [SerializeField]
  private PostProcessingStackOverride.AntialiasingOverride antialiasing;
  [SerializeField]
  private PostProcessingStackOverride.AmbientOcclusionOverride ambientOcclusion;
  [SerializeField]
  private PostProcessingStackOverride.ScreenSpaceReflectionOverride screenSpaceReflection;
  [SerializeField]
  private PostProcessingStackOverride.DepthOfFieldOverride depthOfField;
  [SerializeField]
  private PostProcessingStackOverride.MotionBlurOverride motionBlur;
  [SerializeField]
  private PostProcessingStackOverride.ColorGradingOverride colorGrading;
  [SerializeField]
  private PostProcessingStackOverride.ChromaticAberrationOverride chromaticAberration;
  [SerializeField]
  private PostProcessingStackOverride.VignetteOverride vignette;
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
    get => this.nestedOverride;
    set => this.nestedOverride = value;
  }

  public PostProcessingStackOverride.AntialiasingOverride Antialiasing => this.antialiasing;

  public PostProcessingStackOverride.AmbientOcclusionOverride AmbientOcclusion
  {
    get => this.ambientOcclusion;
  }

  public PostProcessingStackOverride.ScreenSpaceReflectionOverride ScreenSpaceReflection
  {
    get => this.screenSpaceReflection;
  }

  public PostProcessingStackOverride.DepthOfFieldOverride DepthOfField => this.depthOfField;

  public PostProcessingStackOverride.MotionBlurOverride MotionBlur => this.motionBlur;

  public PostProcessingStackOverride.ColorGradingOverride ColorGrading => this.colorGrading;

  public PostProcessingStackOverride.ChromaticAberrationOverride ChromaticAberration
  {
    get => this.chromaticAberration;
  }

  public PostProcessingStackOverride.VignetteOverride Vignette => this.vignette;

  public void ApplyTo(PostProcessingProfile source, PostProcessingProfile target)
  {
    if ((UnityEngine.Object) this.nestedOverride != (UnityEngine.Object) null)
      this.nestedOverride.ApplyTo(source, target);
    this.antialiasing.Apply(source.antialiasing, ref target.antialiasing, ref this.antialiasingModel);
    this.ambientOcclusion.Apply(source.ambientOcclusion, ref target.ambientOcclusion, ref this.ambientOcclusionModel);
    this.screenSpaceReflection.Apply(source.screenSpaceReflection, ref target.screenSpaceReflection, ref this.screenSpaceReflectionModel);
    this.depthOfField.Apply(source.depthOfField, ref target.depthOfField, ref this.depthOfFieldModel);
    this.motionBlur.Apply(source.motionBlur, ref target.motionBlur, ref this.motionBlurModel);
    this.colorGrading.Apply(source.colorGrading, ref target.colorGrading, ref this.colorGradingModel);
    this.chromaticAberration.Apply(source.chromaticAberration, ref target.chromaticAberration, ref this.chromaticAberrationModel);
    this.vignette.Apply(source.vignette, ref target.vignette, ref this.vignetteModel);
  }

  [Serializable]
  public abstract class ModelOverride<T> where T : PostProcessingModel, new()
  {
    public bool Override;
    public bool Enabled;

    public void Apply(T source, ref T target, ref T local)
    {
      if (this.Override)
      {
        if ((object) local == null)
          local = new T();
        if ((object) target != (object) local)
          target = local;
        local.enabled = this.Enabled;
        this.ApplySettings(source, local);
      }
      else
      {
        if ((object) target != (object) local)
          return;
        target = source;
      }
    }

    protected abstract void ApplySettings(T source, T target);
  }

  [Serializable]
  public class AntialiasingOverride : PostProcessingStackOverride.ModelOverride<AntialiasingModel>
  {
    protected override void ApplySettings(AntialiasingModel source, AntialiasingModel target)
    {
      target.settings = source.settings;
    }
  }

  [Serializable]
  public class AmbientOcclusionOverride : 
    PostProcessingStackOverride.ModelOverride<AmbientOcclusionModel>
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
    PostProcessingStackOverride.ModelOverride<ScreenSpaceReflectionModel>
  {
    protected override void ApplySettings(
      ScreenSpaceReflectionModel source,
      ScreenSpaceReflectionModel target)
    {
      target.settings = source.settings;
    }
  }

  [Serializable]
  public class DepthOfFieldOverride : PostProcessingStackOverride.ModelOverride<DepthOfFieldModel>
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
        focusDistance = this.FocusDistance,
        aperture = this.Aperture,
        useCameraFov = this.UseCameraFov,
        focalLength = this.FocalLength
      };
      target.settings = settings;
    }
  }

  [Serializable]
  public class MotionBlurOverride : PostProcessingStackOverride.ModelOverride<MotionBlurModel>
  {
    [Range(0.0f, 360f)]
    [Tooltip("The angle of rotary shutter relative to 30 fps. Larger values give longer exposure.")]
    public float ShutterAngle = MotionBlurModel.Settings.defaultSettings.shutterAngle;

    protected override void ApplySettings(MotionBlurModel source, MotionBlurModel target)
    {
      MotionBlurModel.Settings settings = source.settings with
      {
        shutterAngle = this.ShutterAngle
      };
      target.settings = settings;
    }
  }

  [Serializable]
  public class ColorGradingOverride : PostProcessingStackOverride.ModelOverride<ColorGradingModel>
  {
    public ColorGradingModel.BasicSettings Basic = ColorGradingModel.Settings.defaultSettings.basic;
    public ColorGradingModel.ChannelMixerSettings ChannelMixer = ColorGradingModel.Settings.defaultSettings.channelMixer;

    protected override void ApplySettings(ColorGradingModel source, ColorGradingModel target)
    {
      ColorGradingModel.Settings settings = source.settings with
      {
        basic = this.Basic,
        channelMixer = this.ChannelMixer
      };
      target.settings = settings;
    }
  }

  [Serializable]
  public class ChromaticAberrationOverride : 
    PostProcessingStackOverride.ModelOverride<ChromaticAberrationModel>
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
        intensity = this.Intensity
      };
      target.settings = settings;
    }
  }

  [Serializable]
  public class VignetteOverride : PostProcessingStackOverride.ModelOverride<VignetteModel>
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
        color = this.Color,
        intensity = this.Intensity
      };
      target.settings = settings;
    }
  }
}
