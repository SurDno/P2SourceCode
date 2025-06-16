using UnityEngine;

namespace Engine.Source.Utility;

public static class FunctionUtility {
	public static float DefaultFunction(float progress, float total) {
		return 1f;
	}

	public static float EyeFunction(float progress, float total) {
		var num = total * 0.5f;
		return (float)(Mathf.Clamp01(1f - Mathf.Pow((float)(progress / (double)num - 1.0), 2f)) * 0.5 + 0.5);
	}
}