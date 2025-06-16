using UnityEngine;

public class TOD_WeatherManager : MonoBehaviour
{
  public ParticleSystem RainParticleSystem = (ParticleSystem) null;
  public float FadeTime = 10f;
  public TOD_WeatherManager.RainType Rain = TOD_WeatherManager.RainType.None;
  public TOD_WeatherManager.CloudType Clouds = TOD_WeatherManager.CloudType.None;
  public TOD_WeatherManager.AtmosphereType Atmosphere = TOD_WeatherManager.AtmosphereType.Clear;
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

  protected void Start()
  {
    TOD_Sky instance = TOD_Sky.Instance;
    this.cloudOpacity = instance.Clouds.Opacity;
    this.cloudCoverage = instance.Clouds.Coverage;
    this.cloudBrightness = instance.Clouds.Brightness;
    this.atmosphereFog = instance.Atmosphere.Fogginess;
    this.atmosphereBrightness = instance.Atmosphere.Brightness;
    this.rainEmission = (bool) (Object) this.RainParticleSystem ? this.RainParticleSystem.emissionRate : 0.0f;
    this.cloudOpacityMax = this.cloudOpacity;
    this.cloudBrightnessMax = this.cloudBrightness;
    this.atmosphereBrightnessMax = this.atmosphereBrightness;
    this.rainEmissionMax = this.rainEmission;
  }

  protected void Update()
  {
    TOD_Sky instance = TOD_Sky.Instance;
    switch (this.Rain)
    {
      case TOD_WeatherManager.RainType.None:
        this.rainEmission = 0.0f;
        break;
      case TOD_WeatherManager.RainType.Light:
        this.rainEmission = this.rainEmissionMax * 0.5f;
        break;
      case TOD_WeatherManager.RainType.Heavy:
        this.rainEmission = this.rainEmissionMax;
        break;
    }
    switch (this.Clouds)
    {
      case TOD_WeatherManager.CloudType.None:
        this.cloudOpacity = 0.0f;
        this.cloudCoverage = 0.0f;
        break;
      case TOD_WeatherManager.CloudType.Few:
        this.cloudOpacity = this.cloudOpacityMax;
        this.cloudCoverage = 0.1f;
        break;
      case TOD_WeatherManager.CloudType.Scattered:
        this.cloudOpacity = this.cloudOpacityMax;
        this.cloudCoverage = 0.3f;
        break;
      case TOD_WeatherManager.CloudType.Broken:
        this.cloudOpacity = this.cloudOpacityMax;
        this.cloudCoverage = 0.6f;
        break;
      case TOD_WeatherManager.CloudType.Overcast:
        this.cloudOpacity = this.cloudOpacityMax;
        this.cloudCoverage = 1f;
        break;
    }
    switch (this.Atmosphere)
    {
      case TOD_WeatherManager.AtmosphereType.Clear:
        this.cloudBrightness = this.cloudBrightnessMax;
        this.atmosphereBrightness = this.atmosphereBrightnessMax;
        this.atmosphereFog = 0.0f;
        break;
      case TOD_WeatherManager.AtmosphereType.Storm:
        this.cloudBrightness = this.cloudBrightnessMax * 0.3f;
        this.atmosphereBrightness = this.atmosphereBrightnessMax * 0.5f;
        this.atmosphereFog = 1f;
        break;
      case TOD_WeatherManager.AtmosphereType.Dust:
        this.cloudBrightness = this.cloudBrightnessMax;
        this.atmosphereBrightness = this.atmosphereBrightnessMax;
        this.atmosphereFog = 0.5f;
        break;
      case TOD_WeatherManager.AtmosphereType.Fog:
        this.cloudBrightness = this.cloudBrightnessMax;
        this.atmosphereBrightness = this.atmosphereBrightnessMax;
        this.atmosphereFog = 1f;
        break;
    }
    float t = Time.deltaTime / this.FadeTime;
    instance.Clouds.Opacity = Mathf.Lerp(instance.Clouds.Opacity, this.cloudOpacity, t);
    instance.Clouds.Coverage = Mathf.Lerp(instance.Clouds.Coverage, this.cloudCoverage, t);
    instance.Clouds.Brightness = Mathf.Lerp(instance.Clouds.Brightness, this.cloudBrightness, t);
    instance.Atmosphere.Fogginess = Mathf.Lerp(instance.Atmosphere.Fogginess, this.atmosphereFog, t);
    instance.Atmosphere.Brightness = Mathf.Lerp(instance.Atmosphere.Brightness, this.atmosphereBrightness, t);
    if (!(bool) (Object) this.RainParticleSystem)
      return;
    this.RainParticleSystem.emissionRate = Mathf.Lerp(this.RainParticleSystem.emissionRate, this.rainEmission, t);
  }

  public enum RainType
  {
    None,
    Light,
    Heavy,
  }

  public enum CloudType
  {
    None,
    Few,
    Scattered,
    Broken,
    Overcast,
  }

  public enum AtmosphereType
  {
    Clear,
    Storm,
    Dust,
    Fog,
  }
}
