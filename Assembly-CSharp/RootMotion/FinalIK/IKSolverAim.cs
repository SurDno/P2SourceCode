using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverAim : IKSolverHeuristic
  {
    public Transform transform;
    public Vector3 axis = Vector3.forward;
    public Vector3 poleAxis = Vector3.up;
    public Vector3 polePosition;
    [Range(0.0f, 1f)]
    public float poleWeight;
    public Transform poleTarget;
    [Range(0.0f, 1f)]
    public float clampWeight = 0.1f;
    [Range(0.0f, 2f)]
    public int clampSmoothing = 2;
    public IKSolver.IterationDelegate OnPreIteration;
    private float step;
    private Vector3 clampedIKPosition;
    private RotationLimit transformLimit;
    private Transform lastTransform;

    public float GetAngle()
    {
      return Vector3.Angle(this.transformAxis, this.IKPosition - this.transform.position);
    }

    public Vector3 transformAxis => this.transform.rotation * this.axis;

    public Vector3 transformPoleAxis => this.transform.rotation * this.poleAxis;

    protected override void OnInitiate()
    {
      if ((this.firstInitiation || !Application.isPlaying) && (UnityEngine.Object) this.transform != (UnityEngine.Object) null)
      {
        this.IKPosition = this.transform.position + this.transformAxis * 3f;
        this.polePosition = this.transform.position + this.transformPoleAxis * 3f;
      }
      for (int index = 0; index < this.bones.Length; ++index)
      {
        if ((UnityEngine.Object) this.bones[index].rotationLimit != (UnityEngine.Object) null)
          this.bones[index].rotationLimit.Disable();
      }
      this.step = 1f / (float) this.bones.Length;
      if (!Application.isPlaying)
        return;
      this.axis = this.axis.normalized;
    }

    protected override void OnUpdate()
    {
      if (this.axis == Vector3.zero)
      {
        if (Warning.logged)
          return;
        this.LogWarning("IKSolverAim axis is Vector3.zero.");
      }
      else if (this.poleAxis == Vector3.zero && (double) this.poleWeight > 0.0)
      {
        if (Warning.logged)
          return;
        this.LogWarning("IKSolverAim poleAxis is Vector3.zero.");
      }
      else
      {
        if ((UnityEngine.Object) this.target != (UnityEngine.Object) null)
          this.IKPosition = this.target.position;
        if ((UnityEngine.Object) this.poleTarget != (UnityEngine.Object) null)
          this.polePosition = this.poleTarget.position;
        if (this.XY)
          this.IKPosition.z = this.bones[0].transform.position.z;
        if ((double) this.IKPositionWeight <= 0.0)
          return;
        this.IKPositionWeight = Mathf.Clamp(this.IKPositionWeight, 0.0f, 1f);
        if ((UnityEngine.Object) this.transform != (UnityEngine.Object) this.lastTransform)
        {
          this.transformLimit = this.transform.GetComponent<RotationLimit>();
          if ((UnityEngine.Object) this.transformLimit != (UnityEngine.Object) null)
            this.transformLimit.enabled = false;
          this.lastTransform = this.transform;
        }
        if ((UnityEngine.Object) this.transformLimit != (UnityEngine.Object) null)
          this.transformLimit.Apply();
        if ((UnityEngine.Object) this.transform == (UnityEngine.Object) null)
        {
          if (Warning.logged)
            return;
          this.LogWarning("Aim Transform unassigned in Aim IK solver. Please Assign a Transform (lineal descendant to the last bone in the spine) that you want to be aimed at IKPosition");
        }
        else
        {
          this.clampWeight = Mathf.Clamp(this.clampWeight, 0.0f, 1f);
          this.clampedIKPosition = this.GetClampedIKPosition();
          Vector3 b = this.clampedIKPosition - this.transform.position;
          this.clampedIKPosition = this.transform.position + Vector3.Slerp(this.transformAxis * b.magnitude, b, this.IKPositionWeight);
          for (int i = 0; i < this.maxIterations && (i < 1 || (double) this.tolerance <= 0.0 || (double) this.GetAngle() >= (double) this.tolerance); ++i)
          {
            this.lastLocalDirection = this.localDirection;
            if (this.OnPreIteration != null)
              this.OnPreIteration(i);
            this.Solve();
          }
          this.lastLocalDirection = this.localDirection;
        }
      }
    }

    protected override int minBones => 1;

    private void Solve()
    {
      for (int index = 0; index < this.bones.Length - 1; ++index)
        this.RotateToTarget(this.clampedIKPosition, this.bones[index], this.step * (float) (index + 1) * this.IKPositionWeight * this.bones[index].weight);
      this.RotateToTarget(this.clampedIKPosition, this.bones[this.bones.Length - 1], this.IKPositionWeight * this.bones[this.bones.Length - 1].weight);
    }

    private Vector3 GetClampedIKPosition()
    {
      if ((double) this.clampWeight <= 0.0)
        return this.IKPosition;
      if ((double) this.clampWeight >= 1.0)
        return this.transform.position + this.transformAxis * (this.IKPosition - this.transform.position).magnitude;
      float num1 = (float) (1.0 - (double) Vector3.Angle(this.transformAxis, this.IKPosition - this.transform.position) / 180.0);
      float num2 = (double) this.clampWeight > 0.0 ? Mathf.Clamp((float) (1.0 - ((double) this.clampWeight - (double) num1) / (1.0 - (double) num1)), 0.0f, 1f) : 1f;
      float num3 = (double) this.clampWeight > 0.0 ? Mathf.Clamp(num1 / this.clampWeight, 0.0f, 1f) : 1f;
      for (int index = 0; index < this.clampSmoothing; ++index)
        num3 = Mathf.Sin((float) ((double) num3 * 3.1415927410125732 * 0.5));
      return this.transform.position + Vector3.Slerp(this.transformAxis * 10f, this.IKPosition - this.transform.position, num3 * num2);
    }

    private void RotateToTarget(Vector3 targetPosition, IKSolver.Bone bone, float weight)
    {
      if (this.XY)
      {
        if ((double) weight >= 0.0)
        {
          Vector3 transformAxis = this.transformAxis;
          Vector3 vector3 = targetPosition - this.transform.position;
          float current = Mathf.Atan2(transformAxis.x, transformAxis.y) * 57.29578f;
          float target = Mathf.Atan2(vector3.x, vector3.y) * 57.29578f;
          bone.transform.rotation = Quaternion.AngleAxis(Mathf.DeltaAngle(current, target), Vector3.back) * bone.transform.rotation;
        }
      }
      else
      {
        if ((double) weight >= 0.0)
        {
          Quaternion rotation = Quaternion.FromToRotation(this.transformAxis, targetPosition - this.transform.position);
          if ((double) weight >= 1.0)
            bone.transform.rotation = rotation * bone.transform.rotation;
          else
            bone.transform.rotation = Quaternion.Lerp(Quaternion.identity, rotation, weight) * bone.transform.rotation;
        }
        if ((double) this.poleWeight > 0.0)
        {
          Vector3 tangent = this.polePosition - this.transform.position;
          Vector3 transformAxis = this.transformAxis;
          Vector3.OrthoNormalize(ref transformAxis, ref tangent);
          Quaternion rotation = Quaternion.FromToRotation(this.transformPoleAxis, tangent);
          bone.transform.rotation = Quaternion.Lerp(Quaternion.identity, rotation, weight * this.poleWeight) * bone.transform.rotation;
        }
      }
      if (!this.useRotationLimits || !((UnityEngine.Object) bone.rotationLimit != (UnityEngine.Object) null))
        return;
      bone.rotationLimit.Apply();
    }

    protected override Vector3 localDirection
    {
      get
      {
        return this.bones[0].transform.InverseTransformDirection(this.bones[this.bones.Length - 1].transform.forward);
      }
    }
  }
}
