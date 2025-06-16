using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class Finger
  {
    [Tooltip("Master Weight for the finger.")]
    [Range(0.0f, 1f)]
    public float weight = 1f;
    [Tooltip("The weight of rotating the finger tip and bending the finger to the target.")]
    [Range(0.0f, 1f)]
    public float rotationWeight = 1f;
    [Tooltip("Rotational degrees of freedom. When set to 'One' the fingers will be able to be rotated only around a single axis. When 3, all 3 axes are free to rotate around.")]
    public DOF rotationDOF;
    [Tooltip("The first bone of the finger.")]
    public Transform bone1;
    [Tooltip("The second bone of the finger.")]
    public Transform bone2;
    [Tooltip("The (optional) third bone of the finger. This can be ignored for thumbs.")]
    public Transform bone3;
    [Tooltip("The fingertip object. If your character doesn't have tip bones, you can create an empty GameObject and parent it to the last bone in the finger. Place it to the tip of the finger.")]
    public Transform tip;
    [Tooltip("The IK target (optional, can use IKPosition and IKRotation directly).")]
    public Transform target;
    private IKSolverLimb solver;
    private Quaternion bone3RelativeToTarget;
    private Vector3 bone3DefaultLocalPosition;
    private Quaternion bone3DefaultLocalRotation;
    private Vector3 bone1Axis;
    private Vector3 tipAxis;

    public bool initiated { get; private set; }

    public Vector3 IKPosition
    {
      get => solver.IKPosition;
      set => solver.IKPosition = value;
    }

    public Quaternion IKRotation
    {
      get => solver.IKRotation;
      set => solver.IKRotation = value;
    }

    public bool IsValid(ref string errorMessage)
    {
      if (!(bone1 == null) && !(bone2 == null) && !(tip == null))
        return true;
      errorMessage = "One of the bones in the Finger Rig is null, can not initiate solvers.";
      return false;
    }

    public void Initiate(Transform hand, int index)
    {
      initiated = false;
      string empty = string.Empty;
      if (!IsValid(ref empty))
      {
        Warning.Log(empty, hand);
      }
      else
      {
        solver = new IKSolverLimb();
        solver.IKPositionWeight = weight;
        solver.bendModifier = IKSolverLimb.BendModifier.Target;
        solver.bendModifierWeight = 1f;
        Vector3 vector3 = Vector3.Cross(bone2.position - bone1.position, tip.position - bone1.position);
        bone1Axis = Quaternion.Inverse(bone1.rotation) * vector3;
        tipAxis = Quaternion.Inverse(tip.rotation) * vector3;
        IKPosition = tip.position;
        IKRotation = tip.rotation;
        if (bone3 != null)
        {
          bone3RelativeToTarget = Quaternion.Inverse(IKRotation) * bone3.rotation;
          bone3DefaultLocalPosition = bone3.localPosition;
          bone3DefaultLocalRotation = bone3.localRotation;
        }
        solver.SetChain(bone1, bone2, tip, hand);
        solver.Initiate(hand);
        initiated = true;
      }
    }

    public void FixTransforms()
    {
      if (!initiated)
        return;
      solver.FixTransforms();
      if (!(bone3 != null))
        return;
      bone3.localPosition = bone3DefaultLocalPosition;
      bone3.localRotation = bone3DefaultLocalRotation;
    }

    public void Update(float masterWeight)
    {
      if (!initiated)
        return;
      float num = weight * masterWeight;
      if (num <= 0.0)
        return;
      solver.target = target;
      if (target != null)
      {
        IKPosition = target.position;
        IKRotation = target.rotation;
      }
      if (rotationDOF == DOF.One)
        IKRotation = Quaternion.FromToRotation(IKRotation * tipAxis, bone1.rotation * bone1Axis) * IKRotation;
      if (bone3 != null)
        bone3.rotation = num * (double) rotationWeight < 1.0 ? Quaternion.Lerp(bone3.rotation, IKRotation * bone3RelativeToTarget, num * rotationWeight) : IKRotation * bone3RelativeToTarget;
      solver.IKPositionWeight = num;
      solver.IKRotationWeight = rotationWeight;
      solver.bendModifierWeight = rotationWeight;
      solver.Update();
    }

    [Serializable]
    public enum DOF
    {
      One,
      Three,
    }
  }
}
