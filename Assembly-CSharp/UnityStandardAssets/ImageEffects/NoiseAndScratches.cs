﻿using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Noise/Noise and Scratches")]
  public class NoiseAndScratches : MonoBehaviour
  {
    public bool monochrome = true;
    private bool rgbFallback;
    [Range(0.0f, 5f)]
    public float grainIntensityMin = 0.1f;
    [Range(0.0f, 5f)]
    public float grainIntensityMax = 0.2f;
    [Range(0.1f, 50f)]
    public float grainSize = 2f;
    [Range(0.0f, 5f)]
    public float scratchIntensityMin = 0.05f;
    [Range(0.0f, 5f)]
    public float scratchIntensityMax = 0.25f;
    [Range(1f, 30f)]
    public float scratchFPS = 10f;
    [Range(0.0f, 1f)]
    public float scratchJitter = 0.01f;
    public Texture grainTexture;
    public Texture scratchTexture;
    public Shader shaderRGB;
    public Shader shaderYUV;
    private Material m_MaterialRGB;
    private Material m_MaterialYUV;
    private float scratchTimeLeft;
    private float scratchX;
    private float scratchY;

    protected void Start()
    {
      if (!SystemInfo.supportsImageEffects)
        enabled = false;
      else if (shaderRGB == null || shaderYUV == null)
      {
        Debug.Log("Noise shaders are not set up! Disabling noise effect.");
        enabled = false;
      }
      else if (!shaderRGB.isSupported)
        enabled = false;
      else if (!shaderYUV.isSupported)
        rgbFallback = true;
    }

    protected Material material
    {
      get
      {
        if (m_MaterialRGB == null)
        {
          m_MaterialRGB = new Material(shaderRGB);
          m_MaterialRGB.hideFlags = HideFlags.HideAndDontSave;
        }
        if (m_MaterialYUV == null && !rgbFallback)
        {
          m_MaterialYUV = new Material(shaderYUV);
          m_MaterialYUV.hideFlags = HideFlags.HideAndDontSave;
        }
        return rgbFallback || monochrome ? m_MaterialRGB : m_MaterialYUV;
      }
    }

    protected void OnDisable()
    {
      if ((bool) (Object) m_MaterialRGB)
        DestroyImmediate(m_MaterialRGB);
      if (!(bool) (Object) m_MaterialYUV)
        return;
      DestroyImmediate(m_MaterialYUV);
    }

    private void SanitizeParameters()
    {
      grainIntensityMin = Mathf.Clamp(grainIntensityMin, 0.0f, 5f);
      grainIntensityMax = Mathf.Clamp(grainIntensityMax, 0.0f, 5f);
      scratchIntensityMin = Mathf.Clamp(scratchIntensityMin, 0.0f, 5f);
      scratchIntensityMax = Mathf.Clamp(scratchIntensityMax, 0.0f, 5f);
      scratchFPS = Mathf.Clamp(scratchFPS, 1f, 30f);
      scratchJitter = Mathf.Clamp(scratchJitter, 0.0f, 1f);
      grainSize = Mathf.Clamp(grainSize, 0.1f, 50f);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      SanitizeParameters();
      if (scratchTimeLeft <= 0.0)
      {
        scratchTimeLeft = Random.value * 2f / scratchFPS;
        scratchX = Random.value;
        scratchY = Random.value;
      }
      scratchTimeLeft -= Time.deltaTime;
      Material material = this.material;
      material.SetTexture("_GrainTex", grainTexture);
      material.SetTexture("_ScratchTex", scratchTexture);
      float num = 1f / grainSize;
      material.SetVector("_GrainOffsetScale", new Vector4(Random.value, Random.value, Screen.width / (float) grainTexture.width * num, Screen.height / (float) grainTexture.height * num));
      material.SetVector("_ScratchOffsetScale", new Vector4(scratchX + Random.value * scratchJitter, scratchY + Random.value * scratchJitter, Screen.width / (float) scratchTexture.width, Screen.height / (float) scratchTexture.height));
      material.SetVector("_Intensity", new Vector4(Random.Range(grainIntensityMin, grainIntensityMax), Random.Range(scratchIntensityMin, scratchIntensityMax), 0.0f, 0.0f));
      Graphics.Blit(source, destination, material);
    }
  }
}
