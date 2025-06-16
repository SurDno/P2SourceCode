// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IKSolverVR
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverVR : IKSolver
  {
    private Transform[] solverTransforms = new Transform[0];
    private bool hasChest;
    private bool hasNeck;
    private bool hasShoulders;
    private bool hasToes;
    private Vector3[] readPositions = new Vector3[0];
    private Quaternion[] readRotations = new Quaternion[0];
    private Vector3[] solvedPositions = new Vector3[22];
    private Quaternion[] solvedRotations = new Quaternion[22];
    private Quaternion[] defaultLocalRotations = new Quaternion[21];
    private Vector3[] defaultLocalPositions = new Vector3[21];
    private Vector3 rootV;
    private Vector3 rootVelocity;
    private Vector3 bodyOffset;
    private int supportLegIndex;
    [Tooltip("If true, will keep the toes planted even if head target is out of reach.")]
    public bool plantFeet = true;
    [Tooltip("The spine solver.")]
    public IKSolverVR.Spine spine = new IKSolverVR.Spine();
    [Tooltip("The left arm solver.")]
    public IKSolverVR.Arm leftArm = new IKSolverVR.Arm();
    [Tooltip("The right arm solver.")]
    public IKSolverVR.Arm rightArm = new IKSolverVR.Arm();
    [Tooltip("The left leg solver.")]
    public IKSolverVR.Leg leftLeg = new IKSolverVR.Leg();
    [Tooltip("The right leg solver.")]
    public IKSolverVR.Leg rightLeg = new IKSolverVR.Leg();
    [Tooltip("The procedural locomotion solver.")]
    public IKSolverVR.Locomotion locomotion = new IKSolverVR.Locomotion();
    private IKSolverVR.Leg[] legs = new IKSolverVR.Leg[2];
    private IKSolverVR.Arm[] arms = new IKSolverVR.Arm[2];
    private Vector3 headPosition;
    private Vector3 headDeltaPosition;
    private Vector3 raycastOriginPelvis;
    private Vector3 lastOffset;
    private Vector3 debugPos1;
    private Vector3 debugPos2;
    private Vector3 debugPos3;
    private Vector3 debugPos4;

    public void SetToReferences(VRIK.References references)
    {
      if (!references.isFilled)
      {
        Debug.LogError((object) "Invalid references, one or more Transforms are missing.");
      }
      else
      {
        this.solverTransforms = references.GetTransforms();
        this.hasChest = (UnityEngine.Object) this.solverTransforms[3] != (UnityEngine.Object) null;
        this.hasNeck = (UnityEngine.Object) this.solverTransforms[4] != (UnityEngine.Object) null;
        this.hasShoulders = (UnityEngine.Object) this.solverTransforms[6] != (UnityEngine.Object) null && (UnityEngine.Object) this.solverTransforms[10] != (UnityEngine.Object) null;
        this.hasToes = (UnityEngine.Object) this.solverTransforms[17] != (UnityEngine.Object) null && (UnityEngine.Object) this.solverTransforms[21] != (UnityEngine.Object) null;
        this.readPositions = new Vector3[this.solverTransforms.Length];
        this.readRotations = new Quaternion[this.solverTransforms.Length];
        this.DefaultAnimationCurves();
        this.GuessHandOrientations(references, true);
      }
    }

    public void GuessHandOrientations(VRIK.References references, bool onlyIfZero)
    {
      if (!references.isFilled)
      {
        Debug.LogWarning((object) "VRIK References are not filled in, can not guess hand orientations. Right-click on VRIK header and slect 'Guess Hand Orientations' when you have filled in the References.");
      }
      else
      {
        if (this.leftArm.wristToPalmAxis == Vector3.zero || !onlyIfZero)
          this.leftArm.wristToPalmAxis = this.GuessWristToPalmAxis(references.leftHand, references.leftForearm);
        if (this.leftArm.palmToThumbAxis == Vector3.zero || !onlyIfZero)
          this.leftArm.palmToThumbAxis = this.GuessPalmToThumbAxis(references.leftHand, references.leftForearm);
        if (this.rightArm.wristToPalmAxis == Vector3.zero || !onlyIfZero)
          this.rightArm.wristToPalmAxis = this.GuessWristToPalmAxis(references.rightHand, references.rightForearm);
        if (!(this.rightArm.palmToThumbAxis == Vector3.zero) && onlyIfZero)
          return;
        this.rightArm.palmToThumbAxis = this.GuessPalmToThumbAxis(references.rightHand, references.rightForearm);
      }
    }

    public void DefaultAnimationCurves()
    {
      if (this.locomotion.stepHeight == null)
        this.locomotion.stepHeight = new AnimationCurve();
      if (this.locomotion.heelHeight == null)
        this.locomotion.heelHeight = new AnimationCurve();
      if (this.locomotion.stepHeight.keys.Length == 0)
        this.locomotion.stepHeight.keys = IKSolverVR.GetSineKeyframes(0.03f);
      if (this.locomotion.heelHeight.keys.Length != 0)
        return;
      this.locomotion.heelHeight.keys = IKSolverVR.GetSineKeyframes(0.03f);
    }

    public void AddPositionOffset(IKSolverVR.PositionOffset positionOffset, Vector3 value)
    {
      switch (positionOffset)
      {
        case IKSolverVR.PositionOffset.Pelvis:
          this.spine.pelvisPositionOffset += value;
          break;
        case IKSolverVR.PositionOffset.Chest:
          this.spine.chestPositionOffset += value;
          break;
        case IKSolverVR.PositionOffset.Head:
          this.spine.headPositionOffset += value;
          break;
        case IKSolverVR.PositionOffset.LeftHand:
          this.leftArm.handPositionOffset += value;
          break;
        case IKSolverVR.PositionOffset.RightHand:
          this.rightArm.handPositionOffset += value;
          break;
        case IKSolverVR.PositionOffset.LeftFoot:
          this.leftLeg.footPositionOffset += value;
          break;
        case IKSolverVR.PositionOffset.RightFoot:
          this.rightLeg.footPositionOffset += value;
          break;
        case IKSolverVR.PositionOffset.LeftHeel:
          this.leftLeg.heelPositionOffset += value;
          break;
        case IKSolverVR.PositionOffset.RightHeel:
          this.rightLeg.heelPositionOffset += value;
          break;
      }
    }

    public void AddRotationOffset(IKSolverVR.RotationOffset rotationOffset, Vector3 value)
    {
      this.AddRotationOffset(rotationOffset, Quaternion.Euler(value));
    }

    public void AddRotationOffset(IKSolverVR.RotationOffset rotationOffset, Quaternion value)
    {
      switch (rotationOffset)
      {
        case IKSolverVR.RotationOffset.Pelvis:
          this.spine.pelvisRotationOffset = value * this.spine.pelvisRotationOffset;
          break;
        case IKSolverVR.RotationOffset.Chest:
          this.spine.chestRotationOffset = value * this.spine.chestRotationOffset;
          break;
        case IKSolverVR.RotationOffset.Head:
          this.spine.headRotationOffset = value * this.spine.headRotationOffset;
          break;
      }
    }

    public void AddPlatformMotion(
      Vector3 deltaPosition,
      Quaternion deltaRotation,
      Vector3 platformPivot)
    {
      this.locomotion.AddDeltaPosition(deltaPosition);
      this.raycastOriginPelvis += deltaPosition;
      this.locomotion.AddDeltaRotation(deltaRotation, platformPivot);
      this.spine.faceDirection = deltaRotation * this.spine.faceDirection;
    }

    public void Reset()
    {
      if (!this.initiated)
        return;
      this.UpdateSolverTransforms();
      this.Read(this.readPositions, this.readRotations, this.hasChest, this.hasNeck, this.hasShoulders, this.hasToes);
      this.spine.faceDirection = this.rootBone.readRotation * Vector3.forward;
      this.locomotion.Reset(this.readPositions, this.readRotations);
      this.raycastOriginPelvis = this.spine.pelvis.readPosition;
    }

    public override void StoreDefaultLocalState()
    {
      for (int index = 1; index < this.solverTransforms.Length; ++index)
      {
        if ((UnityEngine.Object) this.solverTransforms[index] != (UnityEngine.Object) null)
        {
          this.defaultLocalPositions[index - 1] = this.solverTransforms[index].localPosition;
          this.defaultLocalRotations[index - 1] = this.solverTransforms[index].localRotation;
        }
      }
    }

    public override void FixTransforms()
    {
      if (!this.initiated)
        return;
      for (int index = 1; index < this.solverTransforms.Length; ++index)
      {
        if ((UnityEngine.Object) this.solverTransforms[index] != (UnityEngine.Object) null)
        {
          if (index == 1 | (index > 5 && index < 14))
            this.solverTransforms[index].localPosition = this.defaultLocalPositions[index - 1];
          this.solverTransforms[index].localRotation = this.defaultLocalRotations[index - 1];
        }
      }
    }

    public override IKSolver.Point[] GetPoints()
    {
      Debug.LogError((object) "GetPoints() is not applicable to IKSolverVR.");
      return (IKSolver.Point[]) null;
    }

    public override IKSolver.Point GetPoint(Transform transform)
    {
      Debug.LogError((object) "GetPoint is not applicable to IKSolverVR.");
      return (IKSolver.Point) null;
    }

    public override bool IsValid(ref string message)
    {
      if (this.solverTransforms == null || this.solverTransforms.Length == 0)
      {
        message = "Trying to initiate IKSolverVR with invalid bone references.";
        return false;
      }
      if (this.leftArm.wristToPalmAxis == Vector3.zero)
      {
        message = "Left arm 'Wrist To Palm Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the wrist towards the palm. If the arrow points away from the palm, axis must be negative.";
        return false;
      }
      if (this.rightArm.wristToPalmAxis == Vector3.zero)
      {
        message = "Right arm 'Wrist To Palm Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the wrist towards the palm. If the arrow points away from the palm, axis must be negative.";
        return false;
      }
      if (this.leftArm.palmToThumbAxis == Vector3.zero)
      {
        message = "Left arm 'Palm To Thumb Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the palm towards the thumb. If the arrow points away from the thumb, axis must be negative.";
        return false;
      }
      if (!(this.rightArm.palmToThumbAxis == Vector3.zero))
        return true;
      message = "Right arm 'Palm To Thumb Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the palm towards the thumb. If the arrow points away from the thumb, axis must be negative.";
      return false;
    }

    private Vector3 GetNormal(Transform[] transforms)
    {
      Vector3 zero1 = Vector3.zero;
      Vector3 zero2 = Vector3.zero;
      for (int index = 0; index < transforms.Length; ++index)
        zero2 += transforms[index].position;
      Vector3 vector3 = zero2 / (float) transforms.Length;
      for (int index = 0; index < transforms.Length - 1; ++index)
        zero1 += Vector3.Cross(transforms[index].position - vector3, transforms[index + 1].position - vector3).normalized;
      return zero1;
    }

    private Vector3 GuessWristToPalmAxis(Transform hand, Transform forearm)
    {
      Vector3 vector3 = forearm.position - hand.position;
      Vector3 palmAxis = AxisTools.ToVector3(AxisTools.GetAxisToDirection(hand, vector3));
      if ((double) Vector3.Dot(vector3, hand.rotation * palmAxis) > 0.0)
        palmAxis = -palmAxis;
      return palmAxis;
    }

    private Vector3 GuessPalmToThumbAxis(Transform hand, Transform forearm)
    {
      if (hand.childCount == 0)
      {
        Debug.LogWarning((object) ("Hand " + hand.name + " does not have any fingers, VRIK can not guess the hand bone's orientation. Please assign 'Wrist To Palm Axis' and 'Palm To Thumb Axis' manually for both arms in VRIK settings."), (UnityEngine.Object) hand);
        return Vector3.zero;
      }
      float num1 = float.PositiveInfinity;
      int index1 = 0;
      for (int index2 = 0; index2 < hand.childCount; ++index2)
      {
        float num2 = Vector3.SqrMagnitude(hand.GetChild(index2).position - hand.position);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          index1 = index2;
        }
      }
      Vector3 vector3 = Vector3.Cross(Vector3.Cross(hand.position - forearm.position, hand.GetChild(index1).position - hand.position), hand.position - forearm.position);
      Vector3 thumbAxis = AxisTools.ToVector3(AxisTools.GetAxisToDirection(hand, vector3));
      if ((double) Vector3.Dot(vector3, hand.rotation * thumbAxis) < 0.0)
        thumbAxis = -thumbAxis;
      return thumbAxis;
    }

    private static Keyframe[] GetSineKeyframes(float mag)
    {
      Keyframe[] sineKeyframes = new Keyframe[3];
      sineKeyframes[0].time = 0.0f;
      sineKeyframes[0].value = 0.0f;
      sineKeyframes[1].time = 0.5f;
      sineKeyframes[1].value = mag;
      sineKeyframes[2].time = 1f;
      sineKeyframes[2].value = 0.0f;
      return sineKeyframes;
    }

    private void UpdateSolverTransforms()
    {
      for (int index = 0; index < this.solverTransforms.Length; ++index)
      {
        if ((UnityEngine.Object) this.solverTransforms[index] != (UnityEngine.Object) null)
        {
          this.readPositions[index] = this.solverTransforms[index].position;
          this.readRotations[index] = this.solverTransforms[index].rotation;
        }
      }
    }

    protected override void OnInitiate()
    {
      this.UpdateSolverTransforms();
      this.Read(this.readPositions, this.readRotations, this.hasChest, this.hasNeck, this.hasShoulders, this.hasToes);
    }

    protected override void OnUpdate()
    {
      if ((double) this.IKPositionWeight <= 0.0)
        return;
      this.UpdateSolverTransforms();
      this.Read(this.readPositions, this.readRotations, this.hasChest, this.hasNeck, this.hasShoulders, this.hasToes);
      this.Solve();
      this.Write();
      this.WriteTransforms();
    }

    private void WriteTransforms()
    {
      for (int index = 0; index < this.solverTransforms.Length; ++index)
      {
        if ((UnityEngine.Object) this.solverTransforms[index] != (UnityEngine.Object) null)
        {
          if (index < 2 | (index > 5 && index < 14))
            this.solverTransforms[index].position = V3Tools.Lerp(this.solverTransforms[index].position, this.GetPosition(index), this.IKPositionWeight);
          this.solverTransforms[index].rotation = QuaTools.Lerp(this.solverTransforms[index].rotation, this.GetRotation(index), this.IKPositionWeight);
        }
      }
    }

    private void Read(
      Vector3[] positions,
      Quaternion[] rotations,
      bool hasChest,
      bool hasNeck,
      bool hasShoulders,
      bool hasToes)
    {
      if (this.rootBone == null)
        this.rootBone = new IKSolverVR.VirtualBone(positions[0], rotations[0]);
      else
        this.rootBone.Read(positions[0], rotations[0]);
      this.spine.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, 0, 1);
      this.leftArm.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, hasChest ? 3 : 2, 6);
      this.rightArm.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, hasChest ? 3 : 2, 10);
      this.leftLeg.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, 1, 14);
      this.rightLeg.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, 1, 18);
      for (int index = 0; index < rotations.Length; ++index)
      {
        this.solvedPositions[index] = positions[index];
        this.solvedRotations[index] = rotations[index];
      }
      if (this.initiated)
        return;
      this.legs = new IKSolverVR.Leg[2]
      {
        this.leftLeg,
        this.rightLeg
      };
      this.arms = new IKSolverVR.Arm[2]
      {
        this.leftArm,
        this.rightArm
      };
      this.locomotion.Initiate(positions, rotations, hasToes);
      this.raycastOriginPelvis = this.spine.pelvis.readPosition;
      this.spine.faceDirection = this.readRotations[0] * Vector3.forward;
    }

    private void Solve()
    {
      this.spine.PreSolve();
      foreach (IKSolverVR.BodyPart arm in this.arms)
        arm.PreSolve();
      foreach (IKSolverVR.BodyPart leg in this.legs)
        leg.PreSolve();
      foreach (IKSolverVR.BodyPart arm in this.arms)
        arm.ApplyOffsets();
      this.spine.ApplyOffsets();
      this.spine.Solve(this.rootBone, this.legs, this.arms);
      if ((double) this.spine.pelvisPositionWeight > 0.0 && this.plantFeet)
        Warning.Log("If VRIK 'Pelvis Position Weight' is > 0, 'Plant Feet' should be disabled to improve performance and stability.", this.root);
      if ((double) this.locomotion.weight > 0.0)
      {
        Vector3 leftFootPosition = Vector3.zero;
        Vector3 rightFootPosition = Vector3.zero;
        Quaternion leftFootRotation = Quaternion.identity;
        Quaternion rightFootRotation = Quaternion.identity;
        float leftFootOffset = 0.0f;
        float rightFootOffset = 0.0f;
        float leftHeelOffset = 0.0f;
        float rightHeelOffset = 0.0f;
        this.locomotion.Solve(this.rootBone, this.spine, this.leftLeg, this.rightLeg, this.leftArm, this.rightArm, this.supportLegIndex, out leftFootPosition, out rightFootPosition, out leftFootRotation, out rightFootRotation, out leftFootOffset, out rightFootOffset, out leftHeelOffset, out rightHeelOffset);
        leftFootPosition += this.root.up * leftFootOffset;
        rightFootPosition += this.root.up * rightFootOffset;
        this.leftLeg.footPositionOffset += (leftFootPosition - this.leftLeg.lastBone.solverPosition) * this.IKPositionWeight * (1f - this.leftLeg.positionWeight) * this.locomotion.weight;
        this.rightLeg.footPositionOffset += (rightFootPosition - this.rightLeg.lastBone.solverPosition) * this.IKPositionWeight * (1f - this.rightLeg.positionWeight) * this.locomotion.weight;
        this.leftLeg.heelPositionOffset += this.root.up * leftHeelOffset * this.locomotion.weight;
        this.rightLeg.heelPositionOffset += this.root.up * rightHeelOffset * this.locomotion.weight;
        Quaternion rotation1 = QuaTools.FromToRotation(this.leftLeg.lastBone.solverRotation, leftFootRotation);
        Quaternion rotation2 = QuaTools.FromToRotation(this.rightLeg.lastBone.solverRotation, rightFootRotation);
        Quaternion quaternion1 = Quaternion.Lerp(Quaternion.identity, rotation1, this.IKPositionWeight * (1f - this.leftLeg.rotationWeight) * this.locomotion.weight);
        Quaternion quaternion2 = Quaternion.Lerp(Quaternion.identity, rotation2, this.IKPositionWeight * (1f - this.rightLeg.rotationWeight) * this.locomotion.weight);
        this.leftLeg.footRotationOffset = quaternion1 * this.leftLeg.footRotationOffset;
        this.rightLeg.footRotationOffset = quaternion2 * this.rightLeg.footRotationOffset;
        Vector3 plane = V3Tools.PointToPlane(Vector3.Lerp(this.leftLeg.position + this.leftLeg.footPositionOffset, this.rightLeg.position + this.rightLeg.footPositionOffset, 0.5f), this.rootBone.solverPosition, this.root.up);
        this.rootBone.solverPosition = Vector3.Lerp(this.rootBone.solverPosition + this.rootVelocity * Time.deltaTime * 2f * this.locomotion.weight, plane, Time.deltaTime * this.locomotion.rootSpeed * this.locomotion.weight);
        this.rootVelocity += (plane - this.rootBone.solverPosition) * Time.deltaTime * 10f;
        this.rootVelocity -= V3Tools.ExtractVertical(this.rootVelocity, this.root.up, 1f);
        this.bodyOffset = Vector3.Lerp(this.bodyOffset, this.root.up * (leftFootOffset + rightFootOffset), Time.deltaTime * 3f);
        this.bodyOffset = Vector3.Lerp(Vector3.zero, this.bodyOffset, this.locomotion.weight);
      }
      foreach (IKSolverVR.BodyPart leg in this.legs)
        leg.ApplyOffsets();
      if (!this.plantFeet)
      {
        this.spine.InverseTranslateToHead(this.legs, false, false, this.bodyOffset, 1f);
        foreach (IKSolverVR.BodyPart leg in this.legs)
          leg.TranslateRoot(this.spine.pelvis.solverPosition, this.spine.pelvis.solverRotation);
        foreach (IKSolverVR.Leg leg in this.legs)
          leg.Solve();
      }
      else
      {
        for (int index = 0; index < 2; ++index)
        {
          this.spine.InverseTranslateToHead(this.legs, true, index == 0, this.bodyOffset, 1f);
          foreach (IKSolverVR.BodyPart leg in this.legs)
            leg.TranslateRoot(this.spine.pelvis.solverPosition, this.spine.pelvis.solverRotation);
          foreach (IKSolverVR.Leg leg in this.legs)
            leg.Solve();
        }
      }
      for (int index = 0; index < this.arms.Length; ++index)
        this.arms[index].TranslateRoot(this.spine.chest.solverPosition, this.spine.chest.solverRotation);
      for (int index = 0; index < this.arms.Length; ++index)
        this.arms[index].Solve(index == 0);
      this.spine.ResetOffsets();
      foreach (IKSolverVR.BodyPart leg in this.legs)
        leg.ResetOffsets();
      foreach (IKSolverVR.BodyPart arm in this.arms)
        arm.ResetOffsets();
      this.spine.pelvisPositionOffset += this.GetPelvisOffset();
      this.spine.chestPositionOffset += this.spine.pelvisPositionOffset;
      this.Write();
      this.supportLegIndex = -1;
      float num1 = float.PositiveInfinity;
      for (int index = 0; index < this.legs.Length; ++index)
      {
        float num2 = Vector3.SqrMagnitude(this.legs[index].lastBone.solverPosition - this.legs[index].bones[0].solverPosition);
        if ((double) num2 < (double) num1)
        {
          this.supportLegIndex = index;
          num1 = num2;
        }
      }
    }

    private Vector3 GetPosition(int index) => this.solvedPositions[index];

    private Quaternion GetRotation(int index) => this.solvedRotations[index];

    [HideInInspector]
    public IKSolverVR.VirtualBone rootBone { get; private set; }

    private void Write()
    {
      this.solvedPositions[0] = this.rootBone.solverPosition;
      this.solvedRotations[0] = this.rootBone.solverRotation;
      this.spine.Write(ref this.solvedPositions, ref this.solvedRotations);
      foreach (IKSolverVR.BodyPart leg in this.legs)
        leg.Write(ref this.solvedPositions, ref this.solvedRotations);
      foreach (IKSolverVR.BodyPart arm in this.arms)
        arm.Write(ref this.solvedPositions, ref this.solvedRotations);
    }

    private Vector3 GetPelvisOffset()
    {
      if ((double) this.locomotion.weight <= 0.0 || (int) this.locomotion.blockingLayers == -1)
        return Vector3.zero;
      Vector3 raycastOriginPelvis = this.raycastOriginPelvis with
      {
        y = this.spine.pelvis.solverPosition.y
      };
      Vector3 origin = this.spine.pelvis.readPosition with
      {
        y = this.spine.pelvis.solverPosition.y
      };
      Vector3 direction1 = origin - raycastOriginPelvis;
      RaycastHit hitInfo;
      if ((double) this.locomotion.raycastRadius <= 0.0)
      {
        if (Physics.Raycast(raycastOriginPelvis, direction1, out hitInfo, direction1.magnitude * 1.1f, (int) this.locomotion.blockingLayers))
          origin = hitInfo.point;
      }
      else if (Physics.SphereCast(raycastOriginPelvis, this.locomotion.raycastRadius * 1.1f, direction1, out hitInfo, direction1.magnitude, (int) this.locomotion.blockingLayers))
        origin = raycastOriginPelvis + direction1.normalized * hitInfo.distance / 1.1f;
      Vector3 vector3 = this.spine.pelvis.solverPosition;
      Vector3 direction2 = vector3 - origin;
      if ((double) this.locomotion.raycastRadius <= 0.0)
      {
        if (Physics.Raycast(origin, direction2, out hitInfo, direction2.magnitude, (int) this.locomotion.blockingLayers))
          vector3 = hitInfo.point;
      }
      else if (Physics.SphereCast(origin, this.locomotion.raycastRadius, direction2, out hitInfo, direction2.magnitude, (int) this.locomotion.blockingLayers))
        vector3 = origin + direction2.normalized * hitInfo.distance;
      this.lastOffset = Vector3.Lerp(this.lastOffset, Vector3.zero, Time.deltaTime * 3f);
      this.lastOffset = Vector3.Lerp(this.lastOffset, (vector3 + Vector3.ClampMagnitude(this.lastOffset, 0.75f)) with
      {
        y = this.spine.pelvis.solverPosition.y
      } - this.spine.pelvis.solverPosition, Time.deltaTime * 15f);
      return this.lastOffset;
    }

    [Serializable]
    public class Arm : IKSolverVR.BodyPart
    {
      [Tooltip("The hand target")]
      public Transform target;
      [Tooltip("The elbow will be bent towards this Transform if 'Bend Goal Weight' > 0.")]
      public Transform bendGoal;
      [Tooltip("Positional weight of the hand target.")]
      [Range(0.0f, 1f)]
      public float positionWeight = 1f;
      [Tooltip("Rotational weight of the hand target")]
      [Range(0.0f, 1f)]
      public float rotationWeight = 1f;
      [Tooltip("Different techniques for shoulder bone rotation.")]
      public IKSolverVR.Arm.ShoulderRotationMode shoulderRotationMode = IKSolverVR.Arm.ShoulderRotationMode.YawPitch;
      [Tooltip("The weight of shoulder rotation")]
      [Range(0.0f, 1f)]
      public float shoulderRotationWeight = 1f;
      [Tooltip("If greater than 0, will bend the elbow towards the 'Bend Goal' Transform.")]
      [Range(0.0f, 1f)]
      public float bendGoalWeight;
      [Tooltip("Angular offset of the elbow bending direction.")]
      [Range(-180f, 180f)]
      public float swivelOffset;
      [Tooltip("Local axis of the hand bone that points from the wrist towards the palm. Used for defining hand bone orientation.")]
      public Vector3 wristToPalmAxis = Vector3.zero;
      [Tooltip("Local axis of the hand bone that points from the palm towards the thumb. Used for defining hand bone orientation.")]
      public Vector3 palmToThumbAxis = Vector3.zero;
      [Tooltip("Use this to make the arm shorter/longer.")]
      [Range(0.01f, 2f)]
      public float armLengthMlp = 1f;
      [Tooltip("Evaluates stretching of the arm by target distance relative to arm length. Value at time 1 represents stretching amount at the point where distance to the target is equal to arm length. Value at time 2 represents stretching amount at the point where distance to the target is double the arm length. Value represents the amount of stretching. Linear stretching would be achieved with a linear curve going up by 45 degrees. Increase the range of stretching by moving the last key up and right at the same amount. Smoothing in the curve can help reduce elbow snapping (start stretching the arm slightly before target distance reaches arm length).")]
      public AnimationCurve stretchCurve;
      [HideInInspector]
      [NonSerialized]
      public Vector3 IKPosition;
      [HideInInspector]
      [NonSerialized]
      public Quaternion IKRotation = Quaternion.identity;
      [HideInInspector]
      [NonSerialized]
      public Vector3 bendDirection = Vector3.back;
      [HideInInspector]
      [NonSerialized]
      public Vector3 handPositionOffset;
      private bool hasShoulder;
      private Vector3 chestForwardAxis;
      private Vector3 chestUpAxis;
      private Quaternion chestRotation = Quaternion.identity;
      private Vector3 chestForward;
      private Vector3 chestUp;
      private Quaternion forearmRelToUpperArm = Quaternion.identity;
      private const float yawOffsetAngle = 45f;
      private const float pitchOffsetAngle = -30f;

      public Vector3 position { get; private set; }

      public Quaternion rotation { get; private set; }

      private IKSolverVR.VirtualBone shoulder => this.bones[0];

      private IKSolverVR.VirtualBone upperArm => this.bones[1];

      private IKSolverVR.VirtualBone forearm => this.bones[2];

      private IKSolverVR.VirtualBone hand => this.bones[3];

      protected override void OnRead(
        Vector3[] positions,
        Quaternion[] rotations,
        bool hasChest,
        bool hasNeck,
        bool hasShoulders,
        bool hasToes,
        int rootIndex,
        int index)
      {
        Vector3 position1 = positions[index];
        Quaternion rotation1 = rotations[index];
        Vector3 position2 = positions[index + 1];
        Quaternion rotation2 = rotations[index + 1];
        Vector3 position3 = positions[index + 2];
        Quaternion rotation3 = rotations[index + 2];
        Vector3 position4 = positions[index + 3];
        Quaternion rotation4 = rotations[index + 3];
        if (!this.initiated)
        {
          this.IKPosition = position4;
          this.IKRotation = rotation4;
          this.rotation = this.IKRotation;
          this.hasShoulder = hasShoulders;
          this.bones = new IKSolverVR.VirtualBone[this.hasShoulder ? 4 : 3];
          if (this.hasShoulder)
          {
            this.bones[0] = new IKSolverVR.VirtualBone(position1, rotation1);
            this.bones[1] = new IKSolverVR.VirtualBone(position2, rotation2);
            this.bones[2] = new IKSolverVR.VirtualBone(position3, rotation3);
            this.bones[3] = new IKSolverVR.VirtualBone(position4, rotation4);
          }
          else
          {
            this.bones[0] = new IKSolverVR.VirtualBone(position2, rotation2);
            this.bones[1] = new IKSolverVR.VirtualBone(position3, rotation3);
            this.bones[2] = new IKSolverVR.VirtualBone(position4, rotation4);
          }
          this.chestForwardAxis = Quaternion.Inverse(this.rootRotation) * (rotations[0] * Vector3.forward);
          this.chestUpAxis = Quaternion.Inverse(this.rootRotation) * (rotations[0] * Vector3.up);
        }
        if (this.hasShoulder)
        {
          this.bones[0].Read(position1, rotation1);
          this.bones[1].Read(position2, rotation2);
          this.bones[2].Read(position3, rotation3);
          this.bones[3].Read(position4, rotation4);
        }
        else
        {
          this.bones[0].Read(position2, rotation2);
          this.bones[1].Read(position3, rotation3);
          this.bones[2].Read(position4, rotation4);
        }
      }

      public override void PreSolve()
      {
        if ((UnityEngine.Object) this.target != (UnityEngine.Object) null)
        {
          this.IKPosition = this.target.position;
          this.IKRotation = this.target.rotation;
        }
        this.position = V3Tools.Lerp(this.hand.solverPosition, this.IKPosition, this.positionWeight);
        this.rotation = QuaTools.Lerp(this.hand.solverRotation, this.IKRotation, this.rotationWeight);
        this.shoulder.axis = this.shoulder.axis.normalized;
        this.forearmRelToUpperArm = Quaternion.Inverse(this.upperArm.solverRotation) * this.forearm.solverRotation;
      }

      public override void ApplyOffsets() => this.position += this.handPositionOffset;

      private void Stretching()
      {
        float num1 = this.upperArm.length + this.forearm.length;
        Vector3 zero1 = Vector3.zero;
        Vector3 zero2 = Vector3.zero;
        if ((double) this.armLengthMlp != 1.0)
        {
          num1 *= this.armLengthMlp;
          Vector3 vector3_1 = (this.forearm.solverPosition - this.upperArm.solverPosition) * (this.armLengthMlp - 1f);
          Vector3 vector3_2 = (this.hand.solverPosition - this.forearm.solverPosition) * (this.armLengthMlp - 1f);
          this.forearm.solverPosition += vector3_1;
          this.hand.solverPosition += vector3_1 + vector3_2;
        }
        float num2 = this.stretchCurve.Evaluate(Vector3.Distance(this.upperArm.solverPosition, this.position) / num1) * this.positionWeight;
        Vector3 vector3_3 = (this.forearm.solverPosition - this.upperArm.solverPosition) * num2;
        Vector3 vector3_4 = (this.hand.solverPosition - this.forearm.solverPosition) * num2;
        this.forearm.solverPosition += vector3_3;
        this.hand.solverPosition += vector3_3 + vector3_4;
      }

      public void Solve(bool isLeft)
      {
        this.chestRotation = Quaternion.LookRotation(this.rootRotation * this.chestForwardAxis, this.rootRotation * this.chestUpAxis);
        this.chestForward = this.chestRotation * Vector3.forward;
        this.chestUp = this.chestRotation * Vector3.up;
        if (this.hasShoulder && (double) this.shoulderRotationWeight > 0.0)
        {
          switch (this.shoulderRotationMode)
          {
            case IKSolverVR.Arm.ShoulderRotationMode.YawPitch:
              Vector3 vector3_1 = this.position - this.shoulder.solverPosition;
              vector3_1 = vector3_1.normalized;
              float num1 = isLeft ? 45f : -45f;
              Quaternion rotation1 = Quaternion.AngleAxis((isLeft ? -90f : 90f) + num1, this.chestUp) * this.chestRotation;
              Vector3 lhs = Quaternion.Inverse(rotation1) * vector3_1;
              float num2 = Mathf.Atan2(lhs.x, lhs.z) * 57.29578f * (1f - Mathf.Abs(Vector3.Dot(lhs, Vector3.up))) - num1;
              float num3 = isLeft ? -20f : -50f;
              float num4 = isLeft ? 50f : 20f;
              float angle1 = this.DamperValue(num2, num3 - num1, num4 - num1, 0.7f);
              Quaternion rotation2 = Quaternion.FromToRotation(this.shoulder.solverRotation * this.shoulder.axis, rotation1 * (Quaternion.AngleAxis(angle1, Vector3.up) * Vector3.forward));
              Quaternion quaternion1 = Quaternion.AngleAxis(isLeft ? -90f : 90f, this.chestUp) * this.chestRotation;
              Quaternion rotation3 = Quaternion.AngleAxis(isLeft ? -30f : 30f, this.chestForward) * quaternion1;
              vector3_1 = this.position - (this.shoulder.solverPosition + this.chestRotation * (isLeft ? Vector3.right : Vector3.left) * this.mag);
              Vector3 vector3_2 = Quaternion.Inverse(rotation3) * vector3_1;
              float num5 = this.DamperValue(Mathf.Atan2(vector3_2.y, vector3_2.z) * 57.29578f - -30f, -15f, 75f);
              Quaternion quaternion2 = Quaternion.AngleAxis(-num5, rotation3 * Vector3.right) * rotation2;
              if ((double) this.shoulderRotationWeight * (double) this.positionWeight < 1.0)
                quaternion2 = Quaternion.Lerp(Quaternion.identity, quaternion2, this.shoulderRotationWeight * this.positionWeight);
              IKSolverVR.VirtualBone.RotateBy(this.bones, quaternion2);
              this.Stretching();
              IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, 1, 2, 3, this.position, this.GetBendNormal(this.position - this.upperArm.solverPosition), this.positionWeight);
              float angle2 = Mathf.Clamp(num5 * 2f * this.positionWeight, 0.0f, 180f);
              this.shoulder.solverRotation = Quaternion.AngleAxis(angle2, this.shoulder.solverRotation * (isLeft ? this.shoulder.axis : -this.shoulder.axis)) * this.shoulder.solverRotation;
              this.upperArm.solverRotation = Quaternion.AngleAxis(angle2, this.upperArm.solverRotation * (isLeft ? this.upperArm.axis : -this.upperArm.axis)) * this.upperArm.solverRotation;
              break;
            case IKSolverVR.Arm.ShoulderRotationMode.FromTo:
              Quaternion solverRotation = this.shoulder.solverRotation;
              IKSolverVR.VirtualBone.RotateBy(this.bones, Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation((this.upperArm.solverPosition - this.shoulder.solverPosition).normalized + this.chestForward, this.position - this.shoulder.solverPosition), 0.5f * this.shoulderRotationWeight * this.positionWeight));
              this.Stretching();
              IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, 0, 2, 3, this.position, Vector3.Cross(this.forearm.solverPosition - this.shoulder.solverPosition, this.hand.solverPosition - this.shoulder.solverPosition), 0.5f * this.shoulderRotationWeight * this.positionWeight);
              IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, 1, 2, 3, this.position, this.GetBendNormal(this.position - this.upperArm.solverPosition), this.positionWeight);
              Quaternion quaternion3 = Quaternion.Inverse(Quaternion.LookRotation(this.chestUp, this.chestForward));
              Vector3 vector3_3 = quaternion3 * (solverRotation * this.shoulder.axis);
              Vector3 vector3_4 = quaternion3 * (this.shoulder.solverRotation * this.shoulder.axis);
              float num6 = Mathf.DeltaAngle(Mathf.Atan2(vector3_3.x, vector3_3.z) * 57.29578f, Mathf.Atan2(vector3_4.x, vector3_4.z) * 57.29578f);
              if (isLeft)
                num6 = -num6;
              float angle3 = Mathf.Clamp(num6 * 2f * this.positionWeight, 0.0f, 180f);
              this.shoulder.solverRotation = Quaternion.AngleAxis(angle3, this.shoulder.solverRotation * (isLeft ? this.shoulder.axis : -this.shoulder.axis)) * this.shoulder.solverRotation;
              this.upperArm.solverRotation = Quaternion.AngleAxis(angle3, this.upperArm.solverRotation * (isLeft ? this.upperArm.axis : -this.upperArm.axis)) * this.upperArm.solverRotation;
              break;
          }
        }
        else
        {
          this.Stretching();
          IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, 1, 2, 3, this.position, this.GetBendNormal(this.position - this.upperArm.solverPosition), this.positionWeight);
        }
        Quaternion quaternion4 = this.upperArm.solverRotation * this.forearmRelToUpperArm;
        this.RotateTo(this.forearm, Quaternion.FromToRotation(quaternion4 * this.forearm.axis, this.hand.solverPosition - this.forearm.solverPosition) * quaternion4, this.positionWeight);
        if ((double) this.rotationWeight >= 1.0)
        {
          this.hand.solverRotation = this.rotation;
        }
        else
        {
          if ((double) this.rotationWeight <= 0.0)
            return;
          this.hand.solverRotation = Quaternion.Lerp(this.hand.solverRotation, this.rotation, this.rotationWeight);
        }
      }

      public override void ResetOffsets() => this.handPositionOffset = Vector3.zero;

      public override void Write(ref Vector3[] solvedPositions, ref Quaternion[] solvedRotations)
      {
        if (this.hasShoulder)
        {
          solvedPositions[this.index] = this.shoulder.solverPosition;
          solvedRotations[this.index] = this.shoulder.solverRotation;
        }
        solvedPositions[this.index + 1] = this.upperArm.solverPosition;
        solvedPositions[this.index + 2] = this.forearm.solverPosition;
        solvedPositions[this.index + 3] = this.hand.solverPosition;
        solvedRotations[this.index + 1] = this.upperArm.solverRotation;
        solvedRotations[this.index + 2] = this.forearm.solverRotation;
        solvedRotations[this.index + 3] = this.hand.solverRotation;
      }

      private float DamperValue(float value, float min, float max, float weight = 1f)
      {
        float num1 = max - min;
        if ((double) weight < 1.0)
        {
          float num2 = max - num1 * 0.5f;
          float num3 = (value - num2) * 0.5f;
          value = num2 + num3;
        }
        value -= min;
        float t = Interp.Float(Mathf.Clamp(value / num1, 0.0f, 1f), InterpolationMode.InOutQuintic);
        return Mathf.Lerp(min, max, t);
      }

      private Vector3 GetBendNormal(Vector3 dir)
      {
        if ((UnityEngine.Object) this.bendGoal != (UnityEngine.Object) null)
          this.bendDirection = this.bendGoal.position - this.bones[1].solverPosition;
        Vector3 vector3_1 = this.bones[0].solverRotation * this.bones[0].axis;
        Vector3 vector3_2 = Quaternion.FromToRotation(Vector3.down, Quaternion.Inverse(this.chestRotation) * dir.normalized + Vector3.forward) * Vector3.back;
        Vector3 vector3_3 = this.chestRotation * (Quaternion.FromToRotation(Quaternion.Inverse(this.chestRotation) * vector3_1, Quaternion.Inverse(this.chestRotation) * dir) * vector3_2) + vector3_1 - this.rotation * this.wristToPalmAxis - this.rotation * this.palmToThumbAxis * 0.5f;
        if ((double) this.bendGoalWeight > 0.0)
          vector3_3 = Vector3.Slerp(vector3_3, this.bendDirection, this.bendGoalWeight);
        if ((double) this.swivelOffset != 0.0)
          vector3_3 = Quaternion.AngleAxis(this.swivelOffset, -dir) * vector3_3;
        return Vector3.Cross(vector3_3, dir);
      }

      private void Visualize(
        IKSolverVR.VirtualBone bone1,
        IKSolverVR.VirtualBone bone2,
        IKSolverVR.VirtualBone bone3,
        Color color)
      {
        Debug.DrawLine(bone1.solverPosition, bone2.solverPosition, color);
        Debug.DrawLine(bone2.solverPosition, bone3.solverPosition, color);
      }

      [Serializable]
      public enum ShoulderRotationMode
      {
        YawPitch,
        FromTo,
      }
    }

    [Serializable]
    public abstract class BodyPart
    {
      [HideInInspector]
      public IKSolverVR.VirtualBone[] bones = new IKSolverVR.VirtualBone[0];
      protected bool initiated;
      protected Vector3 rootPosition;
      protected Quaternion rootRotation = Quaternion.identity;
      protected int index = -1;

      protected abstract void OnRead(
        Vector3[] positions,
        Quaternion[] rotations,
        bool hasChest,
        bool hasNeck,
        bool hasShoulders,
        bool hasToes,
        int rootIndex,
        int index);

      public abstract void PreSolve();

      public abstract void Write(ref Vector3[] solvedPositions, ref Quaternion[] solvedRotations);

      public abstract void ApplyOffsets();

      public abstract void ResetOffsets();

      public float sqrMag { get; private set; }

      public float mag { get; private set; }

      public void Read(
        Vector3[] positions,
        Quaternion[] rotations,
        bool hasChest,
        bool hasNeck,
        bool hasShoulders,
        bool hasToes,
        int rootIndex,
        int index)
      {
        this.index = index;
        this.rootPosition = positions[rootIndex];
        this.rootRotation = rotations[rootIndex];
        this.OnRead(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, rootIndex, index);
        this.mag = IKSolverVR.VirtualBone.PreSolve(ref this.bones);
        this.sqrMag = this.mag * this.mag;
        this.initiated = true;
      }

      public void MovePosition(Vector3 position)
      {
        Vector3 vector3 = position - this.bones[0].solverPosition;
        foreach (IKSolverVR.VirtualBone bone in this.bones)
          bone.solverPosition += vector3;
      }

      public void MoveRotation(Quaternion rotation)
      {
        IKSolverVR.VirtualBone.RotateAroundPoint(this.bones, 0, this.bones[0].solverPosition, QuaTools.FromToRotation(this.bones[0].solverRotation, rotation));
      }

      public void Translate(Vector3 position, Quaternion rotation)
      {
        this.MovePosition(position);
        this.MoveRotation(rotation);
      }

      public void TranslateRoot(Vector3 newRootPos, Quaternion newRootRot)
      {
        Vector3 vector3 = newRootPos - this.rootPosition;
        this.rootPosition = newRootPos;
        foreach (IKSolverVR.VirtualBone bone in this.bones)
          bone.solverPosition += vector3;
        Quaternion rotation = QuaTools.FromToRotation(this.rootRotation, newRootRot);
        this.rootRotation = newRootRot;
        IKSolverVR.VirtualBone.RotateAroundPoint(this.bones, 0, newRootPos, rotation);
      }

      public void RotateTo(IKSolverVR.VirtualBone bone, Quaternion rotation, float weight = 1f)
      {
        if ((double) weight <= 0.0)
          return;
        Quaternion quaternion = QuaTools.FromToRotation(bone.solverRotation, rotation);
        if ((double) weight < 1.0)
          quaternion = Quaternion.Slerp(Quaternion.identity, quaternion, weight);
        for (int index = 0; index < this.bones.Length; ++index)
        {
          if (this.bones[index] == bone)
          {
            IKSolverVR.VirtualBone.RotateAroundPoint(this.bones, index, this.bones[index].solverPosition, quaternion);
            break;
          }
        }
      }

      public void Visualize(Color color)
      {
        for (int index = 0; index < this.bones.Length - 1; ++index)
          Debug.DrawLine(this.bones[index].solverPosition, this.bones[index + 1].solverPosition, color);
      }

      public void Visualize() => this.Visualize(Color.white);
    }

    [Serializable]
    public class Footstep
    {
      public float stepSpeed = 3f;
      public Vector3 characterSpaceOffset;
      public Vector3 position;
      public Quaternion rotation = Quaternion.identity;
      public Quaternion stepToRootRot = Quaternion.identity;
      public bool isSupportLeg;
      public Vector3 stepFrom;
      public Vector3 stepTo;
      public Quaternion stepFromRot = Quaternion.identity;
      public Quaternion stepToRot = Quaternion.identity;
      private Quaternion footRelativeToRoot = Quaternion.identity;
      private float supportLegW;
      private float supportLegWV;

      public bool isStepping => (double) this.stepProgress < 1.0;

      public float stepProgress { get; private set; }

      public Footstep(
        Quaternion rootRotation,
        Vector3 footPosition,
        Quaternion footRotation,
        Vector3 characterSpaceOffset)
      {
        this.characterSpaceOffset = characterSpaceOffset;
        this.Reset(rootRotation, footPosition, footRotation);
      }

      public void Reset(Quaternion rootRotation, Vector3 footPosition, Quaternion footRotation)
      {
        this.position = footPosition;
        this.rotation = footRotation;
        this.stepFrom = this.position;
        this.stepTo = this.position;
        this.stepFromRot = this.rotation;
        this.stepToRot = this.rotation;
        this.stepToRootRot = rootRotation;
        this.stepProgress = 1f;
        this.footRelativeToRoot = Quaternion.Inverse(rootRotation) * this.rotation;
      }

      public void StepTo(Vector3 p, Quaternion rootRotation)
      {
        this.stepFrom = this.position;
        this.stepTo = p;
        this.stepFromRot = this.rotation;
        this.stepToRootRot = rootRotation;
        this.stepToRot = rootRotation * this.footRelativeToRoot;
        this.stepProgress = 0.0f;
      }

      public void UpdateStepping(Vector3 p, Quaternion rootRotation, float speed)
      {
        this.stepTo = Vector3.Lerp(this.stepTo, p, Time.deltaTime * speed);
        this.stepToRot = Quaternion.Lerp(this.stepToRot, rootRotation * this.footRelativeToRoot, Time.deltaTime * speed);
        this.stepToRootRot = this.stepToRot * Quaternion.Inverse(this.footRelativeToRoot);
      }

      public void UpdateStanding(Quaternion rootRotation, float minAngle, float speed)
      {
        if ((double) speed <= 0.0 || (double) minAngle >= 180.0)
          return;
        Quaternion quaternion = rootRotation * this.footRelativeToRoot;
        float num = Quaternion.Angle(this.rotation, quaternion);
        if ((double) num <= (double) minAngle)
          return;
        this.rotation = Quaternion.RotateTowards(this.rotation, quaternion, Mathf.Min((float) ((double) Time.deltaTime * (double) speed * (1.0 - (double) this.supportLegW)), num - minAngle));
      }

      public void Update(InterpolationMode interpolation, UnityEvent onStep)
      {
        this.supportLegW = Mathf.SmoothDamp(this.supportLegW, this.isSupportLeg ? 1f : 0.0f, ref this.supportLegWV, 0.2f);
        if (!this.isStepping)
          return;
        this.stepProgress = Mathf.MoveTowards(this.stepProgress, 1f, Time.deltaTime * this.stepSpeed);
        if ((double) this.stepProgress >= 1.0)
          onStep.Invoke();
        float t = Interp.Float(this.stepProgress, interpolation);
        this.position = Vector3.Lerp(this.stepFrom, this.stepTo, t);
        this.rotation = Quaternion.Lerp(this.stepFromRot, this.stepToRot, t);
      }
    }

    [Serializable]
    public class Leg : IKSolverVR.BodyPart
    {
      [Tooltip("The toe/foot target.")]
      public Transform target;
      [Tooltip("The knee will be bent towards this Transform if 'Bend Goal Weight' > 0.")]
      public Transform bendGoal;
      [Tooltip("Positional weight of the toe/foot target.")]
      [Range(0.0f, 1f)]
      public float positionWeight;
      [Tooltip("Rotational weight of the toe/foot target.")]
      [Range(0.0f, 1f)]
      public float rotationWeight;
      [Tooltip("If greater than 0, will bend the knee towards the 'Bend Goal' Transform.")]
      [Range(0.0f, 1f)]
      public float bendGoalWeight;
      [Tooltip("Angular offset of the knee bending direction.")]
      [Range(-180f, 180f)]
      public float swivelOffset;
      [HideInInspector]
      [NonSerialized]
      public Vector3 IKPosition;
      [HideInInspector]
      [NonSerialized]
      public Quaternion IKRotation = Quaternion.identity;
      [HideInInspector]
      [NonSerialized]
      public Vector3 footPositionOffset;
      [HideInInspector]
      [NonSerialized]
      public Vector3 heelPositionOffset;
      [HideInInspector]
      [NonSerialized]
      public Quaternion footRotationOffset = Quaternion.identity;
      [HideInInspector]
      [NonSerialized]
      public float currentMag;
      private Vector3 footPosition;
      private Quaternion footRotation = Quaternion.identity;
      private Vector3 bendNormal;
      private Quaternion calfRelToThigh = Quaternion.identity;

      public Vector3 position { get; private set; }

      public Quaternion rotation { get; private set; }

      public bool hasToes { get; private set; }

      public IKSolverVR.VirtualBone thigh => this.bones[0];

      private IKSolverVR.VirtualBone calf => this.bones[1];

      private IKSolverVR.VirtualBone foot => this.bones[2];

      private IKSolverVR.VirtualBone toes => this.bones[3];

      public IKSolverVR.VirtualBone lastBone => this.bones[this.bones.Length - 1];

      public Vector3 thighRelativeToPelvis { get; private set; }

      protected override void OnRead(
        Vector3[] positions,
        Quaternion[] rotations,
        bool hasChest,
        bool hasNeck,
        bool hasShoulders,
        bool hasToes,
        int rootIndex,
        int index)
      {
        Vector3 position1 = positions[index];
        Quaternion rotation1 = rotations[index];
        Vector3 position2 = positions[index + 1];
        Quaternion rotation2 = rotations[index + 1];
        Vector3 position3 = positions[index + 2];
        Quaternion rotation3 = rotations[index + 2];
        Vector3 position4 = positions[index + 3];
        Quaternion rotation4 = rotations[index + 3];
        if (!this.initiated)
        {
          this.hasToes = hasToes;
          this.bones = new IKSolverVR.VirtualBone[hasToes ? 4 : 3];
          if (hasToes)
          {
            this.bones[0] = new IKSolverVR.VirtualBone(position1, rotation1);
            this.bones[1] = new IKSolverVR.VirtualBone(position2, rotation2);
            this.bones[2] = new IKSolverVR.VirtualBone(position3, rotation3);
            this.bones[3] = new IKSolverVR.VirtualBone(position4, rotation4);
            this.IKPosition = position4;
            this.IKRotation = rotation4;
          }
          else
          {
            this.bones[0] = new IKSolverVR.VirtualBone(position1, rotation1);
            this.bones[1] = new IKSolverVR.VirtualBone(position2, rotation2);
            this.bones[2] = new IKSolverVR.VirtualBone(position3, rotation3);
            this.IKPosition = position3;
            this.IKRotation = rotation3;
          }
          this.rotation = this.IKRotation;
        }
        if (hasToes)
        {
          this.bones[0].Read(position1, rotation1);
          this.bones[1].Read(position2, rotation2);
          this.bones[2].Read(position3, rotation3);
          this.bones[3].Read(position4, rotation4);
        }
        else
        {
          this.bones[0].Read(position1, rotation1);
          this.bones[1].Read(position2, rotation2);
          this.bones[2].Read(position3, rotation3);
        }
      }

      public override void PreSolve()
      {
        if ((UnityEngine.Object) this.target != (UnityEngine.Object) null)
        {
          this.IKPosition = this.target.position;
          this.IKRotation = this.target.rotation;
        }
        this.footPosition = this.foot.solverPosition;
        this.footRotation = this.foot.solverRotation;
        this.position = this.lastBone.solverPosition;
        this.rotation = this.lastBone.solverRotation;
        if ((double) this.rotationWeight > 0.0)
          this.ApplyRotationOffset(QuaTools.FromToRotation(this.rotation, this.IKRotation), this.rotationWeight);
        if ((double) this.positionWeight > 0.0)
          this.ApplyPositionOffset(this.IKPosition - this.position, this.positionWeight);
        this.thighRelativeToPelvis = Quaternion.Inverse(this.rootRotation) * (this.thigh.solverPosition - this.rootPosition);
        this.calfRelToThigh = Quaternion.Inverse(this.thigh.solverRotation) * this.calf.solverRotation;
        this.bendNormal = Vector3.Cross(this.calf.solverPosition - this.thigh.solverPosition, this.foot.solverPosition - this.calf.solverPosition);
      }

      public override void ApplyOffsets()
      {
        this.ApplyPositionOffset(this.footPositionOffset, 1f);
        this.ApplyRotationOffset(this.footRotationOffset, 1f);
        Quaternion rotation = Quaternion.FromToRotation(this.footPosition - this.position, this.footPosition + this.heelPositionOffset - this.position);
        this.footPosition = this.position + rotation * (this.footPosition - this.position);
        this.footRotation = rotation * this.footRotation;
        float num = 0.0f;
        if ((UnityEngine.Object) this.bendGoal != (UnityEngine.Object) null && (double) this.bendGoalWeight > 0.0)
        {
          Vector3 vector3_1 = Vector3.Cross(this.bendGoal.position - this.thigh.solverPosition, this.position - this.thigh.solverPosition);
          Vector3 vector3_2 = Quaternion.Inverse(Quaternion.LookRotation(this.bendNormal, this.thigh.solverPosition - this.foot.solverPosition)) * vector3_1;
          num = Mathf.Atan2(vector3_2.x, vector3_2.z) * 57.29578f * this.bendGoalWeight;
        }
        float angle = this.swivelOffset + num;
        if ((double) angle == 0.0)
          return;
        this.bendNormal = Quaternion.AngleAxis(angle, this.thigh.solverPosition - this.lastBone.solverPosition) * this.bendNormal;
        this.thigh.solverRotation = Quaternion.AngleAxis(-angle, this.thigh.solverRotation * this.thigh.axis) * this.thigh.solverRotation;
      }

      private void ApplyPositionOffset(Vector3 offset, float weight)
      {
        if ((double) weight <= 0.0)
          return;
        offset *= weight;
        this.footPosition += offset;
        this.position += offset;
      }

      private void ApplyRotationOffset(Quaternion offset, float weight)
      {
        if ((double) weight <= 0.0)
          return;
        if ((double) weight < 1.0)
          offset = Quaternion.Lerp(Quaternion.identity, offset, weight);
        this.footRotation = offset * this.footRotation;
        this.rotation = offset * this.rotation;
        this.bendNormal = offset * this.bendNormal;
        this.footPosition = this.position + offset * (this.footPosition - this.position);
      }

      public void Solve()
      {
        IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, 0, 1, 2, this.footPosition, this.bendNormal, 1f);
        this.RotateTo(this.foot, this.footRotation);
        if (!this.hasToes)
          return;
        IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, 0, 2, 3, this.position, Vector3.Cross(this.foot.solverPosition - this.thigh.solverPosition, this.toes.solverPosition - this.foot.solverPosition), 1f);
        Quaternion quaternion = this.thigh.solverRotation * this.calfRelToThigh;
        this.calf.solverRotation = Quaternion.FromToRotation(quaternion * this.calf.axis, this.foot.solverPosition - this.calf.solverPosition) * quaternion;
        this.toes.solverRotation = this.rotation;
      }

      public override void Write(ref Vector3[] solvedPositions, ref Quaternion[] solvedRotations)
      {
        solvedRotations[this.index] = this.thigh.solverRotation;
        solvedRotations[this.index + 1] = this.calf.solverRotation;
        solvedRotations[this.index + 2] = this.foot.solverRotation;
        if (!this.hasToes)
          return;
        solvedRotations[this.index + 3] = this.toes.solverRotation;
      }

      public override void ResetOffsets()
      {
        this.footPositionOffset = Vector3.zero;
        this.footRotationOffset = Quaternion.identity;
        this.heelPositionOffset = Vector3.zero;
      }
    }

    [Serializable]
    public class Locomotion
    {
      [Tooltip("Used for blending in/out of procedural locomotion.")]
      [Range(0.0f, 1f)]
      public float weight = 1f;
      [Tooltip("Tries to maintain this distance between the legs.")]
      public float footDistance = 0.3f;
      [Tooltip("Makes a step only if step target position is at least this far from the current footstep or the foot does not reach the current footstep anymore or footstep angle is past the 'Angle Threshold'.")]
      public float stepThreshold = 0.4f;
      [Tooltip("Makes a step only if step target position is at least 'Step Threshold' far from the current footstep or the foot does not reach the current footstep anymore or footstep angle is past this value.")]
      public float angleThreshold = 60f;
      [Tooltip("Multiplies angle of the center of mass - center of pressure vector. Larger value makes the character step sooner if losing balance.")]
      public float comAngleMlp = 1f;
      [Tooltip("Maximum magnitude of head/hand target velocity used in prediction.")]
      public float maxVelocity = 0.4f;
      [Tooltip("The amount of head/hand target velocity prediction.")]
      public float velocityFactor = 0.4f;
      [Tooltip("How much can a leg be extended before it is forced to step to another position? 1 means fully stretched.")]
      [Range(0.9f, 1f)]
      public float maxLegStretch = 1f;
      [Tooltip("The speed of lerping the root of the character towards the horizontal mid-point of the footsteps.")]
      public float rootSpeed = 20f;
      [Tooltip("The speed of steps.")]
      public float stepSpeed = 3f;
      [Tooltip("The height of the foot by normalized step progress (0 - 1).")]
      public AnimationCurve stepHeight;
      [Tooltip("The height offset of the heel by normalized step progress (0 - 1).")]
      public AnimationCurve heelHeight;
      [Tooltip("Rotates the foot while the leg is not stepping to relax the twist rotation of the leg if ideal rotation is past this angle.")]
      [Range(0.0f, 180f)]
      public float relaxLegTwistMinAngle = 20f;
      [Tooltip("The speed of rotating the foot while the leg is not stepping to relax the twist rotation of the leg.")]
      public float relaxLegTwistSpeed = 400f;
      [Tooltip("Interpolation mode of the step.")]
      public InterpolationMode stepInterpolation = InterpolationMode.InOutSine;
      [Tooltip("Offset for the approximated center of mass.")]
      public Vector3 offset;
      [HideInInspector]
      public bool blockingEnabled;
      [HideInInspector]
      public LayerMask blockingLayers;
      [HideInInspector]
      public float raycastRadius = 0.2f;
      [HideInInspector]
      public float raycastHeight = 0.2f;
      [Tooltip("Called when the left foot has finished a step.")]
      public UnityEvent onLeftFootstep = new UnityEvent();
      [Tooltip("Called when the right foot has finished a step")]
      public UnityEvent onRightFootstep = new UnityEvent();
      private IKSolverVR.Footstep[] footsteps = new IKSolverVR.Footstep[0];
      private Vector3 lastComPosition;
      private Vector3 comVelocity;
      private int leftFootIndex;
      private int rightFootIndex;

      public Vector3 centerOfMass { get; private set; }

      public void Initiate(Vector3[] positions, Quaternion[] rotations, bool hasToes)
      {
        this.leftFootIndex = hasToes ? 17 : 16;
        this.rightFootIndex = hasToes ? 21 : 20;
        this.footsteps = new IKSolverVR.Footstep[2]
        {
          new IKSolverVR.Footstep(rotations[0], positions[this.leftFootIndex], rotations[this.leftFootIndex], this.footDistance * Vector3.left),
          new IKSolverVR.Footstep(rotations[0], positions[this.rightFootIndex], rotations[this.rightFootIndex], this.footDistance * Vector3.right)
        };
      }

      public void Reset(Vector3[] positions, Quaternion[] rotations)
      {
        this.lastComPosition = Vector3.Lerp(positions[1], positions[5], 0.25f) + rotations[0] * this.offset;
        this.comVelocity = Vector3.zero;
        this.footsteps[0].Reset(rotations[0], positions[this.leftFootIndex], rotations[this.leftFootIndex]);
        this.footsteps[1].Reset(rotations[0], positions[this.rightFootIndex], rotations[this.rightFootIndex]);
      }

      public void AddDeltaRotation(Quaternion delta, Vector3 pivot)
      {
        Vector3 vector3_1 = this.lastComPosition - pivot;
        this.lastComPosition = pivot + delta * vector3_1;
        foreach (IKSolverVR.Footstep footstep in this.footsteps)
        {
          footstep.rotation = delta * footstep.rotation;
          footstep.stepFromRot = delta * footstep.stepFromRot;
          footstep.stepToRot = delta * footstep.stepToRot;
          footstep.stepToRootRot = delta * footstep.stepToRootRot;
          Vector3 vector3_2 = footstep.position - pivot;
          footstep.position = pivot + delta * vector3_2;
          Vector3 vector3_3 = footstep.stepFrom - pivot;
          footstep.stepFrom = pivot + delta * vector3_3;
          Vector3 vector3_4 = footstep.stepTo - pivot;
          footstep.stepTo = pivot + delta * vector3_4;
        }
      }

      public void AddDeltaPosition(Vector3 delta)
      {
        this.lastComPosition += delta;
        foreach (IKSolverVR.Footstep footstep in this.footsteps)
        {
          footstep.position += delta;
          footstep.stepFrom += delta;
          footstep.stepTo += delta;
        }
      }

      public void Solve(
        IKSolverVR.VirtualBone rootBone,
        IKSolverVR.Spine spine,
        IKSolverVR.Leg leftLeg,
        IKSolverVR.Leg rightLeg,
        IKSolverVR.Arm leftArm,
        IKSolverVR.Arm rightArm,
        int supportLegIndex,
        out Vector3 leftFootPosition,
        out Vector3 rightFootPosition,
        out Quaternion leftFootRotation,
        out Quaternion rightFootRotation,
        out float leftFootOffset,
        out float rightFootOffset,
        out float leftHeelOffset,
        out float rightHeelOffset)
      {
        if ((double) this.weight <= 0.0)
        {
          leftFootPosition = Vector3.zero;
          rightFootPosition = Vector3.zero;
          leftFootRotation = Quaternion.identity;
          rightFootRotation = Quaternion.identity;
          leftFootOffset = 0.0f;
          rightFootOffset = 0.0f;
          leftHeelOffset = 0.0f;
          rightHeelOffset = 0.0f;
        }
        else
        {
          Vector3 vector3_1 = rootBone.solverRotation * Vector3.up;
          Vector3 vector3_2 = spine.pelvis.solverPosition + spine.pelvis.solverRotation * leftLeg.thighRelativeToPelvis;
          Vector3 vector3_3 = spine.pelvis.solverPosition + spine.pelvis.solverRotation * rightLeg.thighRelativeToPelvis;
          this.footsteps[0].characterSpaceOffset = this.footDistance * Vector3.left;
          this.footsteps[1].characterSpaceOffset = this.footDistance * Vector3.right;
          Vector3 faceDirection = spine.faceDirection;
          Vector3 vertical = V3Tools.ExtractVertical(faceDirection, vector3_1, 1f);
          Quaternion quaternion = Quaternion.LookRotation(faceDirection - vertical, vector3_1);
          float num1 = 1f;
          float num2 = 1f;
          float num3 = 0.2f;
          float num4 = (float) ((double) num1 + (double) num2 + 2.0 * (double) num3);
          this.centerOfMass = Vector3.zero;
          this.centerOfMass += spine.pelvis.solverPosition * num1;
          this.centerOfMass += spine.head.solverPosition * num2;
          this.centerOfMass += leftArm.position * num3;
          this.centerOfMass += rightArm.position * num3;
          this.centerOfMass /= num4;
          this.centerOfMass += rootBone.solverRotation * this.offset;
          this.comVelocity = (double) Time.deltaTime > 0.0 ? (this.centerOfMass - this.lastComPosition) / Time.deltaTime : Vector3.zero;
          this.lastComPosition = this.centerOfMass;
          this.comVelocity = Vector3.ClampMagnitude(this.comVelocity, this.maxVelocity) * this.velocityFactor;
          Vector3 point = this.centerOfMass + this.comVelocity;
          Vector3 plane1 = V3Tools.PointToPlane(spine.pelvis.solverPosition, rootBone.solverPosition, vector3_1);
          Vector3 plane2 = V3Tools.PointToPlane(point, rootBone.solverPosition, vector3_1);
          Vector3 vector3_4 = Vector3.Lerp(this.footsteps[0].position, this.footsteps[1].position, 0.5f);
          float num5 = Vector3.Angle(point - vector3_4, rootBone.solverRotation * Vector3.up) * this.comAngleMlp;
          for (int index = 0; index < this.footsteps.Length; ++index)
            this.footsteps[index].isSupportLeg = supportLegIndex == index;
          for (int index = 0; index < this.footsteps.Length; ++index)
          {
            if (this.footsteps[index].isStepping)
            {
              Vector3 vector3_5 = plane2 + rootBone.solverRotation * this.footsteps[index].characterSpaceOffset;
              if (!this.StepBlocked(this.footsteps[index].stepFrom, vector3_5, rootBone.solverPosition))
                this.footsteps[index].UpdateStepping(vector3_5, quaternion, 10f);
            }
            else
              this.footsteps[index].UpdateStanding(quaternion, this.relaxLegTwistMinAngle, this.relaxLegTwistSpeed);
          }
          if (this.CanStep())
          {
            int index1 = -1;
            float num6 = float.NegativeInfinity;
            for (int index2 = 0; index2 < this.footsteps.Length; ++index2)
            {
              if (!this.footsteps[index2].isStepping)
              {
                Vector3 vector3_6 = plane2 + rootBone.solverRotation * this.footsteps[index2].characterSpaceOffset;
                float num7 = index2 == 0 ? leftLeg.mag : rightLeg.mag;
                Vector3 b = index2 == 0 ? vector3_2 : vector3_3;
                float num8 = Vector3.Distance(this.footsteps[index2].position, b);
                bool flag1 = false;
                if ((double) num8 >= (double) num7 * (double) this.maxLegStretch)
                {
                  vector3_6 = plane1 + rootBone.solverRotation * this.footsteps[index2].characterSpaceOffset;
                  flag1 = true;
                }
                bool flag2 = false;
                for (int index3 = 0; index3 < this.footsteps.Length; ++index3)
                {
                  if (index3 != index2 && !flag1)
                  {
                    if ((double) Vector3.Distance(this.footsteps[index2].position, this.footsteps[index3].position) >= 0.25 || (double) (this.footsteps[index2].position - vector3_6).sqrMagnitude >= (double) (this.footsteps[index3].position - vector3_6).sqrMagnitude)
                      flag2 = IKSolverVR.Locomotion.GetLineSphereCollision(this.footsteps[index2].position, vector3_6, this.footsteps[index3].position, 0.25f);
                    if (flag2)
                      break;
                  }
                }
                float num9 = Quaternion.Angle(quaternion, this.footsteps[index2].stepToRootRot);
                if (!flag2 || (double) num9 > (double) this.angleThreshold)
                {
                  float num10 = Vector3.Distance(this.footsteps[index2].position, vector3_6);
                  float num11 = Mathf.Lerp(this.stepThreshold, this.stepThreshold * 0.1f, num5 * 0.015f);
                  if (flag1)
                    num11 *= 0.5f;
                  if (index2 == 0)
                    num11 *= 0.9f;
                  if (!this.StepBlocked(this.footsteps[index2].position, vector3_6, rootBone.solverPosition) && ((double) num10 > (double) num11 || (double) num9 > (double) this.angleThreshold))
                  {
                    float num12 = 0.0f - num10;
                    if ((double) num12 > (double) num6)
                    {
                      index1 = index2;
                      num6 = num12;
                    }
                  }
                }
              }
            }
            if (index1 != -1)
            {
              Vector3 p = plane2 + rootBone.solverRotation * this.footsteps[index1].characterSpaceOffset;
              this.footsteps[index1].stepSpeed = UnityEngine.Random.Range(this.stepSpeed, this.stepSpeed * 1.5f);
              this.footsteps[index1].StepTo(p, quaternion);
            }
          }
          this.footsteps[0].Update(this.stepInterpolation, this.onLeftFootstep);
          this.footsteps[1].Update(this.stepInterpolation, this.onRightFootstep);
          leftFootPosition = this.footsteps[0].position;
          rightFootPosition = this.footsteps[1].position;
          leftFootPosition = V3Tools.PointToPlane(leftFootPosition, leftLeg.lastBone.readPosition, vector3_1);
          rightFootPosition = V3Tools.PointToPlane(rightFootPosition, rightLeg.lastBone.readPosition, vector3_1);
          leftFootOffset = this.stepHeight.Evaluate(this.footsteps[0].stepProgress);
          rightFootOffset = this.stepHeight.Evaluate(this.footsteps[1].stepProgress);
          leftHeelOffset = this.heelHeight.Evaluate(this.footsteps[0].stepProgress);
          rightHeelOffset = this.heelHeight.Evaluate(this.footsteps[1].stepProgress);
          leftFootRotation = this.footsteps[0].rotation;
          rightFootRotation = this.footsteps[1].rotation;
        }
      }

      public Vector3 leftFootstepPosition => this.footsteps[0].position;

      public Vector3 rightFootstepPosition => this.footsteps[1].position;

      public Quaternion leftFootstepRotation => this.footsteps[0].rotation;

      public Quaternion rightFootstepRotation => this.footsteps[1].rotation;

      private bool StepBlocked(Vector3 fromPosition, Vector3 toPosition, Vector3 rootPosition)
      {
        if ((int) this.blockingLayers == -1 || !this.blockingEnabled)
          return false;
        Vector3 origin = fromPosition with
        {
          y = rootPosition.y + this.raycastHeight + this.raycastRadius
        };
        Vector3 direction = (toPosition - origin) with
        {
          y = 0.0f
        };
        RaycastHit hitInfo;
        return (double) this.raycastRadius <= 0.0 ? Physics.Raycast(origin, direction, out hitInfo, direction.magnitude, (int) this.blockingLayers) : Physics.SphereCast(origin, this.raycastRadius, direction, out hitInfo, direction.magnitude, (int) this.blockingLayers);
      }

      private bool CanStep()
      {
        foreach (IKSolverVR.Footstep footstep in this.footsteps)
        {
          if (footstep.isStepping && (double) footstep.stepProgress < 0.800000011920929)
            return false;
        }
        return true;
      }

      private static bool GetLineSphereCollision(
        Vector3 lineStart,
        Vector3 lineEnd,
        Vector3 sphereCenter,
        float sphereRadius)
      {
        Vector3 forward = lineEnd - lineStart;
        Vector3 upwards = sphereCenter - lineStart;
        float num = upwards.magnitude - sphereRadius;
        if ((double) num > (double) forward.magnitude)
          return false;
        Vector3 vector3 = Quaternion.Inverse(Quaternion.LookRotation(forward, upwards)) * upwards;
        return (double) vector3.z < 0.0 ? (double) num < 0.0 : (double) vector3.y - (double) sphereRadius < 0.0;
      }
    }

    [Serializable]
    public class Spine : IKSolverVR.BodyPart
    {
      [Tooltip("The head target.")]
      public Transform headTarget;
      [Tooltip("The pelvis target, useful with seated rigs.")]
      public Transform pelvisTarget;
      [Tooltip("Positional weight of the head target.")]
      [Range(0.0f, 1f)]
      public float positionWeight = 1f;
      [Tooltip("Rotational weight of the head target.")]
      [Range(0.0f, 1f)]
      public float rotationWeight = 1f;
      [Tooltip("Positional weight of the pelvis target.")]
      [Range(0.0f, 1f)]
      public float pelvisPositionWeight;
      [Tooltip("Rotational weight of the pelvis target.")]
      [Range(0.0f, 1f)]
      public float pelvisRotationWeight;
      [Tooltip("If 'Chest Goal Weight' is greater than 0, the chest will be turned towards this Transform.")]
      public Transform chestGoal;
      [Tooltip("Rotational weight of the chest target.")]
      [Range(0.0f, 1f)]
      public float chestGoalWeight;
      [Tooltip("Minimum height of the head from the root of the character.")]
      public float minHeadHeight = 0.8f;
      [Tooltip("Determines how much the body will follow the position of the head.")]
      [Range(0.0f, 1f)]
      public float bodyPosStiffness = 0.55f;
      [Tooltip("Determines how much the body will follow the rotation of the head.")]
      [Range(0.0f, 1f)]
      public float bodyRotStiffness = 0.1f;
      [Tooltip("Determines how much the chest will rotate to the rotation of the head.")]
      [FormerlySerializedAs("chestRotationWeight")]
      [Range(0.0f, 1f)]
      public float neckStiffness = 0.2f;
      [Tooltip("The amount of rotation applied to the chest based on hand positions.")]
      [Range(0.0f, 1f)]
      public float rotateChestByHands = 1f;
      [Tooltip("Clamps chest rotation.")]
      [Range(0.0f, 1f)]
      public float chestClampWeight = 0.5f;
      [Tooltip("Clamps head rotation.")]
      [Range(0.0f, 1f)]
      public float headClampWeight = 0.6f;
      [Tooltip("Moves the body horizontally along -character.forward axis by that value when the player is crouching.")]
      public float moveBodyBackWhenCrouching = 0.5f;
      [Tooltip("How much will the pelvis maintain it's animated position?")]
      [Range(0.0f, 1f)]
      public float maintainPelvisPosition = 0.2f;
      [Tooltip("Will automatically rotate the root of the character if the head target has turned past this angle.")]
      [Range(0.0f, 180f)]
      public float maxRootAngle = 25f;
      [HideInInspector]
      [NonSerialized]
      public Vector3 IKPositionHead;
      [HideInInspector]
      [NonSerialized]
      public Quaternion IKRotationHead = Quaternion.identity;
      [HideInInspector]
      [NonSerialized]
      public Vector3 IKPositionPelvis;
      [HideInInspector]
      [NonSerialized]
      public Quaternion IKRotationPelvis = Quaternion.identity;
      [HideInInspector]
      [NonSerialized]
      public Vector3 goalPositionChest;
      [HideInInspector]
      [NonSerialized]
      public Vector3 pelvisPositionOffset;
      [HideInInspector]
      [NonSerialized]
      public Vector3 chestPositionOffset;
      [HideInInspector]
      [NonSerialized]
      public Vector3 headPositionOffset;
      [HideInInspector]
      [NonSerialized]
      public Quaternion pelvisRotationOffset = Quaternion.identity;
      [HideInInspector]
      [NonSerialized]
      public Quaternion chestRotationOffset = Quaternion.identity;
      [HideInInspector]
      [NonSerialized]
      public Quaternion headRotationOffset = Quaternion.identity;
      [HideInInspector]
      [NonSerialized]
      public Vector3 faceDirection;
      [HideInInspector]
      [NonSerialized]
      public Vector3 locomotionHeadPositionOffset;
      [HideInInspector]
      [NonSerialized]
      public Vector3 headPosition;
      private Quaternion headRotation = Quaternion.identity;
      private Quaternion anchorRelativeToHead = Quaternion.identity;
      private Quaternion pelvisRelativeRotation = Quaternion.identity;
      private Quaternion chestRelativeRotation = Quaternion.identity;
      private Vector3 headDeltaPosition;
      private Quaternion pelvisDeltaRotation = Quaternion.identity;
      private Quaternion chestTargetRotation = Quaternion.identity;
      private int pelvisIndex = 0;
      private int spineIndex = 1;
      private int chestIndex = -1;
      private int neckIndex = -1;
      private int headIndex = -1;
      private float length;
      private bool hasChest;
      private bool hasNeck;
      private float headHeight;
      private float sizeMlp;
      private Vector3 chestForward;

      public IKSolverVR.VirtualBone pelvis => this.bones[this.pelvisIndex];

      public IKSolverVR.VirtualBone firstSpineBone => this.bones[this.spineIndex];

      public IKSolverVR.VirtualBone chest
      {
        get => this.hasChest ? this.bones[this.chestIndex] : this.bones[this.spineIndex];
      }

      private IKSolverVR.VirtualBone neck => this.bones[this.neckIndex];

      public IKSolverVR.VirtualBone head => this.bones[this.headIndex];

      public Quaternion anchorRotation { get; private set; }

      protected override void OnRead(
        Vector3[] positions,
        Quaternion[] rotations,
        bool hasChest,
        bool hasNeck,
        bool hasShoulders,
        bool hasToes,
        int rootIndex,
        int index)
      {
        Vector3 position1 = positions[index];
        Quaternion rotation1 = rotations[index];
        Vector3 position2 = positions[index + 1];
        Quaternion rotation2 = rotations[index + 1];
        Vector3 position3 = positions[index + 2];
        Quaternion rotation3 = rotations[index + 2];
        Vector3 position4 = positions[index + 3];
        Quaternion rotation4 = rotations[index + 3];
        Vector3 position5 = positions[index + 4];
        Quaternion rotation5 = rotations[index + 4];
        if (!hasChest)
        {
          position3 = position2;
          rotation3 = rotation2;
        }
        if (!this.initiated)
        {
          this.hasChest = hasChest;
          this.hasNeck = hasNeck;
          this.headHeight = V3Tools.ExtractVertical(position5 - positions[0], rotations[0] * Vector3.up, 1f).magnitude;
          int length = 3;
          if (hasChest)
            ++length;
          if (hasNeck)
            ++length;
          this.bones = new IKSolverVR.VirtualBone[length];
          this.chestIndex = hasChest ? 2 : 1;
          this.neckIndex = 1;
          if (hasChest)
            ++this.neckIndex;
          if (hasNeck)
            ++this.neckIndex;
          this.headIndex = 2;
          if (hasChest)
            ++this.headIndex;
          if (hasNeck)
            ++this.headIndex;
          this.bones[0] = new IKSolverVR.VirtualBone(position1, rotation1);
          this.bones[1] = new IKSolverVR.VirtualBone(position2, rotation2);
          if (hasChest)
            this.bones[this.chestIndex] = new IKSolverVR.VirtualBone(position3, rotation3);
          if (hasNeck)
            this.bones[this.neckIndex] = new IKSolverVR.VirtualBone(position4, rotation4);
          this.bones[this.headIndex] = new IKSolverVR.VirtualBone(position5, rotation5);
          this.pelvisRotationOffset = Quaternion.identity;
          this.chestRotationOffset = Quaternion.identity;
          this.headRotationOffset = Quaternion.identity;
          this.anchorRelativeToHead = Quaternion.Inverse(rotation5) * rotations[0];
          this.pelvisRelativeRotation = Quaternion.Inverse(rotation5) * rotation1;
          this.chestRelativeRotation = Quaternion.Inverse(rotation5) * rotation3;
          this.chestForward = Quaternion.Inverse(rotation3) * (rotations[0] * Vector3.forward);
          this.faceDirection = rotations[0] * Vector3.forward;
          this.IKPositionHead = position5;
          this.IKRotationHead = rotation5;
          this.IKPositionPelvis = position1;
          this.IKRotationPelvis = rotation1;
          this.goalPositionChest = position3 + rotations[0] * Vector3.forward;
        }
        this.bones[0].Read(position1, rotation1);
        this.bones[1].Read(position2, rotation2);
        if (hasChest)
          this.bones[this.chestIndex].Read(position3, rotation3);
        if (hasNeck)
          this.bones[this.neckIndex].Read(position4, rotation4);
        this.bones[this.headIndex].Read(position5, rotation5);
        this.sizeMlp = Vector3.Distance(position1, position5) / 0.7f;
      }

      public override void PreSolve()
      {
        if ((UnityEngine.Object) this.headTarget != (UnityEngine.Object) null)
        {
          this.IKPositionHead = this.headTarget.position;
          this.IKRotationHead = this.headTarget.rotation;
        }
        if ((UnityEngine.Object) this.chestGoal != (UnityEngine.Object) null)
          this.goalPositionChest = this.chestGoal.position;
        if ((UnityEngine.Object) this.pelvisTarget != (UnityEngine.Object) null)
        {
          this.IKPositionPelvis = this.pelvisTarget.position;
          this.IKRotationPelvis = this.pelvisTarget.rotation;
        }
        this.headPosition = V3Tools.Lerp(this.head.solverPosition, this.IKPositionHead, this.positionWeight);
        this.headRotation = QuaTools.Lerp(this.head.solverRotation, this.IKRotationHead, this.rotationWeight);
      }

      public override void ApplyOffsets()
      {
        this.headPosition += this.headPositionOffset;
        Vector3 vector3 = this.rootRotation * Vector3.up;
        if (vector3 == Vector3.up)
        {
          this.headPosition.y = Math.Max(this.rootPosition.y + this.minHeadHeight, this.headPosition.y);
        }
        else
        {
          Vector3 v = this.headPosition - this.rootPosition;
          Vector3 horizontal = V3Tools.ExtractHorizontal(v, vector3, 1f);
          Vector3 lhs = v - horizontal;
          if ((double) Vector3.Dot(lhs, vector3) > 0.0)
          {
            if ((double) lhs.magnitude < (double) this.minHeadHeight)
              lhs = lhs.normalized * this.minHeadHeight;
          }
          else
            lhs = -lhs.normalized * this.minHeadHeight;
          this.headPosition = this.rootPosition + horizontal + lhs;
        }
        this.headRotation = this.headRotationOffset * this.headRotation;
        this.headDeltaPosition = this.headPosition - this.head.solverPosition;
        this.pelvisDeltaRotation = QuaTools.FromToRotation(this.pelvis.solverRotation, this.headRotation * this.pelvisRelativeRotation);
        this.anchorRotation = this.headRotation * this.anchorRelativeToHead;
      }

      private void CalculateChestTargetRotation(
        IKSolverVR.VirtualBone rootBone,
        IKSolverVR.Arm[] arms)
      {
        this.chestTargetRotation = this.headRotation * this.chestRelativeRotation;
        this.AdjustChestByHands(ref this.chestTargetRotation, arms);
        this.faceDirection = Vector3.Cross(this.anchorRotation * Vector3.right, rootBone.readRotation * Vector3.up) + this.anchorRotation * Vector3.forward;
      }

      public void Solve(
        IKSolverVR.VirtualBone rootBone,
        IKSolverVR.Leg[] legs,
        IKSolverVR.Arm[] arms)
      {
        this.CalculateChestTargetRotation(rootBone, arms);
        if ((double) this.maxRootAngle < 180.0)
        {
          Vector3 vector3 = Quaternion.Inverse(rootBone.solverRotation) * this.faceDirection;
          float num = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
          float angle = 0.0f;
          float maxRootAngle = this.maxRootAngle;
          if ((double) num > (double) maxRootAngle)
            angle = num - maxRootAngle;
          if ((double) num < -(double) maxRootAngle)
            angle = num + maxRootAngle;
          rootBone.solverRotation = Quaternion.AngleAxis(angle, rootBone.readRotation * Vector3.up) * rootBone.solverRotation;
        }
        Vector3 solverPosition = this.pelvis.solverPosition;
        Vector3 rootUp = rootBone.solverRotation * Vector3.up;
        this.TranslatePelvis(legs, this.headDeltaPosition, this.pelvisDeltaRotation);
        this.FABRIKPass(solverPosition, rootUp);
        this.Bend(this.bones, this.pelvisIndex, this.chestIndex, this.chestTargetRotation, this.chestRotationOffset, this.chestClampWeight, false, this.neckStiffness);
        if ((double) this.chestGoalWeight > 0.0)
          this.Bend(this.bones, this.pelvisIndex, this.chestIndex, Quaternion.FromToRotation(this.bones[this.chestIndex].solverRotation * this.chestForward, this.goalPositionChest - this.bones[this.chestIndex].solverPosition) * this.bones[this.chestIndex].solverRotation, this.chestRotationOffset, this.chestClampWeight, false, this.chestGoalWeight);
        this.InverseTranslateToHead(legs, false, false, Vector3.zero, 1f);
        this.FABRIKPass(solverPosition, rootUp);
        this.Bend(this.bones, this.neckIndex, this.headIndex, this.headRotation, this.headClampWeight, true, 1f);
        this.SolvePelvis();
      }

      private void FABRIKPass(Vector3 animatedPelvisPos, Vector3 rootUp)
      {
        IKSolverVR.VirtualBone.SolveFABRIK(this.bones, Vector3.Lerp(this.pelvis.solverPosition, animatedPelvisPos, this.maintainPelvisPosition) + this.pelvisPositionOffset - this.chestPositionOffset, this.headPosition - this.chestPositionOffset, 1f, 1f, 1, this.mag, rootUp * (this.bones[this.bones.Length - 1].solverPosition - this.bones[0].solverPosition).magnitude);
      }

      private void SolvePelvis()
      {
        if ((double) this.pelvisPositionWeight <= 0.0)
          return;
        Quaternion solverRotation = this.head.solverRotation;
        Vector3 vector3 = (this.IKPositionPelvis + this.pelvisPositionOffset - this.pelvis.solverPosition) * this.pelvisPositionWeight;
        foreach (IKSolverVR.VirtualBone bone in this.bones)
          bone.solverPosition += vector3;
        Vector3 bendNormal = this.anchorRotation * Vector3.right;
        if (this.hasChest && this.hasNeck)
        {
          IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, this.pelvisIndex, this.spineIndex, this.headIndex, this.headPosition, bendNormal, this.pelvisPositionWeight * 0.6f);
          IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, this.spineIndex, this.chestIndex, this.headIndex, this.headPosition, bendNormal, this.pelvisPositionWeight * 0.6f);
          IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, this.chestIndex, this.neckIndex, this.headIndex, this.headPosition, bendNormal, this.pelvisPositionWeight * 1f);
        }
        else if (this.hasChest && !this.hasNeck)
        {
          IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, this.pelvisIndex, this.spineIndex, this.headIndex, this.headPosition, bendNormal, this.pelvisPositionWeight * 0.75f);
          IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, this.spineIndex, this.chestIndex, this.headIndex, this.headPosition, bendNormal, this.pelvisPositionWeight * 1f);
        }
        else if (!this.hasChest && this.hasNeck)
        {
          IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, this.pelvisIndex, this.spineIndex, this.headIndex, this.headPosition, bendNormal, this.pelvisPositionWeight * 0.75f);
          IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, this.spineIndex, this.neckIndex, this.headIndex, this.headPosition, bendNormal, this.pelvisPositionWeight * 1f);
        }
        else if (!this.hasNeck && !this.hasChest)
          IKSolverVR.VirtualBone.SolveTrigonometric(this.bones, this.pelvisIndex, this.spineIndex, this.headIndex, this.headPosition, bendNormal, this.pelvisPositionWeight);
        this.head.solverRotation = solverRotation;
      }

      public override void Write(ref Vector3[] solvedPositions, ref Quaternion[] solvedRotations)
      {
        solvedPositions[this.index] = this.bones[0].solverPosition;
        solvedRotations[this.index] = this.bones[0].solverRotation;
        solvedRotations[this.index + 1] = this.bones[1].solverRotation;
        if (this.hasChest)
          solvedRotations[this.index + 2] = this.bones[this.chestIndex].solverRotation;
        if (this.hasNeck)
          solvedRotations[this.index + 3] = this.bones[this.neckIndex].solverRotation;
        solvedRotations[this.index + 4] = this.bones[this.headIndex].solverRotation;
      }

      public override void ResetOffsets()
      {
        this.pelvisPositionOffset = Vector3.zero;
        this.chestPositionOffset = Vector3.zero;
        this.headPositionOffset = this.locomotionHeadPositionOffset;
        this.pelvisRotationOffset = Quaternion.identity;
        this.chestRotationOffset = Quaternion.identity;
        this.headRotationOffset = Quaternion.identity;
      }

      private void AdjustChestByHands(ref Quaternion chestTargetRotation, IKSolverVR.Arm[] arms)
      {
        Quaternion quaternion1 = Quaternion.Inverse(this.anchorRotation);
        Vector3 vector3_1 = quaternion1 * (arms[0].position - this.headPosition) / this.sizeMlp;
        Vector3 vector3_2 = quaternion1 * (arms[1].position - this.headPosition) / this.sizeMlp;
        Vector3 forward = Vector3.forward;
        forward.x += vector3_1.x * Mathf.Abs(vector3_1.x);
        forward.x += vector3_1.z * Mathf.Abs(vector3_1.z);
        forward.x += vector3_2.x * Mathf.Abs(vector3_2.x);
        forward.x -= vector3_2.z * Mathf.Abs(vector3_2.z);
        forward.x *= 5f * this.rotateChestByHands;
        Quaternion quaternion2 = Quaternion.AngleAxis(Mathf.Atan2(forward.x, forward.z) * 57.29578f, this.rootRotation * Vector3.up);
        chestTargetRotation = quaternion2 * chestTargetRotation;
        Vector3 up = Vector3.up;
        up.x += vector3_1.y;
        up.x -= vector3_2.y;
        up.x *= 0.5f * this.rotateChestByHands;
        Quaternion quaternion3 = Quaternion.AngleAxis(Mathf.Atan2(up.x, up.y) * 57.29578f, this.rootRotation * Vector3.back);
        chestTargetRotation = quaternion3 * chestTargetRotation;
      }

      public void InverseTranslateToHead(
        IKSolverVR.Leg[] legs,
        bool limited,
        bool useCurrentLegMag,
        Vector3 offset,
        float w)
      {
        Vector3 pelvisPosition = this.pelvis.solverPosition + (this.headPosition + offset - this.head.solverPosition) * w * (1f - this.pelvisPositionWeight);
        this.MovePosition(limited ? this.LimitPelvisPosition(legs, pelvisPosition, useCurrentLegMag) : pelvisPosition);
      }

      private void TranslatePelvis(
        IKSolverVR.Leg[] legs,
        Vector3 deltaPosition,
        Quaternion deltaRotation)
      {
        Vector3 solverPosition = this.head.solverPosition;
        deltaRotation = QuaTools.ClampRotation(deltaRotation, this.chestClampWeight, 2);
        IKSolverVR.VirtualBone.RotateAroundPoint(this.bones, 0, this.pelvis.solverPosition, this.pelvisRotationOffset * Quaternion.Slerp(Quaternion.Slerp(Quaternion.identity, deltaRotation, this.bodyRotStiffness), QuaTools.FromToRotation(this.pelvis.solverRotation, this.IKRotationPelvis), this.pelvisRotationWeight));
        deltaPosition -= this.head.solverPosition - solverPosition;
        Vector3 vector3 = this.rootRotation * Vector3.forward;
        float num = V3Tools.ExtractVertical(deltaPosition, this.rootRotation * Vector3.up, 1f).magnitude * -this.moveBodyBackWhenCrouching * this.headHeight;
        deltaPosition += vector3 * num;
        this.MovePosition(this.LimitPelvisPosition(legs, this.pelvis.solverPosition + deltaPosition * this.bodyPosStiffness, false));
      }

      private Vector3 LimitPelvisPosition(
        IKSolverVR.Leg[] legs,
        Vector3 pelvisPosition,
        bool useCurrentLegMag,
        int it = 2)
      {
        if (useCurrentLegMag)
        {
          foreach (IKSolverVR.Leg leg in legs)
            leg.currentMag = Vector3.Distance(leg.thigh.solverPosition, leg.lastBone.solverPosition);
        }
        for (int index = 0; index < it; ++index)
        {
          foreach (IKSolverVR.Leg leg in legs)
          {
            Vector3 vector3_1 = pelvisPosition - this.pelvis.solverPosition;
            Vector3 vector3_2 = leg.thigh.solverPosition + vector3_1;
            Vector3 vector = vector3_2 - leg.position;
            float maxLength = useCurrentLegMag ? leg.currentMag : leg.mag;
            Vector3 vector3_3 = leg.position + Vector3.ClampMagnitude(vector, maxLength);
            pelvisPosition += vector3_3 - vector3_2;
          }
        }
        return pelvisPosition;
      }

      private void Bend(
        IKSolverVR.VirtualBone[] bones,
        int firstIndex,
        int lastIndex,
        Quaternion targetRotation,
        float clampWeight,
        bool uniformWeight,
        float w)
      {
        if ((double) w <= 0.0 || bones.Length == 0)
          return;
        int num1 = lastIndex + 1 - firstIndex;
        if (num1 < 1)
          return;
        Quaternion b = QuaTools.ClampRotation(QuaTools.FromToRotation(bones[lastIndex].solverRotation, targetRotation), clampWeight, 2);
        float num2 = uniformWeight ? 1f / (float) num1 : 0.0f;
        for (int index = firstIndex; index < lastIndex + 1; ++index)
        {
          if (!uniformWeight)
            num2 = Mathf.Clamp((float) ((index - firstIndex + 1) / num1), 0.0f, 1f);
          IKSolverVR.VirtualBone.RotateAroundPoint(bones, index, bones[index].solverPosition, Quaternion.Slerp(Quaternion.identity, b, num2 * w));
        }
      }

      private void Bend(
        IKSolverVR.VirtualBone[] bones,
        int firstIndex,
        int lastIndex,
        Quaternion targetRotation,
        Quaternion rotationOffset,
        float clampWeight,
        bool uniformWeight,
        float w)
      {
        if ((double) w <= 0.0 || bones.Length == 0)
          return;
        int num = lastIndex + 1 - firstIndex;
        if (num < 1)
          return;
        Quaternion b = QuaTools.ClampRotation(QuaTools.FromToRotation(bones[lastIndex].solverRotation, targetRotation), clampWeight, 2);
        float t = uniformWeight ? 1f / (float) num : 0.0f;
        for (int index = firstIndex; index < lastIndex + 1; ++index)
        {
          if (!uniformWeight)
            t = Mathf.Clamp((float) ((index - firstIndex + 1) / num), 0.0f, 1f);
          IKSolverVR.VirtualBone.RotateAroundPoint(bones, index, bones[index].solverPosition, Quaternion.Slerp(Quaternion.Slerp(Quaternion.identity, rotationOffset, t), b, t * w));
        }
      }
    }

    [Serializable]
    public enum PositionOffset
    {
      Pelvis,
      Chest,
      Head,
      LeftHand,
      RightHand,
      LeftFoot,
      RightFoot,
      LeftHeel,
      RightHeel,
    }

    [Serializable]
    public enum RotationOffset
    {
      Pelvis,
      Chest,
      Head,
    }

    [Serializable]
    public class VirtualBone
    {
      public Vector3 readPosition;
      public Quaternion readRotation;
      public Vector3 solverPosition;
      public Quaternion solverRotation;
      public float length;
      public float sqrMag;
      public Vector3 axis;

      public VirtualBone(Vector3 position, Quaternion rotation) => this.Read(position, rotation);

      public void Read(Vector3 position, Quaternion rotation)
      {
        this.readPosition = position;
        this.readRotation = rotation;
        this.solverPosition = position;
        this.solverRotation = rotation;
      }

      public static void SwingRotation(
        IKSolverVR.VirtualBone[] bones,
        int index,
        Vector3 swingTarget,
        float weight = 1f)
      {
        if ((double) weight <= 0.0)
          return;
        Quaternion b = Quaternion.FromToRotation(bones[index].solverRotation * bones[index].axis, swingTarget - bones[index].solverPosition);
        if ((double) weight < 1.0)
          b = Quaternion.Lerp(Quaternion.identity, b, weight);
        for (int index1 = index; index1 < bones.Length; ++index1)
          bones[index1].solverRotation = b * bones[index1].solverRotation;
      }

      public static float PreSolve(ref IKSolverVR.VirtualBone[] bones)
      {
        float num = 0.0f;
        for (int index = 0; index < bones.Length; ++index)
        {
          if (index < bones.Length - 1)
          {
            bones[index].sqrMag = (bones[index + 1].solverPosition - bones[index].solverPosition).sqrMagnitude;
            bones[index].length = Mathf.Sqrt(bones[index].sqrMag);
            num += bones[index].length;
            bones[index].axis = Quaternion.Inverse(bones[index].solverRotation) * (bones[index + 1].solverPosition - bones[index].solverPosition);
          }
          else
          {
            bones[index].sqrMag = 0.0f;
            bones[index].length = 0.0f;
          }
        }
        return num;
      }

      public static void RotateAroundPoint(
        IKSolverVR.VirtualBone[] bones,
        int index,
        Vector3 point,
        Quaternion rotation)
      {
        for (int index1 = index; index1 < bones.Length; ++index1)
        {
          if (bones[index1] != null)
          {
            Vector3 vector3 = bones[index1].solverPosition - point;
            bones[index1].solverPosition = point + rotation * vector3;
            bones[index1].solverRotation = rotation * bones[index1].solverRotation;
          }
        }
      }

      public static void RotateBy(IKSolverVR.VirtualBone[] bones, int index, Quaternion rotation)
      {
        for (int index1 = index; index1 < bones.Length; ++index1)
        {
          if (bones[index1] != null)
          {
            Vector3 vector3 = bones[index1].solverPosition - bones[index].solverPosition;
            bones[index1].solverPosition = bones[index].solverPosition + rotation * vector3;
            bones[index1].solverRotation = rotation * bones[index1].solverRotation;
          }
        }
      }

      public static void RotateBy(IKSolverVR.VirtualBone[] bones, Quaternion rotation)
      {
        for (int index = 0; index < bones.Length; ++index)
        {
          if (bones[index] != null)
          {
            if (index > 0)
            {
              Vector3 vector3 = bones[index].solverPosition - bones[0].solverPosition;
              bones[index].solverPosition = bones[0].solverPosition + rotation * vector3;
            }
            bones[index].solverRotation = rotation * bones[index].solverRotation;
          }
        }
      }

      public static void RotateTo(IKSolverVR.VirtualBone[] bones, int index, Quaternion rotation)
      {
        Quaternion rotation1 = QuaTools.FromToRotation(bones[index].solverRotation, rotation);
        IKSolverVR.VirtualBone.RotateAroundPoint(bones, index, bones[index].solverPosition, rotation1);
      }

      public static void SolveTrigonometric(
        IKSolverVR.VirtualBone[] bones,
        int first,
        int second,
        int third,
        Vector3 targetPosition,
        Vector3 bendNormal,
        float weight)
      {
        if ((double) weight <= 0.0)
          return;
        targetPosition = Vector3.Lerp(bones[third].solverPosition, targetPosition, weight);
        Vector3 vector3 = targetPosition - bones[first].solverPosition;
        float sqrMagnitude1 = vector3.sqrMagnitude;
        if ((double) sqrMagnitude1 == 0.0)
          return;
        float directionMag = Mathf.Sqrt(sqrMagnitude1);
        float sqrMagnitude2 = (bones[second].solverPosition - bones[first].solverPosition).sqrMagnitude;
        float sqrMagnitude3 = (bones[third].solverPosition - bones[second].solverPosition).sqrMagnitude;
        Vector3 bendDirection = Vector3.Cross(vector3, bendNormal);
        Vector3 directionToBendPoint = IKSolverVR.VirtualBone.GetDirectionToBendPoint(vector3, directionMag, bendDirection, sqrMagnitude2, sqrMagnitude3);
        Quaternion quaternion1 = Quaternion.FromToRotation(bones[second].solverPosition - bones[first].solverPosition, directionToBendPoint);
        if ((double) weight < 1.0)
          quaternion1 = Quaternion.Lerp(Quaternion.identity, quaternion1, weight);
        IKSolverVR.VirtualBone.RotateAroundPoint(bones, first, bones[first].solverPosition, quaternion1);
        Quaternion quaternion2 = Quaternion.FromToRotation(bones[third].solverPosition - bones[second].solverPosition, targetPosition - bones[second].solverPosition);
        if ((double) weight < 1.0)
          quaternion2 = Quaternion.Lerp(Quaternion.identity, quaternion2, weight);
        IKSolverVR.VirtualBone.RotateAroundPoint(bones, second, bones[second].solverPosition, quaternion2);
      }

      private static Vector3 GetDirectionToBendPoint(
        Vector3 direction,
        float directionMag,
        Vector3 bendDirection,
        float sqrMag1,
        float sqrMag2)
      {
        float z = (float) (((double) directionMag * (double) directionMag + ((double) sqrMag1 - (double) sqrMag2)) / 2.0) / directionMag;
        float y = (float) Math.Sqrt((double) Mathf.Clamp(sqrMag1 - z * z, 0.0f, float.PositiveInfinity));
        return direction == Vector3.zero ? Vector3.zero : Quaternion.LookRotation(direction, bendDirection) * new Vector3(0.0f, y, z);
      }

      public static void SolveFABRIK(
        IKSolverVR.VirtualBone[] bones,
        Vector3 startPosition,
        Vector3 targetPosition,
        float weight,
        float minNormalizedTargetDistance,
        int iterations,
        float length,
        Vector3 startOffset)
      {
        if ((double) weight <= 0.0)
          return;
        if ((double) minNormalizedTargetDistance > 0.0)
        {
          Vector3 vector3 = targetPosition - startPosition;
          float magnitude = vector3.magnitude;
          targetPosition = startPosition + vector3 / magnitude * Mathf.Max(length * minNormalizedTargetDistance, magnitude);
        }
        foreach (IKSolverVR.VirtualBone bone in bones)
          bone.solverPosition += startOffset;
        for (int index1 = 0; index1 < iterations; ++index1)
        {
          bones[bones.Length - 1].solverPosition = Vector3.Lerp(bones[bones.Length - 1].solverPosition, targetPosition, weight);
          for (int index2 = bones.Length - 2; index2 > -1; --index2)
            bones[index2].solverPosition = IKSolverVR.VirtualBone.SolveFABRIKJoint(bones[index2].solverPosition, bones[index2 + 1].solverPosition, bones[index2].length);
          bones[0].solverPosition = startPosition;
          for (int index3 = 1; index3 < bones.Length; ++index3)
            bones[index3].solverPosition = IKSolverVR.VirtualBone.SolveFABRIKJoint(bones[index3].solverPosition, bones[index3 - 1].solverPosition, bones[index3 - 1].length);
        }
        for (int index = 0; index < bones.Length - 1; ++index)
          IKSolverVR.VirtualBone.SwingRotation(bones, index, bones[index + 1].solverPosition);
      }

      private static Vector3 SolveFABRIKJoint(Vector3 pos1, Vector3 pos2, float length)
      {
        return pos2 + (pos1 - pos2).normalized * length;
      }

      public static void SolveCCD(
        IKSolverVR.VirtualBone[] bones,
        Vector3 targetPosition,
        float weight,
        int iterations)
      {
        if ((double) weight <= 0.0)
          return;
        for (int index1 = 0; index1 < iterations; ++index1)
        {
          for (int index2 = bones.Length - 2; index2 > -1; --index2)
          {
            Quaternion rotation = Quaternion.FromToRotation(bones[bones.Length - 1].solverPosition - bones[index2].solverPosition, targetPosition - bones[index2].solverPosition);
            if ((double) weight >= 1.0)
              IKSolverVR.VirtualBone.RotateBy(bones, index2, rotation);
            else
              IKSolverVR.VirtualBone.RotateBy(bones, index2, Quaternion.Lerp(Quaternion.identity, rotation, weight));
          }
        }
      }
    }
  }
}
