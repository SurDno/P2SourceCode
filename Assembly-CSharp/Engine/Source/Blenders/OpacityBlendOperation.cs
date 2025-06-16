using Engine.Common.Blenders;
using UnityEngine;

namespace Engine.Source.Blenders;

public class OpacityBlendOperation : IBlendOperation, IPureBlendOperation {
	public float Opacity { get; set; }

	public Color Blend(Color a, Color b) {
		return a * (1f - Opacity) + b * Opacity;
	}

	public Vector2 Blend(Vector2 a, Vector2 b) {
		return a * (1f - Opacity) + b * Opacity;
	}

	public int Blend(int a, int b) {
		return (int)(a * (1.0 - Opacity) + b * (double)Opacity);
	}

	public float Blend(float a, float b) {
		return (float)(a * (1.0 - Opacity) + b * (double)Opacity);
	}
}