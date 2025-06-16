using UnityEngine;

namespace RootMotion;

public class AxisTools {
	public static Vector3 ToVector3(Axis axis) {
		if (axis == Axis.X)
			return Vector3.right;
		return axis == Axis.Y ? Vector3.up : Vector3.forward;
	}

	public static Axis ToAxis(Vector3 v) {
		var num1 = Mathf.Abs(v.x);
		var num2 = Mathf.Abs(v.y);
		var num3 = Mathf.Abs(v.z);
		var axis = Axis.X;
		if (num2 > (double)num1 && num2 > (double)num3)
			axis = Axis.Y;
		if (num3 > (double)num1 && num3 > (double)num2)
			axis = Axis.Z;
		return axis;
	}

	public static Axis GetAxisToPoint(Transform t, Vector3 worldPosition) {
		var axisVectorToPoint = GetAxisVectorToPoint(t, worldPosition);
		if (axisVectorToPoint == Vector3.right)
			return Axis.X;
		return axisVectorToPoint == Vector3.up ? Axis.Y : Axis.Z;
	}

	public static Axis GetAxisToDirection(Transform t, Vector3 direction) {
		var vectorToDirection = GetAxisVectorToDirection(t, direction);
		if (vectorToDirection == Vector3.right)
			return Axis.X;
		return vectorToDirection == Vector3.up ? Axis.Y : Axis.Z;
	}

	public static Vector3 GetAxisVectorToPoint(Transform t, Vector3 worldPosition) {
		return GetAxisVectorToDirection(t, worldPosition - t.position);
	}

	public static Vector3 GetAxisVectorToDirection(Transform t, Vector3 direction) {
		direction = direction.normalized;
		var vectorToDirection = Vector3.right;
		var num1 = Mathf.Abs(Vector3.Dot(Vector3.Normalize(t.right), direction));
		var num2 = Mathf.Abs(Vector3.Dot(Vector3.Normalize(t.up), direction));
		if (num2 > (double)num1)
			vectorToDirection = Vector3.up;
		var num3 = Mathf.Abs(Vector3.Dot(Vector3.Normalize(t.forward), direction));
		if (num3 > (double)num1 && num3 > (double)num2)
			vectorToDirection = Vector3.forward;
		return vectorToDirection;
	}
}