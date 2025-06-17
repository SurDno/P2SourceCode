using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
  [Serializable]
  public struct BipedRagdollReferences
  {
    public Transform root;
    public Transform hips;
    public Transform spine;
    public Transform chest;
    public Transform head;
    public Transform leftUpperLeg;
    public Transform leftLowerLeg;
    public Transform leftFoot;
    public Transform rightUpperLeg;
    public Transform rightLowerLeg;
    public Transform rightFoot;
    public Transform leftUpperArm;
    public Transform leftLowerArm;
    public Transform leftHand;
    public Transform rightUpperArm;
    public Transform rightLowerArm;
    public Transform rightHand;

    public bool IsValid(ref string msg)
    {
      if (root == null || hips == null || head == null || leftUpperArm == null || leftLowerArm == null || leftHand == null || rightUpperArm == null || rightLowerArm == null || rightHand == null || leftUpperLeg == null || leftLowerLeg == null || leftFoot == null || rightUpperLeg == null || rightLowerLeg == null || rightFoot == null)
      {
        msg = "Invalid References, one or more Transforms missing.";
        return false;
      }
      Transform[] transformArray = [
        root,
        hips,
        head,
        leftUpperArm,
        leftLowerArm,
        leftHand,
        rightUpperArm,
        rightLowerArm,
        rightHand,
        leftUpperLeg,
        leftLowerLeg,
        leftFoot,
        rightUpperLeg,
        rightLowerLeg,
        rightFoot
      ];
      for (int index = 1; index < transformArray.Length; ++index)
      {
        if (!IsChildRecursive(transformArray[index], root))
        {
          msg = "Invalid References, " + transformArray[index].name + " is not in the Root's hierarchy.";
          return false;
        }
      }
      for (int index1 = 0; index1 < transformArray.Length; ++index1)
      {
        for (int index2 = 0; index2 < transformArray.Length; ++index2)
        {
          if (index1 != index2 && transformArray[index1] == transformArray[index2])
          {
            msg = "Invalid References, " + transformArray[index1].name + " is represented more than once.";
            return false;
          }
        }
      }
      return true;
    }

    private bool IsChildRecursive(Transform t, Transform parent)
    {
      if (t.parent == parent)
        return true;
      return t.parent != null && IsChildRecursive(t.parent, parent);
    }

    public bool IsEmpty(bool considerRoot)
    {
      return (!considerRoot || !(root != null)) && !(hips != null) && !(head != null) && !(spine != null) && !(chest != null) && !(leftUpperArm != null) && !(leftLowerArm != null) && !(leftHand != null) && !(rightUpperArm != null) && !(rightLowerArm != null) && !(rightHand != null) && !(leftUpperLeg != null) && !(leftLowerLeg != null) && !(leftFoot != null) && !(rightUpperLeg != null) && !(rightLowerLeg != null) && !(rightFoot != null);
    }

    public bool Contains(Transform t, bool ignoreRoot = false)
    {
      return !ignoreRoot && root == t || hips == t || spine == t || chest == t || leftUpperLeg == t || leftLowerLeg == t || leftFoot == t || rightUpperLeg == t || rightLowerLeg == t || rightFoot == t || leftUpperArm == t || leftLowerArm == t || leftHand == t || rightUpperArm == t || rightLowerArm == t || rightHand == t || head == t;
    }

    public Transform[] GetRagdollTransforms()
    {
      return [
        hips,
        spine,
        chest,
        head,
        leftUpperArm,
        leftLowerArm,
        leftHand,
        rightUpperArm,
        rightLowerArm,
        rightHand,
        leftUpperLeg,
        leftLowerLeg,
        leftFoot,
        rightUpperLeg,
        rightLowerLeg,
        rightFoot
      ];
    }

    public static BipedRagdollReferences FromAvatar(Animator animator)
    {
      BipedRagdollReferences ragdollReferences = new BipedRagdollReferences();
      if (!animator.isHuman)
        return ragdollReferences;
      ragdollReferences.root = animator.transform;
      ragdollReferences.hips = animator.GetBoneTransform(HumanBodyBones.Hips);
      ragdollReferences.spine = animator.GetBoneTransform(HumanBodyBones.Spine);
      ragdollReferences.chest = animator.GetBoneTransform(HumanBodyBones.Chest);
      ragdollReferences.head = animator.GetBoneTransform(HumanBodyBones.Head);
      ragdollReferences.leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
      ragdollReferences.leftLowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
      ragdollReferences.leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
      ragdollReferences.rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
      ragdollReferences.rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
      ragdollReferences.rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
      ragdollReferences.leftUpperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
      ragdollReferences.leftLowerLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
      ragdollReferences.leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
      ragdollReferences.rightUpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
      ragdollReferences.rightLowerLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
      ragdollReferences.rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
      return ragdollReferences;
    }

    public static BipedRagdollReferences FromBipedReferences(BipedReferences biped)
    {
      BipedRagdollReferences ragdollReferences = new BipedRagdollReferences();
      ragdollReferences.root = biped.root;
      ragdollReferences.hips = biped.pelvis;
      if (biped.spine != null && biped.spine.Length != 0)
      {
        ragdollReferences.spine = biped.spine[0];
        if (biped.spine.Length > 1)
          ragdollReferences.chest = biped.spine[biped.spine.Length - 1];
      }
      ragdollReferences.head = biped.head;
      ragdollReferences.leftUpperArm = biped.leftUpperArm;
      ragdollReferences.leftLowerArm = biped.leftForearm;
      ragdollReferences.leftHand = biped.leftHand;
      ragdollReferences.rightUpperArm = biped.rightUpperArm;
      ragdollReferences.rightLowerArm = biped.rightForearm;
      ragdollReferences.rightHand = biped.rightHand;
      ragdollReferences.leftUpperLeg = biped.leftThigh;
      ragdollReferences.leftLowerLeg = biped.leftCalf;
      ragdollReferences.leftFoot = biped.leftFoot;
      ragdollReferences.rightUpperLeg = biped.rightThigh;
      ragdollReferences.rightLowerLeg = biped.rightCalf;
      ragdollReferences.rightFoot = biped.rightFoot;
      return ragdollReferences;
    }
  }
}
