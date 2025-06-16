using Engine.Source.Settings.External;
using Inspectors;

namespace Engine.Source.Settings;

public class GraphicsGameSettings : SettingsInstanceByRequest<GraphicsGameSettings> {
	[Inspected] public IValue<bool> AdditionalReflections = new BoolValue(nameof(AdditionalReflections),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.AdditionalReflections);

	[Inspected] public IValue<float> AILodDistance = new FloatValue(nameof(AILodDistance),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.AILodDistance,
		ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.AiLodsMinDistance,
		ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.AiLodsDistance);

	[Inspected] public IValue<bool> AnisotropicFiltering = new BoolValue(nameof(AnisotropicFiltering),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.AnisotropicFiltering);

	[Inspected] public IValue<bool> Antialiasing = new BoolValue(nameof(Antialiasing),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.Antialiasing);

	[Inspected] public IValue<bool> AOFields = new BoolValue(nameof(AOFields),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.AOFields);

	[Inspected] public IValue<bool> ContactShadows = new BoolValue(nameof(ContactShadows),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.ContactShadows);

	[Inspected] public IValue<bool> DOF = new BoolValue(nameof(DOF),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.DOF);

	[Inspected] public IValue<float> Gamma = new FloatValue(nameof(Gamma), 0.5f, 0.0f, 1f);

	[Inspected] public IValue<float> FieldOfView = new FloatValue(nameof(FieldOfView),
		ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.Fov,
		ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MinFov,
		ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MaxFov);

	[Inspected] public IValue<float> LevelOfDetails = new FloatValue(nameof(LevelOfDetails),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.LevelOfDetails,
		ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MinLevelOfDetails,
		ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MaxLevelOfDetails);

	[Inspected] public IValue<bool> MotionBlur = new BoolValue(nameof(MotionBlur),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.MotionBlur);

	[Inspected] public IValue<float> RenderScale = new FloatValue(nameof(RenderScale), 1f, 0.25f, 2f);

	[Inspected] public IValue<float> ShadowDistance = new FloatValue(nameof(ShadowDistance),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.ShadowDistance,
		ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MinShadowDistance,
		ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MaxShadowDistance);

	[Inspected] public IValue<int> ShadowQuality = new IntValue(nameof(ShadowQuality),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.ShadowQuality, 0,
		ScriptableObjectInstance<GraphicSettingsData>.Instance.ShadowSettings.Length - 1);

	[Inspected] public IValue<bool> SoftParticles = new BoolValue(nameof(SoftParticles),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.SoftParticles);

	[Inspected] public IValue<bool> SSAO = new BoolValue(nameof(SSAO),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.SSAO);

	[Inspected] public IValue<bool> SSR = new BoolValue(nameof(SSR),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.SSR);

	[Inspected] public IValue<int> TextureQuality = new IntValue(nameof(TextureQuality),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.MaxSupportedTextureSettings(), 0,
		ScriptableObjectInstance<GraphicSettingsData>.Instance.TextureSettings.Length - 1);

	[Inspected] public IValue<int> VolumetricLighting = new IntValue(nameof(VolumetricLighting),
		ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.VolumetricLighting, 0, 3);

	[Inspected] public IValue<int> VSync = new IntValue(nameof(VSync), minValue: 0, maxValue: 4);
}