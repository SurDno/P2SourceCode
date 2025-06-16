using System;
using UnityEngine;

namespace RootMotion.Dynamics;

public abstract class RagdollCreator : MonoBehaviour {
	public static void ClearAll(Transform root) {
		if (root == null)
			return;
		var transform = root;
		var componentInChildren = root.GetComponentInChildren<Animator>();
		if (componentInChildren != null && componentInChildren.isHuman) {
			var boneTransform = componentInChildren.GetBoneTransform(HumanBodyBones.Hips);
			if (boneTransform != null && boneTransform.GetComponentsInChildren<Transform>().Length > 2)
				transform = boneTransform;
		}

		var componentsInChildren = transform.GetComponentsInChildren<Transform>();
		if (componentsInChildren.Length < 2)
			return;
		for (var index = !(componentInChildren != null) || !componentInChildren.isHuman ? 1 : 0;
		     index < componentsInChildren.Length;
		     ++index)
			ClearTransform(componentsInChildren[index]);
	}

	protected static void ClearTransform(Transform transform) {
		if (transform == null)
			return;
		foreach (var component in transform.GetComponents<Collider>())
			if (component != null && !component.isTrigger)
				DestroyImmediate(component);
		var component1 = transform.GetComponent<Joint>();
		if (component1 != null)
			DestroyImmediate(component1);
		var component2 = transform.GetComponent<Rigidbody>();
		if (!(component2 != null))
			return;
		DestroyImmediate(component2);
	}

	protected static void CreateCollider(
		Transform t,
		Vector3 startPoint,
		Vector3 endPoint,
		ColliderType colliderType,
		float lengthOverlap,
		float width) {
		var direction = endPoint - startPoint;
		var num = direction.magnitude * (1f + lengthOverlap);
		var vectorToDirection = AxisTools.GetAxisVectorToDirection(t, direction);
		t.gameObject.AddComponent<Rigidbody>();
		var scaleF = GetScaleF(t);
		switch (colliderType) {
			case ColliderType.Box:
				var vector3 = Vector3.Scale(vectorToDirection, new Vector3(num, num, num));
				if (vector3.x == 0.0)
					vector3.x = width;
				if (vector3.y == 0.0)
					vector3.y = width;
				if (vector3.z == 0.0)
					vector3.z = width;
				var boxCollider = t.gameObject.AddComponent<BoxCollider>();
				boxCollider.size = vector3 / scaleF;
				boxCollider.size = new Vector3(Mathf.Abs(boxCollider.size.x), Mathf.Abs(boxCollider.size.y),
					Mathf.Abs(boxCollider.size.z));
				boxCollider.center = t.InverseTransformPoint(Vector3.Lerp(startPoint, endPoint, 0.5f));
				break;
			case ColliderType.Capsule:
				var capsuleCollider = t.gameObject.AddComponent<CapsuleCollider>();
				capsuleCollider.height = Mathf.Abs(num / scaleF);
				capsuleCollider.radius = Mathf.Abs(width * 0.75f / scaleF);
				capsuleCollider.direction = DirectionVector3ToInt(vectorToDirection);
				capsuleCollider.center = t.InverseTransformPoint(Vector3.Lerp(startPoint, endPoint, 0.5f));
				break;
		}
	}

	protected static void CreateCollider(
		Transform t,
		Vector3 startPoint,
		Vector3 endPoint,
		ColliderType colliderType,
		float lengthOverlap,
		float width,
		float proportionAspect,
		Vector3 widthDirection) {
		if (colliderType == ColliderType.Capsule)
			CreateCollider(t, startPoint, endPoint, colliderType, lengthOverlap, width * proportionAspect);
		else {
			var direction = endPoint - startPoint;
			var num = direction.magnitude * (1f + lengthOverlap);
			var vectorToDirection = AxisTools.GetAxisVectorToDirection(t, direction);
			var a = AxisTools.GetAxisVectorToDirection(t, widthDirection);
			if (a == vectorToDirection) {
				Debug.LogWarning("Width axis = height axis on " + t.name, t);
				a = new Vector3(vectorToDirection.y, vectorToDirection.z, vectorToDirection.x);
			}

			t.gameObject.AddComponent<Rigidbody>();
			var vector3 = Vector3.Scale(vectorToDirection, new Vector3(num, num, num)) +
			              Vector3.Scale(a, new Vector3(width, width, width));
			if (vector3.x == 0.0)
				vector3.x = width * proportionAspect;
			if (vector3.y == 0.0)
				vector3.y = width * proportionAspect;
			if (vector3.z == 0.0)
				vector3.z = width * proportionAspect;
			var boxCollider = t.gameObject.AddComponent<BoxCollider>();
			boxCollider.size = vector3 / GetScaleF(t);
			boxCollider.center = t.InverseTransformPoint(Vector3.Lerp(startPoint, endPoint, 0.5f));
		}
	}

	protected static float GetScaleF(Transform t) {
		var lossyScale = t.lossyScale;
		return (float)((lossyScale.x + (double)lossyScale.y + lossyScale.z) / 3.0);
	}

	protected static Vector3 Abs(Vector3 v) {
		Vector3Abs(ref v);
		return v;
	}

	protected static void Vector3Abs(ref Vector3 v) {
		v.x = Mathf.Abs(v.x);
		v.y = Mathf.Abs(v.y);
		v.z = Mathf.Abs(v.z);
	}

	protected static Vector3 DirectionIntToVector3(int dir) {
		if (dir == 0)
			return Vector3.right;
		return dir == 1 ? Vector3.up : Vector3.forward;
	}

	protected static Vector3 DirectionToVector3(Direction dir) {
		if (dir == Direction.X)
			return Vector3.right;
		return dir == Direction.Y ? Vector3.up : Vector3.forward;
	}

	protected static int DirectionVector3ToInt(Vector3 dir) {
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

	protected static Vector3 GetLocalOrthoDirection(Transform transform, Vector3 worldDir) {
		worldDir = worldDir.normalized;
		var f1 = Vector3.Dot(worldDir, transform.right);
		var f2 = Vector3.Dot(worldDir, transform.up);
		var f3 = Vector3.Dot(worldDir, transform.forward);
		var num1 = Mathf.Abs(f1);
		var num2 = Mathf.Abs(f2);
		var num3 = Mathf.Abs(f3);
		var localOrthoDirection = Vector3.right;
		if (num2 > (double)num1 && num2 > (double)num3)
			localOrthoDirection = Vector3.up;
		if (num3 > (double)num1 && num3 > (double)num2)
			localOrthoDirection = Vector3.forward;
		if (Vector3.Dot(worldDir, transform.rotation * localOrthoDirection) < 0.0)
			localOrthoDirection = -localOrthoDirection;
		return localOrthoDirection;
	}

	protected static Rigidbody GetConnectedBody(Transform bone, ref Transform[] bones) {
		if (bone.parent == null)
			return null;
		foreach (var transform in bones)
			if (bone.parent == transform && transform.GetComponent<Rigidbody>() != null)
				return transform.GetComponent<Rigidbody>();
		return GetConnectedBody(bone.parent, ref bones);
	}

	protected static void CreateJoint(CreateJointParams p) {
		var localOrthoDirection = GetLocalOrthoDirection(p.rigidbody.transform, p.worldSwingAxis);
		var rhs = Vector3.forward;
		if (p.child != null)
			rhs = GetLocalOrthoDirection(p.rigidbody.transform, p.child.position - p.rigidbody.transform.position);
		else if (p.connectedBody != null)
			rhs = GetLocalOrthoDirection(p.rigidbody.transform,
				p.rigidbody.transform.position - p.connectedBody.transform.position);
		var vector3 = Vector3.Cross(localOrthoDirection, rhs);
		if (p.type == JointType.Configurable) {
			var configurableJoint = p.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
			configurableJoint.connectedBody = p.connectedBody;
			var configurableJointMotion1 =
				p.connectedBody != null ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
			var configurableJointMotion2 =
				p.connectedBody != null ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free;
			configurableJoint.xMotion = configurableJointMotion1;
			configurableJoint.yMotion = configurableJointMotion1;
			configurableJoint.zMotion = configurableJointMotion1;
			configurableJoint.angularXMotion = configurableJointMotion2;
			configurableJoint.angularYMotion = configurableJointMotion2;
			configurableJoint.angularZMotion = configurableJointMotion2;
			if (p.connectedBody != null) {
				configurableJoint.axis = localOrthoDirection;
				configurableJoint.secondaryAxis = vector3;
				configurableJoint.lowAngularXLimit = ToSoftJointLimit(p.limits.minSwing);
				configurableJoint.highAngularXLimit = ToSoftJointLimit(p.limits.maxSwing);
				configurableJoint.angularYLimit = ToSoftJointLimit(p.limits.swing2);
				configurableJoint.angularZLimit = ToSoftJointLimit(p.limits.twist);
			}

			configurableJoint.anchor = Vector3.zero;
		} else {
			if (p.connectedBody == null)
				return;
			var characterJoint = p.rigidbody.gameObject.AddComponent<CharacterJoint>();
			characterJoint.connectedBody = p.connectedBody;
			characterJoint.axis = localOrthoDirection;
			characterJoint.swingAxis = vector3;
			characterJoint.lowTwistLimit = ToSoftJointLimit(p.limits.minSwing);
			characterJoint.highTwistLimit = ToSoftJointLimit(p.limits.maxSwing);
			characterJoint.swing1Limit = ToSoftJointLimit(p.limits.swing2);
			characterJoint.swing2Limit = ToSoftJointLimit(p.limits.twist);
			characterJoint.anchor = Vector3.zero;
		}
	}

	private static SoftJointLimit ToSoftJointLimit(float limit) {
		return new SoftJointLimit { limit = limit };
	}

	[Serializable]
	public enum ColliderType {
		Box,
		Capsule
	}

	[Serializable]
	public enum JointType {
		Configurable,
		Character
	}

	[Serializable]
	public enum Direction {
		X,
		Y,
		Z
	}

	public struct CreateJointParams {
		public Rigidbody rigidbody;
		public Rigidbody connectedBody;
		public Transform child;
		public Vector3 worldSwingAxis;
		public Limits limits;
		public JointType type;

		public CreateJointParams(
			Rigidbody rigidbody,
			Rigidbody connectedBody,
			Transform child,
			Vector3 worldSwingAxis,
			Limits limits,
			JointType type) {
			this.rigidbody = rigidbody;
			this.connectedBody = connectedBody;
			this.child = child;
			this.worldSwingAxis = worldSwingAxis;
			this.limits = limits;
			this.type = type;
		}

		public struct Limits {
			public float minSwing;
			public float maxSwing;
			public float swing2;
			public float twist;

			public Limits(float minSwing, float maxSwing, float swing2, float twist) {
				this.minSwing = minSwing;
				this.maxSwing = maxSwing;
				this.swing2 = swing2;
				this.twist = twist;
			}
		}
	}
}