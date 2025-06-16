using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class ColorGradingEffectNode : FlowControlNode, IUpdatable
  {
    [Port("Post Exposure")]
    private ValueInput<float> postExposureInput;
    [Port("Temperature")]
    private ValueInput<float> temperatureInput;
    [Port("Tint")]
    private ValueInput<float> tintInput;
    [Port("Hue Shift")]
    private ValueInput<float> hueShiftInput;
    [Port("Saturation")]
    private ValueInput<float> saturationInput;
    [Port("Contrast")]
    private ValueInput<float> contrastInput;
    [Port("Red Color")]
    private ValueInput<Color> redColorInput;
    [Port("Green Color")]
    private ValueInput<Color> greenColorInput;
    [Port("Blue Color")]
    private ValueInput<Color> blueColorInput;
    private float prevPostExposure;
    private float prevTemperature;
    private float prevTint;
    private float prevHueShift;
    private float prevSaturation;
    private float prevContrast;
    private Color prevRedColor;
    private Color prevGreenColor;
    private Color prevBlueColor;
    private PostProcessingStackOverride postProcessingOverride = (PostProcessingStackOverride) null;

    public void Update()
    {
      if ((Object) this.postProcessingOverride == (Object) null)
      {
        this.GetOverrideColorGrading();
      }
      else
      {
        float num1 = this.postExposureInput.value;
        if ((double) this.prevPostExposure != (double) num1)
        {
          this.postProcessingOverride.ColorGrading.Basic.postExposure = num1;
          this.prevPostExposure = num1;
        }
        float num2 = this.temperatureInput.value;
        if ((double) this.prevTemperature != (double) num2)
        {
          this.postProcessingOverride.ColorGrading.Basic.temperature = num2;
          this.prevTemperature = num2;
        }
        float num3 = this.tintInput.value;
        if ((double) this.prevTint != (double) num3)
        {
          this.postProcessingOverride.ColorGrading.Basic.tint = num3;
          this.prevTint = num3;
        }
        float num4 = this.hueShiftInput.value;
        if ((double) this.prevHueShift != (double) num4)
        {
          this.postProcessingOverride.ColorGrading.Basic.hueShift = num4;
          this.prevHueShift = num4;
        }
        float num5 = this.saturationInput.value;
        if ((double) this.prevSaturation != (double) num5)
        {
          this.postProcessingOverride.ColorGrading.Basic.saturation = num5;
          this.prevSaturation = num5;
        }
        float num6 = this.contrastInput.value;
        if ((double) this.prevContrast != (double) num6)
        {
          this.postProcessingOverride.ColorGrading.Basic.contrast = num6;
          this.prevContrast = num6;
        }
        Color color1 = this.redColorInput.value;
        if (this.prevRedColor != color1)
        {
          this.postProcessingOverride.ColorGrading.ChannelMixer.red = new Vector3(color1.r, color1.g, color1.b);
          this.prevRedColor = color1;
        }
        Color color2 = this.greenColorInput.value;
        if (this.prevGreenColor != color2)
        {
          this.postProcessingOverride.ColorGrading.ChannelMixer.green = new Vector3(color2.r, color2.g, color2.b);
          this.prevGreenColor = color2;
        }
        Color color3 = this.blueColorInput.value;
        if (!(this.prevBlueColor != color3))
          return;
        this.postProcessingOverride.ColorGrading.ChannelMixer.blue = new Vector3(color3.r, color3.g, color3.b);
        this.prevBlueColor = color3;
      }
    }

    private void GetOverrideColorGrading()
    {
      this.postProcessingOverride = GameCamera.Instance.GamePostProcessingOverride;
      if (!((Object) this.postProcessingOverride != (Object) null))
        return;
      this.postProcessingOverride.ColorGrading.Override = true;
      this.postProcessingOverride.ColorGrading.Enabled = true;
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (!((Object) this.postProcessingOverride != (Object) null))
        return;
      this.postProcessingOverride.ColorGrading.Override = false;
    }
  }
}
