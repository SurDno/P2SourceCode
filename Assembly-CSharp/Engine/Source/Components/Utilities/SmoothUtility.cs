using UnityEngine;

namespace Engine.Source.Components.Utilities;

public static class SmoothUtility {
	public static float Smooth12(float x) {
		var num = 1f - x;
		return Mathf.Lerp(x, (float)(1.0 - num * (double)num), x);
	}

	public static float Smooth13(float x) {
		var num = 1f - x;
		return Mathf.Lerp(x, (float)(1.0 - num * (double)num * num), x);
	}

	public static float Smooth22(float x) {
		var num = 1f - x;
		return Mathf.Lerp(x * x, (float)(1.0 - num * (double)num), x);
	}

	public static float Smooth32(float x) {
		var num = 1f - x;
		return Mathf.Lerp(x * x * x, (float)(1.0 - num * (double)num), x);
	}

	public static float Smooth33(float x) {
		var num = 1f - x;
		return Mathf.Lerp(x * x * x, (float)(1.0 - num * (double)num * num), x);
	}

	public static float Smooth42(float x) {
		var num = 1f - x;
		return Mathf.Lerp(x * x * x * x, (float)(1.0 - num * (double)num), x);
	}

	public static float Smooth43(float x) {
		var num = 1f - x;
		return Mathf.Lerp(x * x * x * x, (float)(1.0 - num * (double)num * num), x);
	}

	public static float Smooth44(float x) {
		var num = 1f - x;
		return Mathf.Lerp(x * x * x * x, (float)(1.0 - num * (double)num * num * num), x);
	}
}