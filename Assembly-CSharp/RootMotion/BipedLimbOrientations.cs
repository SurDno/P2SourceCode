using System;
using UnityEngine;

namespace RootMotion
{
  [Serializable]
  public class BipedLimbOrientations(
    BipedLimbOrientations.LimbOrientation leftArm,
    BipedLimbOrientations.LimbOrientation rightArm,
    BipedLimbOrientations.LimbOrientation leftLeg,
    BipedLimbOrientations.LimbOrientation rightLeg) {
    public LimbOrientation leftArm = leftArm;
    public LimbOrientation rightArm = rightArm;
    public LimbOrientation leftLeg = leftLeg;
    public LimbOrientation rightLeg = rightLeg;

    public static BipedLimbOrientations UMA => new(new LimbOrientation(Vector3.forward, Vector3.forward, Vector3.forward), new LimbOrientation(Vector3.forward, Vector3.forward, Vector3.back), new LimbOrientation(Vector3.forward, Vector3.forward, Vector3.down), new LimbOrientation(Vector3.forward, Vector3.forward, Vector3.down));

    public static BipedLimbOrientations MaxBiped => new(new LimbOrientation(Vector3.down, Vector3.down, Vector3.down), new LimbOrientation(Vector3.down, Vector3.down, Vector3.up), new LimbOrientation(Vector3.up, Vector3.up, Vector3.back), new LimbOrientation(Vector3.up, Vector3.up, Vector3.back));

    [Serializable]
    public class LimbOrientation(
      Vector3 upperBoneForwardAxis,
      Vector3 lowerBoneForwardAxis,
      Vector3 lastBoneLeftAxis) {
      public Vector3 upperBoneForwardAxis = upperBoneForwardAxis;
      public Vector3 lowerBoneForwardAxis = lowerBoneForwardAxis;
      public Vector3 lastBoneLeftAxis = lastBoneLeftAxis;
    }
  }
}
