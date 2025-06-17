using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Color Adjustments/Color Correction (Curves, Saturation)")]
  public class ColorCorrectionCurves : PostEffectsBase
  {
    public AnimationCurve redChannel = new(new Keyframe(0.0f, 0.0f), new Keyframe(1f, 1f));
    public AnimationCurve greenChannel = new(new Keyframe(0.0f, 0.0f), new Keyframe(1f, 1f));
    public AnimationCurve blueChannel = new(new Keyframe(0.0f, 0.0f), new Keyframe(1f, 1f));
    public bool useDepthCorrection;
    public AnimationCurve zCurve = new(new Keyframe(0.0f, 0.0f), new Keyframe(1f, 1f));
    public AnimationCurve depthRedChannel = new(new Keyframe(0.0f, 0.0f), new Keyframe(1f, 1f));
    public AnimationCurve depthGreenChannel = new(new Keyframe(0.0f, 0.0f), new Keyframe(1f, 1f));
    public AnimationCurve depthBlueChannel = new(new Keyframe(0.0f, 0.0f), new Keyframe(1f, 1f));
    private Material ccMaterial;
    private Material ccDepthMaterial;
    private Material selectiveCcMaterial;
    private Texture2D rgbChannelTex;
    private Texture2D rgbDepthChannelTex;
    private Texture2D zCurveTex;
    public float saturation = 1f;
    public bool selectiveCc;
    public Color selectiveFromColor = Color.white;
    public Color selectiveToColor = Color.white;
    public ColorCorrectionMode mode;
    public bool updateTextures = true;
    public Shader colorCorrectionCurvesShader;
    public Shader simpleColorCorrectionCurvesShader;
    public Shader colorCorrectionSelectiveShader;
    private bool updateTexturesOnStartup = true;

    private new void Start()
    {
      base.Start();
      updateTexturesOnStartup = true;
    }

    private void Awake()
    {
    }

    public override bool CheckResources()
    {
      CheckSupport(mode == ColorCorrectionMode.Advanced);
      ccMaterial = CheckShaderAndCreateMaterial(simpleColorCorrectionCurvesShader, ccMaterial);
      ccDepthMaterial = CheckShaderAndCreateMaterial(colorCorrectionCurvesShader, ccDepthMaterial);
      selectiveCcMaterial = CheckShaderAndCreateMaterial(colorCorrectionSelectiveShader, selectiveCcMaterial);
      if (!(bool) (Object) rgbChannelTex)
        rgbChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, false, true);
      if (!(bool) (Object) rgbDepthChannelTex)
        rgbDepthChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, false, true);
      if (!(bool) (Object) zCurveTex)
        zCurveTex = new Texture2D(256, 1, TextureFormat.ARGB32, false, true);
      rgbChannelTex.hideFlags = HideFlags.DontSave;
      rgbDepthChannelTex.hideFlags = HideFlags.DontSave;
      zCurveTex.hideFlags = HideFlags.DontSave;
      rgbChannelTex.wrapMode = TextureWrapMode.Clamp;
      rgbDepthChannelTex.wrapMode = TextureWrapMode.Clamp;
      zCurveTex.wrapMode = TextureWrapMode.Clamp;
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    public void UpdateParameters()
    {
      CheckResources();
      if (redChannel == null || greenChannel == null || blueChannel == null)
        return;
      for (float time = 0.0f; time <= 1.0; time += 0.003921569f)
      {
        float num1 = Mathf.Clamp(redChannel.Evaluate(time), 0.0f, 1f);
        float num2 = Mathf.Clamp(greenChannel.Evaluate(time), 0.0f, 1f);
        float num3 = Mathf.Clamp(blueChannel.Evaluate(time), 0.0f, 1f);
        rgbChannelTex.SetPixel((int) Mathf.Floor(time * byte.MaxValue), 0, new Color(num1, num1, num1));
        rgbChannelTex.SetPixel((int) Mathf.Floor(time * byte.MaxValue), 1, new Color(num2, num2, num2));
        rgbChannelTex.SetPixel((int) Mathf.Floor(time * byte.MaxValue), 2, new Color(num3, num3, num3));
        float num4 = Mathf.Clamp(zCurve.Evaluate(time), 0.0f, 1f);
        zCurveTex.SetPixel((int) Mathf.Floor(time * byte.MaxValue), 0, new Color(num4, num4, num4));
        float num5 = Mathf.Clamp(depthRedChannel.Evaluate(time), 0.0f, 1f);
        float num6 = Mathf.Clamp(depthGreenChannel.Evaluate(time), 0.0f, 1f);
        float num7 = Mathf.Clamp(depthBlueChannel.Evaluate(time), 0.0f, 1f);
        rgbDepthChannelTex.SetPixel((int) Mathf.Floor(time * byte.MaxValue), 0, new Color(num5, num5, num5));
        rgbDepthChannelTex.SetPixel((int) Mathf.Floor(time * byte.MaxValue), 1, new Color(num6, num6, num6));
        rgbDepthChannelTex.SetPixel((int) Mathf.Floor(time * byte.MaxValue), 2, new Color(num7, num7, num7));
      }
      rgbChannelTex.Apply();
      rgbDepthChannelTex.Apply();
      zCurveTex.Apply();
    }

    private void UpdateTextures() => UpdateParameters();

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit(source, destination);
      }
      else
      {
        if (updateTexturesOnStartup)
        {
          UpdateParameters();
          updateTexturesOnStartup = false;
        }
        if (useDepthCorrection)
          GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
        RenderTexture renderTexture = destination;
        if (selectiveCc)
          renderTexture = RenderTexture.GetTemporary(source.width, source.height);
        if (useDepthCorrection)
        {
          ccDepthMaterial.SetTexture("_RgbTex", rgbChannelTex);
          ccDepthMaterial.SetTexture("_ZCurve", zCurveTex);
          ccDepthMaterial.SetTexture("_RgbDepthTex", rgbDepthChannelTex);
          ccDepthMaterial.SetFloat("_Saturation", saturation);
          Graphics.Blit(source, renderTexture, ccDepthMaterial);
        }
        else
        {
          ccMaterial.SetTexture("_RgbTex", rgbChannelTex);
          ccMaterial.SetFloat("_Saturation", saturation);
          Graphics.Blit(source, renderTexture, ccMaterial);
        }
        if (!selectiveCc)
          return;
        selectiveCcMaterial.SetColor("selColor", selectiveFromColor);
        selectiveCcMaterial.SetColor("targetColor", selectiveToColor);
        Graphics.Blit(renderTexture, destination, selectiveCcMaterial);
        RenderTexture.ReleaseTemporary(renderTexture);
      }
    }

    public enum ColorCorrectionMode
    {
      Simple,
      Advanced,
    }
  }
}
