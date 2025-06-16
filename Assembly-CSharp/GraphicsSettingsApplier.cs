using Engine.Source.Commons;
using Engine.Source.Settings;
using Engine.Source.Settings.External;
using System;
using UnityEngine;

public class GraphicsSettingsApplier : EngineDependent
{
  private void Apply()
  {
    GraphicsGameSettings instance = InstanceByRequest<GraphicsGameSettings>.Instance;
    DynamicResolution.Instance.SetScale(instance.RenderScale.Value);
    AOField.Allowed = instance.AOFields.Value;
    this.ApplyGameCameraSettings(instance);
    this.ApplyGamma(instance.Gamma.Value);
    this.ApplyQualitySettings(instance);
  }

  public void ApplyAmplifyOcclusion(Camera camera, bool value)
  {
    AmplifyOcclusionEffect component = camera.GetComponent<AmplifyOcclusionEffect>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.enabled = value;
  }

  private void ApplyCameraSettings(GameCamera gameCamera, GraphicsGameSettings settings)
  {
    Camera camera = gameCamera.Camera;
    if ((UnityEngine.Object) camera == (UnityEngine.Object) null)
      return;
    this.ApplyAmplifyOcclusion(camera, settings.SSAO.Value);
    this.ApplyContactShadows(camera, settings.ContactShadows.Value);
    this.ApplyReflections(camera, settings.AdditionalReflections.Value);
    this.ApplyVolumetricLight(camera, settings.VolumetricLighting.Value);
  }

  public void ApplyContactShadows(Camera camera, bool value)
  {
    NGSS_ContactShadows component = camera.GetComponent<NGSS_ContactShadows>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.enabled = value;
  }

  private void ApplyGameCameraSettings(GraphicsGameSettings settings)
  {
    GameCamera instance = GameCamera.Instance;
    if ((UnityEngine.Object) instance == (UnityEngine.Object) null)
      return;
    this.ApplyPostProcessingOverrides(instance, settings);
    this.ApplyCameraSettings(instance, settings);
  }

  private void ApplyGamma(float value) => OverlayCamera.Gamma = 4.4f / Mathf.Pow(4f, value);

  private void ApplyPostProcessingOverrides(GameCamera gameCamera, GraphicsGameSettings settings)
  {
    PostProcessingStackOverride processingOverride = gameCamera.SettingsPostProcessingOverride;
    if ((UnityEngine.Object) processingOverride == (UnityEngine.Object) null)
      return;
    PostProcessingStackOverride.AntialiasingOverride antialiasing = processingOverride.Antialiasing;
    antialiasing.Enabled = false;
    antialiasing.Override = !settings.Antialiasing.Value;
    PostProcessingStackOverride.AmbientOcclusionOverride ambientOcclusion = processingOverride.AmbientOcclusion;
    ambientOcclusion.Enabled = false;
    ambientOcclusion.Override = !settings.SSAO.Value;
    PostProcessingStackOverride.DepthOfFieldOverride depthOfField = processingOverride.DepthOfField;
    depthOfField.Enabled = false;
    depthOfField.Override = !settings.DOF.Value;
    PostProcessingStackOverride.MotionBlurOverride motionBlur = processingOverride.MotionBlur;
    motionBlur.Enabled = false;
    motionBlur.Override = !settings.MotionBlur.Value;
    PostProcessingStackOverride.ScreenSpaceReflectionOverride screenSpaceReflection = processingOverride.ScreenSpaceReflection;
    screenSpaceReflection.Enabled = false;
    screenSpaceReflection.Override = !settings.SSR.Value;
  }

  public void ApplyReflections(Camera camera, bool value)
  {
    Transform transform = camera.transform.Find("Local Reflection Probe");
    if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
      transform.gameObject.SetActive(value);
    PlanarReflectionCapture component = camera.GetComponent<PlanarReflectionCapture>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.ReflectWorld = value;
  }

  public void ApplyVolumetricLight(Camera camera, int value)
  {
    VolumetricLightRenderer component = camera.GetComponent<VolumetricLightRenderer>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    switch (value)
    {
      case 0:
        component.enabled = false;
        return;
      case 1:
        component.Resolution = VolumetricLightRenderer.VolumtericResolution.Quarter;
        break;
      case 2:
        component.Resolution = VolumetricLightRenderer.VolumtericResolution.Half;
        break;
      case 3:
        component.Resolution = VolumetricLightRenderer.VolumtericResolution.Full;
        break;
    }
    component.enabled = true;
  }

  private void ApplyQualitySettings(GraphicsGameSettings settings)
  {
    QualitySettings.anisotropicFiltering = settings.AnisotropicFiltering.Value ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable;
    QualitySettings.lodBias = settings.LevelOfDetails.Value;
    QualitySettings.softParticles = settings.SoftParticles.Value;
    QualitySettings.vSyncCount = settings.VSync.Value;
    float num = settings.ShadowDistance.Value;
    QualitySettings.shadowDistance = num;
    if ((double) num <= 5.0)
      QualitySettings.shadowCascades = 1;
    else if ((double) num <= 15.0)
    {
      QualitySettings.shadowCascades = 2;
      QualitySettings.shadowCascade2Split = 0.3333333f;
    }
    else
    {
      QualitySettings.shadowCascades = 4;
      QualitySettings.shadowCascade4Split = new Vector3(0.06666667f, 0.2f, 0.4666667f);
    }
    ScriptableObjectInstance<GraphicSettingsData>.Instance.ShadowSettings[settings.ShadowQuality.Value].Apply();
    ScriptableObjectInstance<GraphicSettingsData>.Instance.TextureSettings[settings.TextureQuality.Value].Apply();
    QualitySettings.streamingMipmapsActive = ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.StreamingMipmapsActive;
  }

  protected override void OnConnectToEngine()
  {
    this.UpdateScreen();
    this.Apply();
    InstanceByRequest<GraphicsGameSettings>.Instance.OnApply += new Action(this.Apply);
  }

  protected override void OnDisconnectFromEngine()
  {
    InstanceByRequest<GraphicsGameSettings>.Instance.OnApply -= new Action(this.Apply);
  }

  private void Start()
  {
    if (this.Connected)
      return;
    this.ApplyGamma(InstanceByRequest<GraphicsGameSettings>.Instance.Gamma.Value);
  }

  private void LateUpdate()
  {
    if (!this.Connected || Screen.width < 1 || Screen.height < 1 || Screen.width == InstanceByRequest<ScreenGameSettings>.Instance.ScreenWidth && Screen.height == InstanceByRequest<ScreenGameSettings>.Instance.ScreenHeight && Screen.fullScreenMode == InstanceByRequest<ScreenGameSettings>.Instance.FullScreenMode)
      return;
    this.UpdateScreen();
    InstanceByRequest<ScreenGameSettings>.Instance.Apply();
  }

  private void UpdateScreen()
  {
    InstanceByRequest<ScreenGameSettings>.Instance.ScreenWidth = Screen.width;
    InstanceByRequest<ScreenGameSettings>.Instance.ScreenHeight = Screen.height;
    InstanceByRequest<ScreenGameSettings>.Instance.FullScreenMode = Screen.fullScreenMode;
  }
}
