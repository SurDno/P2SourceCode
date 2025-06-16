using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Cinematic/Lens Aberrations")]
  public class LensAberrations : MonoBehaviour
  {
    [LensAberrations.SettingsGroup]
    public LensAberrations.DistortionSettings distortion = LensAberrations.DistortionSettings.defaultSettings;
    [LensAberrations.SettingsGroup]
    public LensAberrations.VignetteSettings vignette = LensAberrations.VignetteSettings.defaultSettings;
    [LensAberrations.SettingsGroup]
    public LensAberrations.ChromaticAberrationSettings chromaticAberration = LensAberrations.ChromaticAberrationSettings.defaultSettings;
    [SerializeField]
    private Shader m_Shader;
    private Material m_Material;
    private RenderTextureUtility m_RTU;

    public Shader shader
    {
      get
      {
        if ((UnityEngine.Object) this.m_Shader == (UnityEngine.Object) null)
          this.m_Shader = Shader.Find("Hidden/LensAberrations");
        return this.m_Shader;
      }
    }

    public Material material
    {
      get
      {
        if ((UnityEngine.Object) this.m_Material == (UnityEngine.Object) null)
          this.m_Material = ImageEffectHelper.CheckShaderAndCreateMaterial(this.shader);
        return this.m_Material;
      }
    }

    private void OnEnable()
    {
      if (!ImageEffectHelper.IsSupported(this.shader, false, false, (MonoBehaviour) this))
        this.enabled = false;
      this.m_RTU = new RenderTextureUtility();
    }

    private void OnDisable()
    {
      if ((UnityEngine.Object) this.m_Material != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_Material);
      this.m_Material = (Material) null;
      this.m_RTU.ReleaseAllTemporaryRenderTextures();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!this.vignette.enabled && !this.chromaticAberration.enabled && !this.distortion.enabled)
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        this.material.shaderKeywords = (string[]) null;
        if (this.distortion.enabled)
        {
          float num = (float) Math.PI / 180f * Math.Min(160f, 1.6f * Math.Max(Mathf.Abs(this.distortion.amount), 1f));
          float y = 2f * Mathf.Tan(num * 0.5f);
          Vector4 vector4 = new Vector4(this.distortion.centerX, this.distortion.centerY, Mathf.Max(this.distortion.amountX, 0.0001f), Mathf.Max(this.distortion.amountY, 0.0001f));
          Vector3 vector3 = new Vector3((double) this.distortion.amount >= 0.0 ? num : 1f / num, y, 1f / this.distortion.scale);
          this.material.EnableKeyword((double) this.distortion.amount >= 0.0 ? "DISTORT" : "UNDISTORT");
          this.material.SetVector("_DistCenterScale", vector4);
          this.material.SetVector("_DistAmount", (Vector4) vector3);
        }
        if (this.chromaticAberration.enabled)
        {
          this.material.EnableKeyword("CHROMATIC_ABERRATION");
          this.material.SetVector("_ChromaticAberration", new Vector4(this.chromaticAberration.color.r, this.chromaticAberration.color.g, this.chromaticAberration.color.b, this.chromaticAberration.amount * (1f / 1000f)));
        }
        if (this.vignette.enabled)
        {
          this.material.SetColor("_VignetteColor", this.vignette.color);
          if ((double) this.vignette.blur > 0.0)
          {
            int width = source.width / 2;
            int height = source.height / 2;
            RenderTexture temporaryRenderTexture1 = this.m_RTU.GetTemporaryRenderTexture(width, height, format: source.format);
            RenderTexture temporaryRenderTexture2 = this.m_RTU.GetTemporaryRenderTexture(width, height, format: source.format);
            this.material.SetVector("_BlurPass", (Vector4) new Vector2(1f / (float) width, 0.0f));
            Graphics.Blit((Texture) source, temporaryRenderTexture1, this.material, 0);
            if (this.distortion.enabled)
            {
              this.material.DisableKeyword("DISTORT");
              this.material.DisableKeyword("UNDISTORT");
            }
            this.material.SetVector("_BlurPass", (Vector4) new Vector2(0.0f, 1f / (float) height));
            Graphics.Blit((Texture) temporaryRenderTexture1, temporaryRenderTexture2, this.material, 0);
            this.material.SetVector("_BlurPass", (Vector4) new Vector2(1f / (float) width, 0.0f));
            Graphics.Blit((Texture) temporaryRenderTexture2, temporaryRenderTexture1, this.material, 0);
            this.material.SetVector("_BlurPass", (Vector4) new Vector2(0.0f, 1f / (float) height));
            Graphics.Blit((Texture) temporaryRenderTexture1, temporaryRenderTexture2, this.material, 0);
            this.material.SetTexture("_BlurTex", (Texture) temporaryRenderTexture2);
            this.material.SetFloat("_VignetteBlur", this.vignette.blur * 3f);
            this.material.EnableKeyword("VIGNETTE_BLUR");
            if (this.distortion.enabled)
              this.material.EnableKeyword((double) this.distortion.amount >= 0.0 ? "DISTORT" : "UNDISTORT");
          }
          if ((double) this.vignette.desaturate > 0.0)
          {
            this.material.EnableKeyword("VIGNETTE_DESAT");
            this.material.SetFloat("_VignetteDesat", 1f - this.vignette.desaturate);
          }
          this.material.SetVector("_VignetteCenter", (Vector4) this.vignette.center);
          if (Mathf.Approximately(this.vignette.roundness, 1f))
          {
            this.material.EnableKeyword("VIGNETTE_CLASSIC");
            this.material.SetVector("_VignetteSettings", (Vector4) new Vector2(this.vignette.intensity, this.vignette.smoothness));
          }
          else
          {
            this.material.EnableKeyword("VIGNETTE_FILMIC");
            this.material.SetVector("_VignetteSettings", (Vector4) new Vector3(this.vignette.intensity, this.vignette.smoothness, (float) ((1.0 - (double) this.vignette.roundness) * 6.0) + this.vignette.roundness));
          }
        }
        int pass = 0;
        if (this.vignette.enabled && this.chromaticAberration.enabled && this.distortion.enabled)
          pass = 7;
        else if (this.vignette.enabled && this.chromaticAberration.enabled)
          pass = 5;
        else if (this.vignette.enabled && this.distortion.enabled)
          pass = 6;
        else if (this.chromaticAberration.enabled && this.distortion.enabled)
          pass = 4;
        else if (this.vignette.enabled)
          pass = 3;
        else if (this.chromaticAberration.enabled)
          pass = 1;
        else if (this.distortion.enabled)
          pass = 2;
        Graphics.Blit((Texture) source, destination, this.material, pass);
        this.m_RTU.ReleaseAllTemporaryRenderTextures();
      }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SettingsGroup : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AdvancedSetting : Attribute
    {
    }

    [Serializable]
    public struct DistortionSettings
    {
      public bool enabled;
      [Range(-100f, 100f)]
      [Tooltip("Distortion amount.")]
      public float amount;
      [Range(-1f, 1f)]
      [Tooltip("Distortion center point (X axis).")]
      public float centerX;
      [Range(-1f, 1f)]
      [Tooltip("Distortion center point (Y axis).")]
      public float centerY;
      [Range(0.0f, 1f)]
      [Tooltip("Amount multiplier on X axis. Set it to 0 to disable distortion on this axis.")]
      public float amountX;
      [Range(0.0f, 1f)]
      [Tooltip("Amount multiplier on Y axis. Set it to 0 to disable distortion on this axis.")]
      public float amountY;
      [Range(0.01f, 5f)]
      [Tooltip("Global screen scaling.")]
      public float scale;

      public static LensAberrations.DistortionSettings defaultSettings
      {
        get
        {
          return new LensAberrations.DistortionSettings()
          {
            enabled = false,
            amount = 0.0f,
            centerX = 0.0f,
            centerY = 0.0f,
            amountX = 1f,
            amountY = 1f,
            scale = 1f
          };
        }
      }
    }

    [Serializable]
    public struct VignetteSettings
    {
      public bool enabled;
      [ColorUsage(false)]
      [Tooltip("Vignette color. Use the alpha channel for transparency.")]
      public Color color;
      [Tooltip("Sets the vignette center point (screen center is [0.5,0.5]).")]
      public Vector2 center;
      [Range(0.0f, 3f)]
      [Tooltip("Amount of vignetting on screen.")]
      public float intensity;
      [Range(0.01f, 3f)]
      [Tooltip("Smoothness of the vignette borders.")]
      public float smoothness;
      [LensAberrations.AdvancedSetting]
      [Range(0.0f, 1f)]
      [Tooltip("Lower values will make a square-ish vignette.")]
      public float roundness;
      [Range(0.0f, 1f)]
      [Tooltip("Blurs the corners of the screen. Leave this at 0 to disable it.")]
      public float blur;
      [Range(0.0f, 1f)]
      [Tooltip("Desaturate the corners of the screen. Leave this to 0 to disable it.")]
      public float desaturate;

      public static LensAberrations.VignetteSettings defaultSettings
      {
        get
        {
          return new LensAberrations.VignetteSettings()
          {
            enabled = false,
            color = new Color(0.0f, 0.0f, 0.0f, 1f),
            center = new Vector2(0.5f, 0.5f),
            intensity = 1.4f,
            smoothness = 0.8f,
            roundness = 1f,
            blur = 0.0f,
            desaturate = 0.0f
          };
        }
      }
    }

    [Serializable]
    public struct ChromaticAberrationSettings
    {
      public bool enabled;
      [ColorUsage(false)]
      [Tooltip("Channels to apply chromatic aberration to.")]
      public Color color;
      [Range(-50f, 50f)]
      [Tooltip("Amount of tangential distortion.")]
      public float amount;

      public static LensAberrations.ChromaticAberrationSettings defaultSettings
      {
        get
        {
          return new LensAberrations.ChromaticAberrationSettings()
          {
            enabled = false,
            color = Color.green,
            amount = 0.0f
          };
        }
      }
    }

    private enum Pass
    {
      BlurPrePass,
      Chroma,
      Distort,
      Vignette,
      ChromaDistort,
      ChromaVignette,
      DistortVignette,
      ChromaDistortVignette,
    }
  }
}
