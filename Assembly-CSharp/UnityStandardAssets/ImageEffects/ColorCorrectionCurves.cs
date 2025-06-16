using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Color Adjustments/Color Correction (Curves, Saturation)")]
  public class ColorCorrectionCurves : PostEffectsBase
  {
    public AnimationCurve redChannel = new AnimationCurve(new Keyframe[2]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(1f, 1f)
    });
    public AnimationCurve greenChannel = new AnimationCurve(new Keyframe[2]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(1f, 1f)
    });
    public AnimationCurve blueChannel = new AnimationCurve(new Keyframe[2]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(1f, 1f)
    });
    public bool useDepthCorrection = false;
    public AnimationCurve zCurve = new AnimationCurve(new Keyframe[2]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(1f, 1f)
    });
    public AnimationCurve depthRedChannel = new AnimationCurve(new Keyframe[2]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(1f, 1f)
    });
    public AnimationCurve depthGreenChannel = new AnimationCurve(new Keyframe[2]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(1f, 1f)
    });
    public AnimationCurve depthBlueChannel = new AnimationCurve(new Keyframe[2]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(1f, 1f)
    });
    private Material ccMaterial;
    private Material ccDepthMaterial;
    private Material selectiveCcMaterial;
    private Texture2D rgbChannelTex;
    private Texture2D rgbDepthChannelTex;
    private Texture2D zCurveTex;
    public float saturation = 1f;
    public bool selectiveCc = false;
    public Color selectiveFromColor = Color.white;
    public Color selectiveToColor = Color.white;
    public ColorCorrectionCurves.ColorCorrectionMode mode;
    public bool updateTextures = true;
    public Shader colorCorrectionCurvesShader = (Shader) null;
    public Shader simpleColorCorrectionCurvesShader = (Shader) null;
    public Shader colorCorrectionSelectiveShader = (Shader) null;
    private bool updateTexturesOnStartup = true;

    private new void Start()
    {
      base.Start();
      this.updateTexturesOnStartup = true;
    }

    private void Awake()
    {
    }

    public override bool CheckResources()
    {
      this.CheckSupport(this.mode == ColorCorrectionCurves.ColorCorrectionMode.Advanced);
      this.ccMaterial = this.CheckShaderAndCreateMaterial(this.simpleColorCorrectionCurvesShader, this.ccMaterial);
      this.ccDepthMaterial = this.CheckShaderAndCreateMaterial(this.colorCorrectionCurvesShader, this.ccDepthMaterial);
      this.selectiveCcMaterial = this.CheckShaderAndCreateMaterial(this.colorCorrectionSelectiveShader, this.selectiveCcMaterial);
      if (!(bool) (Object) this.rgbChannelTex)
        this.rgbChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, false, true);
      if (!(bool) (Object) this.rgbDepthChannelTex)
        this.rgbDepthChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, false, true);
      if (!(bool) (Object) this.zCurveTex)
        this.zCurveTex = new Texture2D(256, 1, TextureFormat.ARGB32, false, true);
      this.rgbChannelTex.hideFlags = HideFlags.DontSave;
      this.rgbDepthChannelTex.hideFlags = HideFlags.DontSave;
      this.zCurveTex.hideFlags = HideFlags.DontSave;
      this.rgbChannelTex.wrapMode = TextureWrapMode.Clamp;
      this.rgbDepthChannelTex.wrapMode = TextureWrapMode.Clamp;
      this.zCurveTex.wrapMode = TextureWrapMode.Clamp;
      if (!this.isSupported)
        this.ReportAutoDisable();
      return this.isSupported;
    }

    public void UpdateParameters()
    {
      this.CheckResources();
      if (this.redChannel == null || this.greenChannel == null || this.blueChannel == null)
        return;
      for (float time = 0.0f; (double) time <= 1.0; time += 0.003921569f)
      {
        float num1 = Mathf.Clamp(this.redChannel.Evaluate(time), 0.0f, 1f);
        float num2 = Mathf.Clamp(this.greenChannel.Evaluate(time), 0.0f, 1f);
        float num3 = Mathf.Clamp(this.blueChannel.Evaluate(time), 0.0f, 1f);
        this.rgbChannelTex.SetPixel((int) Mathf.Floor(time * (float) byte.MaxValue), 0, new Color(num1, num1, num1));
        this.rgbChannelTex.SetPixel((int) Mathf.Floor(time * (float) byte.MaxValue), 1, new Color(num2, num2, num2));
        this.rgbChannelTex.SetPixel((int) Mathf.Floor(time * (float) byte.MaxValue), 2, new Color(num3, num3, num3));
        float num4 = Mathf.Clamp(this.zCurve.Evaluate(time), 0.0f, 1f);
        this.zCurveTex.SetPixel((int) Mathf.Floor(time * (float) byte.MaxValue), 0, new Color(num4, num4, num4));
        float num5 = Mathf.Clamp(this.depthRedChannel.Evaluate(time), 0.0f, 1f);
        float num6 = Mathf.Clamp(this.depthGreenChannel.Evaluate(time), 0.0f, 1f);
        float num7 = Mathf.Clamp(this.depthBlueChannel.Evaluate(time), 0.0f, 1f);
        this.rgbDepthChannelTex.SetPixel((int) Mathf.Floor(time * (float) byte.MaxValue), 0, new Color(num5, num5, num5));
        this.rgbDepthChannelTex.SetPixel((int) Mathf.Floor(time * (float) byte.MaxValue), 1, new Color(num6, num6, num6));
        this.rgbDepthChannelTex.SetPixel((int) Mathf.Floor(time * (float) byte.MaxValue), 2, new Color(num7, num7, num7));
      }
      this.rgbChannelTex.Apply();
      this.rgbDepthChannelTex.Apply();
      this.zCurveTex.Apply();
    }

    private void UpdateTextures() => this.UpdateParameters();

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!this.CheckResources())
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        if (this.updateTexturesOnStartup)
        {
          this.UpdateParameters();
          this.updateTexturesOnStartup = false;
        }
        if (this.useDepthCorrection)
          this.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
        RenderTexture renderTexture = destination;
        if (this.selectiveCc)
          renderTexture = RenderTexture.GetTemporary(source.width, source.height);
        if (this.useDepthCorrection)
        {
          this.ccDepthMaterial.SetTexture("_RgbTex", (Texture) this.rgbChannelTex);
          this.ccDepthMaterial.SetTexture("_ZCurve", (Texture) this.zCurveTex);
          this.ccDepthMaterial.SetTexture("_RgbDepthTex", (Texture) this.rgbDepthChannelTex);
          this.ccDepthMaterial.SetFloat("_Saturation", this.saturation);
          Graphics.Blit((Texture) source, renderTexture, this.ccDepthMaterial);
        }
        else
        {
          this.ccMaterial.SetTexture("_RgbTex", (Texture) this.rgbChannelTex);
          this.ccMaterial.SetFloat("_Saturation", this.saturation);
          Graphics.Blit((Texture) source, renderTexture, this.ccMaterial);
        }
        if (!this.selectiveCc)
          return;
        this.selectiveCcMaterial.SetColor("selColor", this.selectiveFromColor);
        this.selectiveCcMaterial.SetColor("targetColor", this.selectiveToColor);
        Graphics.Blit((Texture) renderTexture, destination, this.selectiveCcMaterial);
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
