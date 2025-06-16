using UnityEngine;

namespace RootMotion.Dynamics;

public static class PuppetMasterTools {
	public static void PositionRagdoll(PuppetMaster puppetMaster) {
		var componentsInChildren = puppetMaster.transform.GetComponentsInChildren<Rigidbody>();
		if (componentsInChildren.Length == 0)
			return;
		foreach (var muscle in puppetMaster.muscles)
			if (muscle.joint == null || muscle.target == null)
				return;
		var vector3Array = new Vector3[componentsInChildren.Length];
		for (var index = 0; index < componentsInChildren.Length; ++index)
			if (componentsInChildren[index].transform.childCount == 1)
				vector3Array[index] = componentsInChildren[index].transform
					.InverseTransformDirection(componentsInChildren[index].transform.GetChild(0).position -
					                           componentsInChildren[index].transform.position);
		foreach (var rigidbody in componentsInChildren) {
			foreach (var muscle in puppetMaster.muscles)
				if (muscle.joint.GetComponent<Rigidbody>() == rigidbody)
					rigidbody.transform.position = muscle.target.position;
		}

		for (var index = 0; index < componentsInChildren.Length; ++index)
			if (componentsInChildren[index].transform.childCount == 1) {
				var position = componentsInChildren[index].transform.GetChild(0).position;
				componentsInChildren[index].transform.rotation =
					Quaternion.FromToRotation(componentsInChildren[index].transform.rotation * vector3Array[index],
						position - componentsInChildren[index].transform.position) *
					componentsInChildren[index].transform.rotation;
				componentsInChildren[index].transform.GetChild(0).position = position;
			}
	}

	public static void RealignRagdoll(PuppetMaster puppetMaster) {
		foreach (var muscle in puppetMaster.muscles)
			if (muscle.joint == null || muscle.joint.transform == null || muscle.target == null) {
				Debug.LogWarning("Muscles incomplete, can not realign ragdoll.");
				return;
			}

		foreach (var muscle in puppetMaster.muscles)
			if (muscle.target != null) {
				var transformArray = new Transform[muscle.joint.transform.childCount];
				for (var index = 0; index < transformArray.Length; ++index)
					transformArray[index] = muscle.joint.transform.GetChild(index);
				foreach (var transform in transformArray)
					transform.parent = null;
				var component1 = muscle.joint.GetComponent<BoxCollider>();
				var vector1 = Vector3.zero;
				var vector2 = Vector3.zero;
				if (component1 != null) {
					vector1 = component1.transform.TransformVector(component1.size);
					vector2 = component1.transform.TransformVector(component1.center);
				}

				var component2 = muscle.joint.GetComponent<CapsuleCollider>();
				var vector3 = Vector3.zero;
				var direction = Vector3.zero;
				if (component2 != null) {
					vector3 = component2.transform.TransformVector(component2.center);
					direction = component2.transform.TransformVector(DirectionIntToVector3(component2.direction));
				}

				var component3 = muscle.joint.GetComponent<SphereCollider>();
				var vector4 = Vector3.zero;
				if (component3 != null)
					vector4 = component3.transform.TransformVector(component3.center);
				var vector5 = muscle.joint.transform.TransformVector(muscle.joint.axis);
				var vector6 = muscle.joint.transform.TransformVector(muscle.joint.secondaryAxis);
				muscle.joint.transform.rotation = muscle.target.rotation;
				if (component1 != null) {
					component1.size = component1.transform.InverseTransformVector(vector1);
					component1.center = component1.transform.InverseTransformVector(vector2);
				}

				if (component2 != null) {
					component2.center = component2.transform.InverseTransformVector(vector3);
					var dir = component2.transform.InverseTransformDirection(direction);
					component2.direction = DirectionVector3ToInt(dir);
				}

				if (component3 != null)
					component3.center = component3.transform.InverseTransformVector(vector4);
				muscle.joint.axis = muscle.joint.transform.InverseTransformVector(vector5);
				muscle.joint.secondaryAxis = muscle.joint.transform.InverseTransformVector(vector6);
				foreach (var transform in transformArray)
					transform.parent = muscle.joint.transform;
			}
	}

	private static Vector3 DirectionIntToVector3(int dir) {
		if (dir == 0)
			return Vector3.right;
		return dir == 1 ? Vector3.up : Vector3.forward;
	}

	private static int DirectionVector3ToInt(Vector3 dir) {
		var f1 = Vector3.Dot(dir, Vector3.right);
		var f2 = Vector3.Dot(dir, Vector3.up);
		var f3 = Vector3.Dot(dir, Vector3.forward);
		var num1 = Mathf.Abs(f1);
		var num2 = Mathf.Abs(f2);
		var num3 = Mathf.Abs(f3);
		var num4 = 0;
		if (num2 > (double)num1 && num2 > (double)num3)
			num4 = 1;
		if (num3 > (double)num1 && num3 > (double)num2)
			num4 = 2;
		return num4;
	}
}