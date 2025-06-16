using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverLimb : IKSolverTrigonometric
  {
    public AvatarIKGoal goal;
    public BendModifier bendModifier;
    [Range(0.0f, 1f)]
    public float maintainRotationWeight;
    [Range(0.0f, 1f)]
    public float bendModifierWeight = 1f;
    public Transform bendGoal;
    private bool maintainBendFor1Frame;
    private bool maintainRotationFor1Frame;
    private Quaternion defaultRootRotation;
    private Quaternion parentDefaultRotation;
    private Quaternion bone3RotationBeforeSolve;
    private Quaternion maintainRotation;
    private Quaternion bone3DefaultRotation;
    private Vector3 _bendNormal;
    private Vector3 animationNormal;
    private AxisDirection[] axisDirectionsLeft = new AxisDirection[4];
    private AxisDirection[] axisDirectionsRight = new AxisDirection[4];

    public void MaintainRotation()
    {
      if (!initiated)
        return;
      maintainRotation = bone3.transform.rotation;
      maintainRotationFor1Frame = true;
    }

    public void MaintainBend()
    {
      if (!initiated)
        return;
      animationNormal = bone1.GetBendNormalFromCurrentRotation();
      maintainBendFor1Frame = true;
    }

    protected override void OnInitiateVirtual()
    {
      defaultRootRotation = root.rotation;
      if (bone1.transform.parent != null)
        parentDefaultRotation = Quaternion.Inverse(defaultRootRotation) * bone1.transform.parent.rotation;
      if (bone3.rotationLimit != null)
        bone3.rotationLimit.Disable();
      bone3DefaultRotation = bone3.transform.rotation;
      Vector3 vector3 = Vector3.Cross(bone2.transform.position - bone1.transform.position, bone3.transform.position - bone2.transform.position);
      if (vector3 != Vector3.zero)
        bendNormal = vector3;
      animationNormal = bendNormal;
      StoreAxisDirections(ref axisDirectionsLeft);
      StoreAxisDirections(ref axisDirectionsRight);
    }

    protected override void OnUpdateVirtual()
    {
      if (IKPositionWeight > 0.0)
      {
        bendModifierWeight = Mathf.Clamp(bendModifierWeight, 0.0f, 1f);
        maintainRotationWeight = Mathf.Clamp(maintainRotationWeight, 0.0f, 1f);
        _bendNormal = bendNormal;
        bendNormal = GetModifiedBendNormal();
      }
      if (maintainRotationWeight * (double) IKPositionWeight <= 0.0)
        return;
      bone3RotationBeforeSolve = maintainRotationFor1Frame ? maintainRotation : bone3.transform.rotation;
      maintainRotationFor1Frame = false;
    }

    protected override void OnPostSolveVirtual()
    {
      if (IKPositionWeight > 0.0)
        bendNormal = _bendNormal;
      if (maintainRotationWeight * (double) IKPositionWeight <= 0.0)
        return;
      bone3.transform.rotation = Quaternion.Slerp(bone3.transform.rotation, bone3RotationBeforeSolve, maintainRotationWeight * IKPositionWeight);
    }

    public IKSolverLimb()
    {
    }

    public IKSolverLimb(AvatarIKGoal goal) => this.goal = goal;

    private AxisDirection[] axisDirections
    {
      get
      {
        return goal == AvatarIKGoal.LeftHand ? axisDirectionsLeft : axisDirectionsRight;
      }
    }

    private void StoreAxisDirections(ref AxisDirection[] axisDirections)
    {
      axisDirections[0] = new AxisDirection(Vector3.zero, new Vector3(-1f, 0.0f, 0.0f));
      axisDirections[1] = new AxisDirection(new Vector3(0.5f, 0.0f, -0.2f), new Vector3(-0.5f, -1f, 1f));
      axisDirections[2] = new AxisDirection(new Vector3(-0.5f, -1f, -0.2f), new Vector3(0.0f, 0.5f, -1f));
      axisDirections[3] = new AxisDirection(new Vector3(-0.5f, -0.5f, 1f), new Vector3(-1f, -1f, -1f));
    }

    private Vector3 GetModifiedBendNormal()
    {
      float bendModifierWeight = this.bendModifierWeight;
      if (bendModifierWeight <= 0.0)
        return bendNormal;
      switch (bendModifier)
      {
        case BendModifier.Animation:
          if (!maintainBendFor1Frame)
            MaintainBend();
          maintainBendFor1Frame = false;
          return Vector3.Lerp(bendNormal, animationNormal, bendModifierWeight);
        case BendModifier.Target:
          return Quaternion.Slerp(Quaternion.identity, IKRotation * Quaternion.Inverse(bone3DefaultRotation), bendModifierWeight) * bendNormal;
        case BendModifier.Parent:
          return bone1.transform.parent == null ? bendNormal : Quaternion.Slerp(Quaternion.identity, bone1.transform.parent.rotation * Quaternion.Inverse(parentDefaultRotation) * Quaternion.Inverse(defaultRootRotation), bendModifierWeight) * bendNormal;
        case BendModifier.Arm:
          if (bone1.transform.parent == null)
            return bendNormal;
          if (goal == AvatarIKGoal.LeftFoot || goal == AvatarIKGoal.RightFoot)
          {
            if (!Warning.logged)
              LogWarning("Trying to use the 'Arm' bend modifier on a leg.");
            return bendNormal;
          }
          Vector3 normalized = (IKPosition - bone1.transform.position).normalized;
          Vector3 rhs = Quaternion.Inverse(bone1.transform.parent.rotation * Quaternion.Inverse(parentDefaultRotation)) * normalized;
          if (goal == AvatarIKGoal.LeftHand)
            rhs.x = -rhs.x;
          for (int index = 1; index < axisDirections.Length; ++index)
          {
            axisDirections[index].dot = Mathf.Clamp(Vector3.Dot(axisDirections[index].direction, rhs), 0.0f, 1f);
            axisDirections[index].dot = Interp.Float(axisDirections[index].dot, InterpolationMode.InOutQuintic);
          }
          Vector3 a = axisDirections[0].axis;
          for (int index = 1; index < axisDirections.Length; ++index)
            a = Vector3.Slerp(a, axisDirections[index].axis, axisDirections[index].dot);
          if (goal == AvatarIKGoal.LeftHand)
          {
            a.x = -a.x;
            a = -a;
          }
          Vector3 b1 = bone1.transform.parent.rotation * Quaternion.Inverse(parentDefaultRotation) * a;
          return bendModifierWeight >= 1.0 ? b1 : Vector3.Lerp(bendNormal, b1, bendModifierWeight);
        case BendModifier.Goal:
          if (bendGoal == null)
          {
            if (!Warning.logged)
              LogWarning("Trying to use the 'Goal' Bend Modifier, but the Bend Goal is unassigned.");
            return bendNormal;
          }
          Vector3 b2 = Vector3.Cross(bendGoal.position - bone1.transform.position, IKPosition - bone1.transform.position);
          if (b2 == Vector3.zero)
            return bendNormal;
          return bendModifierWeight >= 1.0 ? b2 : Vector3.Lerp(bendNormal, b2, bendModifierWeight);
        default:
          return bendNormal;
      }
    }

    [Serializable]
    public enum BendModifier
    {
      Animation,
      Target,
      Parent,
      Arm,
      Goal,
    }

    [Serializable]
    public struct AxisDirection
    {
      public Vector3 direction;
      public Vector3 axis;
      public float dot;

      public AxisDirection(Vector3 direction, Vector3 axis)
      {
        this.direction = direction.normalized;
        this.axis = axis.normalized;
        dot = 0.0f;
      }
    }
  }
}
