using System;

[Serializable]
public class TOD_NightParameters
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
  [Tooltip("Color of the moon mesh.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
  public Gradient MoonColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(1f, 0.5f),
      new GradientAlphaKey(1f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey((Color) new Color32(byte.MaxValue, (byte) 233, (byte) 200, byte.MaxValue), 0.0f),
      new GradientColorKey((Color) new Color32(byte.MaxValue, (byte) 233, (byte) 200, byte.MaxValue), 0.5f),
      new GradientColorKey((Color) new Color32(byte.MaxValue, (byte) 233, (byte) 200, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Color of the light that hits the ground.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
  public Gradient LightColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(1f, 0.5f),
      new GradientAlphaKey(1f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 0.0f),
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 0.5f),
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Color of the god rays.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
  public Gradient RayColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(0.2f, 0.5f),
      new GradientAlphaKey(0.2f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 0.0f),
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 0.5f),
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Color of the light that hits the atmosphere.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
  public Gradient SkyColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(0.2f, 0.5f),
      new GradientAlphaKey(0.2f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 0.0f),
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 0.5f),
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Color of the clouds.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
  public Gradient CloudColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(0.1f, 0.5f),
      new GradientAlphaKey(0.1f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 0.0f),
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 0.5f),
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Color of the atmosphere fog.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
  public Gradient FogColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(0.2f, 0.5f),
      new GradientAlphaKey(0.2f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 0.0f),
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 0.5f),
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Color of the ambient light.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
  public Gradient AmbientColor = new Gradient {
    alphaKeys = new GradientAlphaKey[3]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(0.2f, 0.5f),
      new GradientAlphaKey(0.2f, 1f)
    },
    colorKeys = new GradientColorKey[3]
    {
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 0.0f),
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 0.5f),
      new GradientColorKey((Color) new Color32((byte) 25, (byte) 40, (byte) 65, byte.MaxValue), 1f)
    }
  };
  [Tooltip("Intensity of the light source.")]
  [Range(0.0f, 1f)]
  public float LightIntensity = 0.1f;
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
