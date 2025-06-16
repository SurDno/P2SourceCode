using Engine.Common.Weather;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("Weather")]
public class VMWeather : VMComponent {
	public const string ComponentName = "Weather";

	public override void Initialize(VMBaseEntity parent) {
		base.Initialize(parent);
	}

	[Method("Set weather layer weight", "Weather layer,Weight,Time to blend", "")]
	public virtual void SetWeatherLayerWeight(
		WeatherLayer weatherLayer,
		float weight,
		float timeToBlend) { }

	[Method("Set weather layer weight in gametime", "Weather layer,Weight,Game time to blend", "")]
	public virtual void SetWeatherLayerWeightGT(
		WeatherLayer weatherLayer,
		float weight,
		GameTime timeToBlend) { }

	[Method("Set weather template", "Weather layer,Wather snapshot sample:ISnapshot,Time to blend", "")]
	public virtual void SetWeatherSample(
		WeatherLayer weatherLayer,
		ISampleRef weatherSample,
		float timeToBlend) { }

	[Method("Set weather template in gametime", "Weather layer,Weather snapshot sample:ISnapshot,Game time to blend",
		"")]
	public virtual void SetWeatherSampleGT(
		WeatherLayer weatherLayer,
		ISampleRef weatherSample,
		GameTime timeToBlend) { }
}