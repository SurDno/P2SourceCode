using System;

namespace RootMotion
{
  [Serializable]
  public class BipedReferences
  {
    public Transform root;
    public Transform pelvis;
    public Transform leftThigh;
    public Transform leftCalf;
    public Transform leftFoot;
    public Transform rightThigh;
    public Transform rightCalf;
    public Transform rightFoot;
    public Transform leftUpperArm;
    public Transform leftForearm;
    public Transform leftHand;
    public Transform rightUpperArm;
    public Transform rightForearm;
    public Transform rightHand;
    public Transform head;
    public Transform[] spine = new Transform[0];
    public Transform[] eyes = new Transform[0];

    public virtual bool isFilled
    {
      get
      {
        if ((UnityEngine.Object) root == (UnityEngine.Object) null || (UnityEngine.Object) pelvis == (UnityEngine.Object) null || (UnityEngine.Object) leftThigh == (UnityEngine.Object) null || (UnityEngine.Object) leftCalf == (UnityEngine.Object) null || (UnityEngine.Object) leftFoot == (UnityEngine.Object) null || (UnityEngine.Object) rightThigh == (UnityEngine.Object) null || (UnityEngine.Object) rightCalf == (UnityEngine.Object) null || (UnityEngine.Object) rightFoot == (UnityEngine.Object) null || (UnityEngine.Object) leftUpperArm == (UnityEngine.Object) null || (UnityEngine.Object) leftForearm == (UnityEngine.Object) null || (UnityEngine.Object) leftHand == (UnityEngine.Object) null || (UnityEngine.Object) rightUpperArm == (UnityEngine.Object) null || (UnityEngine.Object) rightForearm == (UnityEngine.Object) null || (UnityEngine.Object) rightHand == (UnityEngine.Object) null)
          return false;
        foreach (UnityEngine.Object @object in spine)
        {
          if (@object == (UnityEngine.Object) null)
            return false;
        }
        foreach (UnityEngine.Object eye in eyes)
        {
          if (eye == (UnityEngine.Object) null)
            return false;
        }
        return true;
      }
    }

    public bool isEmpty => IsEmpty(true);

    public virtual bool IsEmpty(bool includeRoot)
    {
      if (includeRoot && (UnityEngine.Object) root != (UnityEngine.Object) null || (UnityEngine.Object) pelvis != (UnityEngine.Object) null || (UnityEngine.Object) head != (UnityEngine.Object) null || (UnityEngine.Object) leftThigh != (UnityEngine.Object) null || (UnityEngine.Object) leftCalf != (UnityEngine.Object) null || (UnityEngine.Object) leftFoot != (UnityEngine.Object) null || (UnityEngine.Object) rightThigh != (UnityEngine.Object) null || (UnityEngine.Object) rightCalf != (UnityEngine.Object) null || (UnityEngine.Object) rightFoot != (UnityEngine.Object) null || (UnityEngine.Object) leftUpperArm != (UnityEngine.Object) null || (UnityEngine.Object) leftForearm != (UnityEngine.Object) null || (UnityEngine.Object) leftHand != (UnityEngine.Object) null || (UnityEngine.Object) rightUpperArm != (UnityEngine.Object) null || (UnityEngine.Object) rightForearm != (UnityEngine.Object) null || (UnityEngine.Object) rightHand != (UnityEngine.Object) null)
        return false;
      foreach (UnityEngine.Object @object in spine)
      {
        if (@object != (UnityEngine.Object) null)
          return false;
      }
      foreach (UnityEngine.Object eye in eyes)
      {
        if (eye != (UnityEngine.Object) null)
          return false;
      }
      return true;
    }

    public virtual bool Contains(Transform t, bool ignoreRoot = false)
    {
      if (!ignoreRoot && (UnityEngine.Object) root == (UnityEngine.Object) t || (UnityEngine.Object) pelvis == (UnityEngine.Object) t || (UnityEngine.Object) leftThigh == (UnityEngine.Object) t || (UnityEngine.Object) leftCalf == (UnityEngine.Object) t || (UnityEngine.Object) leftFoot == (UnityEngine.Object) t || (UnityEngine.Object) rightThigh == (UnityEngine.Object) t || (UnityEngine.Object) rightCalf == (UnityEngine.Object) t || (UnityEngine.Object) rightFoot == (UnityEngine.Object) t || (UnityEngine.Object) leftUpperArm == (UnityEngine.Object) t || (UnityEngine.Object) leftForearm == (UnityEngine.Object) t || (UnityEngine.Object) leftHand == (UnityEngine.Object) t || (UnityEngine.Object) rightUpperArm == (UnityEngine.Object) t || (UnityEngine.Object) rightForearm == (UnityEngine.Object) t || (UnityEngine.Object) rightHand == (UnityEngine.Object) t || (UnityEngine.Object) head == (UnityEngine.Object) t)
        return true;
      foreach (UnityEngine.Object @object in spine)
      {
        if (@object == (UnityEngine.Object) t)
          return true;
      }
      foreach (UnityEngine.Object eye in eyes)
      {
        if (eye == (UnityEngine.Object) t)
          return true;
      }
      return false;
    }

    public static bool AutoDetectReferences(
      ref BipedReferences references,
      Transform root,
      AutoDetectParams autoDetectParams)
    {
      if (references == null)
        references = new BipedReferences();
      references.root = root;
      Animator component = root.GetComponent<Animator>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null && component.isHuman)
      {
        AssignHumanoidReferences(ref references, component, autoDetectParams);
        return true;
      }
      DetectReferencesByNaming(ref references, root, autoDetectParams);
      Warning.logged = false;
      if (!references.isFilled)
      {
        Warning.Log("BipedReferences contains one or more missing Transforms.", root, true);
        return false;
      }
      string message = "";
      if (SetupError(references, ref message))
      {
        Warning.Log(message, references.root, true);
        return false;
      }
      if (SetupWarning(references, ref message))
        Warning.Log(message, references.root, true);
      return true;
    }

    public static void DetectReferencesByNaming(
      ref BipedReferences references,
      Transform root,
      AutoDetectParams autoDetectParams)
    {
      if (references == null)
        references = new BipedReferences();
      Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>();
      DetectLimb(BipedNaming.BoneType.Arm, BipedNaming.BoneSide.Left, ref references.leftUpperArm, ref references.leftForearm, ref references.leftHand, componentsInChildren);
      DetectLimb(BipedNaming.BoneType.Arm, BipedNaming.BoneSide.Right, ref references.rightUpperArm, ref references.rightForearm, ref references.rightHand, componentsInChildren);
      DetectLimb(BipedNaming.BoneType.Leg, BipedNaming.BoneSide.Left, ref references.leftThigh, ref references.leftCalf, ref references.leftFoot, componentsInChildren);
      DetectLimb(BipedNaming.BoneType.Leg, BipedNaming.BoneSide.Right, ref references.rightThigh, ref references.rightCalf, ref references.rightFoot, componentsInChildren);
      references.head = BipedNaming.GetBone(componentsInChildren, BipedNaming.BoneType.Head);
      references.pelvis = BipedNaming.GetNamingMatch(componentsInChildren, BipedNaming.pelvis);
      if (((UnityEngine.Object) references.pelvis == (UnityEngine.Object) null || !Hierarchy.IsAncestor(references.leftThigh, references.pelvis)) && (UnityEngine.Object) references.leftThigh != (UnityEngine.Object) null)
        references.pelvis = references.leftThigh.parent;
      if ((UnityEngine.Object) references.leftUpperArm != (UnityEngine.Object) null && (UnityEngine.Object) references.rightUpperArm != (UnityEngine.Object) null && (UnityEngine.Object) references.pelvis != (UnityEngine.Object) null && (UnityEngine.Object) references.leftThigh != (UnityEngine.Object) null)
      {
        Transform firstCommonAncestor = Hierarchy.GetFirstCommonAncestor(references.leftUpperArm, references.rightUpperArm);
        if ((UnityEngine.Object) firstCommonAncestor != (UnityEngine.Object) null)
        {
          Transform[] array = new Transform[1]
          {
            firstCommonAncestor
          };
          Hierarchy.AddAncestors(array[0], references.pelvis, ref array);
          references.spine = new Transform[0];
          for (int index = array.Length - 1; index > -1; --index)
          {
            if (AddBoneToSpine(array[index], ref references, autoDetectParams))
            {
              Array.Resize(ref references.spine, references.spine.Length + 1);
              references.spine[references.spine.Length - 1] = array[index];
            }
          }
          if ((UnityEngine.Object) references.head == (UnityEngine.Object) null)
          {
            for (int index = 0; index < firstCommonAncestor.childCount; ++index)
            {
              Transform child = firstCommonAncestor.GetChild(index);
              if (!Hierarchy.ContainsChild(child, references.leftUpperArm) && !Hierarchy.ContainsChild(child, references.rightUpperArm))
              {
                references.head = child;
                break;
              }
            }
          }
        }
      }
      Transform[] bonesOfType = BipedNaming.GetBonesOfType(BipedNaming.BoneType.Eye, componentsInChildren);
      references.eyes = new Transform[0];
      if (!autoDetectParams.includeEyes)
        return;
      for (int index = 0; index < bonesOfType.Length; ++index)
      {
        if (AddBoneToEyes(bonesOfType[index], ref references, autoDetectParams))
        {
          Array.Resize(ref references.eyes, references.eyes.Length + 1);
          references.eyes[references.eyes.Length - 1] = bonesOfType[index];
        }
      }
    }

    public static void AssignHumanoidReferences(
      ref BipedReferences references,
      Animator animator,
      AutoDetectParams autoDetectParams)
    {
      if (references == null)
        references = new BipedReferences();
      if ((UnityEngine.Object) animator == (UnityEngine.Object) null || !animator.isHuman)
        return;
      references.spine = new Transform[0];
      references.eyes = new Transform[0];
      references.head = animator.GetBoneTransform(HumanBodyBones.Head);
      references.leftThigh = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
      references.leftCalf = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
      references.leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
      references.rightThigh = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
      references.rightCalf = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
      references.rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
      references.leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
      references.leftForearm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
      references.leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
      references.rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
      references.rightForearm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
      references.rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
      references.pelvis = animator.GetBoneTransform(HumanBodyBones.Hips);
      AddBoneToHierarchy(ref references.spine, animator.GetBoneTransform(HumanBodyBones.Spine));
      AddBoneToHierarchy(ref references.spine, animator.GetBoneTransform(HumanBodyBones.Chest));
      if ((UnityEngine.Object) references.leftUpperArm != (UnityEngine.Object) null && !IsNeckBone(animator.GetBoneTransform(HumanBodyBones.Neck), references.leftUpperArm))
        AddBoneToHierarchy(ref references.spine, animator.GetBoneTransform(HumanBodyBones.Neck));
      if (!autoDetectParams.includeEyes)
        return;
      AddBoneToHierarchy(ref references.eyes, animator.GetBoneTransform(HumanBodyBones.LeftEye));
      AddBoneToHierarchy(ref references.eyes, animator.GetBoneTransform(HumanBodyBones.RightEye));
    }

    public static bool SetupError(BipedReferences references, ref string errorMessage)
    {
      if (!references.isFilled)
      {
        errorMessage = "BipedReferences contains one or more missing Transforms.";
        return true;
      }
      return LimbError(references.leftThigh, references.leftCalf, references.leftFoot, ref errorMessage) || LimbError(references.rightThigh, references.rightCalf, references.rightFoot, ref errorMessage) || LimbError(references.leftUpperArm, references.leftForearm, references.leftHand, ref errorMessage) || LimbError(references.rightUpperArm, references.rightForearm, references.rightHand, ref errorMessage) || SpineError(references, ref errorMessage) || EyesError(references, ref errorMessage);
    }

    public static bool SetupWarning(BipedReferences references, ref string warningMessage)
    {
      return LimbWarning(references.leftThigh, references.leftCalf, references.leftFoot, ref warningMessage) || LimbWarning(references.rightThigh, references.rightCalf, references.rightFoot, ref warningMessage) || LimbWarning(references.leftUpperArm, references.leftForearm, references.leftHand, ref warningMessage) || LimbWarning(references.rightUpperArm, references.rightForearm, references.rightHand, ref warningMessage) || SpineWarning(references, ref warningMessage) || EyesWarning(references, ref warningMessage) || RootHeightWarning(references, ref warningMessage) || FacingAxisWarning(references, ref warningMessage);
    }

    private static bool IsNeckBone(Transform bone, Transform leftUpperArm)
    {
      return (!((UnityEngine.Object) leftUpperArm.parent != (UnityEngine.Object) null) || !((UnityEngine.Object) leftUpperArm.parent == (UnityEngine.Object) bone)) && !Hierarchy.IsAncestor(leftUpperArm, bone);
    }

    private static bool AddBoneToEyes(
      Transform bone,
      ref BipedReferences references,
      AutoDetectParams autoDetectParams)
    {
      return (!((UnityEngine.Object) references.head != (UnityEngine.Object) null) || Hierarchy.IsAncestor(bone, references.head)) && !((UnityEngine.Object) bone.GetComponent<SkinnedMeshRenderer>() != (UnityEngine.Object) null);
    }

    private static bool AddBoneToSpine(
      Transform bone,
      ref BipedReferences references,
      AutoDetectParams autoDetectParams)
    {
      return !((UnityEngine.Object) bone == (UnityEngine.Object) references.root) && (!((UnityEngine.Object) bone == (UnityEngine.Object) references.leftThigh.parent) || autoDetectParams.legsParentInSpine) && (!((UnityEngine.Object) references.pelvis != (UnityEngine.Object) null) || !((UnityEngine.Object) bone == (UnityEngine.Object) references.pelvis) && !Hierarchy.IsAncestor(references.pelvis, bone));
    }

    private static void DetectLimb(
      BipedNaming.BoneType boneType,
      BipedNaming.BoneSide boneSide,
      ref Transform firstBone,
      ref Transform secondBone,
      ref Transform lastBone,
      Transform[] transforms)
    {
      Transform[] bonesOfTypeAndSide = BipedNaming.GetBonesOfTypeAndSide(boneType, boneSide, transforms);
      if (bonesOfTypeAndSide.Length < 3)
        return;
      if (bonesOfTypeAndSide.Length == 3)
      {
        firstBone = bonesOfTypeAndSide[0];
        secondBone = bonesOfTypeAndSide[1];
        lastBone = bonesOfTypeAndSide[2];
      }
      if (bonesOfTypeAndSide.Length <= 3)
        return;
      firstBone = bonesOfTypeAndSide[0];
      secondBone = bonesOfTypeAndSide[2];
      lastBone = bonesOfTypeAndSide[bonesOfTypeAndSide.Length - 1];
    }

    private static void AddBoneToHierarchy(ref Transform[] bones, Transform transform)
    {
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
        return;
      Array.Resize(ref bones, bones.Length + 1);
      bones[bones.Length - 1] = transform;
    }

    private static bool LimbError(
      Transform bone1,
      Transform bone2,
      Transform bone3,
      ref string errorMessage)
    {
      if ((UnityEngine.Object) bone1 == (UnityEngine.Object) null)
      {
        errorMessage = "Bone 1 of a BipedReferences limb is null.";
        return true;
      }
      if ((UnityEngine.Object) bone2 == (UnityEngine.Object) null)
      {
        errorMessage = "Bone 2 of a BipedReferences limb is null.";
        return true;
      }
      if ((UnityEngine.Object) bone3 == (UnityEngine.Object) null)
      {
        errorMessage = "Bone 3 of a BipedReferences limb is null.";
        return true;
      }
      Transform transform = (Transform) Hierarchy.ContainsDuplicate((UnityEngine.Object[]) new Transform[3]
      {
        bone1,
        bone2,
        bone3
      });
      if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
      {
        errorMessage = transform.name + " is represented multiple times in the same BipedReferences limb.";
        return true;
      }
      if (bone2.position == bone1.position)
      {
        errorMessage = "Second bone's position equals first bone's position in the biped's limb.";
        return true;
      }
      if (bone3.position == bone2.position)
      {
        errorMessage = "Third bone's position equals second bone's position in the biped's limb.";
        return true;
      }
      if (Hierarchy.HierarchyIsValid(new Transform[3]
      {
        bone1,
        bone2,
        bone3
      }))
        return false;
      errorMessage = "BipedReferences limb hierarchy is invalid. Bone transforms in a limb do not belong to the same ancestry. Please make sure the bones are parented to each other. Bones: " + bone1.name + ", " + bone2.name + ", " + bone3.name;
      return true;
    }

    private static bool LimbWarning(
      Transform bone1,
      Transform bone2,
      Transform bone3,
      ref string warningMessage)
    {
      if (!(Vector3.Cross(bone2.position - bone1.position, bone3.position - bone1.position) == Vector3.zero))
        return false;
      warningMessage = "BipedReferences limb is completely stretched out in the initial pose. IK solver can not calculate the default bend plane for the limb. Please make sure you character's limbs are at least slightly bent in the initial pose. First bone: " + bone1.name + ", second bone: " + bone2.name + ".";
      return true;
    }

    private static bool SpineError(BipedReferences references, ref string errorMessage)
    {
      if (references.spine.Length == 0)
        return false;
      for (int index = 0; index < references.spine.Length; ++index)
      {
        if ((UnityEngine.Object) references.spine[index] == (UnityEngine.Object) null)
        {
          errorMessage = "BipedReferences spine bone at index " + index + " is null.";
          return true;
        }
      }
      Transform transform = (Transform) Hierarchy.ContainsDuplicate((UnityEngine.Object[]) references.spine);
      if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
      {
        errorMessage = transform.name + " is represented multiple times in BipedReferences spine.";
        return true;
      }
      if (!Hierarchy.HierarchyIsValid(references.spine))
      {
        errorMessage = "BipedReferences spine hierarchy is invalid. Bone transforms in the spine do not belong to the same ancestry. Please make sure the bones are parented to each other.";
        return true;
      }
      for (int index = 0; index < references.spine.Length; ++index)
      {
        bool flag = false;
        if (index == 0 && references.spine[index].position == references.pelvis.position)
          flag = true;
        if (index != 0 && references.spine.Length > 1 && references.spine[index].position == references.spine[index - 1].position)
          flag = true;
        if (flag)
        {
          errorMessage = "Biped's spine bone nr " + index + " position is the same as it's parent spine/pelvis bone's position. Please remove this bone from the spine.";
          return true;
        }
      }
      return false;
    }

    private static bool SpineWarning(BipedReferences references, ref string warningMessage)
    {
      return false;
    }

    private static bool EyesError(BipedReferences references, ref string errorMessage)
    {
      if (references.eyes.Length == 0)
        return false;
      for (int index = 0; index < references.eyes.Length; ++index)
      {
        if ((UnityEngine.Object) references.eyes[index] == (UnityEngine.Object) null)
        {
          errorMessage = "BipedReferences eye bone at index " + index + " is null.";
          return true;
        }
      }
      Transform transform = (Transform) Hierarchy.ContainsDuplicate((UnityEngine.Object[]) references.eyes);
      if (!((UnityEngine.Object) transform != (UnityEngine.Object) null))
        return false;
      errorMessage = transform.name + " is represented multiple times in BipedReferences eyes.";
      return true;
    }

    private static bool EyesWarning(BipedReferences references, ref string warningMessage) => false;

    private static bool RootHeightWarning(BipedReferences references, ref string warningMessage)
    {
      if ((UnityEngine.Object) references.head == (UnityEngine.Object) null)
        return false;
      float verticalOffset = GetVerticalOffset(references.head.position, references.leftFoot.position, references.root.rotation);
      if (GetVerticalOffset(references.root.position, references.leftFoot.position, references.root.rotation) / (double) verticalOffset <= 0.20000000298023224)
        return false;
      warningMessage = "Biped's root Transform's position should be at ground level relative to the character (at the character's feet not at it's pelvis).";
      return true;
    }

    private static bool FacingAxisWarning(BipedReferences references, ref string warningMessage)
    {
      Vector3 vector3_1 = references.rightHand.position - references.leftHand.position;
      Vector3 vector3_2 = references.rightFoot.position - references.leftFoot.position;
      float num1 = Vector3.Dot(vector3_1.normalized, references.root.right);
      float num2 = Vector3.Dot(vector3_2.normalized, references.root.right);
      if (num1 >= 0.0 && num2 >= 0.0)
        return false;
      warningMessage = "Biped does not seem to be facing it's forward axis. Please make sure that in the initial pose the character is facing towards the positive Z axis of the Biped root gameobject.";
      return true;
    }

    private static float GetVerticalOffset(Vector3 p1, Vector3 p2, Quaternion rotation)
    {
      return (Quaternion.Inverse(rotation) * (p1 - p2)).y;
    }

    public struct AutoDetectParams
    {
      public bool legsParentInSpine;
      public bool includeEyes;

      public AutoDetectParams(bool legsParentInSpine, bool includeEyes)
      {
        this.legsParentInSpine = legsParentInSpine;
        this.includeEyes = includeEyes;
      }

      public static AutoDetectParams Default
      {
        get => new AutoDetectParams(true, true);
      }
    }
  }
}
