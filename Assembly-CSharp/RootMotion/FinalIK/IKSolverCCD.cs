using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverCCD : IKSolverHeuristic
  {
    public IKSolver.IterationDelegate OnPreIteration;

    public void FadeOutBoneWeights()
    {
      if (this.bones.Length < 2)
        return;
      this.bones[0].weight = 1f;
      float num = 1f / (float) (this.bones.Length - 1);
      for (int index = 1; index < this.bones.Length; ++index)
        this.bones[index].weight = num * (float) (this.bones.Length - 1 - index);
    }

    protected override void OnInitiate()
    {
      if (this.firstInitiation || !Application.isPlaying)
        this.IKPosition = this.bones[this.bones.Length - 1].transform.position;
      this.InitiateBones();
    }

    protected override void OnUpdate()
    {
      if ((double) this.IKPositionWeight <= 0.0)
        return;
      this.IKPositionWeight = Mathf.Clamp(this.IKPositionWeight, 0.0f, 1f);
      if ((UnityEngine.Object) this.target != (UnityEngine.Object) null)
        this.IKPosition = this.target.position;
      if (this.XY)
        this.IKPosition.z = this.bones[0].transform.position.z;
      Vector3 vector3 = this.maxIterations > 1 ? this.GetSingularityOffset() : Vector3.zero;
      for (int i = 0; i < this.maxIterations && (!(vector3 == Vector3.zero) || i < 1 || (double) this.tolerance <= 0.0 || (double) this.positionOffset >= (double) this.tolerance * (double) this.tolerance); ++i)
      {
        this.lastLocalDirection = this.localDirection;
        if (this.OnPreIteration != null)
          this.OnPreIteration(i);
        this.Solve(this.IKPosition + (i == 0 ? vector3 : Vector3.zero));
      }
      this.lastLocalDirection = this.localDirection;
    }

    private void Solve(Vector3 targetPosition)
    {
      if (this.XY)
      {
        for (int index = this.bones.Length - 2; index > -1; --index)
        {
          float num = this.bones[index].weight * this.IKPositionWeight;
          if ((double) num > 0.0)
          {
            Vector3 vector3_1 = this.bones[this.bones.Length - 1].transform.position - this.bones[index].transform.position;
            Vector3 vector3_2 = targetPosition - this.bones[index].transform.position;
            float current = Mathf.Atan2(vector3_1.x, vector3_1.y) * 57.29578f;
            float target = Mathf.Atan2(vector3_2.x, vector3_2.y) * 57.29578f;
            this.bones[index].transform.rotation = Quaternion.AngleAxis(Mathf.DeltaAngle(current, target) * num, Vector3.back) * this.bones[index].transform.rotation;
          }
          if (this.useRotationLimits && (UnityEngine.Object) this.bones[index].rotationLimit != (UnityEngine.Object) null)
            this.bones[index].rotationLimit.Apply();
        }
      }
      else
      {
        for (int index = this.bones.Length - 2; index > -1; --index)
        {
          float t = this.bones[index].weight * this.IKPositionWeight;
          if ((double) t > 0.0)
          {
            Quaternion b = Quaternion.FromToRotation(this.bones[this.bones.Length - 1].transform.position - this.bones[index].transform.position, targetPosition - this.bones[index].transform.position) * this.bones[index].transform.rotation;
            if ((double) t >= 1.0)
              this.bones[index].transform.rotation = b;
            else
              this.bones[index].transform.rotation = Quaternion.Lerp(this.bones[index].transform.rotation, b, t);
          }
          if (this.useRotationLimits && (UnityEngine.Object) this.bones[index].rotationLimit != (UnityEngine.Object) null)
            this.bones[index].rotationLimit.Apply();
        }
      }
    }
  }
}
