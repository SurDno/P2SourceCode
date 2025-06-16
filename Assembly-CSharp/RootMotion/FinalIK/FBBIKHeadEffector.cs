using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  public class FBBIKHeadEffector : MonoBehaviour
  {
    [Tooltip("Reference to the FBBIK component.")]
    public FullBodyBipedIK ik;
    [LargeHeader("Position")]
    [Tooltip("Master weight for positioning the head.")]
    [Range(0.0f, 1f)]
    public float positionWeight = 1f;
    [Tooltip("The weight of moving the body along with the head")]
    [Range(0.0f, 1f)]
    public float bodyWeight = 0.8f;
    [Tooltip("The weight of moving the thighs along with the head")]
    [Range(0.0f, 1f)]
    public float thighWeight = 0.8f;
    [Tooltip("If false, hands will not pull the head away if they are too far. Disabling this will improve performance significantly.")]
    public bool handsPullBody = true;
    [LargeHeader("Rotation")]
    [Tooltip("The weight of rotating the head bone after solving")]
    [Range(0.0f, 1f)]
    public float rotationWeight = 0.0f;
    [Tooltip("Clamping the rotation of the body")]
    [Range(0.0f, 1f)]
    public float bodyClampWeight = 0.5f;
    [Tooltip("Clamping the rotation of the head")]
    [Range(0.0f, 1f)]
    public float headClampWeight = 0.5f;
    [Tooltip("The master weight of bending/twisting the spine to the rotation of the head effector. This is similar to CCD, but uses the rotation of the head effector not the position.")]
    [Range(0.0f, 1f)]
    public float bendWeight = 1f;
    [Tooltip("The bones to use for bending.")]
    public FBBIKHeadEffector.BendBone[] bendBones = new FBBIKHeadEffector.BendBone[0];
    [LargeHeader("CCD")]
    [Tooltip("Optional. The master weight of the CCD (Cyclic Coordinate Descent) IK effect that bends the spine towards the head effector before FBBIK solves.")]
    [Range(0.0f, 1f)]
    public float CCDWeight = 1f;
    [Tooltip("The weight of rolling the bones in towards the target")]
    [Range(0.0f, 1f)]
    public float roll = 0.0f;
    [Tooltip("Smoothing the CCD effect.")]
    [Range(0.0f, 1000f)]
    public float damper = 500f;
    [Tooltip("Bones to use for the CCD pass. Assign spine and/or neck bones.")]
    public Transform[] CCDBones = new Transform[0];
    [LargeHeader("Stretching")]
    [Tooltip("Stretching the spine/neck to help reach the target. This is useful for making sure the head stays locked relative to the VR headset. NB! Stretching is done after FBBIK has solved so if you have the hand effectors pinned and spine bones included in the 'Stretch Bones', the hands might become offset from their target positions.")]
    [Range(0.0f, 1f)]
    public float postStretchWeight = 1f;
    [Tooltip("Stretch magnitude limit.")]
    public float maxStretch = 0.1f;
    [Tooltip("If > 0, dampers the stretching effect.")]
    public float stretchDamper = 0.0f;
    [Tooltip("If true, will fix head position to this Transform no matter what. Good for making sure the head will not budge away from the VR headset")]
    public bool fixHead;
    [Tooltip("Bones to use for stretching. The more bones you add, the less noticable the effect.")]
    public Transform[] stretchBones = new Transform[0];
    [LargeHeader("Chest Direction")]
    public Vector3 chestDirection = Vector3.forward;
    [Range(0.0f, 1f)]
    public float chestDirectionWeight = 1f;
    public Transform[] chestBones = new Transform[0];
    public IKSolver.UpdateDelegate OnPostHeadEffectorFK;
    private Vector3 offset;
    private Vector3 headToBody;
    private Vector3 shoulderCenterToHead;
    private Vector3 headToLeftThigh;
    private Vector3 headToRightThigh;
    private Vector3 leftShoulderPos;
    private Vector3 rightShoulderPos;
    private float shoulderDist;
    private float leftShoulderDist;
    private float rightShoulderDist;
    private Quaternion chestRotation;
    private Quaternion headRotationRelativeToRoot;
    private Quaternion[] ccdDefaultLocalRotations = new Quaternion[0];
    private Vector3 headLocalPosition;
    private Quaternion headLocalRotation;
    private Vector3[] stretchLocalPositions = new Vector3[0];
    private Quaternion[] stretchLocalRotations = new Quaternion[0];
    private Vector3[] chestLocalPositions = new Vector3[0];
    private Quaternion[] chestLocalRotations = new Quaternion[0];
    private int bendBonesCount;
    private int ccdBonesCount;
    private int stretchBonesCount;
    private int chestBonesCount;

    private void Start()
    {
      IKSolverFullBodyBiped solver1 = this.ik.solver;
      solver1.OnPreRead = solver1.OnPreRead + new IKSolver.UpdateDelegate(this.OnPreRead);
      IKSolverFullBodyBiped solver2 = this.ik.solver;
      solver2.OnPreIteration = solver2.OnPreIteration + new IKSolver.IterationDelegate(this.Iterate);
      IKSolverFullBodyBiped solver3 = this.ik.solver;
      solver3.OnPostUpdate = solver3.OnPostUpdate + new IKSolver.UpdateDelegate(this.OnPostUpdate);
      IKSolverFullBodyBiped solver4 = this.ik.solver;
      solver4.OnStoreDefaultLocalState = solver4.OnStoreDefaultLocalState + new IKSolver.UpdateDelegate(this.OnStoreDefaultLocalState);
      IKSolverFullBodyBiped solver5 = this.ik.solver;
      solver5.OnFixTransforms = solver5.OnFixTransforms + new IKSolver.UpdateDelegate(this.OnFixTransforms);
      this.OnStoreDefaultLocalState();
      this.headRotationRelativeToRoot = Quaternion.Inverse(this.ik.references.root.rotation) * this.ik.references.head.rotation;
    }

    private void OnStoreDefaultLocalState()
    {
      foreach (FBBIKHeadEffector.BendBone bendBone in this.bendBones)
        bendBone?.StoreDefaultLocalState();
      this.ccdDefaultLocalRotations = new Quaternion[this.CCDBones.Length];
      for (int index = 0; index < this.CCDBones.Length; ++index)
      {
        if ((UnityEngine.Object) this.CCDBones[index] != (UnityEngine.Object) null)
          this.ccdDefaultLocalRotations[index] = this.CCDBones[index].localRotation;
      }
      this.headLocalPosition = this.ik.references.head.localPosition;
      this.headLocalRotation = this.ik.references.head.localRotation;
      this.stretchLocalPositions = new Vector3[this.stretchBones.Length];
      this.stretchLocalRotations = new Quaternion[this.stretchBones.Length];
      for (int index = 0; index < this.stretchBones.Length; ++index)
      {
        if ((UnityEngine.Object) this.stretchBones[index] != (UnityEngine.Object) null)
        {
          this.stretchLocalPositions[index] = this.stretchBones[index].localPosition;
          this.stretchLocalRotations[index] = this.stretchBones[index].localRotation;
        }
      }
      this.chestLocalPositions = new Vector3[this.chestBones.Length];
      this.chestLocalRotations = new Quaternion[this.chestBones.Length];
      for (int index = 0; index < this.chestBones.Length; ++index)
      {
        if ((UnityEngine.Object) this.chestBones[index] != (UnityEngine.Object) null)
        {
          this.chestLocalPositions[index] = this.chestBones[index].localPosition;
          this.chestLocalRotations[index] = this.chestBones[index].localRotation;
        }
      }
      this.bendBonesCount = this.bendBones.Length;
      this.ccdBonesCount = this.CCDBones.Length;
      this.stretchBonesCount = this.stretchBones.Length;
      this.chestBonesCount = this.chestBones.Length;
    }

    private void OnFixTransforms()
    {
      if (!this.enabled)
        return;
      foreach (FBBIKHeadEffector.BendBone bendBone in this.bendBones)
        bendBone?.FixTransforms();
      for (int index = 0; index < this.CCDBones.Length; ++index)
      {
        if ((UnityEngine.Object) this.CCDBones[index] != (UnityEngine.Object) null)
          this.CCDBones[index].localRotation = this.ccdDefaultLocalRotations[index];
      }
      this.ik.references.head.localPosition = this.headLocalPosition;
      this.ik.references.head.localRotation = this.headLocalRotation;
      for (int index = 0; index < this.stretchBones.Length; ++index)
      {
        if ((UnityEngine.Object) this.stretchBones[index] != (UnityEngine.Object) null)
        {
          this.stretchBones[index].localPosition = this.stretchLocalPositions[index];
          this.stretchBones[index].localRotation = this.stretchLocalRotations[index];
        }
      }
      for (int index = 0; index < this.chestBones.Length; ++index)
      {
        if ((UnityEngine.Object) this.chestBones[index] != (UnityEngine.Object) null)
        {
          this.chestBones[index].localPosition = this.chestLocalPositions[index];
          this.chestBones[index].localRotation = this.chestLocalRotations[index];
        }
      }
    }

    private void OnPreRead()
    {
      if (!this.enabled || !this.gameObject.activeInHierarchy || this.ik.solver.iterations == 0)
        return;
      this.ik.solver.FABRIKPass = this.handsPullBody;
      if (this.bendBonesCount != this.bendBones.Length || this.ccdBonesCount != this.CCDBones.Length || this.stretchBonesCount != this.stretchBones.Length || this.chestBonesCount != this.chestBones.Length)
        this.OnStoreDefaultLocalState();
      this.ChestDirection();
      this.SpineBend();
      this.CCDPass();
      this.offset = this.transform.position - this.ik.references.head.position;
      this.shoulderDist = Vector3.Distance(this.ik.references.leftUpperArm.position, this.ik.references.rightUpperArm.position);
      this.leftShoulderDist = Vector3.Distance(this.ik.references.head.position, this.ik.references.leftUpperArm.position);
      this.rightShoulderDist = Vector3.Distance(this.ik.references.head.position, this.ik.references.rightUpperArm.position);
      this.headToBody = this.ik.solver.rootNode.position - this.ik.references.head.position;
      this.headToLeftThigh = this.ik.references.leftThigh.position - this.ik.references.head.position;
      this.headToRightThigh = this.ik.references.rightThigh.position - this.ik.references.head.position;
      this.leftShoulderPos = this.ik.references.leftUpperArm.position + this.offset * this.bodyWeight;
      this.rightShoulderPos = this.ik.references.rightUpperArm.position + this.offset * this.bodyWeight;
      this.chestRotation = Quaternion.LookRotation(this.ik.references.head.position - this.ik.references.leftUpperArm.position, this.ik.references.rightUpperArm.position - this.ik.references.leftUpperArm.position);
      if (this.OnPostHeadEffectorFK == null)
        return;
      this.OnPostHeadEffectorFK();
    }

    private void SpineBend()
    {
      float num1 = this.bendWeight * this.ik.solver.IKPositionWeight;
      if ((double) num1 <= 0.0 || this.bendBones.Length == 0)
        return;
      Quaternion b = QuaTools.ClampRotation(this.transform.rotation * Quaternion.Inverse(this.ik.references.root.rotation * this.headRotationRelativeToRoot), this.bodyClampWeight, 2);
      float num2 = 1f / (float) this.bendBones.Length;
      for (int index = 0; index < this.bendBones.Length; ++index)
      {
        if ((UnityEngine.Object) this.bendBones[index].transform != (UnityEngine.Object) null)
          this.bendBones[index].transform.rotation = Quaternion.Lerp(Quaternion.identity, b, num2 * this.bendBones[index].weight * num1) * this.bendBones[index].transform.rotation;
      }
    }

    private void CCDPass()
    {
      float num1 = this.CCDWeight * this.ik.solver.IKPositionWeight;
      if ((double) num1 <= 0.0)
        return;
      for (int index = this.CCDBones.Length - 1; index > -1; --index)
      {
        Quaternion quaternion = Quaternion.FromToRotation(this.ik.references.head.position - this.CCDBones[index].position, this.transform.position - this.CCDBones[index].position) * this.CCDBones[index].rotation;
        float num2 = Mathf.Lerp((float) ((this.CCDBones.Length - index) / this.CCDBones.Length), 1f, this.roll);
        float b = Quaternion.Angle(Quaternion.identity, quaternion);
        float num3 = Mathf.Lerp(0.0f, b, (this.damper - b) / this.damper);
        this.CCDBones[index].rotation = Quaternion.RotateTowards(this.CCDBones[index].rotation, quaternion, num3 * num1 * num2);
      }
    }

    private void Iterate(int iteration)
    {
      if (!this.enabled || !this.gameObject.activeInHierarchy || this.ik.solver.iterations == 0)
        return;
      this.leftShoulderPos = this.transform.position + (this.leftShoulderPos - this.transform.position).normalized * this.leftShoulderDist;
      this.rightShoulderPos = this.transform.position + (this.rightShoulderPos - this.transform.position).normalized * this.rightShoulderDist;
      this.Solve(ref this.leftShoulderPos, ref this.rightShoulderPos, this.shoulderDist);
      this.LerpSolverPosition(this.ik.solver.leftShoulderEffector, this.leftShoulderPos, this.positionWeight * this.ik.solver.IKPositionWeight, this.ik.solver.leftShoulderEffector.positionOffset);
      this.LerpSolverPosition(this.ik.solver.rightShoulderEffector, this.rightShoulderPos, this.positionWeight * this.ik.solver.IKPositionWeight, this.ik.solver.rightShoulderEffector.positionOffset);
      Quaternion rotation = QuaTools.FromToRotation(this.chestRotation, Quaternion.LookRotation(this.transform.position - this.leftShoulderPos, this.rightShoulderPos - this.leftShoulderPos));
      this.LerpSolverPosition(this.ik.solver.bodyEffector, this.transform.position + rotation * this.headToBody, this.positionWeight * this.ik.solver.IKPositionWeight, this.ik.solver.bodyEffector.positionOffset - this.ik.solver.pullBodyOffset);
      Quaternion quaternion = Quaternion.Lerp(Quaternion.identity, rotation, this.thighWeight);
      Vector3 vector3_1 = quaternion * this.headToLeftThigh;
      Vector3 vector3_2 = quaternion * this.headToRightThigh;
      this.LerpSolverPosition(this.ik.solver.leftThighEffector, this.transform.position + vector3_1, this.positionWeight * this.ik.solver.IKPositionWeight, this.ik.solver.bodyEffector.positionOffset - this.ik.solver.pullBodyOffset + this.ik.solver.leftThighEffector.positionOffset);
      this.LerpSolverPosition(this.ik.solver.rightThighEffector, this.transform.position + vector3_2, this.positionWeight * this.ik.solver.IKPositionWeight, this.ik.solver.bodyEffector.positionOffset - this.ik.solver.pullBodyOffset + this.ik.solver.rightThighEffector.positionOffset);
    }

    private void OnPostUpdate()
    {
      if (!this.enabled || !this.gameObject.activeInHierarchy)
        return;
      this.PostStretching();
      this.ik.references.head.rotation = Quaternion.Lerp(Quaternion.identity, QuaTools.ClampRotation(QuaTools.FromToRotation(this.ik.references.head.rotation, this.transform.rotation), this.headClampWeight, 2), this.rotationWeight * this.ik.solver.IKPositionWeight) * this.ik.references.head.rotation;
    }

    private void ChestDirection()
    {
      float num = this.chestDirectionWeight * this.ik.solver.IKPositionWeight;
      if ((double) num <= 0.0)
        return;
      bool changed = false;
      this.chestDirection = V3Tools.ClampDirection(this.chestDirection, this.ik.references.root.forward, 0.45f, 2, out changed);
      if (this.chestDirection == Vector3.zero)
        return;
      Quaternion quaternion = Quaternion.Lerp(Quaternion.identity, Quaternion.FromToRotation(this.ik.references.root.forward, this.chestDirection), num * (1f / (float) this.chestBones.Length));
      foreach (Transform chestBone in this.chestBones)
        chestBone.rotation = quaternion * chestBone.rotation;
    }

    private void PostStretching()
    {
      float num = this.postStretchWeight * this.ik.solver.IKPositionWeight;
      if ((double) num > 0.0)
      {
        Vector3 vector3 = Vector3.ClampMagnitude(this.transform.position - this.ik.references.head.position, this.maxStretch) * num;
        this.stretchDamper = Mathf.Max(this.stretchDamper, 0.0f);
        if ((double) this.stretchDamper > 0.0)
          vector3 /= (float) ((1.0 + (double) vector3.magnitude) * (1.0 + (double) this.stretchDamper));
        for (int index = 0; index < this.stretchBones.Length; ++index)
        {
          if ((UnityEngine.Object) this.stretchBones[index] != (UnityEngine.Object) null)
            this.stretchBones[index].position += vector3 / (float) this.stretchBones.Length;
        }
      }
      if (!this.fixHead || (double) this.ik.solver.IKPositionWeight <= 0.0)
        return;
      this.ik.references.head.position = this.transform.position;
    }

    private void LerpSolverPosition(
      IKEffector effector,
      Vector3 position,
      float weight,
      Vector3 offset)
    {
      effector.GetNode((IKSolverFullBody) this.ik.solver).solverPosition = Vector3.Lerp(effector.GetNode((IKSolverFullBody) this.ik.solver).solverPosition, position + offset, weight);
    }

    private void Solve(ref Vector3 pos1, ref Vector3 pos2, float nominalDistance)
    {
      Vector3 vector3_1 = pos2 - pos1;
      float magnitude = vector3_1.magnitude;
      if ((double) magnitude == (double) nominalDistance || (double) magnitude == 0.0)
        return;
      float num = 1f * (float) (1.0 - (double) nominalDistance / (double) magnitude);
      Vector3 vector3_2 = vector3_1 * num * 0.5f;
      pos1 += vector3_2;
      pos2 -= vector3_2;
    }

    private void OnDestroy()
    {
      if (!((UnityEngine.Object) this.ik != (UnityEngine.Object) null))
        return;
      IKSolverFullBodyBiped solver1 = this.ik.solver;
      solver1.OnPreRead = solver1.OnPreRead - new IKSolver.UpdateDelegate(this.OnPreRead);
      IKSolverFullBodyBiped solver2 = this.ik.solver;
      solver2.OnPreIteration = solver2.OnPreIteration - new IKSolver.IterationDelegate(this.Iterate);
      IKSolverFullBodyBiped solver3 = this.ik.solver;
      solver3.OnPostUpdate = solver3.OnPostUpdate - new IKSolver.UpdateDelegate(this.OnPostUpdate);
      IKSolverFullBodyBiped solver4 = this.ik.solver;
      solver4.OnStoreDefaultLocalState = solver4.OnStoreDefaultLocalState - new IKSolver.UpdateDelegate(this.OnStoreDefaultLocalState);
      IKSolverFullBodyBiped solver5 = this.ik.solver;
      solver5.OnFixTransforms = solver5.OnFixTransforms - new IKSolver.UpdateDelegate(this.OnFixTransforms);
    }

    [Serializable]
    public class BendBone
    {
      [Tooltip("Assign spine and/or neck bones.")]
      public Transform transform;
      [Tooltip("The weight of rotating this bone.")]
      [Range(0.0f, 1f)]
      public float weight = 0.5f;
      private Quaternion defaultLocalRotation = Quaternion.identity;

      public BendBone()
      {
      }

      public BendBone(Transform transform, float weight)
      {
        this.transform = transform;
        this.weight = weight;
      }

      public void StoreDefaultLocalState()
      {
        this.defaultLocalRotation = this.transform.localRotation;
      }

      public void FixTransforms() => this.transform.localRotation = this.defaultLocalRotation;
    }
  }
}
