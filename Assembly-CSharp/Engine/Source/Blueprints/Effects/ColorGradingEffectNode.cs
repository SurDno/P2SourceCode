using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class ColorGradingEffectNode : FlowControlNode, IUpdatable {
	[Port("Post Exposure")] private ValueInput<float> postExposureInput;
	[Port("Temperature")] private ValueInput<float> temperatureInput;
	[Port("Tint")] private ValueInput<float> tintInput;
	[Port("Hue Shift")] private ValueInput<float> hueShiftInput;
	[Port("Saturation")] private ValueInput<float> saturationInput;
	[Port("Contrast")] private ValueInput<float> contrastInput;
	[Port("Red Color")] private ValueInput<Color> redColorInput;
	[Port("Green Color")] private ValueInput<Color> greenColorInput;
	[Port("Blue Color")] private ValueInput<Color> blueColorInput;
	private float prevPostExposure;
	private float prevTemperature;
	private float prevTint;
	private float prevHueShift;
	private float prevSaturation;
	private float prevContrast;
	private Color prevRedColor;
	private Color prevGreenColor;
	private Color prevBlueColor;
	private PostProcessingStackOverride postProcessingOverride;

	public void Update() {
		if (postProcessingOverride == null)
			GetOverrideColorGrading();
		else {
			var num1 = postExposureInput.value;
			if (prevPostExposure != (double)num1) {
				postProcessingOverride.ColorGrading.Basic.postExposure = num1;
				prevPostExposure = num1;
			}

			var num2 = temperatureInput.value;
			if (prevTemperature != (double)num2) {
				postProcessingOverride.ColorGrading.Basic.temperature = num2;
				prevTemperature = num2;
			}

			var num3 = tintInput.value;
			if (prevTint != (double)num3) {
				postProcessingOverride.ColorGrading.Basic.tint = num3;
				prevTint = num3;
			}

			var num4 = hueShiftInput.value;
			if (prevHueShift != (double)num4) {
				postProcessingOverride.ColorGrading.Basic.hueShift = num4;
				prevHueShift = num4;
			}

			var num5 = saturationInput.value;
			if (prevSaturation != (double)num5) {
				postProcessingOverride.ColorGrading.Basic.saturation = num5;
				prevSaturation = num5;
			}

			var num6 = contrastInput.value;
			if (prevContrast != (double)num6) {
				postProcessingOverride.ColorGrading.Basic.contrast = num6;
				prevContrast = num6;
			}

			var color1 = redColorInput.value;
			if (prevRedColor != color1) {
				postProcessingOverride.ColorGrading.ChannelMixer.red = new Vector3(color1.r, color1.g, color1.b);
				prevRedColor = color1;
			}

			var color2 = greenColorInput.value;
			if (prevGreenColor != color2) {
				postProcessingOverride.ColorGrading.ChannelMixer.green = new Vector3(color2.r, color2.g, color2.b);
				prevGreenColor = color2;
			}

			var color3 = blueColorInput.value;
			if (!(prevBlueColor != color3))
				return;
			postProcessingOverride.ColorGrading.ChannelMixer.blue = new Vector3(color3.r, color3.g, color3.b);
			prevBlueColor = color3;
		}
	}

	private void GetOverrideColorGrading() {
		postProcessingOverride = GameCamera.Instance.GamePostProcessingOverride;
		if (!(postProcessingOverride != null))
			return;
		postProcessingOverride.ColorGrading.Override = true;
		postProcessingOverride.ColorGrading.Enabled = true;
	}

	public override void OnDestroy() {
		base.OnDestroy();
		if (!(postProcessingOverride != null))
			return;
		postProcessingOverride.ColorGrading.Override = false;
	}
}