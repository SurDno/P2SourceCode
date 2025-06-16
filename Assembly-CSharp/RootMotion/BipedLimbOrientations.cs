// Decompiled with JetBrains decompiler
// Type: RootMotion.BipedLimbOrientations
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion
{
  [Serializable]
  public class BipedLimbOrientations
  {
    public BipedLimbOrientations.LimbOrientation leftArm;
    public BipedLimbOrientations.LimbOrientation rightArm;
    public BipedLimbOrientations.LimbOrientation leftLeg;
    public BipedLimbOrientations.LimbOrientation rightLeg;

    public BipedLimbOrientations(
      BipedLimbOrientations.LimbOrientation leftArm,
      BipedLimbOrientations.LimbOrientation rightArm,
      BipedLimbOrientations.LimbOrientation leftLeg,
      BipedLimbOrientations.LimbOrientation rightLeg)
    {
      this.leftArm = leftArm;
      this.rightArm = rightArm;
      this.leftLeg = leftLeg;
      this.rightLeg = rightLeg;
    }

    public static BipedLimbOrientations UMA
    {
      get
      {
        return new BipedLimbOrientations(new BipedLimbOrientations.LimbOrientation(Vector3.forward, Vector3.forward, Vector3.forward), new BipedLimbOrientations.LimbOrientation(Vector3.forward, Vector3.forward, Vector3.back), new BipedLimbOrientations.LimbOrientation(Vector3.forward, Vector3.forward, Vector3.down), new BipedLimbOrientations.LimbOrientation(Vector3.forward, Vector3.forward, Vector3.down));
      }
    }

    public static BipedLimbOrientations MaxBiped
    {
      get
      {
        return new BipedLimbOrientations(new BipedLimbOrientations.LimbOrientation(Vector3.down, Vector3.down, Vector3.down), new BipedLimbOrientations.LimbOrientation(Vector3.down, Vector3.down, Vector3.up), new BipedLimbOrientations.LimbOrientation(Vector3.up, Vector3.up, Vector3.back), new BipedLimbOrientations.LimbOrientation(Vector3.up, Vector3.up, Vector3.back));
      }
    }

    [Serializable]
    public class LimbOrientation
    {
      public Vector3 upperBoneForwardAxis;
      public Vector3 lowerBoneForwardAxis;
      public Vector3 lastBoneLeftAxis;

      public LimbOrientation(
        Vector3 upperBoneForwardAxis,
        Vector3 lowerBoneForwardAxis,
        Vector3 lastBoneLeftAxis)
      {
        this.upperBoneForwardAxis = upperBoneForwardAxis;
        this.lowerBoneForwardAxis = lowerBoneForwardAxis;
        this.lastBoneLeftAxis = lastBoneLeftAxis;
      }
    }
  }
}
