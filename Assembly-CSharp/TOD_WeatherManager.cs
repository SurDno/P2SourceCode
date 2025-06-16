using UnityEngine;

public class TOD_WeatherManager : MonoBehaviour {
	public ParticleSystem RainParticleSystem;
	public float FadeTime = 10f;
	public RainType Rain = RainType.None;
	public CloudType Clouds = CloudType.None;
	public AtmosphereType Atmosphere = AtmosphereType.Clear;
	private float cloudOpacityMax;
	private float cloudBrightnessMax;
	private float atmosphereBrightnessMax;
	private float rainEmissionMax;
	private float cloudOpacity;
	private float cloudCoverage;
	private float cloudBrightness;
	private float atmosphereFog;
	private float atmosphereBrightness;
	private float rainEmission;

	protected void Start() {
		var instance = TOD_Sky.Instance;
		cloudOpacity = instance.Clouds.Opacity;
		cloudCoverage = instance.Clouds.Coverage;
		cloudBrightness = instance.Clouds.Brightness;
		atmosphereFog = instance.Atmosphere.Fogginess;
		atmosphereBrightness = instance.Atmosphere.Brightness;
		rainEmission = (bool)(Object)RainParticleSystem ? RainParticleSystem.emissionRate : 0.0f;
		cloudOpacityMax = cloudOpacity;
		cloudBrightnessMax = cloudBrightness;
		atmosphereBrightnessMax = atmosphereBrightness;
		rainEmissionMax = rainEmission;
	}

	protected void Update() {
		var instance = TOD_Sky.Instance;
		switch (Rain) {
			case RainType.None:
				rainEmission = 0.0f;
				break;
			case RainType.Light:
				rainEmission = rainEmissionMax * 0.5f;
				break;
			case RainType.Heavy:
				rainEmission = rainEmissionMax;
				break;
		}

		switch (Clouds) {
			case CloudType.None:
				cloudOpacity = 0.0f;
				cloudCoverage = 0.0f;
				break;
			case CloudType.Few:
				cloudOpacity = cloudOpacityMax;
				cloudCoverage = 0.1f;
				break;
			case CloudType.Scattered:
				cloudOpacity = cloudOpacityMax;
				cloudCoverage = 0.3f;
				break;
			case CloudType.Broken:
				cloudOpacity = cloudOpacityMax;
				cloudCoverage = 0.6f;
				break;
			case CloudType.Overcast:
				cloudOpacity = cloudOpacityMax;
				cloudCoverage = 1f;
				break;
		}

		switch (Atmosphere) {
			case AtmosphereType.Clear:
				cloudBrightness = cloudBrightnessMax;
				atmosphereBrightness = atmosphereBrightnessMax;
				atmosphereFog = 0.0f;
				break;
			case AtmosphereType.Storm:
				cloudBrightness = cloudBrightnessMax * 0.3f;
				atmosphereBrightness = atmosphereBrightnessMax * 0.5f;
				atmosphereFog = 1f;
				break;
			case AtmosphereType.Dust:
				cloudBrightness = cloudBrightnessMax;
				atmosphereBrightness = atmosphereBrightnessMax;
				atmosphereFog = 0.5f;
				break;
			case AtmosphereType.Fog:
				cloudBrightness = cloudBrightnessMax;
				atmosphereBrightness = atmosphereBrightnessMax;
				atmosphereFog = 1f;
				break;
		}

		var t = Time.deltaTime / FadeTime;
		instance.Clouds.Opacity = Mathf.Lerp(instance.Clouds.Opacity, cloudOpacity, t);
		instance.Clouds.Coverage = Mathf.Lerp(instance.Clouds.Coverage, cloudCoverage, t);
		instance.Clouds.Brightness = Mathf.Lerp(instance.Clouds.Brightness, cloudBrightness, t);
		instance.Atmosphere.Fogginess = Mathf.Lerp(instance.Atmosphere.Fogginess, atmosphereFog, t);
		instance.Atmosphere.Brightness = Mathf.Lerp(instance.Atmosphere.Brightness, atmosphereBrightness, t);
		if (!(bool)(Object)RainParticleSystem)
			return;
		RainParticleSystem.emissionRate = Mathf.Lerp(RainParticleSystem.emissionRate, rainEmission, t);
	}

	public enum RainType {
		None,
		Light,
		Heavy
	}

	public enum CloudType {
		None,
		Few,
		Scattered,
		Broken,
		Overcast
	}

	public enum AtmosphereType {
		Clear,
		Storm,
		Dust,
		Fog
	}
}