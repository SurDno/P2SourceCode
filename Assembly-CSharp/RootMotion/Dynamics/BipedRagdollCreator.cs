using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
  [HelpURL("https://www.youtube.com/watch?v=y-luLRVmL7E&index=1&list=PLVxSIA1OaTOuE2SB9NUbckQ9r2hTg4mvL")]
  [AddComponentMenu("Scripts/RootMotion.Dynamics/Ragdoll Manager/Biped Ragdoll Creator")]
  public class BipedRagdollCreator : RagdollCreator
  {
    public bool canBuild;
    public BipedRagdollReferences references;
    public Options options = Options.Default;

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page1.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/class_root_motion_1_1_dynamics_1_1_biped_ragdoll_creator.html#details");
    }

    [ContextMenu("TUTORIAL VIDEO")]
    private void OpenTutorial()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=y-luLRVmL7E&index=1&list=PLVxSIA1OaTOuE2SB9NUbckQ9r2hTg4mvL");
    }

    public static Options AutodetectOptions(BipedRagdollReferences r)
    {
      Options options = Options.Default;
      if (r.spine == null)
        options.spine = false;
      if (r.chest == null)
        options.chest = false;
      if (options.chest && Vector3.Dot(r.root.up, r.chest.position - GetUpperArmCentroid(r)) > 0.0)
      {
        options.chest = false;
        if (r.spine != null)
          options.spine = true;
      }
      return options;
    }

    public static void Create(BipedRagdollReferences r, Options options)
    {
      string empty = string.Empty;
      if (!r.IsValid(ref empty))
      {
        Debug.LogWarning(empty);
      }
      else
      {
        ClearAll(r.root);
        CreateColliders(r, options);
        MassDistribution(r, options);
        CreateJoints(r, options);
      }
    }

    private static void CreateColliders(
      BipedRagdollReferences r,
      Options options)
    {
      Vector3 armToHeadCentroid = GetUpperArmToHeadCentroid(r);
      if (r.spine == null)
        options.spine = false;
      if (r.chest == null)
        options.chest = false;
      Vector3 widthDirection = r.rightUpperArm.position - r.leftUpperArm.position;
      float magnitude = widthDirection.magnitude;
      float proportionAspect = 0.6f;
      Vector3 vector3_1 = r.hips.position;
      float num1 = Vector3.Distance(r.head.position, r.root.position);
      if (Vector3.Distance(r.hips.position, r.root.position) < num1 * 0.20000000298023224)
        vector3_1 = Vector3.Lerp(r.leftUpperLeg.position, r.rightUpperLeg.position, 0.5f);
      Vector3 endPoint = options.spine ? r.spine.position : (options.chest ? r.chest.position : armToHeadCentroid);
      Vector3 startPoint1 = vector3_1 + (vector3_1 - armToHeadCentroid) * 0.1f;
      float width1 = options.spine || options.chest ? magnitude * 0.8f : magnitude;
      CreateCollider(r.hips, startPoint1, endPoint, options.torsoColliders, options.colliderLengthOverlap, width1, proportionAspect, widthDirection);
      if (options.spine)
      {
        Vector3 startPoint2 = endPoint;
        endPoint = options.chest ? r.chest.position : armToHeadCentroid;
        float width2 = options.chest ? magnitude * 0.75f : magnitude;
        CreateCollider(r.spine, startPoint2, endPoint, options.torsoColliders, options.colliderLengthOverlap, width2, proportionAspect, widthDirection);
      }
      if (options.chest)
      {
        Vector3 startPoint3 = endPoint;
        endPoint = armToHeadCentroid;
        CreateCollider(r.chest, startPoint3, endPoint, options.torsoColliders, options.colliderLengthOverlap, magnitude, proportionAspect, widthDirection);
      }
      Vector3 vector3_2 = endPoint;
      Vector3 vector3_3 = vector3_2 + (vector3_2 - startPoint1) * 0.45f;
      Vector3 onNormal = r.head.TransformVector(AxisTools.GetAxisVectorToDirection(r.head, vector3_3 - vector3_2));
      Vector3 vector3_4 = vector3_2 + Vector3.Project(vector3_3 - vector3_2, onNormal).normalized * (vector3_3 - vector3_2).magnitude;
      CreateCollider(r.head, vector3_2, vector3_4, options.headCollider, options.colliderLengthOverlap, Vector3.Distance(vector3_2, vector3_4) * 0.8f);
      float num2 = 0.4f;
      float width3 = Vector3.Distance(r.leftUpperArm.position, r.leftLowerArm.position) * num2;
      CreateCollider(r.leftUpperArm, r.leftUpperArm.position, r.leftLowerArm.position, options.armColliders, options.colliderLengthOverlap, width3);
      CreateCollider(r.leftLowerArm, r.leftLowerArm.position, r.leftHand.position, options.armColliders, options.colliderLengthOverlap, width3 * 0.9f);
      float width4 = Vector3.Distance(r.rightUpperArm.position, r.rightLowerArm.position) * num2;
      CreateCollider(r.rightUpperArm, r.rightUpperArm.position, r.rightLowerArm.position, options.armColliders, options.colliderLengthOverlap, width4);
      CreateCollider(r.rightLowerArm, r.rightLowerArm.position, r.rightHand.position, options.armColliders, options.colliderLengthOverlap, width4 * 0.9f);
      float num3 = 0.3f;
      float width5 = Vector3.Distance(r.leftUpperLeg.position, r.leftLowerLeg.position) * num3;
      CreateCollider(r.leftUpperLeg, r.leftUpperLeg.position, r.leftLowerLeg.position, options.legColliders, options.colliderLengthOverlap, width5);
      CreateCollider(r.leftLowerLeg, r.leftLowerLeg.position, r.leftFoot.position, options.legColliders, options.colliderLengthOverlap, width5 * 0.9f);
      float width6 = Vector3.Distance(r.rightUpperLeg.position, r.rightLowerLeg.position) * num3;
      CreateCollider(r.rightUpperLeg, r.rightUpperLeg.position, r.rightLowerLeg.position, options.legColliders, options.colliderLengthOverlap, width6);
      CreateCollider(r.rightLowerLeg, r.rightLowerLeg.position, r.rightFoot.position, options.legColliders, options.colliderLengthOverlap, width6 * 0.9f);
      if (options.hands)
      {
        CreateHandCollider(r.leftHand, r.leftLowerArm, r.root, options);
        CreateHandCollider(r.rightHand, r.rightLowerArm, r.root, options);
      }
      if (!options.feet)
        return;
      CreateFootCollider(r.leftFoot, r.leftLowerLeg, r.leftUpperLeg, r.root, options);
      CreateFootCollider(r.rightFoot, r.rightLowerLeg, r.rightUpperLeg, r.root, options);
    }

    private static void CreateHandCollider(
      Transform hand,
      Transform lowerArm,
      Transform root,
      Options options)
    {
      Vector3 onNormal = hand.TransformVector(AxisTools.GetAxisVectorToPoint(hand, GetChildCentroid(hand, lowerArm.position)));
      Vector3 vector3_1 = hand.position - (lowerArm.position - hand.position) * 0.75f;
      Vector3 position = hand.position;
      Vector3 vector3_2 = Vector3.Project(vector3_1 - hand.position, onNormal);
      Vector3 normalized = vector3_2.normalized;
      vector3_2 = vector3_1 - hand.position;
      double magnitude = vector3_2.magnitude;
      Vector3 vector3_3 = normalized * (float) magnitude;
      Vector3 vector3_4 = position + vector3_3;
      CreateCollider(hand, hand.position, vector3_4, options.handColliders, options.colliderLengthOverlap, Vector3.Distance(vector3_4, hand.position) * 0.5f);
    }

    private static void CreateFootCollider(
      Transform foot,
      Transform lowerLeg,
      Transform upperLeg,
      Transform root,
      Options options)
    {
      float magnitude1 = (upperLeg.position - foot.position).magnitude;
      Vector3 onNormal = foot.TransformVector(AxisTools.GetAxisVectorToPoint(foot, GetChildCentroid(foot, foot.position + root.forward) + root.forward * magnitude1 * 0.2f));
      Vector3 vector3_1 = foot.position + root.forward * magnitude1 * 0.25f;
      Vector3 position1 = foot.position;
      Vector3 vector3_2 = Vector3.Project(vector3_1 - foot.position, onNormal);
      Vector3 normalized = vector3_2.normalized;
      vector3_2 = vector3_1 - foot.position;
      double magnitude2 = vector3_2.magnitude;
      Vector3 vector3_3 = normalized * (float) magnitude2;
      Vector3 a = position1 + vector3_3;
      float width = Vector3.Distance(a, foot.position) * 0.5f;
      Vector3 position2 = foot.position;
      Vector3 vector3_4 = Vector3.Dot(root.up, foot.position - root.position) < 0.0 ? Vector3.zero : Vector3.Project(position2 - root.up * width * 0.5f - root.position, root.up);
      Vector3 vector3_5 = a - position2;
      Vector3 vector3_6 = position2 - vector3_5 * 0.2f;
      CreateCollider(foot, vector3_6 - vector3_4, a - vector3_4, options.footColliders, options.colliderLengthOverlap, width);
    }

    private static Vector3 GetChildCentroid(Transform t, Vector3 fallback)
    {
      if (t.childCount == 0)
        return fallback;
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < t.childCount; ++index)
        zero += t.GetChild(index).position;
      return zero / t.childCount;
    }

    private static void MassDistribution(BipedRagdollReferences r, Options o)
    {
      int num1 = 3;
      if (r.spine == null)
      {
        o.spine = false;
        --num1;
      }
      if (r.chest == null)
      {
        o.chest = false;
        --num1;
      }
      float num2 = 0.508f / num1;
      float num3 = 0.0732f;
      float num4 = 0.027f;
      float num5 = 0.016f;
      float num6 = 0.0066f;
      float num7 = 0.0988f;
      float num8 = 0.0465f;
      float num9 = 0.0145f;
      r.hips.GetComponent<Rigidbody>().mass = num2 * o.weight;
      if (o.spine)
        r.spine.GetComponent<Rigidbody>().mass = num2 * o.weight;
      if (o.chest)
        r.chest.GetComponent<Rigidbody>().mass = num2 * o.weight;
      r.head.GetComponent<Rigidbody>().mass = num3 * o.weight;
      r.leftUpperArm.GetComponent<Rigidbody>().mass = num4 * o.weight;
      r.rightUpperArm.GetComponent<Rigidbody>().mass = r.leftUpperArm.GetComponent<Rigidbody>().mass;
      r.leftLowerArm.GetComponent<Rigidbody>().mass = num5 * o.weight;
      r.rightLowerArm.GetComponent<Rigidbody>().mass = r.leftLowerArm.GetComponent<Rigidbody>().mass;
      if (o.hands)
      {
        r.leftHand.GetComponent<Rigidbody>().mass = num6 * o.weight;
        r.rightHand.GetComponent<Rigidbody>().mass = r.leftHand.GetComponent<Rigidbody>().mass;
      }
      r.leftUpperLeg.GetComponent<Rigidbody>().mass = num7 * o.weight;
      r.rightUpperLeg.GetComponent<Rigidbody>().mass = r.leftUpperLeg.GetComponent<Rigidbody>().mass;
      r.leftLowerLeg.GetComponent<Rigidbody>().mass = num8 * o.weight;
      r.rightLowerLeg.GetComponent<Rigidbody>().mass = r.leftLowerLeg.GetComponent<Rigidbody>().mass;
      if (!o.feet)
        return;
      r.leftFoot.GetComponent<Rigidbody>().mass = num9 * o.weight;
      r.rightFoot.GetComponent<Rigidbody>().mass = r.leftFoot.GetComponent<Rigidbody>().mass;
    }

    private static void CreateJoints(BipedRagdollReferences r, Options o)
    {
      if (r.spine == null)
        o.spine = false;
      if (r.chest == null)
        o.chest = false;
      float minSwing = -30f * o.jointRange;
      float maxSwing = 10f * o.jointRange;
      float swing2 = 25f * o.jointRange;
      float twist = 25f * o.jointRange;
      CreateJoint(new CreateJointParams(r.hips.GetComponent<Rigidbody>(), null, o.spine ? r.spine : (o.chest ? r.chest : r.head), r.root.right, new CreateJointParams.Limits(0.0f, 0.0f, 0.0f, 0.0f), o.joints));
      if (o.spine)
        CreateJoint(new CreateJointParams(r.spine.GetComponent<Rigidbody>(), r.hips.GetComponent<Rigidbody>(), o.chest ? r.chest : r.head, r.root.right, new CreateJointParams.Limits(minSwing, maxSwing, swing2, twist), o.joints));
      if (o.chest)
        CreateJoint(new CreateJointParams(r.chest.GetComponent<Rigidbody>(), o.spine ? r.spine.GetComponent<Rigidbody>() : r.hips.GetComponent<Rigidbody>(), r.head, r.root.right, new CreateJointParams.Limits(minSwing, maxSwing, swing2, twist), o.joints));
      Transform connectedBone = o.chest ? r.chest : (o.spine ? r.spine : r.hips);
      CreateJoint(new CreateJointParams(r.head.GetComponent<Rigidbody>(), connectedBone.GetComponent<Rigidbody>(), null, r.root.right, new CreateJointParams.Limits(-30f, 30f, 30f, 85f), o.joints));
      CreateJointParams.Limits limits1_1 = new CreateJointParams.Limits(-35f * o.jointRange, 120f * o.jointRange, 85f * o.jointRange, 45f * o.jointRange);
      CreateJointParams.Limits limits2_1 = new CreateJointParams.Limits(0.0f, 140f * o.jointRange, 10f * o.jointRange, 45f * o.jointRange);
      CreateJointParams.Limits limits3_1 = new CreateJointParams.Limits(-50f * o.jointRange, 50f * o.jointRange, 50f * o.jointRange, 25f * o.jointRange);
      CreateLimbJoints(connectedBone, r.leftUpperArm, r.leftLowerArm, r.leftHand, r.root, -r.root.right, o.joints, limits1_1, limits2_1, limits3_1);
      CreateLimbJoints(connectedBone, r.rightUpperArm, r.rightLowerArm, r.rightHand, r.root, r.root.right, o.joints, limits1_1, limits2_1, limits3_1);
      CreateJointParams.Limits limits1_2 = new CreateJointParams.Limits(-120f * o.jointRange, 35f * o.jointRange, 85f * o.jointRange, 45f * o.jointRange);
      CreateJointParams.Limits limits2_2 = new CreateJointParams.Limits(0.0f, 140f * o.jointRange, 10f * o.jointRange, 45f * o.jointRange);
      CreateJointParams.Limits limits3_2 = new CreateJointParams.Limits(-50f * o.jointRange, 50f * o.jointRange, 50f * o.jointRange, 25f * o.jointRange);
      CreateLimbJoints(r.hips, r.leftUpperLeg, r.leftLowerLeg, r.leftFoot, r.root, -r.root.up, o.joints, limits1_2, limits2_2, limits3_2);
      CreateLimbJoints(r.hips, r.rightUpperLeg, r.rightLowerLeg, r.rightFoot, r.root, -r.root.up, o.joints, limits1_2, limits2_2, limits3_2);
    }

    private static void CreateLimbJoints(
      Transform connectedBone,
      Transform bone1,
      Transform bone2,
      Transform bone3,
      Transform root,
      Vector3 defaultWorldDirection,
      JointType jointType,
      CreateJointParams.Limits limits1,
      CreateJointParams.Limits limits2,
      CreateJointParams.Limits limits3)
    {
      Quaternion localRotation = bone1.localRotation;
      bone1.rotation = Quaternion.FromToRotation(bone1.rotation * (bone2.position - bone1.position), defaultWorldDirection) * bone1.rotation;
      Vector3 normalized1 = (bone2.position - bone1.position).normalized;
      Vector3 normalized2 = (bone3.position - bone2.position).normalized;
      Vector3 worldSwingAxis = -Vector3.Cross(normalized1, normalized2);
      float num1 = Vector3.Angle(normalized1, normalized2);
      bool flag = Mathf.Abs(Vector3.Dot(normalized1, root.up)) > 0.5;
      float num2 = flag ? 100f : 1f;
      if (num1 < 0.0099999997764825821 * num2)
        worldSwingAxis = !flag ? (Vector3.Dot(normalized1, root.right) > 0.0 ? root.up : -root.up) : (Vector3.Dot(normalized1, root.up) > 0.0 ? root.right : -root.right);
      CreateJoint(new CreateJointParams(bone1.GetComponent<Rigidbody>(), connectedBone.GetComponent<Rigidbody>(), bone2, worldSwingAxis, limits1, jointType));
      CreateJoint(new CreateJointParams(bone2.GetComponent<Rigidbody>(), bone1.GetComponent<Rigidbody>(), bone3, worldSwingAxis, new CreateJointParams.Limits(limits2.minSwing - num1, limits2.maxSwing - num1, limits2.swing2, limits2.twist), jointType));
      if (bone3.GetComponent<Rigidbody>() != null)
        CreateJoint(new CreateJointParams(bone3.GetComponent<Rigidbody>(), bone2.GetComponent<Rigidbody>(), null, worldSwingAxis, limits3, jointType));
      bone1.localRotation = localRotation;
    }

    public static void ClearBipedRagdoll(BipedRagdollReferences r)
    {
      foreach (Transform ragdollTransform in r.GetRagdollTransforms())
        ClearTransform(ragdollTransform);
    }

    public static bool IsClear(BipedRagdollReferences r)
    {
      foreach (Component ragdollTransform in r.GetRagdollTransforms())
      {
        if (ragdollTransform.GetComponent<Rigidbody>() != null)
          return false;
      }
      return true;
    }

    private static Vector3 GetUpperArmToHeadCentroid(BipedRagdollReferences r)
    {
      return Vector3.Lerp(GetUpperArmCentroid(r), r.head.position, 0.5f);
    }

    private static Vector3 GetUpperArmCentroid(BipedRagdollReferences r)
    {
      return Vector3.Lerp(r.leftUpperArm.position, r.rightUpperArm.position, 0.5f);
    }

    [Serializable]
    public struct Options
    {
      public float weight;
      [Header("Optional Bones")]
      public bool spine;
      public bool chest;
      public bool hands;
      public bool feet;
      [Header("Joints")]
      public JointType joints;
      public float jointRange;
      [Header("Colliders")]
      public float colliderLengthOverlap;
      public ColliderType torsoColliders;
      public ColliderType headCollider;
      public ColliderType armColliders;
      public ColliderType handColliders;
      public ColliderType legColliders;
      public ColliderType footColliders;

      public static Options Default
      {
        get
        {
          return new Options {
            weight = 75f,
            colliderLengthOverlap = 0.1f,
            jointRange = 1f,
            chest = true,
            headCollider = ColliderType.Capsule,
            armColliders = ColliderType.Capsule,
            hands = true,
            handColliders = ColliderType.Capsule,
            legColliders = ColliderType.Capsule,
            feet = true
          };
        }
      }
    }
  }
}
