using UnityEngine;

namespace SoundPropagation;

public class SPPortal : MonoBehaviour {
	public SPCell CellA;
	public SPCell CellB;
	public float Occlusion;
	private bool initialized;
	private Shape[] shapes;

	private void Check() {
		if (initialized)
			return;
		shapes = GetComponentsInChildren<Shape>();
		initialized = true;
	}

	public bool ClosestPointToSegment(Vector3 pointA, Vector3 pointB, out Vector3 output) {
		Check();
		if (shapes.Length < 1) {
			output = Vector3.zero;
			return false;
		}

		if (shapes.Length == 1)
			return shapes[0].ClosestPointToSegment(pointA, pointB, out output);
		var num1 = float.MaxValue;
		var vector3 = Vector3.zero;
		var segment = false;
		for (var index = 0; index < shapes.Length; ++index) {
			Vector3 output1;
			if (shapes[0].ClosestPointToSegment(pointA, pointB, out output1)) {
				var num2 = Vector3.Distance(pointA, output1) + Vector3.Distance(output1, pointB);
				if (num2 < (double)num1) {
					vector3 = output1;
					num1 = num2;
					segment = true;
				}
			}
		}

		output = vector3;
		return segment;
	}

	public float Loss => Occlusion;
}