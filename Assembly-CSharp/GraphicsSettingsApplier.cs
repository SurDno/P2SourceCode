using Engine.Source.Commons;
using Engine.Source.Settings;
using Engine.Source.Settings.External;
using UnityEngine;

public class GraphicsSettingsApplier : EngineDependent {
	private void Apply() {
		var instance = InstanceByRequest<GraphicsGameSettings>.Instance;
		DynamicResolution.Instance.SetScale(instance.RenderScale.Value);
		AOField.Allowed = instance.AOFields.Value;
		ApplyGameCameraSettings(instance);
		ApplyGamma(instance.Gamma.Value);
		ApplyQualitySettings(instance);
	}

	public void ApplyAmplifyOcclusion(Camera camera, bool value) {
		var component = camera.GetComponent<AmplifyOcclusionEffect>();
		if (!(component != null))
			return;
		component.enabled = value;
	}

	private void ApplyCameraSettings(GameCamera gameCamera, GraphicsGameSettings settings) {
		var camera = gameCamera.Camera;
		if (camera == null)
			return;
		ApplyAmplifyOcclusion(camera, settings.SSAO.Value);
		ApplyContactShadows(camera, settings.ContactShadows.Value);
		ApplyReflections(camera, settings.AdditionalReflections.Value);
		ApplyVolumetricLight(camera, settings.VolumetricLighting.Value);
	}

	public void ApplyContactShadows(Camera camera, bool value) {
		var component = camera.GetComponent<NGSS_ContactShadows>();
		if (!(component != null))
			return;
		component.enabled = value;
	}

	private void ApplyGameCameraSettings(GraphicsGameSettings settings) {
		var instance = GameCamera.Instance;
		if (instance == null)
			return;
		ApplyPostProcessingOverrides(instance, settings);
		ApplyCameraSettings(instance, settings);
	}

	private void ApplyGamma(float value) {
		OverlayCamera.Gamma = 4.4f / Mathf.Pow(4f, value);
	}

	private void ApplyPostProcessingOverrides(GameCamera gameCamera, GraphicsGameSettings settings) {
		var processingOverride = gameCamera.SettingsPostProcessingOverride;
		if (processingOverride == null)
			return;
		var antialiasing = processingOverride.Antialiasing;
		antialiasing.Enabled = false;
		antialiasing.Override = !settings.Antialiasing.Value;
		var ambientOcclusion = processingOverride.AmbientOcclusion;
		ambientOcclusion.Enabled = false;
		ambientOcclusion.Override = !settings.SSAO.Value;
		var depthOfField = processingOverride.DepthOfField;
		depthOfField.Enabled = false;
		depthOfField.Override = !settings.DOF.Value;
		var motionBlur = processingOverride.MotionBlur;
		motionBlur.Enabled = false;
		motionBlur.Override = !settings.MotionBlur.Value;
		var screenSpaceReflection = processingOverride.ScreenSpaceReflection;
		screenSpaceReflection.Enabled = false;
		screenSpaceReflection.Override = !settings.SSR.Value;
	}

	public void ApplyReflections(Camera camera, bool value) {
		var transform = camera.transform.Find("Local Reflection Probe");
		if (transform != null)
			transform.gameObject.SetActive(value);
		var component = camera.GetComponent<PlanarReflectionCapture>();
		if (!(component != null))
			return;
		component.ReflectWorld = value;
	}

	public void ApplyVolumetricLight(Camera camera, int value) {
		var component = camera.GetComponent<VolumetricLightRenderer>();
		if (component == null)
			return;
		switch (value) {
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

	private void ApplyQualitySettings(GraphicsGameSettings settings) {
		QualitySettings.anisotropicFiltering = settings.AnisotropicFiltering.Value
			? AnisotropicFiltering.ForceEnable
			: AnisotropicFiltering.Disable;
		QualitySettings.lodBias = settings.LevelOfDetails.Value;
		QualitySettings.softParticles = settings.SoftParticles.Value;
		QualitySettings.vSyncCount = settings.VSync.Value;
		var num = settings.ShadowDistance.Value;
		QualitySettings.shadowDistance = num;
		if (num <= 5.0)
			QualitySettings.shadowCascades = 1;
		else if (num <= 15.0) {
			QualitySettings.shadowCascades = 2;
			QualitySettings.shadowCascade2Split = 0.3333333f;
		} else {
			QualitySettings.shadowCascades = 4;
			QualitySettings.shadowCascade4Split = new Vector3(0.06666667f, 0.2f, 0.4666667f);
		}

		ScriptableObjectInstance<GraphicSettingsData>.Instance.ShadowSettings[settings.ShadowQuality.Value].Apply();
		ScriptableObjectInstance<GraphicSettingsData>.Instance.TextureSettings[settings.TextureQuality.Value].Apply();
		QualitySettings.streamingMipmapsActive =
			ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.StreamingMipmapsActive;
	}

	protected override void OnConnectToEngine() {
		UpdateScreen();
		Apply();
		InstanceByRequest<GraphicsGameSettings>.Instance.OnApply += Apply;
	}

	protected override void OnDisconnectFromEngine() {
		InstanceByRequest<GraphicsGameSettings>.Instance.OnApply -= Apply;
	}

	private void Start() {
		if (Connected)
			return;
		ApplyGamma(InstanceByRequest<GraphicsGameSettings>.Instance.Gamma.Value);
	}

	private void LateUpdate() {
		if (!Connected || Screen.width < 1 || Screen.height < 1 ||
		    (Screen.width == InstanceByRequest<ScreenGameSettings>.Instance.ScreenWidth &&
		     Screen.height == InstanceByRequest<ScreenGameSettings>.Instance.ScreenHeight && Screen.fullScreenMode ==
		     InstanceByRequest<ScreenGameSettings>.Instance.FullScreenMode))
			return;
		UpdateScreen();
		InstanceByRequest<ScreenGameSettings>.Instance.Apply();
	}

	private void UpdateScreen() {
		InstanceByRequest<ScreenGameSettings>.Instance.ScreenWidth = Screen.width;
		InstanceByRequest<ScreenGameSettings>.Instance.ScreenHeight = Screen.height;
		InstanceByRequest<ScreenGameSettings>.Instance.FullScreenMode = Screen.fullScreenMode;
	}
}