﻿using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Cinematic/Lens Aberrations")]
  public class LensAberrations : MonoBehaviour
  {
    [SettingsGroup]
    public DistortionSettings distortion = DistortionSettings.defaultSettings;
    [SettingsGroup]
    public VignetteSettings vignette = VignetteSettings.defaultSettings;
    [SettingsGroup]
    public ChromaticAberrationSettings chromaticAberration = ChromaticAberrationSettings.defaultSettings;
    [SerializeField]
    private Shader m_Shader;
    private Material m_Material;
    private RenderTextureUtility m_RTU;

    public Shader shader
    {
      get
      {
        if (m_Shader == null)
          m_Shader = Shader.Find("Hidden/LensAberrations");
        return m_Shader;
      }
    }

    public Material material
    {
      get
      {
        if (m_Material == null)
          m_Material = ImageEffectHelper.CheckShaderAndCreateMaterial(shader);
        return m_Material;
      }
    }

    private void OnEnable()
    {
      if (!ImageEffectHelper.IsSupported(shader, false, false, this))
        enabled = false;
      m_RTU = new RenderTextureUtility();
    }

    private void OnDisable()
    {
      if (m_Material != null)
        DestroyImmediate(m_Material);
      m_Material = null;
      m_RTU.ReleaseAllTemporaryRenderTextures();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!vignette.enabled && !chromaticAberration.enabled && !distortion.enabled)
      {
        Graphics.Blit(source, destination);
      }
      else
      {
        material.shaderKeywords = null;
        if (distortion.enabled)
        {
          float num = (float) Math.PI / 180f * Math.Min(160f, 1.6f * Math.Max(Mathf.Abs(distortion.amount), 1f));
          float y = 2f * Mathf.Tan(num * 0.5f);
          Vector4 vector4 = new Vector4(distortion.centerX, distortion.centerY, Mathf.Max(distortion.amountX, 0.0001f), Mathf.Max(distortion.amountY, 0.0001f));
          Vector3 vector3 = new Vector3(distortion.amount >= 0.0 ? num : 1f / num, y, 1f / distortion.scale);
          material.EnableKeyword(distortion.amount >= 0.0 ? "DISTORT" : "UNDISTORT");
          material.SetVector("_DistCenterScale", vector4);
          material.SetVector("_DistAmount", vector3);
        }
        if (chromaticAberration.enabled)
        {
          material.EnableKeyword("CHROMATIC_ABERRATION");
          material.SetVector("_ChromaticAberration", new Vector4(chromaticAberration.color.r, chromaticAberration.color.g, chromaticAberration.color.b, chromaticAberration.amount * (1f / 1000f)));
        }
        if (vignette.enabled)
        {
          material.SetColor("_VignetteColor", vignette.color);
          if (vignette.blur > 0.0)
          {
            int width = source.width / 2;
            int height = source.height / 2;
            RenderTexture temporaryRenderTexture1 = m_RTU.GetTemporaryRenderTexture(width, height, format: source.format);
            RenderTexture temporaryRenderTexture2 = m_RTU.GetTemporaryRenderTexture(width, height, format: source.format);
            material.SetVector("_BlurPass", new Vector2(1f / width, 0.0f));
            Graphics.Blit(source, temporaryRenderTexture1, material, 0);
            if (distortion.enabled)
            {
              material.DisableKeyword("DISTORT");
              material.DisableKeyword("UNDISTORT");
            }
            material.SetVector("_BlurPass", new Vector2(0.0f, 1f / height));
            Graphics.Blit(temporaryRenderTexture1, temporaryRenderTexture2, material, 0);
            material.SetVector("_BlurPass", new Vector2(1f / width, 0.0f));
            Graphics.Blit(temporaryRenderTexture2, temporaryRenderTexture1, material, 0);
            material.SetVector("_BlurPass", new Vector2(0.0f, 1f / height));
            Graphics.Blit(temporaryRenderTexture1, temporaryRenderTexture2, material, 0);
            material.SetTexture("_BlurTex", temporaryRenderTexture2);
            material.SetFloat("_VignetteBlur", vignette.blur * 3f);
            material.EnableKeyword("VIGNETTE_BLUR");
            if (distortion.enabled)
              material.EnableKeyword(distortion.amount >= 0.0 ? "DISTORT" : "UNDISTORT");
          }
          if (vignette.desaturate > 0.0)
          {
            material.EnableKeyword("VIGNETTE_DESAT");
            material.SetFloat("_VignetteDesat", 1f - vignette.desaturate);
          }
          material.SetVector("_VignetteCenter", vignette.center);
          if (Mathf.Approximately(vignette.roundness, 1f))
          {
            material.EnableKeyword("VIGNETTE_CLASSIC");
            material.SetVector("_VignetteSettings", new Vector2(vignette.intensity, vignette.smoothness));
          }
          else
          {
            material.EnableKeyword("VIGNETTE_FILMIC");
            material.SetVector("_VignetteSettings", new Vector3(vignette.intensity, vignette.smoothness, (float) ((1.0 - vignette.roundness) * 6.0) + vignette.roundness));
          }
        }
        int pass = 0;
        if (vignette.enabled && chromaticAberration.enabled && distortion.enabled)
          pass = 7;
        else if (vignette.enabled && chromaticAberration.enabled)
          pass = 5;
        else if (vignette.enabled && distortion.enabled)
          pass = 6;
        else if (chromaticAberration.enabled && distortion.enabled)
          pass = 4;
        else if (vignette.enabled)
          pass = 3;
        else if (chromaticAberration.enabled)
          pass = 1;
        else if (distortion.enabled)
          pass = 2;
        Graphics.Blit(source, destination, material, pass);
        m_RTU.ReleaseAllTemporaryRenderTextures();
      }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingsGroup : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
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

      public static DistortionSettings defaultSettings =>
        new() {
          enabled = false,
          amount = 0.0f,
          centerX = 0.0f,
          centerY = 0.0f,
          amountX = 1f,
          amountY = 1f,
          scale = 1f
        };
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
      [AdvancedSetting]
      [Range(0.0f, 1f)]
      [Tooltip("Lower values will make a square-ish vignette.")]
      public float roundness;
      [Range(0.0f, 1f)]
      [Tooltip("Blurs the corners of the screen. Leave this at 0 to disable it.")]
      public float blur;
      [Range(0.0f, 1f)]
      [Tooltip("Desaturate the corners of the screen. Leave this to 0 to disable it.")]
      public float desaturate;

      public static VignetteSettings defaultSettings =>
        new() {
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

      public static ChromaticAberrationSettings defaultSettings =>
        new() {
          enabled = false,
          color = Color.green,
          amount = 0.0f
        };
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
