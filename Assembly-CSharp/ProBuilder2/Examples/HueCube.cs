using ProBuilder2.Common;
using UnityEngine;

namespace ProBuilder2.Examples;

public class HueCube : MonoBehaviour {
	private pb_Object pb;

	private void Start() {
		pb = pb_ShapeGenerator.CubeGenerator(Vector3.one);
		var length = pb.sharedIndices.Length;
		var colorArray = new Color[length];
		for (var index = 0; index < length; ++index)
			colorArray[index] = HSVtoRGB((float)(index / (double)length * 360.0), 1f, 1f);
		var colors = pb.colors;
		for (var index1 = 0; index1 < pb.sharedIndices.Length; ++index1)
			foreach (var index2 in pb.sharedIndices[index1].array)
				colors[index2] = colorArray[index1];
		pb.SetColors(colors);
		pb.Refresh();
	}

	private static Color HSVtoRGB(float h, float s, float v) {
		if (s == 0.0)
			return new Color(v, v, v, 1f);
		h /= 60f;
		var num1 = (int)Mathf.Floor(h);
		var num2 = h - num1;
		var num3 = v * (1f - s);
		var num4 = v * (float)(1.0 - s * (double)num2);
		var num5 = v * (float)(1.0 - s * (1.0 - num2));
		float r;
		float g;
		float b;
		switch (num1) {
			case 0:
				r = v;
				g = num5;
				b = num3;
				break;
			case 1:
				r = num4;
				g = v;
				b = num3;
				break;
			case 2:
				r = num3;
				g = v;
				b = num5;
				break;
			case 3:
				r = num3;
				g = num4;
				b = v;
				break;
			case 4:
				r = num5;
				g = num3;
				b = v;
				break;
			default:
				r = v;
				g = num3;
				b = num4;
				break;
		}

		return new Color(r, g, b, 1f);
	}
}