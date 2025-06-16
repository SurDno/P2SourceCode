using UnityEngine;

namespace SoundPropagation;

public class SPEllipseShape : Shape {
	public Vector2 Size = new(1f, 1f);
	private Matrix4x4 plane2world;
	private Matrix4x4 world2plane;

	protected override void Initialize() {
		plane2world = transform.localToWorldMatrix;
		var vector2 = Size * 0.5f;
		plane2world.m00 *= vector2.x;
		plane2world.m10 *= vector2.x;
		plane2world.m20 *= vector2.x;
		plane2world.m01 *= vector2.y;
		plane2world.m11 *= vector2.y;
		plane2world.m21 *= vector2.y;
		world2plane = plane2world.inverse;
	}

	protected override bool ClosestPointToSegmentInternal(
		Vector3 pointA,
		Vector3 pointB,
		out Vector3 output) {
		var segmentOnPlane = ClosestToSegmentOnPlane(world2plane, pointA, pointB);
		var f = (float)(segmentOnPlane.x * (double)segmentOnPlane.x + segmentOnPlane.y * (double)segmentOnPlane.y);
		if (f > 1.0) {
			var num = Mathf.Sqrt(f);
			segmentOnPlane.x /= num;
			segmentOnPlane.y /= num;
		}

		output = plane2world.MultiplyPoint3x4(segmentOnPlane);
		return true;
	}

	private void OnDrawGizmosSelected() {
		plane2world = transform.localToWorldMatrix;
		var vector2 = Size * 0.5f;
		plane2world.m00 *= vector2.x;
		plane2world.m10 *= vector2.x;
		plane2world.m20 *= vector2.x;
		plane2world.m01 *= vector2.y;
		plane2world.m11 *= vector2.y;
		plane2world.m21 *= vector2.y;
		Gizmos.color = gizmoColor;
		var vector3_1 = plane2world.MultiplyPoint(new Vector3(-1f, 0.0f, 0.0f));
		var vector3_2 = plane2world.MultiplyPoint(new Vector3(-0.866f, 0.5f, 0.0f));
		Gizmos.DrawLine(vector3_1, vector3_2);
		var vector3_3 = plane2world.MultiplyPoint(new Vector3(-0.5f, 0.866f, 0.0f));
		Gizmos.DrawLine(vector3_2, vector3_3);
		var vector3_4 = plane2world.MultiplyPoint(new Vector3(0.0f, 1f, 0.0f));
		Gizmos.DrawLine(vector3_3, vector3_4);
		var vector3_5 = plane2world.MultiplyPoint(new Vector3(0.5f, 0.866f, 0.0f));
		Gizmos.DrawLine(vector3_4, vector3_5);
		var vector3_6 = plane2world.MultiplyPoint(new Vector3(0.866f, 0.5f, 0.0f));
		Gizmos.DrawLine(vector3_5, vector3_6);
		var vector3_7 = plane2world.MultiplyPoint(new Vector3(1f, 0.0f, 0.0f));
		Gizmos.DrawLine(vector3_6, vector3_7);
		var vector3_8 = plane2world.MultiplyPoint(new Vector3(0.866f, -0.5f, 0.0f));
		Gizmos.DrawLine(vector3_7, vector3_8);
		var vector3_9 = plane2world.MultiplyPoint(new Vector3(0.5f, -0.866f, 0.0f));
		Gizmos.DrawLine(vector3_8, vector3_9);
		var vector3_10 = plane2world.MultiplyPoint(new Vector3(0.0f, -1f, 0.0f));
		Gizmos.DrawLine(vector3_9, vector3_10);
		var vector3_11 = plane2world.MultiplyPoint(new Vector3(-0.5f, -0.866f, 0.0f));
		Gizmos.DrawLine(vector3_10, vector3_11);
		var vector3_12 = plane2world.MultiplyPoint(new Vector3(-0.866f, -0.5f, 0.0f));
		Gizmos.DrawLine(vector3_11, vector3_12);
		Gizmos.DrawLine(vector3_12, vector3_1);
	}
}