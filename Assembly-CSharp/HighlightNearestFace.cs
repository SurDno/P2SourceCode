using ProBuilder2.Common;
using UnityEngine;

public class HighlightNearestFace : MonoBehaviour {
	public float travel = 50f;
	public float speed = 0.2f;
	private pb_Object target;
	private pb_Face nearest;

	private void Start() {
		target = pb_ShapeGenerator.PlaneGenerator(travel, travel, 25, 25, Axis.Up, false);
		target.SetFaceMaterial(target.faces, pb_Constant.DefaultMaterial);
		target.transform.position = new Vector3(travel * 0.5f, 0.0f, travel * 0.5f);
		target.ToMesh();
		target.Refresh();
		var main = Camera.main;
		main.transform.position = new Vector3(25f, 40f, 0.0f);
		main.transform.localRotation = Quaternion.Euler(new Vector3(65f, 0.0f, 0.0f));
	}

	private void Update() {
		var num1 = Time.time * speed;
		transform.position = new Vector3(Mathf.PerlinNoise(num1, num1) * travel, 2f,
			Mathf.PerlinNoise(num1 + 1f, num1 + 1f) * travel);
		if (target == null)
			Debug.LogWarning("Missing the ProBuilder Mesh target!");
		else {
			var a = target.transform.InverseTransformPoint(transform.position);
			if (nearest != null)
				target.SetFaceColor(nearest, Color.white);
			var length = target.faces.Length;
			var num2 = float.PositiveInfinity;
			nearest = target.faces[0];
			for (var index = 0; index < length; ++index) {
				var num3 = Vector3.Distance(a, FaceCenter(target, target.faces[index]));
				if (num3 < (double)num2) {
					num2 = num3;
					nearest = target.faces[index];
				}
			}

			target.SetFaceColor(nearest, Color.blue);
			target.RefreshColors();
		}
	}

	private Vector3 FaceCenter(pb_Object pb, pb_Face face) {
		var vertices = pb.vertices;
		var zero = Vector3.zero;
		foreach (var distinctIndex in face.distinctIndices) {
			zero.x += vertices[distinctIndex].x;
			zero.y += vertices[distinctIndex].y;
			zero.z += vertices[distinctIndex].z;
		}

		float length = face.distinctIndices.Length;
		zero.x /= length;
		zero.y /= length;
		zero.z /= length;
		return zero;
	}
}