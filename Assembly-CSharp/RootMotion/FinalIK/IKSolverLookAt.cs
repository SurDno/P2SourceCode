using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverLookAt : IKSolver
  {
    public Transform target;
    public LookAtBone[] spine = new LookAtBone[0];
    public LookAtBone head = new LookAtBone();
    public LookAtBone[] eyes = new LookAtBone[0];
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
    public AnimationCurve spineWeightCurve = new AnimationCurve(new Keyframe(0.0f, 0.3f), new Keyframe(1f, 1f));
    public Vector3 spineTargetOffset;
    private Vector3[] spineForwards = new Vector3[0];
    private Vector3[] headForwards = new Vector3[1];
    private Vector3[] eyeForward = new Vector3[1];

    public void SetLookAtWeight(float weight)
    {
      IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
    }

    public void SetLookAtWeight(float weight, float bodyWeight)
    {
      IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
      this.bodyWeight = Mathf.Clamp(bodyWeight, 0.0f, 1f);
    }

    public void SetLookAtWeight(float weight, float bodyWeight, float headWeight)
    {
      IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
      this.bodyWeight = Mathf.Clamp(bodyWeight, 0.0f, 1f);
      this.headWeight = Mathf.Clamp(headWeight, 0.0f, 1f);
    }

    public void SetLookAtWeight(
      float weight,
      float bodyWeight,
      float headWeight,
      float eyesWeight)
    {
      IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
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
      IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
      this.bodyWeight = Mathf.Clamp(bodyWeight, 0.0f, 1f);
      this.headWeight = Mathf.Clamp(headWeight, 0.0f, 1f);
      this.eyesWeight = Mathf.Clamp(eyesWeight, 0.0f, 1f);
      this.clampWeight = Mathf.Clamp(clampWeight, 0.0f, 1f);
      clampWeightHead = this.clampWeight;
      clampWeightEyes = this.clampWeight;
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
      IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
      this.bodyWeight = Mathf.Clamp(bodyWeight, 0.0f, 1f);
      this.headWeight = Mathf.Clamp(headWeight, 0.0f, 1f);
      this.eyesWeight = Mathf.Clamp(eyesWeight, 0.0f, 1f);
      this.clampWeight = Mathf.Clamp(clampWeight, 0.0f, 1f);
      this.clampWeightHead = Mathf.Clamp(clampWeightHead, 0.0f, 1f);
      this.clampWeightEyes = Mathf.Clamp(clampWeightEyes, 0.0f, 1f);
    }

    public override void StoreDefaultLocalState()
    {
      for (int index = 0; index < spine.Length; ++index)
        spine[index].StoreDefaultLocalState();
      for (int index = 0; index < eyes.Length; ++index)
        eyes[index].StoreDefaultLocalState();
      if (head == null || !(head.transform != null))
        return;
      head.StoreDefaultLocalState();
    }

    public override void FixTransforms()
    {
      if (!initiated || IKPositionWeight <= 0.0)
        return;
      for (int index = 0; index < spine.Length; ++index)
        spine[index].FixTransform();
      for (int index = 0; index < eyes.Length; ++index)
        eyes[index].FixTransform();
      if (head == null || !(head.transform != null))
        return;
      head.FixTransform();
    }

    public override bool IsValid(ref string message)
    {
      if (!spineIsValid)
      {
        message = "IKSolverLookAt spine setup is invalid. Can't initiate solver.";
        return false;
      }
      if (!headIsValid)
      {
        message = "IKSolverLookAt head transform is null. Can't initiate solver.";
        return false;
      }
      if (!eyesIsValid)
      {
        message = "IKSolverLookAt eyes setup is invalid. Can't initiate solver.";
        return false;
      }
      if (spineIsEmpty && headIsEmpty && eyesIsEmpty)
      {
        message = "IKSolverLookAt eyes setup is invalid. Can't initiate solver.";
        return false;
      }
      Transform transform1 = ContainsDuplicateBone(spine);
      if (transform1 != null)
      {
        message = transform1.name + " is represented multiple times in a single IK chain. Can't initiate solver.";
        return false;
      }
      Transform transform2 = ContainsDuplicateBone(eyes);
      if (!(transform2 != null))
        return true;
      message = transform2.name + " is represented multiple times in a single IK chain. Can't initiate solver.";
      return false;
    }

    public override Point[] GetPoints()
    {
      Point[] points = new Point[spine.Length + eyes.Length + (head.transform != null ? 1 : 0)];
      for (int index = 0; index < spine.Length; ++index)
        points[index] = spine[index];
      int index1 = 0;
      for (int length = spine.Length; length < points.Length; ++length)
      {
        points[length] = eyes[index1];
        ++index1;
      }
      if (head.transform != null)
        points[points.Length - 1] = head;
      return points;
    }

    public override Point GetPoint(Transform transform)
    {
      foreach (LookAtBone point in spine)
      {
        if (point.transform == transform)
          return point;
      }
      foreach (LookAtBone eye in eyes)
      {
        if (eye.transform == transform)
          return eye;
      }
      return head.transform == transform ? head : (Point) null;
    }

    public bool SetChain(Transform[] spine, Transform head, Transform[] eyes, Transform root)
    {
      SetBones(spine, ref this.spine);
      this.head = new LookAtBone(head);
      SetBones(eyes, ref this.eyes);
      Initiate(root);
      return initiated;
    }

    protected override void OnInitiate()
    {
      if (firstInitiation || !Application.isPlaying)
      {
        if (spine.Length != 0)
          IKPosition = spine[spine.Length - 1].transform.position + root.forward * 3f;
        else if (head.transform != null)
          IKPosition = head.transform.position + root.forward * 3f;
        else if (eyes.Length != 0 && eyes[0].transform != null)
          IKPosition = eyes[0].transform.position + root.forward * 3f;
      }
      foreach (LookAtBone lookAtBone in spine)
        lookAtBone.Initiate(root);
      if (head != null)
        head.Initiate(root);
      foreach (LookAtBone eye in eyes)
        eye.Initiate(root);
      if (spineForwards == null || spineForwards.Length != spine.Length)
        spineForwards = new Vector3[spine.Length];
      if (headForwards == null)
        headForwards = new Vector3[1];
      if (eyeForward != null)
        return;
      eyeForward = new Vector3[1];
    }

    protected override void OnUpdate()
    {
      if (IKPositionWeight <= 0.0)
        return;
      IKPositionWeight = Mathf.Clamp(IKPositionWeight, 0.0f, 1f);
      if (target != null)
        IKPosition = target.position;
      SolveSpine();
      SolveHead();
      SolveEyes();
    }

    private bool spineIsValid
    {
      get
      {
        if (spine == null)
          return false;
        if (spine.Length == 0)
          return true;
        for (int index = 0; index < spine.Length; ++index)
        {
          if (spine[index] == null || spine[index].transform == null)
            return false;
        }
        return true;
      }
    }

    private bool spineIsEmpty => spine.Length == 0;

    private void SolveSpine()
    {
      if (bodyWeight <= 0.0 || spineIsEmpty)
        return;
      GetForwards(ref spineForwards, spine[0].forward, (IKPosition + spineTargetOffset - spine[spine.Length - 1].transform.position).normalized, spine.Length, clampWeight);
      for (int index = 0; index < spine.Length; ++index)
        spine[index].LookAt(spineForwards[index], bodyWeight * IKPositionWeight);
    }

    private bool headIsValid => head != null;

    private bool headIsEmpty => head.transform == null;

    private void SolveHead()
    {
      if (headWeight <= 0.0 || headIsEmpty)
        return;
      Vector3 baseForward = spine.Length == 0 || !(spine[spine.Length - 1].transform != null) ? head.forward : spine[spine.Length - 1].forward;
      Vector3 a = baseForward;
      Vector3 vector3 = IKPosition - head.transform.position;
      Vector3 normalized1 = vector3.normalized;
      double t = headWeight * (double) IKPositionWeight;
      vector3 = Vector3.Lerp(a, normalized1, (float) t);
      Vector3 normalized2 = vector3.normalized;
      GetForwards(ref headForwards, baseForward, normalized2, 1, clampWeightHead);
      head.LookAt(headForwards[0], headWeight * IKPositionWeight);
    }

    private bool eyesIsValid
    {
      get
      {
        if (eyes == null)
          return false;
        if (eyes.Length == 0)
          return true;
        for (int index = 0; index < eyes.Length; ++index)
        {
          if (eyes[index] == null || eyes[index].transform == null)
            return false;
        }
        return true;
      }
    }

    private bool eyesIsEmpty => eyes.Length == 0;

    private void SolveEyes()
    {
      if (eyesWeight <= 0.0 || eyesIsEmpty)
        return;
      for (int index = 0; index < eyes.Length; ++index)
      {
        GetForwards(ref eyeForward, head.transform != null ? head.forward : eyes[index].forward, (IKPosition - eyes[index].transform.position).normalized, 1, clampWeightEyes);
        eyes[index].LookAt(eyeForward[0], eyesWeight * IKPositionWeight);
      }
    }

    private Vector3[] GetForwards(
      ref Vector3[] forwards,
      Vector3 baseForward,
      Vector3 targetForward,
      int bones,
      float clamp)
    {
      if (clamp >= 1.0 || IKPositionWeight <= 0.0)
      {
        for (int index = 0; index < forwards.Length; ++index)
          forwards[index] = baseForward;
        return forwards;
      }
      float num1 = (float) (1.0 - Vector3.Angle(baseForward, targetForward) / 180.0);
      float num2 = clamp > 0.0 ? Mathf.Clamp((float) (1.0 - (clamp - (double) num1) / (1.0 - num1)), 0.0f, 1f) : 1f;
      float num3 = clamp > 0.0 ? Mathf.Clamp(num1 / clamp, 0.0f, 1f) : 1f;
      for (int index = 0; index < clampSmoothing; ++index)
        num3 = Mathf.Sin((float) (num3 * 3.1415927410125732 * 0.5));
      if (forwards.Length == 1)
      {
        forwards[0] = Vector3.Slerp(baseForward, targetForward, num3 * num2);
      }
      else
      {
        float num4 = 1f / (forwards.Length - 1);
        for (int index = 0; index < forwards.Length; ++index)
          forwards[index] = Vector3.Slerp(baseForward, targetForward, spineWeightCurve.Evaluate(num4 * index) * num3 * num2);
      }
      return forwards;
    }

    private void SetBones(Transform[] array, ref LookAtBone[] bones)
    {
      if (array == null)
      {
        bones = new LookAtBone[0];
      }
      else
      {
        if (bones.Length != array.Length)
          bones = new LookAtBone[array.Length];
        for (int index = 0; index < array.Length; ++index)
        {
          if (bones[index] == null)
            bones[index] = new LookAtBone(array[index]);
          else
            bones[index].transform = array[index];
        }
      }
    }

    [Serializable]
    public class LookAtBone : Bone
    {
      public LookAtBone()
      {
      }

      public LookAtBone(Transform transform) => this.transform = transform;

      public void Initiate(Transform root)
      {
        if (transform == null)
          return;
        axis = Quaternion.Inverse(transform.rotation) * root.forward;
      }

      public void LookAt(Vector3 direction, float weight)
      {
        Quaternion rotation1 = Quaternion.FromToRotation(forward, direction);
        Quaternion rotation2 = transform.rotation;
        transform.rotation = Quaternion.Lerp(rotation2, rotation1 * rotation2, weight);
      }

      public Vector3 forward => transform.rotation * axis;
    }
  }
}
