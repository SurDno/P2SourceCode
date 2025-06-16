// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IKSolverLookAt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverLookAt : IKSolver
  {
    public Transform target;
    public IKSolverLookAt.LookAtBone[] spine = new IKSolverLookAt.LookAtBone[0];
    public IKSolverLookAt.LookAtBone head = new IKSolverLookAt.LookAtBone();
    public IKSolverLookAt.LookAtBone[] eyes = new IKSolverLookAt.LookAtBone[0];
    [Range(0.0f, 1f)]
    public float bodyWeight = 0.5f;
    [Range(0.0f, 1f)]
    public float headWeight = 0.5f;
    [Range(0.0f, 1f)]
    public float eyesWeight = 1f;
    [Range(0.0f, 1f)]
    public float clampWeight = 0.5f;
    [Range(0.0f, 1f)]
    public float clampWeightHead = 0.5f;
    [Range(0.0f, 1f)]
    public float clampWeightEyes = 0.5f;
    [Range(0.0f, 2f)]
    public int clampSmoothing = 2;
    public AnimationCurve spineWeightCurve = new AnimationCurve(new Keyframe[2]
    {
      new Keyframe(0.0f, 0.3f),
      new Keyframe(1f, 1f)
    });
    public Vector3 spineTargetOffset;
    private Vector3[] spineForwards = new Vector3[0];
    private Vector3[] headForwards = new Vector3[1];
    private Vector3[] eyeForward = new Vector3[1];

    public void SetLookAtWeight(float weight)
    {
      this.IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
    }

    public void SetLookAtWeight(float weight, float bodyWeight)
    {
      this.IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
      this.bodyWeight = Mathf.Clamp(bodyWeight, 0.0f, 1f);
    }

    public void SetLookAtWeight(float weight, float bodyWeight, float headWeight)
    {
      this.IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
      this.bodyWeight = Mathf.Clamp(bodyWeight, 0.0f, 1f);
      this.headWeight = Mathf.Clamp(headWeight, 0.0f, 1f);
    }

    public void SetLookAtWeight(
      float weight,
      float bodyWeight,
      float headWeight,
      float eyesWeight)
    {
      this.IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
      this.bodyWeight = Mathf.Clamp(bodyWeight, 0.0f, 1f);
      this.headWeight = Mathf.Clamp(headWeight, 0.0f, 1f);
      this.eyesWeight = Mathf.Clamp(eyesWeight, 0.0f, 1f);
    }

    public void SetLookAtWeight(
      float weight,
      float bodyWeight,
      float headWeight,
      float eyesWeight,
      float clampWeight)
    {
      this.IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
      this.bodyWeight = Mathf.Clamp(bodyWeight, 0.0f, 1f);
      this.headWeight = Mathf.Clamp(headWeight, 0.0f, 1f);
      this.eyesWeight = Mathf.Clamp(eyesWeight, 0.0f, 1f);
      this.clampWeight = Mathf.Clamp(clampWeight, 0.0f, 1f);
      this.clampWeightHead = this.clampWeight;
      this.clampWeightEyes = this.clampWeight;
    }

    public void SetLookAtWeight(
      float weight,
      float bodyWeight = 0.0f,
      float headWeight = 1f,
      float eyesWeight = 0.5f,
      float clampWeight = 0.5f,
      float clampWeightHead = 0.5f,
      float clampWeightEyes = 0.3f)
    {
      this.IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
      this.bodyWeight = Mathf.Clamp(bodyWeight, 0.0f, 1f);
      this.headWeight = Mathf.Clamp(headWeight, 0.0f, 1f);
      this.eyesWeight = Mathf.Clamp(eyesWeight, 0.0f, 1f);
      this.clampWeight = Mathf.Clamp(clampWeight, 0.0f, 1f);
      this.clampWeightHead = Mathf.Clamp(clampWeightHead, 0.0f, 1f);
      this.clampWeightEyes = Mathf.Clamp(clampWeightEyes, 0.0f, 1f);
    }

    public override void StoreDefaultLocalState()
    {
      for (int index = 0; index < this.spine.Length; ++index)
        this.spine[index].StoreDefaultLocalState();
      for (int index = 0; index < this.eyes.Length; ++index)
        this.eyes[index].StoreDefaultLocalState();
      if (this.head == null || !((UnityEngine.Object) this.head.transform != (UnityEngine.Object) null))
        return;
      this.head.StoreDefaultLocalState();
    }

    public override void FixTransforms()
    {
      if (!this.initiated || (double) this.IKPositionWeight <= 0.0)
        return;
      for (int index = 0; index < this.spine.Length; ++index)
        this.spine[index].FixTransform();
      for (int index = 0; index < this.eyes.Length; ++index)
        this.eyes[index].FixTransform();
      if (this.head == null || !((UnityEngine.Object) this.head.transform != (UnityEngine.Object) null))
        return;
      this.head.FixTransform();
    }

    public override bool IsValid(ref string message)
    {
      if (!this.spineIsValid)
      {
        message = "IKSolverLookAt spine setup is invalid. Can't initiate solver.";
        return false;
      }
      if (!this.headIsValid)
      {
        message = "IKSolverLookAt head transform is null. Can't initiate solver.";
        return false;
      }
      if (!this.eyesIsValid)
      {
        message = "IKSolverLookAt eyes setup is invalid. Can't initiate solver.";
        return false;
      }
      if (this.spineIsEmpty && this.headIsEmpty && this.eyesIsEmpty)
      {
        message = "IKSolverLookAt eyes setup is invalid. Can't initiate solver.";
        return false;
      }
      Transform transform1 = IKSolver.ContainsDuplicateBone((IKSolver.Bone[]) this.spine);
      if ((UnityEngine.Object) transform1 != (UnityEngine.Object) null)
      {
        message = transform1.name + " is represented multiple times in a single IK chain. Can't initiate solver.";
        return false;
      }
      Transform transform2 = IKSolver.ContainsDuplicateBone((IKSolver.Bone[]) this.eyes);
      if (!((UnityEngine.Object) transform2 != (UnityEngine.Object) null))
        return true;
      message = transform2.name + " is represented multiple times in a single IK chain. Can't initiate solver.";
      return false;
    }

    public override IKSolver.Point[] GetPoints()
    {
      IKSolver.Point[] points = new IKSolver.Point[this.spine.Length + this.eyes.Length + ((UnityEngine.Object) this.head.transform != (UnityEngine.Object) null ? 1 : 0)];
      for (int index = 0; index < this.spine.Length; ++index)
        points[index] = (IKSolver.Point) this.spine[index];
      int index1 = 0;
      for (int length = this.spine.Length; length < points.Length; ++length)
      {
        points[length] = (IKSolver.Point) this.eyes[index1];
        ++index1;
      }
      if ((UnityEngine.Object) this.head.transform != (UnityEngine.Object) null)
        points[points.Length - 1] = (IKSolver.Point) this.head;
      return points;
    }

    public override IKSolver.Point GetPoint(Transform transform)
    {
      foreach (IKSolverLookAt.LookAtBone point in this.spine)
      {
        if ((UnityEngine.Object) point.transform == (UnityEngine.Object) transform)
          return (IKSolver.Point) point;
      }
      foreach (IKSolverLookAt.LookAtBone eye in this.eyes)
      {
        if ((UnityEngine.Object) eye.transform == (UnityEngine.Object) transform)
          return (IKSolver.Point) eye;
      }
      return (UnityEngine.Object) this.head.transform == (UnityEngine.Object) transform ? (IKSolver.Point) this.head : (IKSolver.Point) null;
    }

    public bool SetChain(Transform[] spine, Transform head, Transform[] eyes, Transform root)
    {
      this.SetBones(spine, ref this.spine);
      this.head = new IKSolverLookAt.LookAtBone(head);
      this.SetBones(eyes, ref this.eyes);
      this.Initiate(root);
      return this.initiated;
    }

    protected override void OnInitiate()
    {
      if (this.firstInitiation || !Application.isPlaying)
      {
        if (this.spine.Length != 0)
          this.IKPosition = this.spine[this.spine.Length - 1].transform.position + this.root.forward * 3f;
        else if ((UnityEngine.Object) this.head.transform != (UnityEngine.Object) null)
          this.IKPosition = this.head.transform.position + this.root.forward * 3f;
        else if (this.eyes.Length != 0 && (UnityEngine.Object) this.eyes[0].transform != (UnityEngine.Object) null)
          this.IKPosition = this.eyes[0].transform.position + this.root.forward * 3f;
      }
      foreach (IKSolverLookAt.LookAtBone lookAtBone in this.spine)
        lookAtBone.Initiate(this.root);
      if (this.head != null)
        this.head.Initiate(this.root);
      foreach (IKSolverLookAt.LookAtBone eye in this.eyes)
        eye.Initiate(this.root);
      if (this.spineForwards == null || this.spineForwards.Length != this.spine.Length)
        this.spineForwards = new Vector3[this.spine.Length];
      if (this.headForwards == null)
        this.headForwards = new Vector3[1];
      if (this.eyeForward != null)
        return;
      this.eyeForward = new Vector3[1];
    }

    protected override void OnUpdate()
    {
      if ((double) this.IKPositionWeight <= 0.0)
        return;
      this.IKPositionWeight = Mathf.Clamp(this.IKPositionWeight, 0.0f, 1f);
      if ((UnityEngine.Object) this.target != (UnityEngine.Object) null)
        this.IKPosition = this.target.position;
      this.SolveSpine();
      this.SolveHead();
      this.SolveEyes();
    }

    private bool spineIsValid
    {
      get
      {
        if (this.spine == null)
          return false;
        if (this.spine.Length == 0)
          return true;
        for (int index = 0; index < this.spine.Length; ++index)
        {
          if (this.spine[index] == null || (UnityEngine.Object) this.spine[index].transform == (UnityEngine.Object) null)
            return false;
        }
        return true;
      }
    }

    private bool spineIsEmpty => this.spine.Length == 0;

    private void SolveSpine()
    {
      if ((double) this.bodyWeight <= 0.0 || this.spineIsEmpty)
        return;
      this.GetForwards(ref this.spineForwards, this.spine[0].forward, (this.IKPosition + this.spineTargetOffset - this.spine[this.spine.Length - 1].transform.position).normalized, this.spine.Length, this.clampWeight);
      for (int index = 0; index < this.spine.Length; ++index)
        this.spine[index].LookAt(this.spineForwards[index], this.bodyWeight * this.IKPositionWeight);
    }

    private bool headIsValid => this.head != null;

    private bool headIsEmpty => (UnityEngine.Object) this.head.transform == (UnityEngine.Object) null;

    private void SolveHead()
    {
      if ((double) this.headWeight <= 0.0 || this.headIsEmpty)
        return;
      Vector3 baseForward = this.spine.Length == 0 || !((UnityEngine.Object) this.spine[this.spine.Length - 1].transform != (UnityEngine.Object) null) ? this.head.forward : this.spine[this.spine.Length - 1].forward;
      Vector3 a = baseForward;
      Vector3 vector3 = this.IKPosition - this.head.transform.position;
      Vector3 normalized1 = vector3.normalized;
      double t = (double) this.headWeight * (double) this.IKPositionWeight;
      vector3 = Vector3.Lerp(a, normalized1, (float) t);
      Vector3 normalized2 = vector3.normalized;
      this.GetForwards(ref this.headForwards, baseForward, normalized2, 1, this.clampWeightHead);
      this.head.LookAt(this.headForwards[0], this.headWeight * this.IKPositionWeight);
    }

    private bool eyesIsValid
    {
      get
      {
        if (this.eyes == null)
          return false;
        if (this.eyes.Length == 0)
          return true;
        for (int index = 0; index < this.eyes.Length; ++index)
        {
          if (this.eyes[index] == null || (UnityEngine.Object) this.eyes[index].transform == (UnityEngine.Object) null)
            return false;
        }
        return true;
      }
    }

    private bool eyesIsEmpty => this.eyes.Length == 0;

    private void SolveEyes()
    {
      if ((double) this.eyesWeight <= 0.0 || this.eyesIsEmpty)
        return;
      for (int index = 0; index < this.eyes.Length; ++index)
      {
        this.GetForwards(ref this.eyeForward, (UnityEngine.Object) this.head.transform != (UnityEngine.Object) null ? this.head.forward : this.eyes[index].forward, (this.IKPosition - this.eyes[index].transform.position).normalized, 1, this.clampWeightEyes);
        this.eyes[index].LookAt(this.eyeForward[0], this.eyesWeight * this.IKPositionWeight);
      }
    }

    private Vector3[] GetForwards(
      ref Vector3[] forwards,
      Vector3 baseForward,
      Vector3 targetForward,
      int bones,
      float clamp)
    {
      if ((double) clamp >= 1.0 || (double) this.IKPositionWeight <= 0.0)
      {
        for (int index = 0; index < forwards.Length; ++index)
          forwards[index] = baseForward;
        return forwards;
      }
      float num1 = (float) (1.0 - (double) Vector3.Angle(baseForward, targetForward) / 180.0);
      float num2 = (double) clamp > 0.0 ? Mathf.Clamp((float) (1.0 - ((double) clamp - (double) num1) / (1.0 - (double) num1)), 0.0f, 1f) : 1f;
      float num3 = (double) clamp > 0.0 ? Mathf.Clamp(num1 / clamp, 0.0f, 1f) : 1f;
      for (int index = 0; index < this.clampSmoothing; ++index)
        num3 = Mathf.Sin((float) ((double) num3 * 3.1415927410125732 * 0.5));
      if (forwards.Length == 1)
      {
        forwards[0] = Vector3.Slerp(baseForward, targetForward, num3 * num2);
      }
      else
      {
        float num4 = 1f / (float) (forwards.Length - 1);
        for (int index = 0; index < forwards.Length; ++index)
          forwards[index] = Vector3.Slerp(baseForward, targetForward, this.spineWeightCurve.Evaluate(num4 * (float) index) * num3 * num2);
      }
      return forwards;
    }

    private void SetBones(Transform[] array, ref IKSolverLookAt.LookAtBone[] bones)
    {
      if (array == null)
      {
        bones = new IKSolverLookAt.LookAtBone[0];
      }
      else
      {
        if (bones.Length != array.Length)
          bones = new IKSolverLookAt.LookAtBone[array.Length];
        for (int index = 0; index < array.Length; ++index)
        {
          if (bones[index] == null)
            bones[index] = new IKSolverLookAt.LookAtBone(array[index]);
          else
            bones[index].transform = array[index];
        }
      }
    }

    [Serializable]
    public class LookAtBone : IKSolver.Bone
    {
      public LookAtBone()
      {
      }

      public LookAtBone(Transform transform) => this.transform = transform;

      public void Initiate(Transform root)
      {
        if ((UnityEngine.Object) this.transform == (UnityEngine.Object) null)
          return;
        this.axis = Quaternion.Inverse(this.transform.rotation) * root.forward;
      }

      public void LookAt(Vector3 direction, float weight)
      {
        Quaternion rotation1 = Quaternion.FromToRotation(this.forward, direction);
        Quaternion rotation2 = this.transform.rotation;
        this.transform.rotation = Quaternion.Lerp(rotation2, rotation1 * rotation2, weight);
      }

      public Vector3 forward => this.transform.rotation * this.axis;
    }
  }
}
