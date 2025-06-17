using System;
using UnityEngine;

namespace RootMotion
{
  public static class BipedNaming
  {
    public static string[] typeLeft = [
      " L ",
      "_L_",
      "-L-",
      " l ",
      "_l_",
      "-l-",
      "Left",
      "left",
      "CATRigL"
    ];
    public static string[] typeRight = [
      " R ",
      "_R_",
      "-R-",
      " r ",
      "_r_",
      "-r-",
      "Right",
      "right",
      "CATRigR"
    ];
    public static string[] typeSpine = [
      "Spine",
      "spine",
      "Pelvis",
      nameof (pelvis),
      "Root",
      "root",
      "Torso",
      "torso",
      "Body",
      "body",
      "Hips",
      "hips",
      "Neck",
      "neck",
      "Chest",
      "chest"
    ];
    public static string[] typeHead = [
      "Head",
      "head"
    ];
    public static string[] typeArm = [
      "Arm",
      "arm",
      "Hand",
      nameof (hand),
      "Wrist",
      "Wrist",
      "Elbow",
      "elbow",
      "Palm",
      "palm"
    ];
    public static string[] typeLeg = [
      "Leg",
      "leg",
      "Thigh",
      "thigh",
      "Calf",
      "calf",
      "Femur",
      "femur",
      "Knee",
      "knee",
      "Foot",
      nameof (foot),
      "Ankle",
      "ankle",
      "Hip",
      "hip"
    ];
    public static string[] typeTail = [
      "Tail",
      "tail"
    ];
    public static string[] typeEye = [
      "Eye",
      "eye"
    ];
    public static string[] typeExclude = [
      "Nub",
      "Dummy",
      "dummy",
      "Tip",
      "IK",
      "Mesh"
    ];
    public static string[] typeExcludeSpine = [
      "Head",
      "head"
    ];
    public static string[] typeExcludeHead = [
      "Top",
      "End"
    ];
    public static string[] typeExcludeArm = [
      "Collar",
      "collar",
      "Clavicle",
      "clavicle",
      "Finger",
      "finger",
      "Index",
      "index",
      "Mid",
      "mid",
      "Pinky",
      "pinky",
      "Ring",
      "Thumb",
      "thumb",
      "Adjust",
      "adjust",
      "Twist",
      "twist"
    ];
    public static string[] typeExcludeLeg = [
      "Toe",
      "toe",
      "Platform",
      "Adjust",
      "adjust",
      "Twist",
      "twist"
    ];
    public static string[] typeExcludeTail = [];
    public static string[] typeExcludeEye = [
      "Lid",
      "lid",
      "Brow",
      "brow",
      "Lash",
      "lash"
    ];
    public static string[] pelvis = [
      "Pelvis",
      nameof (pelvis),
      "Hip",
      "hip"
    ];
    public static string[] hand = [
      "Hand",
      nameof (hand),
      "Wrist",
      "wrist",
      "Palm",
      "palm"
    ];
    public static string[] foot = [
      "Foot",
      nameof (foot),
      "Ankle",
      "ankle"
    ];

    public static Transform[] GetBonesOfType(BoneType boneType, Transform[] bones)
    {
      Transform[] array = [];
      foreach (Transform bone in bones)
      {
        if (bone != null && GetBoneType(bone.name) == boneType)
        {
          Array.Resize(ref array, array.Length + 1);
          array[array.Length - 1] = bone;
        }
      }
      return array;
    }

    public static Transform[] GetBonesOfSide(BoneSide boneSide, Transform[] bones)
    {
      Transform[] array = [];
      foreach (Transform bone in bones)
      {
        if (bone != null && GetBoneSide(bone.name) == boneSide)
        {
          Array.Resize(ref array, array.Length + 1);
          array[array.Length - 1] = bone;
        }
      }
      return array;
    }

    public static Transform[] GetBonesOfTypeAndSide(
      BoneType boneType,
      BoneSide boneSide,
      Transform[] bones)
    {
      Transform[] bonesOfType = GetBonesOfType(boneType, bones);
      return GetBonesOfSide(boneSide, bonesOfType);
    }

    public static Transform GetFirstBoneOfTypeAndSide(
      BoneType boneType,
      BoneSide boneSide,
      Transform[] bones)
    {
      Transform[] bonesOfTypeAndSide = GetBonesOfTypeAndSide(boneType, boneSide, bones);
      return bonesOfTypeAndSide.Length == 0 ? null : bonesOfTypeAndSide[0];
    }

    public static Transform GetNamingMatch(Transform[] transforms, params string[][] namings)
    {
      foreach (Transform transform in transforms)
      {
        bool flag = true;
        foreach (string[] naming in namings)
        {
          if (!matchesNaming(transform.name, naming))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          return transform;
      }
      return null;
    }

    public static BoneType GetBoneType(string boneName)
    {
      if (isSpine(boneName))
        return BoneType.Spine;
      if (isHead(boneName))
        return BoneType.Head;
      if (isArm(boneName))
        return BoneType.Arm;
      if (isLeg(boneName))
        return BoneType.Leg;
      if (isTail(boneName))
        return BoneType.Tail;
      return isEye(boneName) ? BoneType.Eye : BoneType.Unassigned;
    }

    public static BoneSide GetBoneSide(string boneName)
    {
      if (isLeft(boneName))
        return BoneSide.Left;
      return isRight(boneName) ? BoneSide.Right : BoneSide.Center;
    }

    public static Transform GetBone(
      Transform[] transforms,
      BoneType boneType,
      BoneSide boneSide = BoneSide.Center,
      params string[][] namings)
    {
      return GetNamingMatch(GetBonesOfTypeAndSide(boneType, boneSide, transforms), namings);
    }

    private static bool isLeft(string boneName)
    {
      return matchesNaming(boneName, typeLeft) || lastLetter(boneName) == "L" || firstLetter(boneName) == "L";
    }

    private static bool isRight(string boneName)
    {
      return matchesNaming(boneName, typeRight) || lastLetter(boneName) == "R" || firstLetter(boneName) == "R";
    }

    private static bool isSpine(string boneName)
    {
      return matchesNaming(boneName, typeSpine) && !excludesNaming(boneName, typeExcludeSpine);
    }

    private static bool isHead(string boneName)
    {
      return matchesNaming(boneName, typeHead) && !excludesNaming(boneName, typeExcludeHead);
    }

    private static bool isArm(string boneName)
    {
      return matchesNaming(boneName, typeArm) && !excludesNaming(boneName, typeExcludeArm);
    }

    private static bool isLeg(string boneName)
    {
      return matchesNaming(boneName, typeLeg) && !excludesNaming(boneName, typeExcludeLeg);
    }

    private static bool isTail(string boneName)
    {
      return matchesNaming(boneName, typeTail) && !excludesNaming(boneName, typeExcludeTail);
    }

    private static bool isEye(string boneName)
    {
      return matchesNaming(boneName, typeEye) && !excludesNaming(boneName, typeExcludeEye);
    }

    private static bool isTypeExclude(string boneName)
    {
      return matchesNaming(boneName, typeExclude);
    }

    private static bool matchesNaming(string boneName, string[] namingConvention)
    {
      if (excludesNaming(boneName, typeExclude))
        return false;
      foreach (string str in namingConvention)
      {
        if (boneName.Contains(str))
          return true;
      }
      return false;
    }

    private static bool excludesNaming(string boneName, string[] namingConvention)
    {
      foreach (string str in namingConvention)
      {
        if (boneName.Contains(str))
          return true;
      }
      return false;
    }

    private static bool matchesLastLetter(string boneName, string[] namingConvention)
    {
      foreach (string letter in namingConvention)
      {
        if (LastLetterIs(boneName, letter))
          return true;
      }
      return false;
    }

    private static bool LastLetterIs(string boneName, string letter)
    {
      return boneName.Substring(boneName.Length - 1, 1) == letter;
    }

    private static string firstLetter(string boneName)
    {
      return boneName.Length > 0 ? boneName.Substring(0, 1) : "";
    }

    private static string lastLetter(string boneName)
    {
      return boneName.Length > 0 ? boneName.Substring(boneName.Length - 1, 1) : "";
    }

    [Serializable]
    public enum BoneType
    {
      Unassigned,
      Spine,
      Head,
      Arm,
      Leg,
      Tail,
      Eye,
    }

    [Serializable]
    public enum BoneSide
    {
      Center,
      Left,
      Right,
    }
  }
}
