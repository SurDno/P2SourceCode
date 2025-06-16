using System;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverFullBodyBiped : IKSolverFullBody
  {
    public Transform rootNode;
    [Range(0.0f, 1f)]
    public float spineStiffness = 0.5f;
    [Range(-1f, 1f)]
    public float pullBodyVertical = 0.5f;
    [Range(-1f, 1f)]
    public float pullBodyHorizontal;
    private Vector3 offset;

    public IKEffector bodyEffector => GetEffector(FullBodyBipedEffector.Body);

    public IKEffector leftShoulderEffector => GetEffector(FullBodyBipedEffector.LeftShoulder);

    public IKEffector rightShoulderEffector
    {
      get => GetEffector(FullBodyBipedEffector.RightShoulder);
    }

    public IKEffector leftThighEffector => GetEffector(FullBodyBipedEffector.LeftThigh);

    public IKEffector rightThighEffector => GetEffector(FullBodyBipedEffector.RightThigh);

    public IKEffector leftHandEffector => GetEffector(FullBodyBipedEffector.LeftHand);

    public IKEffector rightHandEffector => GetEffector(FullBodyBipedEffector.RightHand);

    public IKEffector leftFootEffector => GetEffector(FullBodyBipedEffector.LeftFoot);

    public IKEffector rightFootEffector => GetEffector(FullBodyBipedEffector.RightFoot);

    public FBIKChain leftArmChain => chain[1];

    public FBIKChain rightArmChain => chain[2];

    public FBIKChain leftLegChain => chain[3];

    public FBIKChain rightLegChain => chain[4];

    public IKMappingLimb leftArmMapping => limbMappings[0];

    public IKMappingLimb rightArmMapping => limbMappings[1];

    public IKMappingLimb leftLegMapping => limbMappings[2];

    public IKMappingLimb rightLegMapping => limbMappings[3];

    public IKMappingBone headMapping => boneMappings[0];

    public void SetChainWeights(FullBodyBipedChain c, float pull, float reach = 0.0f)
    {
      GetChain(c).pull = pull;
      GetChain(c).reach = reach;
    }

    public void SetEffectorWeights(
      FullBodyBipedEffector effector,
      float positionWeight,
      float rotationWeight)
    {
      GetEffector(effector).positionWeight = Mathf.Clamp(positionWeight, 0.0f, 1f);
      GetEffector(effector).rotationWeight = Mathf.Clamp(rotationWeight, 0.0f, 1f);
    }

    public FBIKChain GetChain(FullBodyBipedChain c)
    {
      switch (c)
      {
        case FullBodyBipedChain.LeftArm:
          return chain[1];
        case FullBodyBipedChain.RightArm:
          return chain[2];
        case FullBodyBipedChain.LeftLeg:
          return chain[3];
        case FullBodyBipedChain.RightLeg:
          return chain[4];
        default:
          return null;
      }
    }

    public FBIKChain GetChain(FullBodyBipedEffector effector)
    {
      switch (effector)
      {
        case FullBodyBipedEffector.Body:
          return chain[0];
        case FullBodyBipedEffector.LeftShoulder:
          return chain[1];
        case FullBodyBipedEffector.RightShoulder:
          return chain[2];
        case FullBodyBipedEffector.LeftThigh:
          return chain[3];
        case FullBodyBipedEffector.RightThigh:
          return chain[4];
        case FullBodyBipedEffector.LeftHand:
          return chain[1];
        case FullBodyBipedEffector.RightHand:
          return chain[2];
        case FullBodyBipedEffector.LeftFoot:
          return chain[3];
        case FullBodyBipedEffector.RightFoot:
          return chain[4];
        default:
          return null;
      }
    }

    public IKEffector GetEffector(FullBodyBipedEffector effector)
    {
      switch (effector)
      {
        case FullBodyBipedEffector.Body:
          return effectors[0];
        case FullBodyBipedEffector.LeftShoulder:
          return effectors[1];
        case FullBodyBipedEffector.RightShoulder:
          return effectors[2];
        case FullBodyBipedEffector.LeftThigh:
          return effectors[3];
        case FullBodyBipedEffector.RightThigh:
          return effectors[4];
        case FullBodyBipedEffector.LeftHand:
          return effectors[5];
        case FullBodyBipedEffector.RightHand:
          return effectors[6];
        case FullBodyBipedEffector.LeftFoot:
          return effectors[7];
        case FullBodyBipedEffector.RightFoot:
          return effectors[8];
        default:
          return null;
      }
    }

    public IKEffector GetEndEffector(FullBodyBipedChain c)
    {
      switch (c)
      {
        case FullBodyBipedChain.LeftArm:
          return effectors[5];
        case FullBodyBipedChain.RightArm:
          return effectors[6];
        case FullBodyBipedChain.LeftLeg:
          return effectors[7];
        case FullBodyBipedChain.RightLeg:
          return effectors[8];
        default:
          return null;
      }
    }

    public IKMappingLimb GetLimbMapping(FullBodyBipedChain chain)
    {
      switch (chain)
      {
        case FullBodyBipedChain.LeftArm:
          return limbMappings[0];
        case FullBodyBipedChain.RightArm:
          return limbMappings[1];
        case FullBodyBipedChain.LeftLeg:
          return limbMappings[2];
        case FullBodyBipedChain.RightLeg:
          return limbMappings[3];
        default:
          return null;
      }
    }

    public IKMappingLimb GetLimbMapping(FullBodyBipedEffector effector)
    {
      switch (effector)
      {
        case FullBodyBipedEffector.LeftShoulder:
          return limbMappings[0];
        case FullBodyBipedEffector.RightShoulder:
          return limbMappings[1];
        case FullBodyBipedEffector.LeftThigh:
          return limbMappings[2];
        case FullBodyBipedEffector.RightThigh:
          return limbMappings[3];
        case FullBodyBipedEffector.LeftHand:
          return limbMappings[0];
        case FullBodyBipedEffector.RightHand:
          return limbMappings[1];
        case FullBodyBipedEffector.LeftFoot:
          return limbMappings[2];
        case FullBodyBipedEffector.RightFoot:
          return limbMappings[3];
        default:
          return null;
      }
    }

    public IKMappingSpine GetSpineMapping() => spineMapping;

    public IKMappingBone GetHeadMapping() => boneMappings[0];

    public IKConstraintBend GetBendConstraint(FullBodyBipedChain limb)
    {
      switch (limb)
      {
        case FullBodyBipedChain.LeftArm:
          return chain[1].bendConstraint;
        case FullBodyBipedChain.RightArm:
          return chain[2].bendConstraint;
        case FullBodyBipedChain.LeftLeg:
          return chain[3].bendConstraint;
        case FullBodyBipedChain.RightLeg:
          return chain[4].bendConstraint;
        default:
          return null;
      }
    }

    public override bool IsValid(ref string message)
    {
      if (!base.IsValid(ref message))
        return false;
      if ((UnityEngine.Object) rootNode == (UnityEngine.Object) null)
      {
        message = "Root Node bone is null. FBBIK will not initiate.";
        return false;
      }
      if (chain.Length == 5 && chain[0].nodes.Length == 1 && chain[1].nodes.Length == 3 && chain[2].nodes.Length == 3 && chain[3].nodes.Length == 3 && chain[4].nodes.Length == 3 && effectors.Length == 9 && limbMappings.Length == 4)
        return true;
      message = "Invalid FBBIK setup. Please right-click on the component header and select 'Reinitiate'.";
      return false;
    }

    public void SetToReferences(BipedReferences references, Transform rootNode = null)
    {
      root = references.root;
      if ((UnityEngine.Object) rootNode == (UnityEngine.Object) null)
        rootNode = DetectRootNodeBone(references);
      this.rootNode = rootNode;
      if (chain == null || chain.Length != 5)
        chain = new FBIKChain[5];
      for (int index = 0; index < chain.Length; ++index)
      {
        if (chain[index] == null)
          chain[index] = new FBIKChain();
      }
      chain[0].pin = 0.0f;
      chain[0].SetNodes(rootNode);
      chain[0].children = new int[4]{ 1, 2, 3, 4 };
      chain[1].SetNodes(references.leftUpperArm, references.leftForearm, references.leftHand);
      chain[2].SetNodes(references.rightUpperArm, references.rightForearm, references.rightHand);
      chain[3].SetNodes(references.leftThigh, references.leftCalf, references.leftFoot);
      chain[4].SetNodes(references.rightThigh, references.rightCalf, references.rightFoot);
      if (effectors.Length != 9)
        effectors = new IKEffector[9]
        {
          new IKEffector(),
          new IKEffector(),
          new IKEffector(),
          new IKEffector(),
          new IKEffector(),
          new IKEffector(),
          new IKEffector(),
          new IKEffector(),
          new IKEffector()
        };
      effectors[0].bone = rootNode;
      effectors[0].childBones = new Transform[2]
      {
        references.leftThigh,
        references.rightThigh
      };
      effectors[1].bone = references.leftUpperArm;
      effectors[2].bone = references.rightUpperArm;
      effectors[3].bone = references.leftThigh;
      effectors[4].bone = references.rightThigh;
      effectors[5].bone = references.leftHand;
      effectors[6].bone = references.rightHand;
      effectors[7].bone = references.leftFoot;
      effectors[8].bone = references.rightFoot;
      effectors[5].planeBone1 = references.leftUpperArm;
      effectors[5].planeBone2 = references.rightUpperArm;
      effectors[5].planeBone3 = rootNode;
      effectors[6].planeBone1 = references.rightUpperArm;
      effectors[6].planeBone2 = references.leftUpperArm;
      effectors[6].planeBone3 = rootNode;
      effectors[7].planeBone1 = references.leftThigh;
      effectors[7].planeBone2 = references.rightThigh;
      effectors[7].planeBone3 = rootNode;
      effectors[8].planeBone1 = references.rightThigh;
      effectors[8].planeBone2 = references.leftThigh;
      effectors[8].planeBone3 = rootNode;
      chain[0].childConstraints = new FBIKChain.ChildConstraint[4]
      {
        new FBIKChain.ChildConstraint(references.leftUpperArm, references.rightThigh, pullElasticity: 1f),
        new FBIKChain.ChildConstraint(references.rightUpperArm, references.leftThigh, pullElasticity: 1f),
        new FBIKChain.ChildConstraint(references.leftUpperArm, references.rightUpperArm),
        new FBIKChain.ChildConstraint(references.leftThigh, references.rightThigh)
      };
      Transform[] spineBones = new Transform[references.spine.Length + 1];
      spineBones[0] = references.pelvis;
      for (int index = 0; index < references.spine.Length; ++index)
        spineBones[index + 1] = references.spine[index];
      if (spineMapping == null)
      {
        spineMapping = new IKMappingSpine();
        spineMapping.iterations = 3;
      }
      spineMapping.SetBones(spineBones, references.leftUpperArm, references.rightUpperArm, references.leftThigh, references.rightThigh);
      int length = (UnityEngine.Object) references.head != (UnityEngine.Object) null ? 1 : 0;
      if (boneMappings.Length != length)
      {
        boneMappings = new IKMappingBone[length];
        for (int index = 0; index < boneMappings.Length; ++index)
          boneMappings[index] = new IKMappingBone();
        if (length == 1)
          boneMappings[0].maintainRotationWeight = 0.0f;
      }
      if (boneMappings.Length != 0)
        boneMappings[0].bone = references.head;
      if (limbMappings.Length != 4)
      {
        limbMappings = new IKMappingLimb[4]
        {
          new IKMappingLimb(),
          new IKMappingLimb(),
          new IKMappingLimb(),
          new IKMappingLimb()
        };
        limbMappings[2].maintainRotationWeight = 1f;
        limbMappings[3].maintainRotationWeight = 1f;
      }
      limbMappings[0].SetBones(references.leftUpperArm, references.leftForearm, references.leftHand, GetLeftClavicle(references));
      limbMappings[1].SetBones(references.rightUpperArm, references.rightForearm, references.rightHand, GetRightClavicle(references));
      limbMappings[2].SetBones(references.leftThigh, references.leftCalf, references.leftFoot);
      limbMappings[3].SetBones(references.rightThigh, references.rightCalf, references.rightFoot);
      if (!Application.isPlaying)
        return;
      Initiate(references.root);
    }

    public static Transform DetectRootNodeBone(BipedReferences references)
    {
      if (!references.isFilled || references.spine.Length < 1)
        return (Transform) null;
      int length = references.spine.Length;
      if (length == 1)
        return references.spine[0];
      Vector3 vector3_1 = Vector3.Lerp(references.leftThigh.position, references.rightThigh.position, 0.5f);
      Vector3 onNormal = Vector3.Lerp(references.leftUpperArm.position, references.rightUpperArm.position, 0.5f) - vector3_1;
      float magnitude = onNormal.magnitude;
      if (references.spine.Length < 2)
        return references.spine[0];
      int index1 = 0;
      for (int index2 = 1; index2 < length; ++index2)
      {
        Vector3 vector3_2 = Vector3.Project(references.spine[index2].position - vector3_1, onNormal);
        if ((double) Vector3.Dot(vector3_2.normalized, onNormal.normalized) > 0.0 && vector3_2.magnitude / magnitude < 0.5)
          index1 = index2;
      }
      return references.spine[index1];
    }

    public void SetLimbOrientations(BipedLimbOrientations o)
    {
      SetLimbOrientation(FullBodyBipedChain.LeftArm, o.leftArm);
      SetLimbOrientation(FullBodyBipedChain.RightArm, o.rightArm);
      SetLimbOrientation(FullBodyBipedChain.LeftLeg, o.leftLeg);
      SetLimbOrientation(FullBodyBipedChain.RightLeg, o.rightLeg);
    }

    public Vector3 pullBodyOffset { get; private set; }

    private void SetLimbOrientation(
      FullBodyBipedChain chain,
      BipedLimbOrientations.LimbOrientation limbOrientation)
    {
      if (chain == FullBodyBipedChain.LeftArm || chain == FullBodyBipedChain.RightArm)
      {
        GetBendConstraint(chain).SetLimbOrientation(-limbOrientation.upperBoneForwardAxis, -limbOrientation.lowerBoneForwardAxis, -limbOrientation.lastBoneLeftAxis);
        GetLimbMapping(chain).SetLimbOrientation(-limbOrientation.upperBoneForwardAxis, -limbOrientation.lowerBoneForwardAxis);
      }
      else
      {
        GetBendConstraint(chain).SetLimbOrientation(limbOrientation.upperBoneForwardAxis, limbOrientation.lowerBoneForwardAxis, limbOrientation.lastBoneLeftAxis);
        GetLimbMapping(chain).SetLimbOrientation(limbOrientation.upperBoneForwardAxis, limbOrientation.lowerBoneForwardAxis);
      }
    }

    private static Transform GetLeftClavicle(BipedReferences references)
    {
      return (UnityEngine.Object) references.leftUpperArm == (UnityEngine.Object) null || Contains(references.spine, references.leftUpperArm.parent) ? (Transform) null : references.leftUpperArm.parent;
    }

    private static Transform GetRightClavicle(BipedReferences references)
    {
      return (UnityEngine.Object) references.rightUpperArm == (UnityEngine.Object) null || Contains(references.spine, references.rightUpperArm.parent) ? (Transform) null : references.rightUpperArm.parent;
    }

    private static bool Contains(Transform[] array, Transform transform)
    {
      foreach (UnityEngine.Object @object in array)
      {
        if (@object == (UnityEngine.Object) transform)
          return true;
      }
      return false;
    }

    protected override void ReadPose()
    {
      for (int index = 0; index < effectors.Length; ++index)
        effectors[index].SetToTarget();
      PullBody();
      float num = Mathf.Clamp(1f - spineStiffness, 0.0f, 1f);
      chain[0].childConstraints[0].pushElasticity = num;
      chain[0].childConstraints[1].pushElasticity = num;
      base.ReadPose();
    }

    private void PullBody()
    {
      if (iterations < 1 || pullBodyVertical == 0.0 && pullBodyHorizontal == 0.0)
        return;
      Vector3 bodyOffset = GetBodyOffset();
      pullBodyOffset = V3Tools.ExtractVertical(bodyOffset, root.up, pullBodyVertical) + V3Tools.ExtractHorizontal(bodyOffset, root.up, pullBodyHorizontal);
      bodyEffector.positionOffset += pullBodyOffset;
    }

    private Vector3 GetBodyOffset()
    {
      Vector3 offset = Vector3.zero + GetHandBodyPull(leftHandEffector, leftArmChain, Vector3.zero) * Mathf.Clamp(leftHandEffector.positionWeight, 0.0f, 1f);
      return offset + GetHandBodyPull(rightHandEffector, rightArmChain, offset) * Mathf.Clamp(rightHandEffector.positionWeight, 0.0f, 1f);
    }

    private Vector3 GetHandBodyPull(IKEffector effector, FBIKChain arm, Vector3 offset)
    {
      Vector3 vector3 = effector.position - (arm.nodes[0].transform.position + offset);
      float num1 = arm.nodes[0].length + arm.nodes[1].length;
      float magnitude = vector3.magnitude;
      if (magnitude < (double) num1)
        return Vector3.zero;
      float num2 = magnitude - num1;
      return vector3 / magnitude * num2;
    }

    protected override void ApplyBendConstraints()
    {
      if (iterations > 0)
      {
        chain[1].bendConstraint.rotationOffset = leftHandEffector.planeRotationOffset;
        chain[2].bendConstraint.rotationOffset = rightHandEffector.planeRotationOffset;
        chain[3].bendConstraint.rotationOffset = leftFootEffector.planeRotationOffset;
        chain[4].bendConstraint.rotationOffset = rightFootEffector.planeRotationOffset;
      }
      else
      {
        offset = Vector3.Lerp(effectors[0].positionOffset, effectors[0].position - (effectors[0].bone.position + effectors[0].positionOffset), effectors[0].positionWeight);
        for (int index = 0; index < 5; ++index)
        {
          Node node = effectors[index].GetNode(this);
          node.solverPosition = node.solverPosition + offset;
        }
      }
      base.ApplyBendConstraints();
    }

    protected override void WritePose()
    {
      if (iterations == 0)
        spineMapping.spineBones[0].position += offset;
      base.WritePose();
    }
  }
}
