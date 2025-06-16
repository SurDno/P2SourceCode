using System;
using UnityEngine;

[Serializable]
public class TOD_DayParameters
{
  [Tooltip("Intensity of the atmospheric Rayleigh scattering.")]
  [TOD_Min(0.0f)]
  public float RayleighMultiplier = 1f;
  [Tooltip("Intensity of the atmospheric Mie scattering.")]
  [TOD_Min(0.0f)]
  public float MieMultiplier = 1f;
  [Tooltip("Overall brightness of the atmosphere.")]
  [TOD_Min(0.0f)]
  public float Brightness = 1.5f;
  [Tooltip("Overall contrast of the atmosphere.")]
  [TOD_Min(0.0f)]
  public float Contrast = 1.5f;
  [Tooltip("Directionality factor that determines the size of the glow around the sun.")]
  [TOD_Range(0.0f, 1f)]
  public float Directionality = 0.7f;
  [Tooltip("Density of the fog covering the sky.")]
  [TOD_Range(0.0f, 1f)]
  public float Fogginess;
  [Tooltip("Color of the sun spot.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
  public Gradient SunColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(1f, 0.5f),
      new GradientAlphaKey(1f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey(new Color32(253, 171, 50, byte.MaxValue), 0.0f),
      new GradientColorKey(new Color32(253, 171, 50, byte.MaxValue), 0.5f),
      new GradientColorKey(new Color32(253, 171, 50, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Color of the light that hits the ground.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
  public Gradient LightColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(1f, 0.5f),
      new GradientAlphaKey(1f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey(new Color32(byte.MaxValue, 243, 234, byte.MaxValue), 0.0f),
      new GradientColorKey(new Color32(byte.MaxValue, 243, 234, byte.MaxValue), 0.5f),
      new GradientColorKey(new Color32(byte.MaxValue, 154, 0, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Color of the god rays.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
  public Gradient RayColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(1f, 0.5f),
      new GradientAlphaKey(1f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey(new Color32(byte.MaxValue, 243, 234, byte.MaxValue), 0.0f),
      new GradientColorKey(new Color32(byte.MaxValue, 243, 234, byte.MaxValue), 0.5f),
      new GradientColorKey(new Color32(byte.MaxValue, 154, 0, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Color of the light that hits the atmosphere.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
  public Gradient SkyColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(1f, 0.5f),
      new GradientAlphaKey(1f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey(new Color32(byte.MaxValue, 243, 234, byte.MaxValue), 0.0f),
      new GradientColorKey(new Color32(byte.MaxValue, 243, 234, byte.MaxValue), 0.5f),
      new GradientColorKey(new Color32(byte.MaxValue, 243, 234, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Color of the clouds.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
  public Gradient CloudColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(1f, 0.5f),
      new GradientAlphaKey(1f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey(new Color32(224, 235, byte.MaxValue, byte.MaxValue), 0.0f),
      new GradientColorKey(new Color32(224, 235, byte.MaxValue, byte.MaxValue), 0.5f),
      new GradientColorKey(new Color32(byte.MaxValue, 195, 145, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Color of the atmosphere fog.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
  public Gradient FogColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(1f, 0.5f),
      new GradientAlphaKey(1f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey(new Color32(191, 191, 191, byte.MaxValue), 0.0f),
      new GradientColorKey(new Color32(191, 191, 191, byte.MaxValue), 0.5f),
      new GradientColorKey(new Color32(127, 127, 127, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Color of the ambient light.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
  public Gradient AmbientColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(1f, 0.5f),
      new GradientAlphaKey(1f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey(new Color32(94, 89, 87, byte.MaxValue), 0.0f),
      new GradientColorKey(new Color32(94, 89, 87, byte.MaxValue), 0.5f),
      new GradientColorKey(new Color32(94, 89, 87, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Intensity of the light source.")]
  [Range(0.0f, 8f)]
  public float LightIntensity = 1f;
  [Tooltip("Opacity of the shadows dropped by the light source.")]
  [Range(0.0f, 1f)]
  public float ShadowStrength = 1f;
  [Tooltip("Brightness multiplier of the ambient light.")]
  [Range(0.0f, 8f)]
  public float AmbientMultiplier = 1f;
  [Tooltip("Brightness multiplier of the reflection probe.")]
  [Range(0.0f, 1f)]
  public float ReflectionMultiplier = 1f;
}
