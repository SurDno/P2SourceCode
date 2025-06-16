using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
    public Spine spine = new Spine();
    [Tooltip("The left arm solver.")]
    public Arm leftArm = new Arm();
    [Tooltip("The right arm solver.")]
    public Arm rightArm = new Arm();
    [Tooltip("The left leg solver.")]
    public Leg leftLeg = new Leg();
    [Tooltip("The right leg solver.")]
    public Leg rightLeg = new Leg();
    [Tooltip("The procedural locomotion solver.")]
    public Locomotion locomotion = new Locomotion();
    private Leg[] legs = new Leg[2];
    private Arm[] arms = new Arm[2];
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
        Debug.LogError("Invalid references, one or more Transforms are missing.");
      }
      else
      {
        solverTransforms = references.GetTransforms();
        hasChest = solverTransforms[3] != null;
        hasNeck = solverTransforms[4] != null;
        hasShoulders = solverTransforms[6] != null && solverTransforms[10] != null;
        hasToes = solverTransforms[17] != null && solverTransforms[21] != null;
        readPositions = new Vector3[solverTransforms.Length];
        readRotations = new Quaternion[solverTransforms.Length];
        DefaultAnimationCurves();
        GuessHandOrientations(references, true);
      }
    }

    public void GuessHandOrientations(VRIK.References references, bool onlyIfZero)
    {
      if (!references.isFilled)
      {
        Debug.LogWarning("VRIK References are not filled in, can not guess hand orientations. Right-click on VRIK header and slect 'Guess Hand Orientations' when you have filled in the References.");
      }
      else
      {
        if (leftArm.wristToPalmAxis == Vector3.zero || !onlyIfZero)
          leftArm.wristToPalmAxis = GuessWristToPalmAxis(references.leftHand, references.leftForearm);
        if (leftArm.palmToThumbAxis == Vector3.zero || !onlyIfZero)
          leftArm.palmToThumbAxis = GuessPalmToThumbAxis(references.leftHand, references.leftForearm);
        if (rightArm.wristToPalmAxis == Vector3.zero || !onlyIfZero)
          rightArm.wristToPalmAxis = GuessWristToPalmAxis(references.rightHand, references.rightForearm);
        if (!(rightArm.palmToThumbAxis == Vector3.zero) && onlyIfZero)
          return;
        rightArm.palmToThumbAxis = GuessPalmToThumbAxis(references.rightHand, references.rightForearm);
      }
    }

    public void DefaultAnimationCurves()
    {
      if (locomotion.stepHeight == null)
        locomotion.stepHeight = new AnimationCurve();
      if (locomotion.heelHeight == null)
        locomotion.heelHeight = new AnimationCurve();
      if (locomotion.stepHeight.keys.Length == 0)
        locomotion.stepHeight.keys = GetSineKeyframes(0.03f);
      if (locomotion.heelHeight.keys.Length != 0)
        return;
      locomotion.heelHeight.keys = GetSineKeyframes(0.03f);
    }

    public void AddPositionOffset(PositionOffset positionOffset, Vector3 value)
    {
      switch (positionOffset)
      {
        case PositionOffset.Pelvis:
          spine.pelvisPositionOffset += value;
          break;
        case PositionOffset.Chest:
          spine.chestPositionOffset += value;
          break;
        case PositionOffset.Head:
          spine.headPositionOffset += value;
          break;
        case PositionOffset.LeftHand:
          leftArm.handPositionOffset += value;
          break;
        case PositionOffset.RightHand:
          rightArm.handPositionOffset += value;
          break;
        case PositionOffset.LeftFoot:
          leftLeg.footPositionOffset += value;
          break;
        case PositionOffset.RightFoot:
          rightLeg.footPositionOffset += value;
          break;
        case PositionOffset.LeftHeel:
          leftLeg.heelPositionOffset += value;
          break;
        case PositionOffset.RightHeel:
          rightLeg.heelPositionOffset += value;
          break;
      }
    }

    public void AddRotationOffset(RotationOffset rotationOffset, Vector3 value)
    {
      AddRotationOffset(rotationOffset, Quaternion.Euler(value));
    }

    public void AddRotationOffset(RotationOffset rotationOffset, Quaternion value)
    {
      switch (rotationOffset)
      {
        case RotationOffset.Pelvis:
          spine.pelvisRotationOffset = value * spine.pelvisRotationOffset;
          break;
        case RotationOffset.Chest:
          spine.chestRotationOffset = value * spine.chestRotationOffset;
          break;
        case RotationOffset.Head:
          spine.headRotationOffset = value * spine.headRotationOffset;
          break;
      }
    }

    public void AddPlatformMotion(
      Vector3 deltaPosition,
      Quaternion deltaRotation,
      Vector3 platformPivot)
    {
      locomotion.AddDeltaPosition(deltaPosition);
      raycastOriginPelvis += deltaPosition;
      locomotion.AddDeltaRotation(deltaRotation, platformPivot);
      spine.faceDirection = deltaRotation * spine.faceDirection;
    }

    public void Reset()
    {
      if (!initiated)
        return;
      UpdateSolverTransforms();
      Read(readPositions, readRotations, hasChest, hasNeck, hasShoulders, hasToes);
      spine.faceDirection = rootBone.readRotation * Vector3.forward;
      locomotion.Reset(readPositions, readRotations);
      raycastOriginPelvis = spine.pelvis.readPosition;
    }

    public override void StoreDefaultLocalState()
    {
      for (int index = 1; index < solverTransforms.Length; ++index)
      {
        if (solverTransforms[index] != null)
        {
          defaultLocalPositions[index - 1] = solverTransforms[index].localPosition;
          defaultLocalRotations[index - 1] = solverTransforms[index].localRotation;
        }
      }
    }

    public override void FixTransforms()
    {
      if (!initiated)
        return;
      for (int index = 1; index < solverTransforms.Length; ++index)
      {
        if (solverTransforms[index] != null)
        {
          if (index == 1 | (index > 5 && index < 14))
            solverTransforms[index].localPosition = defaultLocalPositions[index - 1];
          solverTransforms[index].localRotation = defaultLocalRotations[index - 1];
        }
      }
    }

    public override Point[] GetPoints()
    {
      Debug.LogError("GetPoints() is not applicable to IKSolverVR.");
      return null;
    }

    public override Point GetPoint(Transform transform)
    {
      Debug.LogError("GetPoint is not applicable to IKSolverVR.");
      return null;
    }

    public override bool IsValid(ref string message)
    {
      if (solverTransforms == null || solverTransforms.Length == 0)
      {
        message = "Trying to initiate IKSolverVR with invalid bone references.";
        return false;
      }
      if (leftArm.wristToPalmAxis == Vector3.zero)
      {
        message = "Left arm 'Wrist To Palm Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the wrist towards the palm. If the arrow points away from the palm, axis must be negative.";
        return false;
      }
      if (rightArm.wristToPalmAxis == Vector3.zero)
      {
        message = "Right arm 'Wrist To Palm Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the wrist towards the palm. If the arrow points away from the palm, axis must be negative.";
        return false;
      }
      if (leftArm.palmToThumbAxis == Vector3.zero)
      {
        message = "Left arm 'Palm To Thumb Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the palm towards the thumb. If the arrow points away from the thumb, axis must be negative.";
        return false;
      }
      if (!(rightArm.palmToThumbAxis == Vector3.zero))
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
      Vector3 vector3 = zero2 / transforms.Length;
      for (int index = 0; index < transforms.Length - 1; ++index)
        zero1 += Vector3.Cross(transforms[index].position - vector3, transforms[index + 1].position - vector3).normalized;
      return zero1;
    }

    private Vector3 GuessWristToPalmAxis(Transform hand, Transform forearm)
    {
      Vector3 vector3 = forearm.position - hand.position;
      Vector3 palmAxis = AxisTools.ToVector3(AxisTools.GetAxisToDirection(hand, vector3));
      if (Vector3.Dot(vector3, hand.rotation * palmAxis) > 0.0)
        palmAxis = -palmAxis;
      return palmAxis;
    }

    private Vector3 GuessPalmToThumbAxis(Transform hand, Transform forearm)
    {
      if (hand.childCount == 0)
      {
        Debug.LogWarning("Hand " + hand.name + " does not have any fingers, VRIK can not guess the hand bone's orientation. Please assign 'Wrist To Palm Axis' and 'Palm To Thumb Axis' manually for both arms in VRIK settings.", hand);
        return Vector3.zero;
      }
      float num1 = float.PositiveInfinity;
      int index1 = 0;
      for (int index2 = 0; index2 < hand.childCount; ++index2)
      {
        float num2 = Vector3.SqrMagnitude(hand.GetChild(index2).position - hand.position);
        if (num2 < (double) num1)
        {
          num1 = num2;
          index1 = index2;
        }
      }
      Vector3 vector3 = Vector3.Cross(Vector3.Cross(hand.position - forearm.position, hand.GetChild(index1).position - hand.position), hand.position - forearm.position);
      Vector3 thumbAxis = AxisTools.ToVector3(AxisTools.GetAxisToDirection(hand, vector3));
      if (Vector3.Dot(vector3, hand.rotation * thumbAxis) < 0.0)
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
      for (int index = 0; index < solverTransforms.Length; ++index)
      {
        if (solverTransforms[index] != null)
        {
          readPositions[index] = solverTransforms[index].position;
          readRotations[index] = solverTransforms[index].rotation;
        }
      }
    }

    protected override void OnInitiate()
    {
      UpdateSolverTransforms();
      Read(readPositions, readRotations, hasChest, hasNeck, hasShoulders, hasToes);
    }

    protected override void OnUpdate()
    {
      if (IKPositionWeight <= 0.0)
        return;
      UpdateSolverTransforms();
      Read(readPositions, readRotations, hasChest, hasNeck, hasShoulders, hasToes);
      Solve();
      Write();
      WriteTransforms();
    }

    private void WriteTransforms()
    {
      for (int index = 0; index < solverTransforms.Length; ++index)
      {
        if (solverTransforms[index] != null)
        {
          if (index < 2 | (index > 5 && index < 14))
            solverTransforms[index].position = V3Tools.Lerp(solverTransforms[index].position, GetPosition(index), IKPositionWeight);
          solverTransforms[index].rotation = QuaTools.Lerp(solverTransforms[index].rotation, GetRotation(index), IKPositionWeight);
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
      if (rootBone == null)
        rootBone = new VirtualBone(positions[0], rotations[0]);
      else
        rootBone.Read(positions[0], rotations[0]);
      spine.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, 0, 1);
      leftArm.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, hasChest ? 3 : 2, 6);
      rightArm.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, hasChest ? 3 : 2, 10);
      leftLeg.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, 1, 14);
      rightLeg.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, 1, 18);
      for (int index = 0; index < rotations.Length; ++index)
      {
        solvedPositions[index] = positions[index];
        solvedRotations[index] = rotations[index];
      }
      if (initiated)
        return;
      legs = new Leg[2]
      {
        leftLeg,
        rightLeg
      };
      arms = new Arm[2]
      {
        leftArm,
        rightArm
      };
      locomotion.Initiate(positions, rotations, hasToes);
      raycastOriginPelvis = spine.pelvis.readPosition;
      spine.faceDirection = readRotations[0] * Vector3.forward;
    }

    private void Solve()
    {
      spine.PreSolve();
      foreach (BodyPart arm in arms)
        arm.PreSolve();
      foreach (BodyPart leg in legs)
        leg.PreSolve();
      foreach (BodyPart arm in arms)
        arm.ApplyOffsets();
      spine.ApplyOffsets();
      spine.Solve(rootBone, legs, arms);
      if (spine.pelvisPositionWeight > 0.0 && plantFeet)
        Warning.Log("If VRIK 'Pelvis Position Weight' is > 0, 'Plant Feet' should be disabled to improve performance and stability.", root);
      if (locomotion.weight > 0.0)
      {
        Vector3 leftFootPosition = Vector3.zero;
        Vector3 rightFootPosition = Vector3.zero;
        Quaternion leftFootRotation = Quaternion.identity;
        Quaternion rightFootRotation = Quaternion.identity;
        float leftFootOffset = 0.0f;
        float rightFootOffset = 0.0f;
        float leftHeelOffset = 0.0f;
        float rightHeelOffset = 0.0f;
        locomotion.Solve(rootBone, spine, leftLeg, rightLeg, leftArm, rightArm, supportLegIndex, out leftFootPosition, out rightFootPosition, out leftFootRotation, out rightFootRotation, out leftFootOffset, out rightFootOffset, out leftHeelOffset, out rightHeelOffset);
        leftFootPosition += root.up * leftFootOffset;
        rightFootPosition += root.up * rightFootOffset;
        leftLeg.footPositionOffset += (leftFootPosition - leftLeg.lastBone.solverPosition) * IKPositionWeight * (1f - leftLeg.positionWeight) * locomotion.weight;
        rightLeg.footPositionOffset += (rightFootPosition - rightLeg.lastBone.solverPosition) * IKPositionWeight * (1f - rightLeg.positionWeight) * locomotion.weight;
        leftLeg.heelPositionOffset += root.up * leftHeelOffset * locomotion.weight;
        rightLeg.heelPositionOffset += root.up * rightHeelOffset * locomotion.weight;
        Quaternion rotation1 = QuaTools.FromToRotation(leftLeg.lastBone.solverRotation, leftFootRotation);
        Quaternion rotation2 = QuaTools.FromToRotation(rightLeg.lastBone.solverRotation, rightFootRotation);
        Quaternion quaternion1 = Quaternion.Lerp(Quaternion.identity, rotation1, IKPositionWeight * (1f - leftLeg.rotationWeight) * locomotion.weight);
        Quaternion quaternion2 = Quaternion.Lerp(Quaternion.identity, rotation2, IKPositionWeight * (1f - rightLeg.rotationWeight) * locomotion.weight);
        leftLeg.footRotationOffset = quaternion1 * leftLeg.footRotationOffset;
        rightLeg.footRotationOffset = quaternion2 * rightLeg.footRotationOffset;
        Vector3 plane = V3Tools.PointToPlane(Vector3.Lerp(leftLeg.position + leftLeg.footPositionOffset, rightLeg.position + rightLeg.footPositionOffset, 0.5f), rootBone.solverPosition, root.up);
        rootBone.solverPosition = Vector3.Lerp(rootBone.solverPosition + rootVelocity * Time.deltaTime * 2f * locomotion.weight, plane, Time.deltaTime * locomotion.rootSpeed * locomotion.weight);
        rootVelocity += (plane - rootBone.solverPosition) * Time.deltaTime * 10f;
        rootVelocity -= V3Tools.ExtractVertical(rootVelocity, root.up, 1f);
        bodyOffset = Vector3.Lerp(bodyOffset, root.up * (leftFootOffset + rightFootOffset), Time.deltaTime * 3f);
        bodyOffset = Vector3.Lerp(Vector3.zero, bodyOffset, locomotion.weight);
      }
      foreach (BodyPart leg in legs)
        leg.ApplyOffsets();
      if (!plantFeet)
      {
        spine.InverseTranslateToHead(legs, false, false, bodyOffset, 1f);
        foreach (BodyPart leg in legs)
          leg.TranslateRoot(spine.pelvis.solverPosition, spine.pelvis.solverRotation);
        foreach (Leg leg in legs)
          leg.Solve();
      }
      else
      {
        for (int index = 0; index < 2; ++index)
        {
          spine.InverseTranslateToHead(legs, true, index == 0, bodyOffset, 1f);
          foreach (BodyPart leg in legs)
            leg.TranslateRoot(spine.pelvis.solverPosition, spine.pelvis.solverRotation);
          foreach (Leg leg in legs)
            leg.Solve();
        }
      }
      for (int index = 0; index < arms.Length; ++index)
        arms[index].TranslateRoot(spine.chest.solverPosition, spine.chest.solverRotation);
      for (int index = 0; index < arms.Length; ++index)
        arms[index].Solve(index == 0);
      spine.ResetOffsets();
      foreach (BodyPart leg in legs)
        leg.ResetOffsets();
      foreach (BodyPart arm in arms)
        arm.ResetOffsets();
      spine.pelvisPositionOffset += GetPelvisOffset();
      spine.chestPositionOffset += spine.pelvisPositionOffset;
      Write();
      supportLegIndex = -1;
      float num1 = float.PositiveInfinity;
      for (int index = 0; index < legs.Length; ++index)
      {
        float num2 = Vector3.SqrMagnitude(legs[index].lastBone.solverPosition - legs[index].bones[0].solverPosition);
        if (num2 < (double) num1)
        {
          supportLegIndex = index;
          num1 = num2;
        }
      }
    }

    private Vector3 GetPosition(int index) => solvedPositions[index];

    private Quaternion GetRotation(int index) => solvedRotations[index];

    [HideInInspector]
    public VirtualBone rootBone { get; private set; }

    private void Write()
    {
      solvedPositions[0] = rootBone.solverPosition;
      solvedRotations[0] = rootBone.solverRotation;
      spine.Write(ref solvedPositions, ref solvedRotations);
      foreach (BodyPart leg in legs)
        leg.Write(ref solvedPositions, ref solvedRotations);
      foreach (BodyPart arm in arms)
        arm.Write(ref solvedPositions, ref solvedRotations);
    }

    private Vector3 GetPelvisOffset()
    {
      if (locomotion.weight <= 0.0 || locomotion.blockingLayers == -1)
        return Vector3.zero;
      Vector3 raycastOriginPelvis = this.raycastOriginPelvis with
      {
        y = spine.pelvis.solverPosition.y
      };
      Vector3 origin = spine.pelvis.readPosition with
      {
        y = spine.pelvis.solverPosition.y
      };
      Vector3 direction1 = origin - raycastOriginPelvis;
      RaycastHit hitInfo;
      if (locomotion.raycastRadius <= 0.0)
      {
        if (Physics.Raycast(raycastOriginPelvis, direction1, out hitInfo, direction1.magnitude * 1.1f, locomotion.blockingLayers))
          origin = hitInfo.point;
      }
      else if (Physics.SphereCast(raycastOriginPelvis, locomotion.raycastRadius * 1.1f, direction1, out hitInfo, direction1.magnitude, locomotion.blockingLayers))
        origin = raycastOriginPelvis + direction1.normalized * hitInfo.distance / 1.1f;
      Vector3 vector3 = spine.pelvis.solverPosition;
      Vector3 direction2 = vector3 - origin;
      if (locomotion.raycastRadius <= 0.0)
      {
        if (Physics.Raycast(origin, direction2, out hitInfo, direction2.magnitude, locomotion.blockingLayers))
          vector3 = hitInfo.point;
      }
      else if (Physics.SphereCast(origin, locomotion.raycastRadius, direction2, out hitInfo, direction2.magnitude, locomotion.blockingLayers))
        vector3 = origin + direction2.normalized * hitInfo.distance;
      lastOffset = Vector3.Lerp(lastOffset, Vector3.zero, Time.deltaTime * 3f);
      lastOffset = Vector3.Lerp(lastOffset, (vector3 + Vector3.ClampMagnitude(lastOffset, 0.75f)) with
      {
        y = spine.pelvis.solverPosition.y
      } - spine.pelvis.solverPosition, Time.deltaTime * 15f);
      return lastOffset;
    }

    [Serializable]
    public class Arm : BodyPart
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
      public ShoulderRotationMode shoulderRotationMode = ShoulderRotationMode.YawPitch;
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

      private VirtualBone shoulder => bones[0];

      private VirtualBone upperArm => bones[1];

      private VirtualBone forearm => bones[2];

      private VirtualBone hand => bones[3];

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
        if (!initiated)
        {
          IKPosition = position4;
          IKRotation = rotation4;
          rotation = IKRotation;
          hasShoulder = hasShoulders;
          bones = new VirtualBone[hasShoulder ? 4 : 3];
          if (hasShoulder)
          {
            bones[0] = new VirtualBone(position1, rotation1);
            bones[1] = new VirtualBone(position2, rotation2);
            bones[2] = new VirtualBone(position3, rotation3);
            bones[3] = new VirtualBone(position4, rotation4);
          }
          else
          {
            bones[0] = new VirtualBone(position2, rotation2);
            bones[1] = new VirtualBone(position3, rotation3);
            bones[2] = new VirtualBone(position4, rotation4);
          }
          chestForwardAxis = Quaternion.Inverse(rootRotation) * (rotations[0] * Vector3.forward);
          chestUpAxis = Quaternion.Inverse(rootRotation) * (rotations[0] * Vector3.up);
        }
        if (hasShoulder)
        {
          bones[0].Read(position1, rotation1);
          bones[1].Read(position2, rotation2);
          bones[2].Read(position3, rotation3);
          bones[3].Read(position4, rotation4);
        }
        else
        {
          bones[0].Read(position2, rotation2);
          bones[1].Read(position3, rotation3);
          bones[2].Read(position4, rotation4);
        }
      }

      public override void PreSolve()
      {
        if (target != null)
        {
          IKPosition = target.position;
          IKRotation = target.rotation;
        }
        position = V3Tools.Lerp(hand.solverPosition, IKPosition, positionWeight);
        rotation = QuaTools.Lerp(hand.solverRotation, IKRotation, rotationWeight);
        shoulder.axis = shoulder.axis.normalized;
        forearmRelToUpperArm = Quaternion.Inverse(upperArm.solverRotation) * forearm.solverRotation;
      }

      public override void ApplyOffsets() => position += handPositionOffset;

      private void Stretching()
      {
        float num1 = upperArm.length + forearm.length;
        Vector3 zero1 = Vector3.zero;
        Vector3 zero2 = Vector3.zero;
        if (armLengthMlp != 1.0)
        {
          num1 *= armLengthMlp;
          Vector3 vector3_1 = (forearm.solverPosition - upperArm.solverPosition) * (armLengthMlp - 1f);
          Vector3 vector3_2 = (hand.solverPosition - forearm.solverPosition) * (armLengthMlp - 1f);
          forearm.solverPosition += vector3_1;
          hand.solverPosition += vector3_1 + vector3_2;
        }
        float num2 = stretchCurve.Evaluate(Vector3.Distance(upperArm.solverPosition, position) / num1) * positionWeight;
        Vector3 vector3_3 = (forearm.solverPosition - upperArm.solverPosition) * num2;
        Vector3 vector3_4 = (hand.solverPosition - forearm.solverPosition) * num2;
        forearm.solverPosition += vector3_3;
        hand.solverPosition += vector3_3 + vector3_4;
      }

      public void Solve(bool isLeft)
      {
        chestRotation = Quaternion.LookRotation(rootRotation * chestForwardAxis, rootRotation * chestUpAxis);
        chestForward = chestRotation * Vector3.forward;
        chestUp = chestRotation * Vector3.up;
        if (hasShoulder && shoulderRotationWeight > 0.0)
        {
          switch (shoulderRotationMode)
          {
            case ShoulderRotationMode.YawPitch:
              Vector3 vector3_1 = position - shoulder.solverPosition;
              vector3_1 = vector3_1.normalized;
              float num1 = isLeft ? 45f : -45f;
              Quaternion rotation1 = Quaternion.AngleAxis((isLeft ? -90f : 90f) + num1, chestUp) * chestRotation;
              Vector3 lhs = Quaternion.Inverse(rotation1) * vector3_1;
              float num2 = Mathf.Atan2(lhs.x, lhs.z) * 57.29578f * (1f - Mathf.Abs(Vector3.Dot(lhs, Vector3.up))) - num1;
              float num3 = isLeft ? -20f : -50f;
              float num4 = isLeft ? 50f : 20f;
              float angle1 = DamperValue(num2, num3 - num1, num4 - num1, 0.7f);
              Quaternion rotation2 = Quaternion.FromToRotation(shoulder.solverRotation * shoulder.axis, rotation1 * (Quaternion.AngleAxis(angle1, Vector3.up) * Vector3.forward));
              Quaternion quaternion1 = Quaternion.AngleAxis(isLeft ? -90f : 90f, chestUp) * chestRotation;
              Quaternion rotation3 = Quaternion.AngleAxis(isLeft ? -30f : 30f, chestForward) * quaternion1;
              vector3_1 = position - (shoulder.solverPosition + chestRotation * (isLeft ? Vector3.right : Vector3.left) * mag);
              Vector3 vector3_2 = Quaternion.Inverse(rotation3) * vector3_1;
              float num5 = DamperValue(Mathf.Atan2(vector3_2.y, vector3_2.z) * 57.29578f - -30f, -15f, 75f);
              Quaternion quaternion2 = Quaternion.AngleAxis(-num5, rotation3 * Vector3.right) * rotation2;
              if (shoulderRotationWeight * (double) positionWeight < 1.0)
                quaternion2 = Quaternion.Lerp(Quaternion.identity, quaternion2, shoulderRotationWeight * positionWeight);
              VirtualBone.RotateBy(bones, quaternion2);
              Stretching();
              VirtualBone.SolveTrigonometric(bones, 1, 2, 3, position, GetBendNormal(position - upperArm.solverPosition), positionWeight);
              float angle2 = Mathf.Clamp(num5 * 2f * positionWeight, 0.0f, 180f);
              shoulder.solverRotation = Quaternion.AngleAxis(angle2, shoulder.solverRotation * (isLeft ? shoulder.axis : -shoulder.axis)) * shoulder.solverRotation;
              upperArm.solverRotation = Quaternion.AngleAxis(angle2, upperArm.solverRotation * (isLeft ? upperArm.axis : -upperArm.axis)) * upperArm.solverRotation;
              break;
            case ShoulderRotationMode.FromTo:
              Quaternion solverRotation = shoulder.solverRotation;
              VirtualBone.RotateBy(bones, Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation((upperArm.solverPosition - shoulder.solverPosition).normalized + chestForward, position - shoulder.solverPosition), 0.5f * shoulderRotationWeight * positionWeight));
              Stretching();
              VirtualBone.SolveTrigonometric(bones, 0, 2, 3, position, Vector3.Cross(forearm.solverPosition - shoulder.solverPosition, hand.solverPosition - shoulder.solverPosition), 0.5f * shoulderRotationWeight * positionWeight);
              VirtualBone.SolveTrigonometric(bones, 1, 2, 3, position, GetBendNormal(position - upperArm.solverPosition), positionWeight);
              Quaternion quaternion3 = Quaternion.Inverse(Quaternion.LookRotation(chestUp, chestForward));
              Vector3 vector3_3 = quaternion3 * (solverRotation * shoulder.axis);
              Vector3 vector3_4 = quaternion3 * (shoulder.solverRotation * shoulder.axis);
              float num6 = Mathf.DeltaAngle(Mathf.Atan2(vector3_3.x, vector3_3.z) * 57.29578f, Mathf.Atan2(vector3_4.x, vector3_4.z) * 57.29578f);
              if (isLeft)
                num6 = -num6;
              float angle3 = Mathf.Clamp(num6 * 2f * positionWeight, 0.0f, 180f);
              shoulder.solverRotation = Quaternion.AngleAxis(angle3, shoulder.solverRotation * (isLeft ? shoulder.axis : -shoulder.axis)) * shoulder.solverRotation;
              upperArm.solverRotation = Quaternion.AngleAxis(angle3, upperArm.solverRotation * (isLeft ? upperArm.axis : -upperArm.axis)) * upperArm.solverRotation;
              break;
          }
        }
        else
        {
          Stretching();
          VirtualBone.SolveTrigonometric(bones, 1, 2, 3, position, GetBendNormal(position - upperArm.solverPosition), positionWeight);
        }
        Quaternion quaternion4 = upperArm.solverRotation * forearmRelToUpperArm;
        RotateTo(forearm, Quaternion.FromToRotation(quaternion4 * forearm.axis, hand.solverPosition - forearm.solverPosition) * quaternion4, positionWeight);
        if (rotationWeight >= 1.0)
        {
          hand.solverRotation = rotation;
        }
        else
        {
          if (rotationWeight <= 0.0)
            return;
          hand.solverRotation = Quaternion.Lerp(hand.solverRotation, rotation, rotationWeight);
        }
      }

      public override void ResetOffsets() => handPositionOffset = Vector3.zero;

      public override void Write(ref Vector3[] solvedPositions, ref Quaternion[] solvedRotations)
      {
        if (hasShoulder)
        {
          solvedPositions[index] = shoulder.solverPosition;
          solvedRotations[index] = shoulder.solverRotation;
        }
        solvedPositions[index + 1] = upperArm.solverPosition;
        solvedPositions[index + 2] = forearm.solverPosition;
        solvedPositions[index + 3] = hand.solverPosition;
        solvedRotations[index + 1] = upperArm.solverRotation;
        solvedRotations[index + 2] = forearm.solverRotation;
        solvedRotations[index + 3] = hand.solverRotation;
      }

      private float DamperValue(float value, float min, float max, float weight = 1f)
      {
        float num1 = max - min;
        if (weight < 1.0)
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
        if (bendGoal != null)
          bendDirection = bendGoal.position - bones[1].solverPosition;
        Vector3 vector3_1 = bones[0].solverRotation * bones[0].axis;
        Vector3 vector3_2 = Quaternion.FromToRotation(Vector3.down, Quaternion.Inverse(chestRotation) * dir.normalized + Vector3.forward) * Vector3.back;
        Vector3 vector3_3 = chestRotation * (Quaternion.FromToRotation(Quaternion.Inverse(chestRotation) * vector3_1, Quaternion.Inverse(chestRotation) * dir) * vector3_2) + vector3_1 - rotation * wristToPalmAxis - rotation * palmToThumbAxis * 0.5f;
        if (bendGoalWeight > 0.0)
          vector3_3 = Vector3.Slerp(vector3_3, bendDirection, bendGoalWeight);
        if (swivelOffset != 0.0)
          vector3_3 = Quaternion.AngleAxis(swivelOffset, -dir) * vector3_3;
        return Vector3.Cross(vector3_3, dir);
      }

      private void Visualize(
        VirtualBone bone1,
        VirtualBone bone2,
        VirtualBone bone3,
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
      public VirtualBone[] bones = new VirtualBone[0];
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
        rootPosition = positions[rootIndex];
        rootRotation = rotations[rootIndex];
        OnRead(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, rootIndex, index);
        mag = VirtualBone.PreSolve(ref bones);
        sqrMag = mag * mag;
        initiated = true;
      }

      public void MovePosition(Vector3 position)
      {
        Vector3 vector3 = position - bones[0].solverPosition;
        foreach (VirtualBone bone in bones)
          bone.solverPosition += vector3;
      }

      public void MoveRotation(Quaternion rotation)
      {
        VirtualBone.RotateAroundPoint(bones, 0, bones[0].solverPosition, QuaTools.FromToRotation(bones[0].solverRotation, rotation));
      }

      public void Translate(Vector3 position, Quaternion rotation)
      {
        MovePosition(position);
        MoveRotation(rotation);
      }

      public void TranslateRoot(Vector3 newRootPos, Quaternion newRootRot)
      {
        Vector3 vector3 = newRootPos - rootPosition;
        rootPosition = newRootPos;
        foreach (VirtualBone bone in bones)
          bone.solverPosition += vector3;
        Quaternion rotation = QuaTools.FromToRotation(rootRotation, newRootRot);
        rootRotation = newRootRot;
        VirtualBone.RotateAroundPoint(bones, 0, newRootPos, rotation);
      }

      public void RotateTo(VirtualBone bone, Quaternion rotation, float weight = 1f)
      {
        if (weight <= 0.0)
          return;
        Quaternion quaternion = QuaTools.FromToRotation(bone.solverRotation, rotation);
        if (weight < 1.0)
          quaternion = Quaternion.Slerp(Quaternion.identity, quaternion, weight);
        for (int index = 0; index < bones.Length; ++index)
        {
          if (bones[index] == bone)
          {
            VirtualBone.RotateAroundPoint(bones, index, bones[index].solverPosition, quaternion);
            break;
          }
        }
      }

      public void Visualize(Color color)
      {
        for (int index = 0; index < bones.Length - 1; ++index)
          Debug.DrawLine(bones[index].solverPosition, bones[index + 1].solverPosition, color);
      }

      public void Visualize() => Visualize(Color.white);
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

      public bool isStepping => stepProgress < 1.0;

      public float stepProgress { get; private set; }

      public Footstep(
        Quaternion rootRotation,
        Vector3 footPosition,
        Quaternion footRotation,
        Vector3 characterSpaceOffset)
      {
        this.characterSpaceOffset = characterSpaceOffset;
        Reset(rootRotation, footPosition, footRotation);
      }

      public void Reset(Quaternion rootRotation, Vector3 footPosition, Quaternion footRotation)
      {
        position = footPosition;
        rotation = footRotation;
        stepFrom = position;
        stepTo = position;
        stepFromRot = rotation;
        stepToRot = rotation;
        stepToRootRot = rootRotation;
        stepProgress = 1f;
        footRelativeToRoot = Quaternion.Inverse(rootRotation) * rotation;
      }

      public void StepTo(Vector3 p, Quaternion rootRotation)
      {
        stepFrom = position;
        stepTo = p;
        stepFromRot = rotation;
        stepToRootRot = rootRotation;
        stepToRot = rootRotation * footRelativeToRoot;
        stepProgress = 0.0f;
      }

      public void UpdateStepping(Vector3 p, Quaternion rootRotation, float speed)
      {
        stepTo = Vector3.Lerp(stepTo, p, Time.deltaTime * speed);
        stepToRot = Quaternion.Lerp(stepToRot, rootRotation * footRelativeToRoot, Time.deltaTime * speed);
        stepToRootRot = stepToRot * Quaternion.Inverse(footRelativeToRoot);
      }

      public void UpdateStanding(Quaternion rootRotation, float minAngle, float speed)
      {
        if (speed <= 0.0 || minAngle >= 180.0)
          return;
        Quaternion quaternion = rootRotation * footRelativeToRoot;
        float num = Quaternion.Angle(rotation, quaternion);
        if (num <= (double) minAngle)
          return;
        rotation = Quaternion.RotateTowards(rotation, quaternion, Mathf.Min((float) (Time.deltaTime * (double) speed * (1.0 - supportLegW)), num - minAngle));
      }

      public void Update(InterpolationMode interpolation, UnityEvent onStep)
      {
        supportLegW = Mathf.SmoothDamp(supportLegW, isSupportLeg ? 1f : 0.0f, ref supportLegWV, 0.2f);
        if (!isStepping)
          return;
        stepProgress = Mathf.MoveTowards(stepProgress, 1f, Time.deltaTime * stepSpeed);
        if (stepProgress >= 1.0)
          onStep.Invoke();
        float t = Interp.Float(stepProgress, interpolation);
        position = Vector3.Lerp(stepFrom, stepTo, t);
        rotation = Quaternion.Lerp(stepFromRot, stepToRot, t);
      }
    }

    [Serializable]
    public class Leg : BodyPart
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

      public VirtualBone thigh => bones[0];

      private VirtualBone calf => bones[1];

      private VirtualBone foot => bones[2];

      private VirtualBone toes => bones[3];

      public VirtualBone lastBone => bones[bones.Length - 1];

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
        if (!initiated)
        {
          this.hasToes = hasToes;
          bones = new VirtualBone[hasToes ? 4 : 3];
          if (hasToes)
          {
            bones[0] = new VirtualBone(position1, rotation1);
            bones[1] = new VirtualBone(position2, rotation2);
            bones[2] = new VirtualBone(position3, rotation3);
            bones[3] = new VirtualBone(position4, rotation4);
            IKPosition = position4;
            IKRotation = rotation4;
          }
          else
          {
            bones[0] = new VirtualBone(position1, rotation1);
            bones[1] = new VirtualBone(position2, rotation2);
            bones[2] = new VirtualBone(position3, rotation3);
            IKPosition = position3;
            IKRotation = rotation3;
          }
          rotation = IKRotation;
        }
        if (hasToes)
        {
          bones[0].Read(position1, rotation1);
          bones[1].Read(position2, rotation2);
          bones[2].Read(position3, rotation3);
          bones[3].Read(position4, rotation4);
        }
        else
        {
          bones[0].Read(position1, rotation1);
          bones[1].Read(position2, rotation2);
          bones[2].Read(position3, rotation3);
        }
      }

      public override void PreSolve()
      {
        if (target != null)
        {
          IKPosition = target.position;
          IKRotation = target.rotation;
        }
        footPosition = foot.solverPosition;
        footRotation = foot.solverRotation;
        position = lastBone.solverPosition;
        rotation = lastBone.solverRotation;
        if (rotationWeight > 0.0)
          ApplyRotationOffset(QuaTools.FromToRotation(rotation, IKRotation), rotationWeight);
        if (positionWeight > 0.0)
          ApplyPositionOffset(IKPosition - position, positionWeight);
        thighRelativeToPelvis = Quaternion.Inverse(rootRotation) * (thigh.solverPosition - rootPosition);
        calfRelToThigh = Quaternion.Inverse(thigh.solverRotation) * calf.solverRotation;
        bendNormal = Vector3.Cross(calf.solverPosition - thigh.solverPosition, foot.solverPosition - calf.solverPosition);
      }

      public override void ApplyOffsets()
      {
        ApplyPositionOffset(footPositionOffset, 1f);
        ApplyRotationOffset(footRotationOffset, 1f);
        Quaternion rotation = Quaternion.FromToRotation(footPosition - position, footPosition + heelPositionOffset - position);
        footPosition = position + rotation * (footPosition - position);
        footRotation = rotation * footRotation;
        float num = 0.0f;
        if (bendGoal != null && bendGoalWeight > 0.0)
        {
          Vector3 vector3_1 = Vector3.Cross(bendGoal.position - thigh.solverPosition, position - thigh.solverPosition);
          Vector3 vector3_2 = Quaternion.Inverse(Quaternion.LookRotation(bendNormal, thigh.solverPosition - foot.solverPosition)) * vector3_1;
          num = Mathf.Atan2(vector3_2.x, vector3_2.z) * 57.29578f * bendGoalWeight;
        }
        float angle = swivelOffset + num;
        if (angle == 0.0)
          return;
        bendNormal = Quaternion.AngleAxis(angle, thigh.solverPosition - lastBone.solverPosition) * bendNormal;
        thigh.solverRotation = Quaternion.AngleAxis(-angle, thigh.solverRotation * thigh.axis) * thigh.solverRotation;
      }

      private void ApplyPositionOffset(Vector3 offset, float weight)
      {
        if (weight <= 0.0)
          return;
        offset *= weight;
        footPosition += offset;
        position += offset;
      }

      private void ApplyRotationOffset(Quaternion offset, float weight)
      {
        if (weight <= 0.0)
          return;
        if (weight < 1.0)
          offset = Quaternion.Lerp(Quaternion.identity, offset, weight);
        footRotation = offset * footRotation;
        rotation = offset * rotation;
        bendNormal = offset * bendNormal;
        footPosition = position + offset * (footPosition - position);
      }

      public void Solve()
      {
        VirtualBone.SolveTrigonometric(bones, 0, 1, 2, footPosition, bendNormal, 1f);
        RotateTo(foot, footRotation);
        if (!hasToes)
          return;
        VirtualBone.SolveTrigonometric(bones, 0, 2, 3, position, Vector3.Cross(foot.solverPosition - thigh.solverPosition, toes.solverPosition - foot.solverPosition), 1f);
        Quaternion quaternion = thigh.solverRotation * calfRelToThigh;
        calf.solverRotation = Quaternion.FromToRotation(quaternion * calf.axis, foot.solverPosition - calf.solverPosition) * quaternion;
        toes.solverRotation = rotation;
      }

      public override void Write(ref Vector3[] solvedPositions, ref Quaternion[] solvedRotations)
      {
        solvedRotations[index] = thigh.solverRotation;
        solvedRotations[index + 1] = calf.solverRotation;
        solvedRotations[index + 2] = foot.solverRotation;
        if (!hasToes)
          return;
        solvedRotations[index + 3] = toes.solverRotation;
      }

      public override void ResetOffsets()
      {
        footPositionOffset = Vector3.zero;
        footRotationOffset = Quaternion.identity;
        heelPositionOffset = Vector3.zero;
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
      private Footstep[] footsteps = new Footstep[0];
      private Vector3 lastComPosition;
      private Vector3 comVelocity;
      private int leftFootIndex;
      private int rightFootIndex;

      public Vector3 centerOfMass { get; private set; }

      public void Initiate(Vector3[] positions, Quaternion[] rotations, bool hasToes)
      {
        leftFootIndex = hasToes ? 17 : 16;
        rightFootIndex = hasToes ? 21 : 20;
        footsteps = new Footstep[2]
        {
          new Footstep(rotations[0], positions[leftFootIndex], rotations[leftFootIndex], footDistance * Vector3.left),
          new Footstep(rotations[0], positions[rightFootIndex], rotations[rightFootIndex], footDistance * Vector3.right)
        };
      }

      public void Reset(Vector3[] positions, Quaternion[] rotations)
      {
        lastComPosition = Vector3.Lerp(positions[1], positions[5], 0.25f) + rotations[0] * offset;
        comVelocity = Vector3.zero;
        footsteps[0].Reset(rotations[0], positions[leftFootIndex], rotations[leftFootIndex]);
        footsteps[1].Reset(rotations[0], positions[rightFootIndex], rotations[rightFootIndex]);
      }

      public void AddDeltaRotation(Quaternion delta, Vector3 pivot)
      {
        Vector3 vector3_1 = lastComPosition - pivot;
        lastComPosition = pivot + delta * vector3_1;
        foreach (Footstep footstep in footsteps)
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
        lastComPosition += delta;
        foreach (Footstep footstep in footsteps)
        {
          footstep.position += delta;
          footstep.stepFrom += delta;
          footstep.stepTo += delta;
        }
      }

      public void Solve(
        VirtualBone rootBone,
        Spine spine,
        Leg leftLeg,
        Leg rightLeg,
        Arm leftArm,
        Arm rightArm,
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
        if (weight <= 0.0)
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
          footsteps[0].characterSpaceOffset = footDistance * Vector3.left;
          footsteps[1].characterSpaceOffset = footDistance * Vector3.right;
          Vector3 faceDirection = spine.faceDirection;
          Vector3 vertical = V3Tools.ExtractVertical(faceDirection, vector3_1, 1f);
          Quaternion quaternion = Quaternion.LookRotation(faceDirection - vertical, vector3_1);
          float num1 = 1f;
          float num2 = 1f;
          float num3 = 0.2f;
          float num4 = (float) (num1 + (double) num2 + 2.0 * num3);
          centerOfMass = Vector3.zero;
          centerOfMass += spine.pelvis.solverPosition * num1;
          centerOfMass += spine.head.solverPosition * num2;
          centerOfMass += leftArm.position * num3;
          centerOfMass += rightArm.position * num3;
          centerOfMass /= num4;
          centerOfMass += rootBone.solverRotation * offset;
          comVelocity = Time.deltaTime > 0.0 ? (centerOfMass - lastComPosition) / Time.deltaTime : Vector3.zero;
          lastComPosition = centerOfMass;
          comVelocity = Vector3.ClampMagnitude(comVelocity, maxVelocity) * velocityFactor;
          Vector3 point = centerOfMass + comVelocity;
          Vector3 plane1 = V3Tools.PointToPlane(spine.pelvis.solverPosition, rootBone.solverPosition, vector3_1);
          Vector3 plane2 = V3Tools.PointToPlane(point, rootBone.solverPosition, vector3_1);
          Vector3 vector3_4 = Vector3.Lerp(footsteps[0].position, footsteps[1].position, 0.5f);
          float num5 = Vector3.Angle(point - vector3_4, rootBone.solverRotation * Vector3.up) * comAngleMlp;
          for (int index = 0; index < footsteps.Length; ++index)
            footsteps[index].isSupportLeg = supportLegIndex == index;
          for (int index = 0; index < footsteps.Length; ++index)
          {
            if (footsteps[index].isStepping)
            {
              Vector3 vector3_5 = plane2 + rootBone.solverRotation * footsteps[index].characterSpaceOffset;
              if (!StepBlocked(footsteps[index].stepFrom, vector3_5, rootBone.solverPosition))
                footsteps[index].UpdateStepping(vector3_5, quaternion, 10f);
            }
            else
              footsteps[index].UpdateStanding(quaternion, relaxLegTwistMinAngle, relaxLegTwistSpeed);
          }
          if (CanStep())
          {
            int index1 = -1;
            float num6 = float.NegativeInfinity;
            for (int index2 = 0; index2 < footsteps.Length; ++index2)
            {
              if (!footsteps[index2].isStepping)
              {
                Vector3 vector3_6 = plane2 + rootBone.solverRotation * footsteps[index2].characterSpaceOffset;
                float num7 = index2 == 0 ? leftLeg.mag : rightLeg.mag;
                Vector3 b = index2 == 0 ? vector3_2 : vector3_3;
                float num8 = Vector3.Distance(footsteps[index2].position, b);
                bool flag1 = false;
                if (num8 >= num7 * (double) maxLegStretch)
                {
                  vector3_6 = plane1 + rootBone.solverRotation * footsteps[index2].characterSpaceOffset;
                  flag1 = true;
                }
                bool flag2 = false;
                for (int index3 = 0; index3 < footsteps.Length; ++index3)
                {
                  if (index3 != index2 && !flag1)
                  {
                    if (Vector3.Distance(footsteps[index2].position, footsteps[index3].position) >= 0.25 || (footsteps[index2].position - vector3_6).sqrMagnitude >= (double) (footsteps[index3].position - vector3_6).sqrMagnitude)
                      flag2 = GetLineSphereCollision(footsteps[index2].position, vector3_6, footsteps[index3].position, 0.25f);
                    if (flag2)
                      break;
                  }
                }
                float num9 = Quaternion.Angle(quaternion, footsteps[index2].stepToRootRot);
                if (!flag2 || num9 > (double) angleThreshold)
                {
                  float num10 = Vector3.Distance(footsteps[index2].position, vector3_6);
                  float num11 = Mathf.Lerp(stepThreshold, stepThreshold * 0.1f, num5 * 0.015f);
                  if (flag1)
                    num11 *= 0.5f;
                  if (index2 == 0)
                    num11 *= 0.9f;
                  if (!StepBlocked(footsteps[index2].position, vector3_6, rootBone.solverPosition) && (num10 > (double) num11 || num9 > (double) angleThreshold))
                  {
                    float num12 = 0.0f - num10;
                    if (num12 > (double) num6)
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
              Vector3 p = plane2 + rootBone.solverRotation * footsteps[index1].characterSpaceOffset;
              footsteps[index1].stepSpeed = Random.Range(stepSpeed, stepSpeed * 1.5f);
              footsteps[index1].StepTo(p, quaternion);
            }
          }
          footsteps[0].Update(stepInterpolation, onLeftFootstep);
          footsteps[1].Update(stepInterpolation, onRightFootstep);
          leftFootPosition = footsteps[0].position;
          rightFootPosition = footsteps[1].position;
          leftFootPosition = V3Tools.PointToPlane(leftFootPosition, leftLeg.lastBone.readPosition, vector3_1);
          rightFootPosition = V3Tools.PointToPlane(rightFootPosition, rightLeg.lastBone.readPosition, vector3_1);
          leftFootOffset = stepHeight.Evaluate(footsteps[0].stepProgress);
          rightFootOffset = stepHeight.Evaluate(footsteps[1].stepProgress);
          leftHeelOffset = heelHeight.Evaluate(footsteps[0].stepProgress);
          rightHeelOffset = heelHeight.Evaluate(footsteps[1].stepProgress);
          leftFootRotation = footsteps[0].rotation;
          rightFootRotation = footsteps[1].rotation;
        }
      }

      public Vector3 leftFootstepPosition => footsteps[0].position;

      public Vector3 rightFootstepPosition => footsteps[1].position;

      public Quaternion leftFootstepRotation => footsteps[0].rotation;

      public Quaternion rightFootstepRotation => footsteps[1].rotation;

      private bool StepBlocked(Vector3 fromPosition, Vector3 toPosition, Vector3 rootPosition)
      {
        if (blockingLayers == -1 || !blockingEnabled)
          return false;
        Vector3 origin = fromPosition with
        {
          y = rootPosition.y + raycastHeight + raycastRadius
        };
        Vector3 direction = (toPosition - origin) with
        {
          y = 0.0f
        };
        RaycastHit hitInfo;
        return raycastRadius <= 0.0 ? Physics.Raycast(origin, direction, out hitInfo, direction.magnitude, blockingLayers) : Physics.SphereCast(origin, raycastRadius, direction, out hitInfo, direction.magnitude, blockingLayers);
      }

      private bool CanStep()
      {
        foreach (Footstep footstep in footsteps)
        {
          if (footstep.isStepping && footstep.stepProgress < 0.800000011920929)
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
        if (num > (double) forward.magnitude)
          return false;
        Vector3 vector3 = Quaternion.Inverse(Quaternion.LookRotation(forward, upwards)) * upwards;
        return vector3.z < 0.0 ? num < 0.0 : vector3.y - (double) sphereRadius < 0.0;
      }
    }

    [Serializable]
    public class Spine : BodyPart
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

      public VirtualBone pelvis => bones[pelvisIndex];

      public VirtualBone firstSpineBone => bones[spineIndex];

      public VirtualBone chest
      {
        get => hasChest ? bones[chestIndex] : bones[spineIndex];
      }

      private VirtualBone neck => bones[neckIndex];

      public VirtualBone head => bones[headIndex];

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
        if (!initiated)
        {
          this.hasChest = hasChest;
          this.hasNeck = hasNeck;
          headHeight = V3Tools.ExtractVertical(position5 - positions[0], rotations[0] * Vector3.up, 1f).magnitude;
          int length = 3;
          if (hasChest)
            ++length;
          if (hasNeck)
            ++length;
          bones = new VirtualBone[length];
          chestIndex = hasChest ? 2 : 1;
          neckIndex = 1;
          if (hasChest)
            ++neckIndex;
          if (hasNeck)
            ++neckIndex;
          headIndex = 2;
          if (hasChest)
            ++headIndex;
          if (hasNeck)
            ++headIndex;
          bones[0] = new VirtualBone(position1, rotation1);
          bones[1] = new VirtualBone(position2, rotation2);
          if (hasChest)
            bones[chestIndex] = new VirtualBone(position3, rotation3);
          if (hasNeck)
            bones[neckIndex] = new VirtualBone(position4, rotation4);
          bones[headIndex] = new VirtualBone(position5, rotation5);
          pelvisRotationOffset = Quaternion.identity;
          chestRotationOffset = Quaternion.identity;
          headRotationOffset = Quaternion.identity;
          anchorRelativeToHead = Quaternion.Inverse(rotation5) * rotations[0];
          pelvisRelativeRotation = Quaternion.Inverse(rotation5) * rotation1;
          chestRelativeRotation = Quaternion.Inverse(rotation5) * rotation3;
          chestForward = Quaternion.Inverse(rotation3) * (rotations[0] * Vector3.forward);
          faceDirection = rotations[0] * Vector3.forward;
          IKPositionHead = position5;
          IKRotationHead = rotation5;
          IKPositionPelvis = position1;
          IKRotationPelvis = rotation1;
          goalPositionChest = position3 + rotations[0] * Vector3.forward;
        }
        bones[0].Read(position1, rotation1);
        bones[1].Read(position2, rotation2);
        if (hasChest)
          bones[chestIndex].Read(position3, rotation3);
        if (hasNeck)
          bones[neckIndex].Read(position4, rotation4);
        bones[headIndex].Read(position5, rotation5);
        sizeMlp = Vector3.Distance(position1, position5) / 0.7f;
      }

      public override void PreSolve()
      {
        if (headTarget != null)
        {
          IKPositionHead = headTarget.position;
          IKRotationHead = headTarget.rotation;
        }
        if (chestGoal != null)
          goalPositionChest = chestGoal.position;
        if (pelvisTarget != null)
        {
          IKPositionPelvis = pelvisTarget.position;
          IKRotationPelvis = pelvisTarget.rotation;
        }
        headPosition = V3Tools.Lerp(head.solverPosition, IKPositionHead, positionWeight);
        headRotation = QuaTools.Lerp(head.solverRotation, IKRotationHead, rotationWeight);
      }

      public override void ApplyOffsets()
      {
        headPosition += headPositionOffset;
        Vector3 vector3 = rootRotation * Vector3.up;
        if (vector3 == Vector3.up)
        {
          headPosition.y = Math.Max(rootPosition.y + minHeadHeight, headPosition.y);
        }
        else
        {
          Vector3 v = headPosition - rootPosition;
          Vector3 horizontal = V3Tools.ExtractHorizontal(v, vector3, 1f);
          Vector3 lhs = v - horizontal;
          if (Vector3.Dot(lhs, vector3) > 0.0)
          {
            if (lhs.magnitude < (double) minHeadHeight)
              lhs = lhs.normalized * minHeadHeight;
          }
          else
            lhs = -lhs.normalized * minHeadHeight;
          headPosition = rootPosition + horizontal + lhs;
        }
        headRotation = headRotationOffset * headRotation;
        headDeltaPosition = headPosition - head.solverPosition;
        pelvisDeltaRotation = QuaTools.FromToRotation(pelvis.solverRotation, headRotation * pelvisRelativeRotation);
        anchorRotation = headRotation * anchorRelativeToHead;
      }

      private void CalculateChestTargetRotation(
        VirtualBone rootBone,
        Arm[] arms)
      {
        chestTargetRotation = headRotation * chestRelativeRotation;
        AdjustChestByHands(ref chestTargetRotation, arms);
        faceDirection = Vector3.Cross(anchorRotation * Vector3.right, rootBone.readRotation * Vector3.up) + anchorRotation * Vector3.forward;
      }

      public void Solve(
        VirtualBone rootBone,
        Leg[] legs,
        Arm[] arms)
      {
        CalculateChestTargetRotation(rootBone, arms);
        if (this.maxRootAngle < 180.0)
        {
          Vector3 vector3 = Quaternion.Inverse(rootBone.solverRotation) * faceDirection;
          float num = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
          float angle = 0.0f;
          float maxRootAngle = this.maxRootAngle;
          if (num > (double) maxRootAngle)
            angle = num - maxRootAngle;
          if (num < -(double) maxRootAngle)
            angle = num + maxRootAngle;
          rootBone.solverRotation = Quaternion.AngleAxis(angle, rootBone.readRotation * Vector3.up) * rootBone.solverRotation;
        }
        Vector3 solverPosition = pelvis.solverPosition;
        Vector3 rootUp = rootBone.solverRotation * Vector3.up;
        TranslatePelvis(legs, headDeltaPosition, pelvisDeltaRotation);
        FABRIKPass(solverPosition, rootUp);
        Bend(bones, pelvisIndex, chestIndex, chestTargetRotation, chestRotationOffset, chestClampWeight, false, neckStiffness);
        if (chestGoalWeight > 0.0)
          Bend(bones, pelvisIndex, chestIndex, Quaternion.FromToRotation(bones[chestIndex].solverRotation * chestForward, goalPositionChest - bones[chestIndex].solverPosition) * bones[chestIndex].solverRotation, chestRotationOffset, chestClampWeight, false, chestGoalWeight);
        InverseTranslateToHead(legs, false, false, Vector3.zero, 1f);
        FABRIKPass(solverPosition, rootUp);
        Bend(bones, neckIndex, headIndex, headRotation, headClampWeight, true, 1f);
        SolvePelvis();
      }

      private void FABRIKPass(Vector3 animatedPelvisPos, Vector3 rootUp)
      {
        VirtualBone.SolveFABRIK(bones, Vector3.Lerp(pelvis.solverPosition, animatedPelvisPos, maintainPelvisPosition) + pelvisPositionOffset - chestPositionOffset, headPosition - chestPositionOffset, 1f, 1f, 1, mag, rootUp * (bones[bones.Length - 1].solverPosition - bones[0].solverPosition).magnitude);
      }

      private void SolvePelvis()
      {
        if (pelvisPositionWeight <= 0.0)
          return;
        Quaternion solverRotation = head.solverRotation;
        Vector3 vector3 = (IKPositionPelvis + pelvisPositionOffset - pelvis.solverPosition) * pelvisPositionWeight;
        foreach (VirtualBone bone in bones)
          bone.solverPosition += vector3;
        Vector3 bendNormal = anchorRotation * Vector3.right;
        if (hasChest && hasNeck)
        {
          VirtualBone.SolveTrigonometric(bones, pelvisIndex, spineIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 0.6f);
          VirtualBone.SolveTrigonometric(bones, spineIndex, chestIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 0.6f);
          VirtualBone.SolveTrigonometric(bones, chestIndex, neckIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 1f);
        }
        else if (hasChest && !hasNeck)
        {
          VirtualBone.SolveTrigonometric(bones, pelvisIndex, spineIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 0.75f);
          VirtualBone.SolveTrigonometric(bones, spineIndex, chestIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 1f);
        }
        else if (!hasChest && hasNeck)
        {
          VirtualBone.SolveTrigonometric(bones, pelvisIndex, spineIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 0.75f);
          VirtualBone.SolveTrigonometric(bones, spineIndex, neckIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 1f);
        }
        else if (!hasNeck && !hasChest)
          VirtualBone.SolveTrigonometric(bones, pelvisIndex, spineIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight);
        head.solverRotation = solverRotation;
      }

      public override void Write(ref Vector3[] solvedPositions, ref Quaternion[] solvedRotations)
      {
        solvedPositions[index] = bones[0].solverPosition;
        solvedRotations[index] = bones[0].solverRotation;
        solvedRotations[index + 1] = bones[1].solverRotation;
        if (hasChest)
          solvedRotations[index + 2] = bones[chestIndex].solverRotation;
        if (hasNeck)
          solvedRotations[index + 3] = bones[neckIndex].solverRotation;
        solvedRotations[index + 4] = bones[headIndex].solverRotation;
      }

      public override void ResetOffsets()
      {
        pelvisPositionOffset = Vector3.zero;
        chestPositionOffset = Vector3.zero;
        headPositionOffset = locomotionHeadPositionOffset;
        pelvisRotationOffset = Quaternion.identity;
        chestRotationOffset = Quaternion.identity;
        headRotationOffset = Quaternion.identity;
      }

      private void AdjustChestByHands(ref Quaternion chestTargetRotation, Arm[] arms)
      {
        Quaternion quaternion1 = Quaternion.Inverse(anchorRotation);
        Vector3 vector3_1 = quaternion1 * (arms[0].position - headPosition) / sizeMlp;
        Vector3 vector3_2 = quaternion1 * (arms[1].position - headPosition) / sizeMlp;
        Vector3 forward = Vector3.forward;
        forward.x += vector3_1.x * Mathf.Abs(vector3_1.x);
        forward.x += vector3_1.z * Mathf.Abs(vector3_1.z);
        forward.x += vector3_2.x * Mathf.Abs(vector3_2.x);
        forward.x -= vector3_2.z * Mathf.Abs(vector3_2.z);
        forward.x *= 5f * rotateChestByHands;
        Quaternion quaternion2 = Quaternion.AngleAxis(Mathf.Atan2(forward.x, forward.z) * 57.29578f, rootRotation * Vector3.up);
        chestTargetRotation = quaternion2 * chestTargetRotation;
        Vector3 up = Vector3.up;
        up.x += vector3_1.y;
        up.x -= vector3_2.y;
        up.x *= 0.5f * rotateChestByHands;
        Quaternion quaternion3 = Quaternion.AngleAxis(Mathf.Atan2(up.x, up.y) * 57.29578f, rootRotation * Vector3.back);
        chestTargetRotation = quaternion3 * chestTargetRotation;
      }

      public void InverseTranslateToHead(
        Leg[] legs,
        bool limited,
        bool useCurrentLegMag,
        Vector3 offset,
        float w)
      {
        Vector3 pelvisPosition = pelvis.solverPosition + (headPosition + offset - head.solverPosition) * w * (1f - pelvisPositionWeight);
        MovePosition(limited ? LimitPelvisPosition(legs, pelvisPosition, useCurrentLegMag) : pelvisPosition);
      }

      private void TranslatePelvis(
        Leg[] legs,
        Vector3 deltaPosition,
        Quaternion deltaRotation)
      {
        Vector3 solverPosition = head.solverPosition;
        deltaRotation = QuaTools.ClampRotation(deltaRotation, chestClampWeight, 2);
        VirtualBone.RotateAroundPoint(bones, 0, pelvis.solverPosition, pelvisRotationOffset * Quaternion.Slerp(Quaternion.Slerp(Quaternion.identity, deltaRotation, bodyRotStiffness), QuaTools.FromToRotation(pelvis.solverRotation, IKRotationPelvis), pelvisRotationWeight));
        deltaPosition -= head.solverPosition - solverPosition;
        Vector3 vector3 = rootRotation * Vector3.forward;
        float num = V3Tools.ExtractVertical(deltaPosition, rootRotation * Vector3.up, 1f).magnitude * -moveBodyBackWhenCrouching * headHeight;
        deltaPosition += vector3 * num;
        MovePosition(LimitPelvisPosition(legs, pelvis.solverPosition + deltaPosition * bodyPosStiffness, false));
      }

      private Vector3 LimitPelvisPosition(
        Leg[] legs,
        Vector3 pelvisPosition,
        bool useCurrentLegMag,
        int it = 2)
      {
        if (useCurrentLegMag)
        {
          foreach (Leg leg in legs)
            leg.currentMag = Vector3.Distance(leg.thigh.solverPosition, leg.lastBone.solverPosition);
        }
        for (int index = 0; index < it; ++index)
        {
          foreach (Leg leg in legs)
          {
            Vector3 vector3_1 = pelvisPosition - pelvis.solverPosition;
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
        VirtualBone[] bones,
        int firstIndex,
        int lastIndex,
        Quaternion targetRotation,
        float clampWeight,
        bool uniformWeight,
        float w)
      {
        if (w <= 0.0 || bones.Length == 0)
          return;
        int num1 = lastIndex + 1 - firstIndex;
        if (num1 < 1)
          return;
        Quaternion b = QuaTools.ClampRotation(QuaTools.FromToRotation(bones[lastIndex].solverRotation, targetRotation), clampWeight, 2);
        float num2 = uniformWeight ? 1f / num1 : 0.0f;
        for (int index = firstIndex; index < lastIndex + 1; ++index)
        {
          if (!uniformWeight)
            num2 = Mathf.Clamp((index - firstIndex + 1) / num1, 0.0f, 1f);
          VirtualBone.RotateAroundPoint(bones, index, bones[index].solverPosition, Quaternion.Slerp(Quaternion.identity, b, num2 * w));
        }
      }

      private void Bend(
        VirtualBone[] bones,
        int firstIndex,
        int lastIndex,
        Quaternion targetRotation,
        Quaternion rotationOffset,
        float clampWeight,
        bool uniformWeight,
        float w)
      {
        if (w <= 0.0 || bones.Length == 0)
          return;
        int num = lastIndex + 1 - firstIndex;
        if (num < 1)
          return;
        Quaternion b = QuaTools.ClampRotation(QuaTools.FromToRotation(bones[lastIndex].solverRotation, targetRotation), clampWeight, 2);
        float t = uniformWeight ? 1f / num : 0.0f;
        for (int index = firstIndex; index < lastIndex + 1; ++index)
        {
          if (!uniformWeight)
            t = Mathf.Clamp((index - firstIndex + 1) / num, 0.0f, 1f);
          VirtualBone.RotateAroundPoint(bones, index, bones[index].solverPosition, Quaternion.Slerp(Quaternion.Slerp(Quaternion.identity, rotationOffset, t), b, t * w));
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

      public VirtualBone(Vector3 position, Quaternion rotation) => Read(position, rotation);

      public void Read(Vector3 position, Quaternion rotation)
      {
        readPosition = position;
        readRotation = rotation;
        solverPosition = position;
        solverRotation = rotation;
      }

      public static void SwingRotation(
        VirtualBone[] bones,
        int index,
        Vector3 swingTarget,
        float weight = 1f)
      {
        if (weight <= 0.0)
          return;
        Quaternion b = Quaternion.FromToRotation(bones[index].solverRotation * bones[index].axis, swingTarget - bones[index].solverPosition);
        if (weight < 1.0)
          b = Quaternion.Lerp(Quaternion.identity, b, weight);
        for (int index1 = index; index1 < bones.Length; ++index1)
          bones[index1].solverRotation = b * bones[index1].solverRotation;
      }

      public static float PreSolve(ref VirtualBone[] bones)
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
        VirtualBone[] bones,
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

      public static void RotateBy(VirtualBone[] bones, int index, Quaternion rotation)
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

      public static void RotateBy(VirtualBone[] bones, Quaternion rotation)
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

      public static void RotateTo(VirtualBone[] bones, int index, Quaternion rotation)
      {
        Quaternion rotation1 = QuaTools.FromToRotation(bones[index].solverRotation, rotation);
        RotateAroundPoint(bones, index, bones[index].solverPosition, rotation1);
      }

      public static void SolveTrigonometric(
        VirtualBone[] bones,
        int first,
        int second,
        int third,
        Vector3 targetPosition,
        Vector3 bendNormal,
        float weight)
      {
        if (weight <= 0.0)
          return;
        targetPosition = Vector3.Lerp(bones[third].solverPosition, targetPosition, weight);
        Vector3 vector3 = targetPosition - bones[first].solverPosition;
        float sqrMagnitude1 = vector3.sqrMagnitude;
        if (sqrMagnitude1 == 0.0)
          return;
        float directionMag = Mathf.Sqrt(sqrMagnitude1);
        float sqrMagnitude2 = (bones[second].solverPosition - bones[first].solverPosition).sqrMagnitude;
        float sqrMagnitude3 = (bones[third].solverPosition - bones[second].solverPosition).sqrMagnitude;
        Vector3 bendDirection = Vector3.Cross(vector3, bendNormal);
        Vector3 directionToBendPoint = GetDirectionToBendPoint(vector3, directionMag, bendDirection, sqrMagnitude2, sqrMagnitude3);
        Quaternion quaternion1 = Quaternion.FromToRotation(bones[second].solverPosition - bones[first].solverPosition, directionToBendPoint);
        if (weight < 1.0)
          quaternion1 = Quaternion.Lerp(Quaternion.identity, quaternion1, weight);
        RotateAroundPoint(bones, first, bones[first].solverPosition, quaternion1);
        Quaternion quaternion2 = Quaternion.FromToRotation(bones[third].solverPosition - bones[second].solverPosition, targetPosition - bones[second].solverPosition);
        if (weight < 1.0)
          quaternion2 = Quaternion.Lerp(Quaternion.identity, quaternion2, weight);
        RotateAroundPoint(bones, second, bones[second].solverPosition, quaternion2);
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

      public static void SolveFABRIK(
        VirtualBone[] bones,
        Vector3 startPosition,
        Vector3 targetPosition,
        float weight,
        float minNormalizedTargetDistance,
        int iterations,
        float length,
        Vector3 startOffset)
      {
        if (weight <= 0.0)
          return;
        if (minNormalizedTargetDistance > 0.0)
        {
          Vector3 vector3 = targetPosition - startPosition;
          float magnitude = vector3.magnitude;
          targetPosition = startPosition + vector3 / magnitude * Mathf.Max(length * minNormalizedTargetDistance, magnitude);
        }
        foreach (VirtualBone bone in bones)
          bone.solverPosition += startOffset;
        for (int index1 = 0; index1 < iterations; ++index1)
        {
          bones[bones.Length - 1].solverPosition = Vector3.Lerp(bones[bones.Length - 1].solverPosition, targetPosition, weight);
          for (int index2 = bones.Length - 2; index2 > -1; --index2)
            bones[index2].solverPosition = SolveFABRIKJoint(bones[index2].solverPosition, bones[index2 + 1].solverPosition, bones[index2].length);
          bones[0].solverPosition = startPosition;
          for (int index3 = 1; index3 < bones.Length; ++index3)
            bones[index3].solverPosition = SolveFABRIKJoint(bones[index3].solverPosition, bones[index3 - 1].solverPosition, bones[index3 - 1].length);
        }
        for (int index = 0; index < bones.Length - 1; ++index)
          SwingRotation(bones, index, bones[index + 1].solverPosition);
      }

      private static Vector3 SolveFABRIKJoint(Vector3 pos1, Vector3 pos2, float length)
      {
        return pos2 + (pos1 - pos2).normalized * length;
      }

      public static void SolveCCD(
        VirtualBone[] bones,
        Vector3 targetPosition,
        float weight,
        int iterations)
      {
        if (weight <= 0.0)
          return;
        for (int index1 = 0; index1 < iterations; ++index1)
        {
          for (int index2 = bones.Length - 2; index2 > -1; --index2)
          {
            Quaternion rotation = Quaternion.FromToRotation(bones[bones.Length - 1].solverPosition - bones[index2].solverPosition, targetPosition - bones[index2].solverPosition);
            if (weight >= 1.0)
              RotateBy(bones, index2, rotation);
            else
              RotateBy(bones, index2, Quaternion.Lerp(Quaternion.identity, rotation, weight));
          }
        }
      }
    }
  }
}
