using UnityEngine;

namespace Engine.Drawing.Gradient;

public static class GradientUtility {
	public static void Copy(UnityEngine.Gradient from, ColorGradient to) {
		if (from == null || to == null)
			return;
		to.AlphaKeys.Clear();
		to.ColorKeys.Clear();
		var colorKeys = from.colorKeys;
		for (var index = 0; index < colorKeys.Length; ++index)
			if (to.ColorKeys.Count <= index)
				to.ColorKeys.Add(colorKeys[index]);
			else
				to.ColorKeys[index] = colorKeys[index];
		var alphaKeys = from.alphaKeys;
		for (var index = 0; index < alphaKeys.Length; ++index)
			if (to.AlphaKeys.Count <= index)
				to.AlphaKeys.Add(alphaKeys[index]);
			else
				to.AlphaKeys[index] = alphaKeys[index];
	}

	public static void Copy(ColorGradient from, UnityEngine.Gradient to) {
		if (from == null || to == null)
			return;
		var gradientColorKeyArray = to.colorKeys;
		if (gradientColorKeyArray.Length != from.ColorKeys.Count)
			gradientColorKeyArray = new GradientColorKey[from.ColorKeys.Count];
		for (var index = 0; index < gradientColorKeyArray.Length; ++index)
			gradientColorKeyArray[index] = from.ColorKeys[index];
		to.colorKeys = gradientColorKeyArray;
		var gradientAlphaKeyArray = to.alphaKeys;
		if (gradientAlphaKeyArray.Length != from.AlphaKeys.Count)
			gradientAlphaKeyArray = new GradientAlphaKey[from.AlphaKeys.Count];
		for (var index = 0; index < gradientAlphaKeyArray.Length; ++index)
			gradientAlphaKeyArray[index] = from.AlphaKeys[index];
		to.alphaKeys = gradientAlphaKeyArray;
	}
}