using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverTrigonometric : IKSolver
  {
    public Transform target;
    [Range(0.0f, 1f)]
    public float IKRotationWeight = 1f;
    public Quaternion IKRotation = Quaternion.identity;
    public Vector3 bendNormal = Vector3.right;
    public TrigonometricBone bone1 = new TrigonometricBone();
    public TrigonometricBone bone2 = new TrigonometricBone();
    public TrigonometricBone bone3 = new TrigonometricBone();
    protected Vector3 weightIKPosition;
    protected bool directHierarchy = true;

    public void SetBendGoalPosition(Vector3 goalPosition, float weight)
    {
      if (!initiated || weight <= 0.0)
        return;
      Vector3 b = Vector3.Cross(goalPosition - bone1.transform.position, IKPosition - bone1.transform.position);
      if (!(b != Vector3.zero))
        return;
      if (weight >= 1.0)
        bendNormal = b;
      else
        bendNormal = Vector3.Lerp(bendNormal, b, weight);
    }

    public void SetBendPlaneToCurrent()
    {
      if (!initiated)
        return;
      Vector3 vector3 = Vector3.Cross(bone2.transform.position - bone1.transform.position, bone3.transform.position - bone2.transform.position);
      if (!(vector3 != Vector3.zero))
        return;
      bendNormal = vector3;
    }

    public void SetIKRotation(Quaternion rotation) => IKRotation = rotation;

    public void SetIKRotationWeight(float weight)
    {
      IKRotationWeight = Mathf.Clamp(weight, 0.0f, 1f);
    }

    public Quaternion GetIKRotation() => IKRotation;

    public float GetIKRotationWeight() => IKRotationWeight;

    public override Point[] GetPoints()
    {
      return new Point[3]
      {
        bone1,
        bone2,
        bone3
      };
    }

    public override Point GetPoint(Transform transform)
    {
      if (bone1.transform == transform)
        return bone1;
      if (bone2.transform == transform)
        return bone2;
      return bone3.transform == transform ? bone3 : (Point) null;
    }

    public override void StoreDefaultLocalState()
    {
      bone1.StoreDefaultLocalState();
      bone2.StoreDefaultLocalState();
      bone3.StoreDefaultLocalState();
    }

    public override void FixTransforms()
    {
      if (!initiated)
        return;
      bone1.FixTransform();
      bone2.FixTransform();
      bone3.FixTransform();
    }

    public override bool IsValid(ref string message)
    {
      if (bone1.transform == null || bone2.transform == null || bone3.transform == null)
      {
        message = "Please assign all Bones to the IK solver.";
        return false;
      }
      Transform transform = (Transform) Hierarchy.ContainsDuplicate(new Transform[3]
      {
        bone1.transform,
        bone2.transform,
        bone3.transform
      });
      if (transform != null)
      {
        message = transform.name + " is represented multiple times in the Bones.";
        return false;
      }
      if (bone1.transform.position == bone2.transform.position)
      {
        message = "first bone position is the same as second bone position.";
        return false;
      }
      if (!(bone2.transform.position == bone3.transform.position))
        return true;
      message = "second bone position is the same as third bone position.";
      return false;
    }

    public bool SetChain(Transform bone1, Transform bone2, Transform bone3, Transform root)
    {
      this.bone1.transform = bone1;
      this.bone2.transform = bone2;
      this.bone3.transform = bone3;
      Initiate(root);
      return initiated;
    }

    public static void Solve(
      Transform bone1,
      Transform bone2,
      Transform bone3,
      Vector3 targetPosition,
      Vector3 bendNormal,
      float weight)
    {
      if (weight <= 0.0)
        return;
      targetPosition = Vector3.Lerp(bone3.position, targetPosition, weight);
      Vector3 vector3_1 = targetPosition - bone1.position;
      float magnitude = vector3_1.magnitude;
      if (magnitude == 0.0)
        return;
      Vector3 vector3_2 = bone2.position - bone1.position;
      float sqrMagnitude1 = vector3_2.sqrMagnitude;
      vector3_2 = bone3.position - bone2.position;
      float sqrMagnitude2 = vector3_2.sqrMagnitude;
      Vector3 bendDirection = Vector3.Cross(vector3_1, bendNormal);
      Vector3 directionToBendPoint = GetDirectionToBendPoint(vector3_1, magnitude, bendDirection, sqrMagnitude1, sqrMagnitude2);
      Quaternion b1 = Quaternion.FromToRotation(bone2.position - bone1.position, directionToBendPoint);
      if (weight < 1.0)
        b1 = Quaternion.Lerp(Quaternion.identity, b1, weight);
      bone1.rotation = b1 * bone1.rotation;
      Quaternion b2 = Quaternion.FromToRotation(bone3.position - bone2.position, targetPosition - bone2.position);
      if (weight < 1.0)
        b2 = Quaternion.Lerp(Quaternion.identity, b2, weight);
      bone2.rotation = b2 * bone2.rotation;
    }

    private static Vector3 GetDirectionToBendPoint(
      Vector3 direction,
      float directionMag,
      Vector3 bendDirection,
      float sqrMag1,
      float sqrMag2)
    {
      float z = (float) ((directionMag * (double) directionMag + (sqrMag1 - (double) sqrMag2)) / 2.0) / directionMag;
      float y = (float) Math.Sqrt(Mathf.Clamp(sqrMag1 - z * z, 0.0f, float.PositiveInfinity));
      return direction == Vector3.zero ? Vector3.zero : Quaternion.LookRotation(direction, bendDirection) * new Vector3(0.0f, y, z);
    }

    protected override void OnInitiate()
    {
      if (bendNormal == Vector3.zero)
        bendNormal = Vector3.right;
      OnInitiateVirtual();
      IKPosition = bone3.transform.position;
      IKRotation = bone3.transform.rotation;
      InitiateBones();
      directHierarchy = IsDirectHierarchy();
    }

    private bool IsDirectHierarchy()
    {
      return !(bone3.transform.parent != bone2.transform) && !(bone2.transform.parent != bone1.transform);
    }

    private void InitiateBones()
    {
      bone1.Initiate(bone2.transform.position, bendNormal);
      bone2.Initiate(bone3.transform.position, bendNormal);
      SetBendPlaneToCurrent();
    }

    protected override void OnUpdate()
    {
      IKPositionWeight = Mathf.Clamp(IKPositionWeight, 0.0f, 1f);
      IKRotationWeight = Mathf.Clamp(IKRotationWeight, 0.0f, 1f);
      if (target != null)
      {
        IKPosition = target.position;
        IKRotation = target.rotation;
      }
      OnUpdateVirtual();
      if (IKPositionWeight > 0.0)
      {
        if (!directHierarchy)
        {
          bone1.Initiate(bone2.transform.position, this.bendNormal);
          bone2.Initiate(bone3.transform.position, this.bendNormal);
        }
        bone1.sqrMag = (bone2.transform.position - bone1.transform.position).sqrMagnitude;
        bone2.sqrMag = (bone3.transform.position - bone2.transform.position).sqrMagnitude;
        if (this.bendNormal == Vector3.zero && !Warning.logged)
          LogWarning("IKSolverTrigonometric Bend Normal is Vector3.zero.");
        weightIKPosition = Vector3.Lerp(bone3.transform.position, IKPosition, IKPositionWeight);
        Vector3 bendNormal = Vector3.Lerp(bone1.GetBendNormalFromCurrentRotation(), this.bendNormal, IKPositionWeight);
        Vector3 direction = Vector3.Lerp(bone2.transform.position - bone1.transform.position, GetBendDirection(weightIKPosition, bendNormal), IKPositionWeight);
        if (direction == Vector3.zero)
          direction = bone2.transform.position - bone1.transform.position;
        bone1.transform.rotation = bone1.GetRotation(direction, bendNormal);
        bone2.transform.rotation = bone2.GetRotation(weightIKPosition - bone2.transform.position, bone2.GetBendNormalFromCurrentRotation());
      }
      if (IKRotationWeight > 0.0)
        bone3.transform.rotation = Quaternion.Slerp(bone3.transform.rotation, IKRotation, IKRotationWeight);
      OnPostSolveVirtual();
    }

    protected virtual void OnInitiateVirtual()
    {
    }

    protected virtual void OnUpdateVirtual()
    {
    }

    protected virtual void OnPostSolveVirtual()
    {
    }

    protected Vector3 GetBendDirection(Vector3 IKPosition, Vector3 bendNormal)
    {
      Vector3 vector3 = IKPosition - bone1.transform.position;
      if (vector3 == Vector3.zero)
        return Vector3.zero;
      float sqrMagnitude = vector3.sqrMagnitude;
      float num = (float) Math.Sqrt(sqrMagnitude);
      float z = (float) ((sqrMagnitude + (double) bone1.sqrMag - bone2.sqrMag) / 2.0) / num;
      float y = (float) Math.Sqrt(Mathf.Clamp(bone1.sqrMag - z * z, 0.0f, float.PositiveInfinity));
      Vector3 upwards = Vector3.Cross(vector3, bendNormal);
      return Quaternion.LookRotation(vector3, upwards) * new Vector3(0.0f, y, z);
    }

    [Serializable]
    public class TrigonometricBone : Bone
    {
      private Quaternion targetToLocalSpace;
      private Vector3 defaultLocalBendNormal;

      public void Initiate(Vector3 childPosition, Vector3 bendNormal)
      {
        targetToLocalSpace = QuaTools.RotationToLocalSpace(transform.rotation, Quaternion.LookRotation(childPosition - transform.position, bendNormal));
        defaultLocalBendNormal = Quaternion.Inverse(transform.rotation) * bendNormal;
      }

      public Quaternion GetRotation(Vector3 direction, Vector3 bendNormal)
      {
        return Quaternion.LookRotation(direction, bendNormal) * targetToLocalSpace;
      }

      public Vector3 GetBendNormalFromCurrentRotation()
      {
        return transform.rotation * defaultLocalBendNormal;
      }
    }
  }
}
