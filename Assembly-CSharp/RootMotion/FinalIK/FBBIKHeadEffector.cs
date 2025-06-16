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
    public float rotationWeight;
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
    public BendBone[] bendBones = new BendBone[0];
    [LargeHeader("CCD")]
    [Tooltip("Optional. The master weight of the CCD (Cyclic Coordinate Descent) IK effect that bends the spine towards the head effector before FBBIK solves.")]
    [Range(0.0f, 1f)]
    public float CCDWeight = 1f;
    [Tooltip("The weight of rolling the bones in towards the target")]
    [Range(0.0f, 1f)]
    public float roll;
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
    public float stretchDamper;
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
      IKSolverFullBodyBiped solver1 = ik.solver;
      solver1.OnPreRead = solver1.OnPreRead + OnPreRead;
      IKSolverFullBodyBiped solver2 = ik.solver;
      solver2.OnPreIteration = solver2.OnPreIteration + Iterate;
      IKSolverFullBodyBiped solver3 = ik.solver;
      solver3.OnPostUpdate = solver3.OnPostUpdate + OnPostUpdate;
      IKSolverFullBodyBiped solver4 = ik.solver;
      solver4.OnStoreDefaultLocalState = solver4.OnStoreDefaultLocalState + OnStoreDefaultLocalState;
      IKSolverFullBodyBiped solver5 = ik.solver;
      solver5.OnFixTransforms = solver5.OnFixTransforms + OnFixTransforms;
      OnStoreDefaultLocalState();
      headRotationRelativeToRoot = Quaternion.Inverse(ik.references.root.rotation) * ik.references.head.rotation;
    }

    private void OnStoreDefaultLocalState()
    {
      foreach (BendBone bendBone in bendBones)
        bendBone?.StoreDefaultLocalState();
      ccdDefaultLocalRotations = new Quaternion[CCDBones.Length];
      for (int index = 0; index < CCDBones.Length; ++index)
      {
        if (CCDBones[index] != null)
          ccdDefaultLocalRotations[index] = CCDBones[index].localRotation;
      }
      headLocalPosition = ik.references.head.localPosition;
      headLocalRotation = ik.references.head.localRotation;
      stretchLocalPositions = new Vector3[stretchBones.Length];
      stretchLocalRotations = new Quaternion[stretchBones.Length];
      for (int index = 0; index < stretchBones.Length; ++index)
      {
        if (stretchBones[index] != null)
        {
          stretchLocalPositions[index] = stretchBones[index].localPosition;
          stretchLocalRotations[index] = stretchBones[index].localRotation;
        }
      }
      chestLocalPositions = new Vector3[chestBones.Length];
      chestLocalRotations = new Quaternion[chestBones.Length];
      for (int index = 0; index < chestBones.Length; ++index)
      {
        if (chestBones[index] != null)
        {
          chestLocalPositions[index] = chestBones[index].localPosition;
          chestLocalRotations[index] = chestBones[index].localRotation;
        }
      }
      bendBonesCount = bendBones.Length;
      ccdBonesCount = CCDBones.Length;
      stretchBonesCount = stretchBones.Length;
      chestBonesCount = chestBones.Length;
    }

    private void OnFixTransforms()
    {
      if (!enabled)
        return;
      foreach (BendBone bendBone in bendBones)
        bendBone?.FixTransforms();
      for (int index = 0; index < CCDBones.Length; ++index)
      {
        if (CCDBones[index] != null)
          CCDBones[index].localRotation = ccdDefaultLocalRotations[index];
      }
      ik.references.head.localPosition = headLocalPosition;
      ik.references.head.localRotation = headLocalRotation;
      for (int index = 0; index < stretchBones.Length; ++index)
      {
        if (stretchBones[index] != null)
        {
          stretchBones[index].localPosition = stretchLocalPositions[index];
          stretchBones[index].localRotation = stretchLocalRotations[index];
        }
      }
      for (int index = 0; index < chestBones.Length; ++index)
      {
        if (chestBones[index] != null)
        {
          chestBones[index].localPosition = chestLocalPositions[index];
          chestBones[index].localRotation = chestLocalRotations[index];
        }
      }
    }

    private void OnPreRead()
    {
      if (!enabled || !gameObject.activeInHierarchy || ik.solver.iterations == 0)
        return;
      ik.solver.FABRIKPass = handsPullBody;
      if (bendBonesCount != bendBones.Length || ccdBonesCount != CCDBones.Length || stretchBonesCount != stretchBones.Length || chestBonesCount != chestBones.Length)
        OnStoreDefaultLocalState();
      ChestDirection();
      SpineBend();
      CCDPass();
      offset = transform.position - ik.references.head.position;
      shoulderDist = Vector3.Distance(ik.references.leftUpperArm.position, ik.references.rightUpperArm.position);
      leftShoulderDist = Vector3.Distance(ik.references.head.position, ik.references.leftUpperArm.position);
      rightShoulderDist = Vector3.Distance(ik.references.head.position, ik.references.rightUpperArm.position);
      headToBody = ik.solver.rootNode.position - ik.references.head.position;
      headToLeftThigh = ik.references.leftThigh.position - ik.references.head.position;
      headToRightThigh = ik.references.rightThigh.position - ik.references.head.position;
      leftShoulderPos = ik.references.leftUpperArm.position + offset * bodyWeight;
      rightShoulderPos = ik.references.rightUpperArm.position + offset * bodyWeight;
      chestRotation = Quaternion.LookRotation(ik.references.head.position - ik.references.leftUpperArm.position, ik.references.rightUpperArm.position - ik.references.leftUpperArm.position);
      if (OnPostHeadEffectorFK == null)
        return;
      OnPostHeadEffectorFK();
    }

    private void SpineBend()
    {
      float num1 = bendWeight * ik.solver.IKPositionWeight;
      if (num1 <= 0.0 || bendBones.Length == 0)
        return;
      Quaternion b = QuaTools.ClampRotation(transform.rotation * Quaternion.Inverse(ik.references.root.rotation * headRotationRelativeToRoot), bodyClampWeight, 2);
      float num2 = 1f / bendBones.Length;
      for (int index = 0; index < bendBones.Length; ++index)
      {
        if (bendBones[index].transform != null)
          bendBones[index].transform.rotation = Quaternion.Lerp(Quaternion.identity, b, num2 * bendBones[index].weight * num1) * bendBones[index].transform.rotation;
      }
    }

    private void CCDPass()
    {
      float num1 = CCDWeight * ik.solver.IKPositionWeight;
      if (num1 <= 0.0)
        return;
      for (int index = CCDBones.Length - 1; index > -1; --index)
      {
        Quaternion quaternion = Quaternion.FromToRotation(ik.references.head.position - CCDBones[index].position, transform.position - CCDBones[index].position) * CCDBones[index].rotation;
        float num2 = Mathf.Lerp((CCDBones.Length - index) / CCDBones.Length, 1f, roll);
        float b = Quaternion.Angle(Quaternion.identity, quaternion);
        float num3 = Mathf.Lerp(0.0f, b, (damper - b) / damper);
        CCDBones[index].rotation = Quaternion.RotateTowards(CCDBones[index].rotation, quaternion, num3 * num1 * num2);
      }
    }

    private void Iterate(int iteration)
    {
      if (!enabled || !gameObject.activeInHierarchy || ik.solver.iterations == 0)
        return;
      leftShoulderPos = transform.position + (leftShoulderPos - transform.position).normalized * leftShoulderDist;
      rightShoulderPos = transform.position + (rightShoulderPos - transform.position).normalized * rightShoulderDist;
      Solve(ref leftShoulderPos, ref rightShoulderPos, shoulderDist);
      LerpSolverPosition(ik.solver.leftShoulderEffector, leftShoulderPos, positionWeight * ik.solver.IKPositionWeight, ik.solver.leftShoulderEffector.positionOffset);
      LerpSolverPosition(ik.solver.rightShoulderEffector, rightShoulderPos, positionWeight * ik.solver.IKPositionWeight, ik.solver.rightShoulderEffector.positionOffset);
      Quaternion rotation = QuaTools.FromToRotation(chestRotation, Quaternion.LookRotation(transform.position - leftShoulderPos, rightShoulderPos - leftShoulderPos));
      LerpSolverPosition(ik.solver.bodyEffector, transform.position + rotation * headToBody, positionWeight * ik.solver.IKPositionWeight, ik.solver.bodyEffector.positionOffset - ik.solver.pullBodyOffset);
      Quaternion quaternion = Quaternion.Lerp(Quaternion.identity, rotation, thighWeight);
      Vector3 vector3_1 = quaternion * headToLeftThigh;
      Vector3 vector3_2 = quaternion * headToRightThigh;
      LerpSolverPosition(ik.solver.leftThighEffector, transform.position + vector3_1, positionWeight * ik.solver.IKPositionWeight, ik.solver.bodyEffector.positionOffset - ik.solver.pullBodyOffset + ik.solver.leftThighEffector.positionOffset);
      LerpSolverPosition(ik.solver.rightThighEffector, transform.position + vector3_2, positionWeight * ik.solver.IKPositionWeight, ik.solver.bodyEffector.positionOffset - ik.solver.pullBodyOffset + ik.solver.rightThighEffector.positionOffset);
    }

    private void OnPostUpdate()
    {
      if (!enabled || !gameObject.activeInHierarchy)
        return;
      PostStretching();
      ik.references.head.rotation = Quaternion.Lerp(Quaternion.identity, QuaTools.ClampRotation(QuaTools.FromToRotation(ik.references.head.rotation, transform.rotation), headClampWeight, 2), rotationWeight * ik.solver.IKPositionWeight) * ik.references.head.rotation;
    }

    private void ChestDirection()
    {
      float num = chestDirectionWeight * ik.solver.IKPositionWeight;
      if (num <= 0.0)
        return;
      bool changed = false;
      chestDirection = V3Tools.ClampDirection(chestDirection, ik.references.root.forward, 0.45f, 2, out changed);
      if (chestDirection == Vector3.zero)
        return;
      Quaternion quaternion = Quaternion.Lerp(Quaternion.identity, Quaternion.FromToRotation(ik.references.root.forward, chestDirection), num * (1f / chestBones.Length));
      foreach (Transform chestBone in chestBones)
        chestBone.rotation = quaternion * chestBone.rotation;
    }

    private void PostStretching()
    {
      float num = postStretchWeight * ik.solver.IKPositionWeight;
      if (num > 0.0)
      {
        Vector3 vector3 = Vector3.ClampMagnitude(transform.position - ik.references.head.position, maxStretch) * num;
        stretchDamper = Mathf.Max(stretchDamper, 0.0f);
        if (stretchDamper > 0.0)
          vector3 /= (float) ((1.0 + vector3.magnitude) * (1.0 + stretchDamper));
        for (int index = 0; index < stretchBones.Length; ++index)
        {
          if (stretchBones[index] != null)
            stretchBones[index].position += vector3 / stretchBones.Length;
        }
      }
      if (!fixHead || ik.solver.IKPositionWeight <= 0.0)
        return;
      ik.references.head.position = transform.position;
    }

    private void LerpSolverPosition(
      IKEffector effector,
      Vector3 position,
      float weight,
      Vector3 offset)
    {
      effector.GetNode(ik.solver).solverPosition = Vector3.Lerp(effector.GetNode(ik.solver).solverPosition, position + offset, weight);
    }

    private void Solve(ref Vector3 pos1, ref Vector3 pos2, float nominalDistance)
    {
      Vector3 vector3_1 = pos2 - pos1;
      float magnitude = vector3_1.magnitude;
      if (magnitude == (double) nominalDistance || magnitude == 0.0)
        return;
      float num = 1f * (float) (1.0 - nominalDistance / (double) magnitude);
      Vector3 vector3_2 = vector3_1 * num * 0.5f;
      pos1 += vector3_2;
      pos2 -= vector3_2;
    }

    private void OnDestroy()
    {
      if (!(ik != null))
        return;
      IKSolverFullBodyBiped solver1 = ik.solver;
      solver1.OnPreRead = solver1.OnPreRead - OnPreRead;
      IKSolverFullBodyBiped solver2 = ik.solver;
      solver2.OnPreIteration = solver2.OnPreIteration - Iterate;
      IKSolverFullBodyBiped solver3 = ik.solver;
      solver3.OnPostUpdate = solver3.OnPostUpdate - OnPostUpdate;
      IKSolverFullBodyBiped solver4 = ik.solver;
      solver4.OnStoreDefaultLocalState = solver4.OnStoreDefaultLocalState - OnStoreDefaultLocalState;
      IKSolverFullBodyBiped solver5 = ik.solver;
      solver5.OnFixTransforms = solver5.OnFixTransforms - OnFixTransforms;
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
        defaultLocalRotation = transform.localRotation;
      }

      public void FixTransforms() => transform.localRotation = defaultLocalRotation;
    }
  }
}
