// Decompiled with JetBrains decompiler
// Type: RootMotion.BipedNaming
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion
{
  public static class BipedNaming
  {
    public static string[] typeLeft = new string[9]
    {
      " L ",
      "_L_",
      "-L-",
      " l ",
      "_l_",
      "-l-",
      "Left",
      "left",
      "CATRigL"
    };
    public static string[] typeRight = new string[9]
    {
      " R ",
      "_R_",
      "-R-",
      " r ",
      "_r_",
      "-r-",
      "Right",
      "right",
      "CATRigR"
    };
    public static string[] typeSpine = new string[16]
    {
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
    };
    public static string[] typeHead = new string[2]
    {
      "Head",
      "head"
    };
    public static string[] typeArm = new string[10]
    {
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
    };
    public static string[] typeLeg = new string[16]
    {
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
    };
    public static string[] typeTail = new string[2]
    {
      "Tail",
      "tail"
    };
    public static string[] typeEye = new string[2]
    {
      "Eye",
      "eye"
    };
    public static string[] typeExclude = new string[6]
    {
      "Nub",
      "Dummy",
      "dummy",
      "Tip",
      "IK",
      "Mesh"
    };
    public static string[] typeExcludeSpine = new string[2]
    {
      "Head",
      "head"
    };
    public static string[] typeExcludeHead = new string[2]
    {
      "Top",
      "End"
    };
    public static string[] typeExcludeArm = new string[19]
    {
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
    };
    public static string[] typeExcludeLeg = new string[7]
    {
      "Toe",
      "toe",
      "Platform",
      "Adjust",
      "adjust",
      "Twist",
      "twist"
    };
    public static string[] typeExcludeTail = new string[0];
    public static string[] typeExcludeEye = new string[6]
    {
      "Lid",
      "lid",
      "Brow",
      "brow",
      "Lash",
      "lash"
    };
    public static string[] pelvis = new string[4]
    {
      "Pelvis",
      nameof (pelvis),
      "Hip",
      "hip"
    };
    public static string[] hand = new string[6]
    {
      "Hand",
      nameof (hand),
      "Wrist",
      "wrist",
      "Palm",
      "palm"
    };
    public static string[] foot = new string[4]
    {
      "Foot",
      nameof (foot),
      "Ankle",
      "ankle"
    };

    public static Transform[] GetBonesOfType(BipedNaming.BoneType boneType, Transform[] bones)
    {
      Transform[] array = new Transform[0];
      foreach (Transform bone in bones)
      {
        if ((UnityEngine.Object) bone != (UnityEngine.Object) null && BipedNaming.GetBoneType(bone.name) == boneType)
        {
          Array.Resize<Transform>(ref array, array.Length + 1);
          array[array.Length - 1] = bone;
        }
      }
      return array;
    }

    public static Transform[] GetBonesOfSide(BipedNaming.BoneSide boneSide, Transform[] bones)
    {
      Transform[] array = new Transform[0];
      foreach (Transform bone in bones)
      {
        if ((UnityEngine.Object) bone != (UnityEngine.Object) null && BipedNaming.GetBoneSide(bone.name) == boneSide)
        {
          Array.Resize<Transform>(ref array, array.Length + 1);
          array[array.Length - 1] = bone;
        }
      }
      return array;
    }

    public static Transform[] GetBonesOfTypeAndSide(
      BipedNaming.BoneType boneType,
      BipedNaming.BoneSide boneSide,
      Transform[] bones)
    {
      Transform[] bonesOfType = BipedNaming.GetBonesOfType(boneType, bones);
      return BipedNaming.GetBonesOfSide(boneSide, bonesOfType);
    }

    public static Transform GetFirstBoneOfTypeAndSide(
      BipedNaming.BoneType boneType,
      BipedNaming.BoneSide boneSide,
      Transform[] bones)
    {
      Transform[] bonesOfTypeAndSide = BipedNaming.GetBonesOfTypeAndSide(boneType, boneSide, bones);
      return bonesOfTypeAndSide.Length == 0 ? (Transform) null : bonesOfTypeAndSide[0];
    }

    public static Transform GetNamingMatch(Transform[] transforms, params string[][] namings)
    {
      foreach (Transform transform in transforms)
      {
        bool flag = true;
        foreach (string[] naming in namings)
        {
          if (!BipedNaming.matchesNaming(transform.name, naming))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          return transform;
      }
      return (Transform) null;
    }

    public static BipedNaming.BoneType GetBoneType(string boneName)
    {
      if (BipedNaming.isSpine(boneName))
        return BipedNaming.BoneType.Spine;
      if (BipedNaming.isHead(boneName))
        return BipedNaming.BoneType.Head;
      if (BipedNaming.isArm(boneName))
        return BipedNaming.BoneType.Arm;
      if (BipedNaming.isLeg(boneName))
        return BipedNaming.BoneType.Leg;
      if (BipedNaming.isTail(boneName))
        return BipedNaming.BoneType.Tail;
      return BipedNaming.isEye(boneName) ? BipedNaming.BoneType.Eye : BipedNaming.BoneType.Unassigned;
    }

    public static BipedNaming.BoneSide GetBoneSide(string boneName)
    {
      if (BipedNaming.isLeft(boneName))
        return BipedNaming.BoneSide.Left;
      return BipedNaming.isRight(boneName) ? BipedNaming.BoneSide.Right : BipedNaming.BoneSide.Center;
    }

    public static Transform GetBone(
      Transform[] transforms,
      BipedNaming.BoneType boneType,
      BipedNaming.BoneSide boneSide = BipedNaming.BoneSide.Center,
      params string[][] namings)
    {
      return BipedNaming.GetNamingMatch(BipedNaming.GetBonesOfTypeAndSide(boneType, boneSide, transforms), namings);
    }

    private static bool isLeft(string boneName)
    {
      return BipedNaming.matchesNaming(boneName, BipedNaming.typeLeft) || BipedNaming.lastLetter(boneName) == "L" || BipedNaming.firstLetter(boneName) == "L";
    }

    private static bool isRight(string boneName)
    {
      return BipedNaming.matchesNaming(boneName, BipedNaming.typeRight) || BipedNaming.lastLetter(boneName) == "R" || BipedNaming.firstLetter(boneName) == "R";
    }

    private static bool isSpine(string boneName)
    {
      return BipedNaming.matchesNaming(boneName, BipedNaming.typeSpine) && !BipedNaming.excludesNaming(boneName, BipedNaming.typeExcludeSpine);
    }

    private static bool isHead(string boneName)
    {
      return BipedNaming.matchesNaming(boneName, BipedNaming.typeHead) && !BipedNaming.excludesNaming(boneName, BipedNaming.typeExcludeHead);
    }

    private static bool isArm(string boneName)
    {
      return BipedNaming.matchesNaming(boneName, BipedNaming.typeArm) && !BipedNaming.excludesNaming(boneName, BipedNaming.typeExcludeArm);
    }

    private static bool isLeg(string boneName)
    {
      return BipedNaming.matchesNaming(boneName, BipedNaming.typeLeg) && !BipedNaming.excludesNaming(boneName, BipedNaming.typeExcludeLeg);
    }

    private static bool isTail(string boneName)
    {
      return BipedNaming.matchesNaming(boneName, BipedNaming.typeTail) && !BipedNaming.excludesNaming(boneName, BipedNaming.typeExcludeTail);
    }

    private static bool isEye(string boneName)
    {
      return BipedNaming.matchesNaming(boneName, BipedNaming.typeEye) && !BipedNaming.excludesNaming(boneName, BipedNaming.typeExcludeEye);
    }

    private static bool isTypeExclude(string boneName)
    {
      return BipedNaming.matchesNaming(boneName, BipedNaming.typeExclude);
    }

    private static bool matchesNaming(string boneName, string[] namingConvention)
    {
      if (BipedNaming.excludesNaming(boneName, BipedNaming.typeExclude))
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
        if (BipedNaming.LastLetterIs(boneName, letter))
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
