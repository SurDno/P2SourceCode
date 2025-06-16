using Engine.Common.Blenders;
using UnityEngine;

namespace Engine.Source.Blenders;

public interface IBlendOperation : IPureBlendOperation {
	float Blend(float a, float b);

	int Blend(int a, int b);

	Color Blend(Color a, Color b);

	Vector2 Blend(Vector2 a, Vector2 b);
}