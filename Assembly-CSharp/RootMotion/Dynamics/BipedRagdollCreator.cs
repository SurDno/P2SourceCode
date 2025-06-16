// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.BipedRagdollCreator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.Dynamics
{
  [HelpURL("https://www.youtube.com/watch?v=y-luLRVmL7E&index=1&list=PLVxSIA1OaTOuE2SB9NUbckQ9r2hTg4mvL")]
  [AddComponentMenu("Scripts/RootMotion.Dynamics/Ragdoll Manager/Biped Ragdoll Creator")]
  public class BipedRagdollCreator : RagdollCreator
  {
    public bool canBuild;
    public BipedRagdollReferences references;
    public BipedRagdollCreator.Options options = BipedRagdollCreator.Options.Default;

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

    public static BipedRagdollCreator.Options AutodetectOptions(BipedRagdollReferences r)
    {
      BipedRagdollCreator.Options options = BipedRagdollCreator.Options.Default;
      if ((UnityEngine.Object) r.spine == (UnityEngine.Object) null)
        options.spine = false;
      if ((UnityEngine.Object) r.chest == (UnityEngine.Object) null)
        options.chest = false;
      if (options.chest && (double) Vector3.Dot(r.root.up, r.chest.position - BipedRagdollCreator.GetUpperArmCentroid(r)) > 0.0)
      {
        options.chest = false;
        if ((UnityEngine.Object) r.spine != (UnityEngine.Object) null)
          options.spine = true;
      }
      return options;
    }

    public static void Create(BipedRagdollReferences r, BipedRagdollCreator.Options options)
    {
      string empty = string.Empty;
      if (!r.IsValid(ref empty))
      {
        Debug.LogWarning((object) empty);
      }
      else
      {
        RagdollCreator.ClearAll(r.root);
        BipedRagdollCreator.CreateColliders(r, options);
        BipedRagdollCreator.MassDistribution(r, options);
        BipedRagdollCreator.CreateJoints(r, options);
      }
    }

    private static void CreateColliders(
      BipedRagdollReferences r,
      BipedRagdollCreator.Options options)
    {
      Vector3 armToHeadCentroid = BipedRagdollCreator.GetUpperArmToHeadCentroid(r);
      if ((UnityEngine.Object) r.spine == (UnityEngine.Object) null)
        options.spine = false;
      if ((UnityEngine.Object) r.chest == (UnityEngine.Object) null)
        options.chest = false;
      Vector3 widthDirection = r.rightUpperArm.position - r.leftUpperArm.position;
      float magnitude = widthDirection.magnitude;
      float proportionAspect = 0.6f;
      Vector3 vector3_1 = r.hips.position;
      float num1 = Vector3.Distance(r.head.position, r.root.position);
      if ((double) Vector3.Distance(r.hips.position, r.root.position) < (double) num1 * 0.20000000298023224)
        vector3_1 = Vector3.Lerp(r.leftUpperLeg.position, r.rightUpperLeg.position, 0.5f);
      Vector3 endPoint = options.spine ? r.spine.position : (options.chest ? r.chest.position : armToHeadCentroid);
      Vector3 startPoint1 = vector3_1 + (vector3_1 - armToHeadCentroid) * 0.1f;
      float width1 = options.spine || options.chest ? magnitude * 0.8f : magnitude;
      RagdollCreator.CreateCollider(r.hips, startPoint1, endPoint, options.torsoColliders, options.colliderLengthOverlap, width1, proportionAspect, widthDirection);
      if (options.spine)
      {
        Vector3 startPoint2 = endPoint;
        endPoint = options.chest ? r.chest.position : armToHeadCentroid;
        float width2 = options.chest ? magnitude * 0.75f : magnitude;
        RagdollCreator.CreateCollider(r.spine, startPoint2, endPoint, options.torsoColliders, options.colliderLengthOverlap, width2, proportionAspect, widthDirection);
      }
      if (options.chest)
      {
        Vector3 startPoint3 = endPoint;
        endPoint = armToHeadCentroid;
        RagdollCreator.CreateCollider(r.chest, startPoint3, endPoint, options.torsoColliders, options.colliderLengthOverlap, magnitude, proportionAspect, widthDirection);
      }
      Vector3 vector3_2 = endPoint;
      Vector3 vector3_3 = vector3_2 + (vector3_2 - startPoint1) * 0.45f;
      Vector3 onNormal = r.head.TransformVector(AxisTools.GetAxisVectorToDirection(r.head, vector3_3 - vector3_2));
      Vector3 vector3_4 = vector3_2 + Vector3.Project(vector3_3 - vector3_2, onNormal).normalized * (vector3_3 - vector3_2).magnitude;
      RagdollCreator.CreateCollider(r.head, vector3_2, vector3_4, options.headCollider, options.colliderLengthOverlap, Vector3.Distance(vector3_2, vector3_4) * 0.8f);
      float num2 = 0.4f;
      float width3 = Vector3.Distance(r.leftUpperArm.position, r.leftLowerArm.position) * num2;
      RagdollCreator.CreateCollider(r.leftUpperArm, r.leftUpperArm.position, r.leftLowerArm.position, options.armColliders, options.colliderLengthOverlap, width3);
      RagdollCreator.CreateCollider(r.leftLowerArm, r.leftLowerArm.position, r.leftHand.position, options.armColliders, options.colliderLengthOverlap, width3 * 0.9f);
      float width4 = Vector3.Distance(r.rightUpperArm.position, r.rightLowerArm.position) * num2;
      RagdollCreator.CreateCollider(r.rightUpperArm, r.rightUpperArm.position, r.rightLowerArm.position, options.armColliders, options.colliderLengthOverlap, width4);
      RagdollCreator.CreateCollider(r.rightLowerArm, r.rightLowerArm.position, r.rightHand.position, options.armColliders, options.colliderLengthOverlap, width4 * 0.9f);
      float num3 = 0.3f;
      float width5 = Vector3.Distance(r.leftUpperLeg.position, r.leftLowerLeg.position) * num3;
      RagdollCreator.CreateCollider(r.leftUpperLeg, r.leftUpperLeg.position, r.leftLowerLeg.position, options.legColliders, options.colliderLengthOverlap, width5);
      RagdollCreator.CreateCollider(r.leftLowerLeg, r.leftLowerLeg.position, r.leftFoot.position, options.legColliders, options.colliderLengthOverlap, width5 * 0.9f);
      float width6 = Vector3.Distance(r.rightUpperLeg.position, r.rightLowerLeg.position) * num3;
      RagdollCreator.CreateCollider(r.rightUpperLeg, r.rightUpperLeg.position, r.rightLowerLeg.position, options.legColliders, options.colliderLengthOverlap, width6);
      RagdollCreator.CreateCollider(r.rightLowerLeg, r.rightLowerLeg.position, r.rightFoot.position, options.legColliders, options.colliderLengthOverlap, width6 * 0.9f);
      if (options.hands)
      {
        BipedRagdollCreator.CreateHandCollider(r.leftHand, r.leftLowerArm, r.root, options);
        BipedRagdollCreator.CreateHandCollider(r.rightHand, r.rightLowerArm, r.root, options);
      }
      if (!options.feet)
        return;
      BipedRagdollCreator.CreateFootCollider(r.leftFoot, r.leftLowerLeg, r.leftUpperLeg, r.root, options);
      BipedRagdollCreator.CreateFootCollider(r.rightFoot, r.rightLowerLeg, r.rightUpperLeg, r.root, options);
    }

    private static void CreateHandCollider(
      Transform hand,
      Transform lowerArm,
      Transform root,
      BipedRagdollCreator.Options options)
    {
      Vector3 onNormal = hand.TransformVector(AxisTools.GetAxisVectorToPoint(hand, BipedRagdollCreator.GetChildCentroid(hand, lowerArm.position)));
      Vector3 vector3_1 = hand.position - (lowerArm.position - hand.position) * 0.75f;
      Vector3 position = hand.position;
      Vector3 vector3_2 = Vector3.Project(vector3_1 - hand.position, onNormal);
      Vector3 normalized = vector3_2.normalized;
      vector3_2 = vector3_1 - hand.position;
      double magnitude = (double) vector3_2.magnitude;
      Vector3 vector3_3 = normalized * (float) magnitude;
      Vector3 vector3_4 = position + vector3_3;
      RagdollCreator.CreateCollider(hand, hand.position, vector3_4, options.handColliders, options.colliderLengthOverlap, Vector3.Distance(vector3_4, hand.position) * 0.5f);
    }

    private static void CreateFootCollider(
      Transform foot,
      Transform lowerLeg,
      Transform upperLeg,
      Transform root,
      BipedRagdollCreator.Options options)
    {
      float magnitude1 = (upperLeg.position - foot.position).magnitude;
      Vector3 onNormal = foot.TransformVector(AxisTools.GetAxisVectorToPoint(foot, BipedRagdollCreator.GetChildCentroid(foot, foot.position + root.forward) + root.forward * magnitude1 * 0.2f));
      Vector3 vector3_1 = foot.position + root.forward * magnitude1 * 0.25f;
      Vector3 position1 = foot.position;
      Vector3 vector3_2 = Vector3.Project(vector3_1 - foot.position, onNormal);
      Vector3 normalized = vector3_2.normalized;
      vector3_2 = vector3_1 - foot.position;
      double magnitude2 = (double) vector3_2.magnitude;
      Vector3 vector3_3 = normalized * (float) magnitude2;
      Vector3 a = position1 + vector3_3;
      float width = Vector3.Distance(a, foot.position) * 0.5f;
      Vector3 position2 = foot.position;
      Vector3 vector3_4 = (double) Vector3.Dot(root.up, foot.position - root.position) < 0.0 ? Vector3.zero : Vector3.Project(position2 - root.up * width * 0.5f - root.position, root.up);
      Vector3 vector3_5 = a - position2;
      Vector3 vector3_6 = position2 - vector3_5 * 0.2f;
      RagdollCreator.CreateCollider(foot, vector3_6 - vector3_4, a - vector3_4, options.footColliders, options.colliderLengthOverlap, width);
    }

    private static Vector3 GetChildCentroid(Transform t, Vector3 fallback)
    {
      if (t.childCount == 0)
        return fallback;
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < t.childCount; ++index)
        zero += t.GetChild(index).position;
      return zero / (float) t.childCount;
    }

    private static void MassDistribution(BipedRagdollReferences r, BipedRagdollCreator.Options o)
    {
      int num1 = 3;
      if ((UnityEngine.Object) r.spine == (UnityEngine.Object) null)
      {
        o.spine = false;
        --num1;
      }
      if ((UnityEngine.Object) r.chest == (UnityEngine.Object) null)
      {
        o.chest = false;
        --num1;
      }
      float num2 = 0.508f / (float) num1;
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

    private static void CreateJoints(BipedRagdollReferences r, BipedRagdollCreator.Options o)
    {
      if ((UnityEngine.Object) r.spine == (UnityEngine.Object) null)
        o.spine = false;
      if ((UnityEngine.Object) r.chest == (UnityEngine.Object) null)
        o.chest = false;
      float minSwing = -30f * o.jointRange;
      float maxSwing = 10f * o.jointRange;
      float swing2 = 25f * o.jointRange;
      float twist = 25f * o.jointRange;
      RagdollCreator.CreateJoint(new RagdollCreator.CreateJointParams(r.hips.GetComponent<Rigidbody>(), (Rigidbody) null, o.spine ? r.spine : (o.chest ? r.chest : r.head), r.root.right, new RagdollCreator.CreateJointParams.Limits(0.0f, 0.0f, 0.0f, 0.0f), o.joints));
      if (o.spine)
        RagdollCreator.CreateJoint(new RagdollCreator.CreateJointParams(r.spine.GetComponent<Rigidbody>(), r.hips.GetComponent<Rigidbody>(), o.chest ? r.chest : r.head, r.root.right, new RagdollCreator.CreateJointParams.Limits(minSwing, maxSwing, swing2, twist), o.joints));
      if (o.chest)
        RagdollCreator.CreateJoint(new RagdollCreator.CreateJointParams(r.chest.GetComponent<Rigidbody>(), o.spine ? r.spine.GetComponent<Rigidbody>() : r.hips.GetComponent<Rigidbody>(), r.head, r.root.right, new RagdollCreator.CreateJointParams.Limits(minSwing, maxSwing, swing2, twist), o.joints));
      Transform connectedBone = o.chest ? r.chest : (o.spine ? r.spine : r.hips);
      RagdollCreator.CreateJoint(new RagdollCreator.CreateJointParams(r.head.GetComponent<Rigidbody>(), connectedBone.GetComponent<Rigidbody>(), (Transform) null, r.root.right, new RagdollCreator.CreateJointParams.Limits(-30f, 30f, 30f, 85f), o.joints));
      RagdollCreator.CreateJointParams.Limits limits1_1 = new RagdollCreator.CreateJointParams.Limits(-35f * o.jointRange, 120f * o.jointRange, 85f * o.jointRange, 45f * o.jointRange);
      RagdollCreator.CreateJointParams.Limits limits2_1 = new RagdollCreator.CreateJointParams.Limits(0.0f, 140f * o.jointRange, 10f * o.jointRange, 45f * o.jointRange);
      RagdollCreator.CreateJointParams.Limits limits3_1 = new RagdollCreator.CreateJointParams.Limits(-50f * o.jointRange, 50f * o.jointRange, 50f * o.jointRange, 25f * o.jointRange);
      BipedRagdollCreator.CreateLimbJoints(connectedBone, r.leftUpperArm, r.leftLowerArm, r.leftHand, r.root, -r.root.right, o.joints, limits1_1, limits2_1, limits3_1);
      BipedRagdollCreator.CreateLimbJoints(connectedBone, r.rightUpperArm, r.rightLowerArm, r.rightHand, r.root, r.root.right, o.joints, limits1_1, limits2_1, limits3_1);
      RagdollCreator.CreateJointParams.Limits limits1_2 = new RagdollCreator.CreateJointParams.Limits(-120f * o.jointRange, 35f * o.jointRange, 85f * o.jointRange, 45f * o.jointRange);
      RagdollCreator.CreateJointParams.Limits limits2_2 = new RagdollCreator.CreateJointParams.Limits(0.0f, 140f * o.jointRange, 10f * o.jointRange, 45f * o.jointRange);
      RagdollCreator.CreateJointParams.Limits limits3_2 = new RagdollCreator.CreateJointParams.Limits(-50f * o.jointRange, 50f * o.jointRange, 50f * o.jointRange, 25f * o.jointRange);
      BipedRagdollCreator.CreateLimbJoints(r.hips, r.leftUpperLeg, r.leftLowerLeg, r.leftFoot, r.root, -r.root.up, o.joints, limits1_2, limits2_2, limits3_2);
      BipedRagdollCreator.CreateLimbJoints(r.hips, r.rightUpperLeg, r.rightLowerLeg, r.rightFoot, r.root, -r.root.up, o.joints, limits1_2, limits2_2, limits3_2);
    }

    private static void CreateLimbJoints(
      Transform connectedBone,
      Transform bone1,
      Transform bone2,
      Transform bone3,
      Transform root,
      Vector3 defaultWorldDirection,
      RagdollCreator.JointType jointType,
      RagdollCreator.CreateJointParams.Limits limits1,
      RagdollCreator.CreateJointParams.Limits limits2,
      RagdollCreator.CreateJointParams.Limits limits3)
    {
      Quaternion localRotation = bone1.localRotation;
      bone1.rotation = Quaternion.FromToRotation(bone1.rotation * (bone2.position - bone1.position), defaultWorldDirection) * bone1.rotation;
      Vector3 normalized1 = (bone2.position - bone1.position).normalized;
      Vector3 normalized2 = (bone3.position - bone2.position).normalized;
      Vector3 worldSwingAxis = -Vector3.Cross(normalized1, normalized2);
      float num1 = Vector3.Angle(normalized1, normalized2);
      bool flag = (double) Mathf.Abs(Vector3.Dot(normalized1, root.up)) > 0.5;
      float num2 = flag ? 100f : 1f;
      if ((double) num1 < 0.0099999997764825821 * (double) num2)
        worldSwingAxis = !flag ? ((double) Vector3.Dot(normalized1, root.right) > 0.0 ? root.up : -root.up) : ((double) Vector3.Dot(normalized1, root.up) > 0.0 ? root.right : -root.right);
      RagdollCreator.CreateJoint(new RagdollCreator.CreateJointParams(bone1.GetComponent<Rigidbody>(), connectedBone.GetComponent<Rigidbody>(), bone2, worldSwingAxis, limits1, jointType));
      RagdollCreator.CreateJoint(new RagdollCreator.CreateJointParams(bone2.GetComponent<Rigidbody>(), bone1.GetComponent<Rigidbody>(), bone3, worldSwingAxis, new RagdollCreator.CreateJointParams.Limits(limits2.minSwing - num1, limits2.maxSwing - num1, limits2.swing2, limits2.twist), jointType));
      if ((UnityEngine.Object) bone3.GetComponent<Rigidbody>() != (UnityEngine.Object) null)
        RagdollCreator.CreateJoint(new RagdollCreator.CreateJointParams(bone3.GetComponent<Rigidbody>(), bone2.GetComponent<Rigidbody>(), (Transform) null, worldSwingAxis, limits3, jointType));
      bone1.localRotation = localRotation;
    }

    public static void ClearBipedRagdoll(BipedRagdollReferences r)
    {
      foreach (Transform ragdollTransform in r.GetRagdollTransforms())
        RagdollCreator.ClearTransform(ragdollTransform);
    }

    public static bool IsClear(BipedRagdollReferences r)
    {
      foreach (Component ragdollTransform in r.GetRagdollTransforms())
      {
        if ((UnityEngine.Object) ragdollTransform.GetComponent<Rigidbody>() != (UnityEngine.Object) null)
          return false;
      }
      return true;
    }

    private static Vector3 GetUpperArmToHeadCentroid(BipedRagdollReferences r)
    {
      return Vector3.Lerp(BipedRagdollCreator.GetUpperArmCentroid(r), r.head.position, 0.5f);
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
      public RagdollCreator.JointType joints;
      public float jointRange;
      [Header("Colliders")]
      public float colliderLengthOverlap;
      public RagdollCreator.ColliderType torsoColliders;
      public RagdollCreator.ColliderType headCollider;
      public RagdollCreator.ColliderType armColliders;
      public RagdollCreator.ColliderType handColliders;
      public RagdollCreator.ColliderType legColliders;
      public RagdollCreator.ColliderType footColliders;

      public static BipedRagdollCreator.Options Default
      {
        get
        {
          return new BipedRagdollCreator.Options()
          {
            weight = 75f,
            colliderLengthOverlap = 0.1f,
            jointRange = 1f,
            chest = true,
            headCollider = RagdollCreator.ColliderType.Capsule,
            armColliders = RagdollCreator.ColliderType.Capsule,
            hands = true,
            handColliders = RagdollCreator.ColliderType.Capsule,
            legColliders = RagdollCreator.ColliderType.Capsule,
            feet = true
          };
        }
      }
    }
  }
}
