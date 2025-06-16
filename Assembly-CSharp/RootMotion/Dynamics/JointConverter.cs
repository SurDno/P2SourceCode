using UnityEngine;

namespace RootMotion.Dynamics;

public static class JointConverter {
	public static void ToConfigurable(GameObject root) {
		var num = 0;
		foreach (var componentsInChild in root.GetComponentsInChildren<CharacterJoint>()) {
			CharacterToConfigurable(componentsInChild);
			++num;
		}

		foreach (var componentsInChild in root.GetComponentsInChildren<HingeJoint>()) {
			HingeToConfigurable(componentsInChild);
			++num;
		}

		foreach (var componentsInChild in root.GetComponentsInChildren<FixedJoint>()) {
			FixedToConfigurable(componentsInChild);
			++num;
		}

		foreach (var componentsInChild in root.GetComponentsInChildren<SpringJoint>()) {
			SpringToConfigurable(componentsInChild);
			++num;
		}

		if (num > 0)
			Debug.Log(num + " joints were successfully converted to ConfigurableJoints.");
		else
			Debug.Log("No joints found in the children of " + root.name + " to convert to ConfigurableJoints.");
	}

	public static void HingeToConfigurable(HingeJoint src) {
		var conf = src.gameObject.AddComponent<ConfigurableJoint>();
		ConvertJoint(ref conf, src);
		conf.secondaryAxis = Vector3.zero;
		conf.xMotion = ConfigurableJointMotion.Locked;
		conf.yMotion = ConfigurableJointMotion.Locked;
		conf.zMotion = ConfigurableJointMotion.Locked;
		conf.angularXMotion = src.useLimits ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free;
		conf.angularYMotion = ConfigurableJointMotion.Locked;
		conf.angularZMotion = ConfigurableJointMotion.Locked;
		conf.highAngularXLimit = ConvertToHighSoftJointLimit(src.limits, src.spring, src.useSpring);
		conf.angularXLimitSpring = ConvertToSoftJointLimitSpring(src.limits, src.spring, src.useSpring);
		conf.lowAngularXLimit = ConvertToLowSoftJointLimit(src.limits, src.spring, src.useSpring);
		if (src.useMotor)
			Debug.LogWarning("Can not convert HingeJoint Motor to ConfigurableJoint.");
		Object.DestroyImmediate(src);
	}

	public static void FixedToConfigurable(FixedJoint src) {
		var conf = src.gameObject.AddComponent<ConfigurableJoint>();
		ConvertJoint(ref conf, src);
		conf.secondaryAxis = Vector3.zero;
		conf.xMotion = ConfigurableJointMotion.Locked;
		conf.yMotion = ConfigurableJointMotion.Locked;
		conf.zMotion = ConfigurableJointMotion.Locked;
		conf.angularXMotion = ConfigurableJointMotion.Locked;
		conf.angularYMotion = ConfigurableJointMotion.Locked;
		conf.angularZMotion = ConfigurableJointMotion.Locked;
		Object.DestroyImmediate(src);
	}

	public static void SpringToConfigurable(SpringJoint src) {
		var conf = src.gameObject.AddComponent<ConfigurableJoint>();
		ConvertJoint(ref conf, src);
		conf.xMotion = ConfigurableJointMotion.Limited;
		conf.yMotion = ConfigurableJointMotion.Limited;
		conf.zMotion = ConfigurableJointMotion.Limited;
		conf.angularXMotion = ConfigurableJointMotion.Free;
		conf.angularYMotion = ConfigurableJointMotion.Free;
		conf.angularZMotion = ConfigurableJointMotion.Free;
		conf.linearLimit = new SoftJointLimit {
			bounciness = 0.0f,
			limit = src.maxDistance
		};
		conf.linearLimitSpring = new SoftJointLimitSpring {
			damper = src.damper,
			spring = src.spring
		};
		Object.DestroyImmediate(src);
	}

	public static void CharacterToConfigurable(CharacterJoint src) {
		var conf = src.gameObject.AddComponent<ConfigurableJoint>();
		ConvertJoint(ref conf, src);
		conf.secondaryAxis = src.swingAxis;
		conf.xMotion = ConfigurableJointMotion.Locked;
		conf.yMotion = ConfigurableJointMotion.Locked;
		conf.zMotion = ConfigurableJointMotion.Locked;
		conf.angularXMotion = ConfigurableJointMotion.Limited;
		conf.angularYMotion = ConfigurableJointMotion.Limited;
		conf.angularZMotion = ConfigurableJointMotion.Limited;
		conf.highAngularXLimit = CopyLimit(src.highTwistLimit);
		conf.lowAngularXLimit = CopyLimit(src.lowTwistLimit);
		conf.angularYLimit = CopyLimit(src.swing1Limit);
		conf.angularZLimit = CopyLimit(src.swing2Limit);
		conf.angularXLimitSpring = CopyLimitSpring(src.twistLimitSpring);
		conf.angularYZLimitSpring = CopyLimitSpring(src.swingLimitSpring);
		conf.enableCollision = src.enableCollision;
		conf.projectionMode = src.enableProjection ? JointProjectionMode.PositionAndRotation : JointProjectionMode.None;
		conf.projectionAngle = src.projectionAngle;
		conf.projectionDistance = src.projectionDistance;
		Object.DestroyImmediate(src);
	}

	private static void ConvertJoint(ref ConfigurableJoint conf, Joint src) {
		conf.anchor = src.anchor;
		conf.autoConfigureConnectedAnchor = src.autoConfigureConnectedAnchor;
		conf.axis = src.axis;
		conf.breakForce = src.breakForce;
		conf.breakTorque = src.breakTorque;
		conf.connectedAnchor = src.connectedAnchor;
		conf.connectedBody = src.connectedBody;
		conf.enableCollision = src.enableCollision;
	}

	private static SoftJointLimit ConvertToHighSoftJointLimit(
		JointLimits src,
		JointSpring spring,
		bool useSpring) {
		return new SoftJointLimit {
			limit = -src.max,
			bounciness = src.bounciness
		};
	}

	private static SoftJointLimit ConvertToLowSoftJointLimit(
		JointLimits src,
		JointSpring spring,
		bool useSpring) {
		return new SoftJointLimit {
			limit = -src.min,
			bounciness = src.bounciness
		};
	}

	private static SoftJointLimitSpring ConvertToSoftJointLimitSpring(
		JointLimits src,
		JointSpring spring,
		bool useSpring) {
		return new SoftJointLimitSpring {
			damper = useSpring ? spring.damper : 0.0f,
			spring = useSpring ? spring.spring : 0.0f
		};
	}

	private static SoftJointLimit CopyLimit(SoftJointLimit src) {
		return new SoftJointLimit {
			limit = src.limit,
			bounciness = src.bounciness
		};
	}

	private static SoftJointLimitSpring CopyLimitSpring(SoftJointLimitSpring src) {
		return new SoftJointLimitSpring {
			damper = src.damper,
			spring = src.spring
		};
	}
}