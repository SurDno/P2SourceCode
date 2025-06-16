// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IKSolverFullBodyBiped
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
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
    public float pullBodyHorizontal = 0.0f;
    private Vector3 offset;

    public IKEffector bodyEffector => this.GetEffector(FullBodyBipedEffector.Body);

    public IKEffector leftShoulderEffector => this.GetEffector(FullBodyBipedEffector.LeftShoulder);

    public IKEffector rightShoulderEffector
    {
      get => this.GetEffector(FullBodyBipedEffector.RightShoulder);
    }

    public IKEffector leftThighEffector => this.GetEffector(FullBodyBipedEffector.LeftThigh);

    public IKEffector rightThighEffector => this.GetEffector(FullBodyBipedEffector.RightThigh);

    public IKEffector leftHandEffector => this.GetEffector(FullBodyBipedEffector.LeftHand);

    public IKEffector rightHandEffector => this.GetEffector(FullBodyBipedEffector.RightHand);

    public IKEffector leftFootEffector => this.GetEffector(FullBodyBipedEffector.LeftFoot);

    public IKEffector rightFootEffector => this.GetEffector(FullBodyBipedEffector.RightFoot);

    public FBIKChain leftArmChain => this.chain[1];

    public FBIKChain rightArmChain => this.chain[2];

    public FBIKChain leftLegChain => this.chain[3];

    public FBIKChain rightLegChain => this.chain[4];

    public IKMappingLimb leftArmMapping => this.limbMappings[0];

    public IKMappingLimb rightArmMapping => this.limbMappings[1];

    public IKMappingLimb leftLegMapping => this.limbMappings[2];

    public IKMappingLimb rightLegMapping => this.limbMappings[3];

    public IKMappingBone headMapping => this.boneMappings[0];

    public void SetChainWeights(FullBodyBipedChain c, float pull, float reach = 0.0f)
    {
      this.GetChain(c).pull = pull;
      this.GetChain(c).reach = reach;
    }

    public void SetEffectorWeights(
      FullBodyBipedEffector effector,
      float positionWeight,
      float rotationWeight)
    {
      this.GetEffector(effector).positionWeight = Mathf.Clamp(positionWeight, 0.0f, 1f);
      this.GetEffector(effector).rotationWeight = Mathf.Clamp(rotationWeight, 0.0f, 1f);
    }

    public FBIKChain GetChain(FullBodyBipedChain c)
    {
      switch (c)
      {
        case FullBodyBipedChain.LeftArm:
          return this.chain[1];
        case FullBodyBipedChain.RightArm:
          return this.chain[2];
        case FullBodyBipedChain.LeftLeg:
          return this.chain[3];
        case FullBodyBipedChain.RightLeg:
          return this.chain[4];
        default:
          return (FBIKChain) null;
      }
    }

    public FBIKChain GetChain(FullBodyBipedEffector effector)
    {
      switch (effector)
      {
        case FullBodyBipedEffector.Body:
          return this.chain[0];
        case FullBodyBipedEffector.LeftShoulder:
          return this.chain[1];
        case FullBodyBipedEffector.RightShoulder:
          return this.chain[2];
        case FullBodyBipedEffector.LeftThigh:
          return this.chain[3];
        case FullBodyBipedEffector.RightThigh:
          return this.chain[4];
        case FullBodyBipedEffector.LeftHand:
          return this.chain[1];
        case FullBodyBipedEffector.RightHand:
          return this.chain[2];
        case FullBodyBipedEffector.LeftFoot:
          return this.chain[3];
        case FullBodyBipedEffector.RightFoot:
          return this.chain[4];
        default:
          return (FBIKChain) null;
      }
    }

    public IKEffector GetEffector(FullBodyBipedEffector effector)
    {
      switch (effector)
      {
        case FullBodyBipedEffector.Body:
          return this.effectors[0];
        case FullBodyBipedEffector.LeftShoulder:
          return this.effectors[1];
        case FullBodyBipedEffector.RightShoulder:
          return this.effectors[2];
        case FullBodyBipedEffector.LeftThigh:
          return this.effectors[3];
        case FullBodyBipedEffector.RightThigh:
          return this.effectors[4];
        case FullBodyBipedEffector.LeftHand:
          return this.effectors[5];
        case FullBodyBipedEffector.RightHand:
          return this.effectors[6];
        case FullBodyBipedEffector.LeftFoot:
          return this.effectors[7];
        case FullBodyBipedEffector.RightFoot:
          return this.effectors[8];
        default:
          return (IKEffector) null;
      }
    }

    public IKEffector GetEndEffector(FullBodyBipedChain c)
    {
      switch (c)
      {
        case FullBodyBipedChain.LeftArm:
          return this.effectors[5];
        case FullBodyBipedChain.RightArm:
          return this.effectors[6];
        case FullBodyBipedChain.LeftLeg:
          return this.effectors[7];
        case FullBodyBipedChain.RightLeg:
          return this.effectors[8];
        default:
          return (IKEffector) null;
      }
    }

    public IKMappingLimb GetLimbMapping(FullBodyBipedChain chain)
    {
      switch (chain)
      {
        case FullBodyBipedChain.LeftArm:
          return this.limbMappings[0];
        case FullBodyBipedChain.RightArm:
          return this.limbMappings[1];
        case FullBodyBipedChain.LeftLeg:
          return this.limbMappings[2];
        case FullBodyBipedChain.RightLeg:
          return this.limbMappings[3];
        default:
          return (IKMappingLimb) null;
      }
    }

    public IKMappingLimb GetLimbMapping(FullBodyBipedEffector effector)
    {
      switch (effector)
      {
        case FullBodyBipedEffector.LeftShoulder:
          return this.limbMappings[0];
        case FullBodyBipedEffector.RightShoulder:
          return this.limbMappings[1];
        case FullBodyBipedEffector.LeftThigh:
          return this.limbMappings[2];
        case FullBodyBipedEffector.RightThigh:
          return this.limbMappings[3];
        case FullBodyBipedEffector.LeftHand:
          return this.limbMappings[0];
        case FullBodyBipedEffector.RightHand:
          return this.limbMappings[1];
        case FullBodyBipedEffector.LeftFoot:
          return this.limbMappings[2];
        case FullBodyBipedEffector.RightFoot:
          return this.limbMappings[3];
        default:
          return (IKMappingLimb) null;
      }
    }

    public IKMappingSpine GetSpineMapping() => this.spineMapping;

    public IKMappingBone GetHeadMapping() => this.boneMappings[0];

    public IKConstraintBend GetBendConstraint(FullBodyBipedChain limb)
    {
      switch (limb)
      {
        case FullBodyBipedChain.LeftArm:
          return this.chain[1].bendConstraint;
        case FullBodyBipedChain.RightArm:
          return this.chain[2].bendConstraint;
        case FullBodyBipedChain.LeftLeg:
          return this.chain[3].bendConstraint;
        case FullBodyBipedChain.RightLeg:
          return this.chain[4].bendConstraint;
        default:
          return (IKConstraintBend) null;
      }
    }

    public override bool IsValid(ref string message)
    {
      if (!base.IsValid(ref message))
        return false;
      if ((UnityEngine.Object) this.rootNode == (UnityEngine.Object) null)
      {
        message = "Root Node bone is null. FBBIK will not initiate.";
        return false;
      }
      if (this.chain.Length == 5 && this.chain[0].nodes.Length == 1 && this.chain[1].nodes.Length == 3 && this.chain[2].nodes.Length == 3 && this.chain[3].nodes.Length == 3 && this.chain[4].nodes.Length == 3 && this.effectors.Length == 9 && this.limbMappings.Length == 4)
        return true;
      message = "Invalid FBBIK setup. Please right-click on the component header and select 'Reinitiate'.";
      return false;
    }

    public void SetToReferences(BipedReferences references, Transform rootNode = null)
    {
      this.root = references.root;
      if ((UnityEngine.Object) rootNode == (UnityEngine.Object) null)
        rootNode = IKSolverFullBodyBiped.DetectRootNodeBone(references);
      this.rootNode = rootNode;
      if (this.chain == null || this.chain.Length != 5)
        this.chain = new FBIKChain[5];
      for (int index = 0; index < this.chain.Length; ++index)
      {
        if (this.chain[index] == null)
          this.chain[index] = new FBIKChain();
      }
      this.chain[0].pin = 0.0f;
      this.chain[0].SetNodes(rootNode);
      this.chain[0].children = new int[4]{ 1, 2, 3, 4 };
      this.chain[1].SetNodes(references.leftUpperArm, references.leftForearm, references.leftHand);
      this.chain[2].SetNodes(references.rightUpperArm, references.rightForearm, references.rightHand);
      this.chain[3].SetNodes(references.leftThigh, references.leftCalf, references.leftFoot);
      this.chain[4].SetNodes(references.rightThigh, references.rightCalf, references.rightFoot);
      if (this.effectors.Length != 9)
        this.effectors = new IKEffector[9]
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
      this.effectors[0].bone = rootNode;
      this.effectors[0].childBones = new Transform[2]
      {
        references.leftThigh,
        references.rightThigh
      };
      this.effectors[1].bone = references.leftUpperArm;
      this.effectors[2].bone = references.rightUpperArm;
      this.effectors[3].bone = references.leftThigh;
      this.effectors[4].bone = references.rightThigh;
      this.effectors[5].bone = references.leftHand;
      this.effectors[6].bone = references.rightHand;
      this.effectors[7].bone = references.leftFoot;
      this.effectors[8].bone = references.rightFoot;
      this.effectors[5].planeBone1 = references.leftUpperArm;
      this.effectors[5].planeBone2 = references.rightUpperArm;
      this.effectors[5].planeBone3 = rootNode;
      this.effectors[6].planeBone1 = references.rightUpperArm;
      this.effectors[6].planeBone2 = references.leftUpperArm;
      this.effectors[6].planeBone3 = rootNode;
      this.effectors[7].planeBone1 = references.leftThigh;
      this.effectors[7].planeBone2 = references.rightThigh;
      this.effectors[7].planeBone3 = rootNode;
      this.effectors[8].planeBone1 = references.rightThigh;
      this.effectors[8].planeBone2 = references.leftThigh;
      this.effectors[8].planeBone3 = rootNode;
      this.chain[0].childConstraints = new FBIKChain.ChildConstraint[4]
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
      if (this.spineMapping == null)
      {
        this.spineMapping = new IKMappingSpine();
        this.spineMapping.iterations = 3;
      }
      this.spineMapping.SetBones(spineBones, references.leftUpperArm, references.rightUpperArm, references.leftThigh, references.rightThigh);
      int length = (UnityEngine.Object) references.head != (UnityEngine.Object) null ? 1 : 0;
      if (this.boneMappings.Length != length)
      {
        this.boneMappings = new IKMappingBone[length];
        for (int index = 0; index < this.boneMappings.Length; ++index)
          this.boneMappings[index] = new IKMappingBone();
        if (length == 1)
          this.boneMappings[0].maintainRotationWeight = 0.0f;
      }
      if (this.boneMappings.Length != 0)
        this.boneMappings[0].bone = references.head;
      if (this.limbMappings.Length != 4)
      {
        this.limbMappings = new IKMappingLimb[4]
        {
          new IKMappingLimb(),
          new IKMappingLimb(),
          new IKMappingLimb(),
          new IKMappingLimb()
        };
        this.limbMappings[2].maintainRotationWeight = 1f;
        this.limbMappings[3].maintainRotationWeight = 1f;
      }
      this.limbMappings[0].SetBones(references.leftUpperArm, references.leftForearm, references.leftHand, IKSolverFullBodyBiped.GetLeftClavicle(references));
      this.limbMappings[1].SetBones(references.rightUpperArm, references.rightForearm, references.rightHand, IKSolverFullBodyBiped.GetRightClavicle(references));
      this.limbMappings[2].SetBones(references.leftThigh, references.leftCalf, references.leftFoot);
      this.limbMappings[3].SetBones(references.rightThigh, references.rightCalf, references.rightFoot);
      if (!Application.isPlaying)
        return;
      this.Initiate(references.root);
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
        if ((double) Vector3.Dot(vector3_2.normalized, onNormal.normalized) > 0.0 && (double) (vector3_2.magnitude / magnitude) < 0.5)
          index1 = index2;
      }
      return references.spine[index1];
    }

    public void SetLimbOrientations(BipedLimbOrientations o)
    {
      this.SetLimbOrientation(FullBodyBipedChain.LeftArm, o.leftArm);
      this.SetLimbOrientation(FullBodyBipedChain.RightArm, o.rightArm);
      this.SetLimbOrientation(FullBodyBipedChain.LeftLeg, o.leftLeg);
      this.SetLimbOrientation(FullBodyBipedChain.RightLeg, o.rightLeg);
    }

    public Vector3 pullBodyOffset { get; private set; }

    private void SetLimbOrientation(
      FullBodyBipedChain chain,
      BipedLimbOrientations.LimbOrientation limbOrientation)
    {
      if (chain == FullBodyBipedChain.LeftArm || chain == FullBodyBipedChain.RightArm)
      {
        this.GetBendConstraint(chain).SetLimbOrientation(-limbOrientation.upperBoneForwardAxis, -limbOrientation.lowerBoneForwardAxis, -limbOrientation.lastBoneLeftAxis);
        this.GetLimbMapping(chain).SetLimbOrientation(-limbOrientation.upperBoneForwardAxis, -limbOrientation.lowerBoneForwardAxis);
      }
      else
      {
        this.GetBendConstraint(chain).SetLimbOrientation(limbOrientation.upperBoneForwardAxis, limbOrientation.lowerBoneForwardAxis, limbOrientation.lastBoneLeftAxis);
        this.GetLimbMapping(chain).SetLimbOrientation(limbOrientation.upperBoneForwardAxis, limbOrientation.lowerBoneForwardAxis);
      }
    }

    private static Transform GetLeftClavicle(BipedReferences references)
    {
      return (UnityEngine.Object) references.leftUpperArm == (UnityEngine.Object) null || IKSolverFullBodyBiped.Contains(references.spine, references.leftUpperArm.parent) ? (Transform) null : references.leftUpperArm.parent;
    }

    private static Transform GetRightClavicle(BipedReferences references)
    {
      return (UnityEngine.Object) references.rightUpperArm == (UnityEngine.Object) null || IKSolverFullBodyBiped.Contains(references.spine, references.rightUpperArm.parent) ? (Transform) null : references.rightUpperArm.parent;
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
      for (int index = 0; index < this.effectors.Length; ++index)
        this.effectors[index].SetToTarget();
      this.PullBody();
      float num = Mathf.Clamp(1f - this.spineStiffness, 0.0f, 1f);
      this.chain[0].childConstraints[0].pushElasticity = num;
      this.chain[0].childConstraints[1].pushElasticity = num;
      base.ReadPose();
    }

    private void PullBody()
    {
      if (this.iterations < 1 || (double) this.pullBodyVertical == 0.0 && (double) this.pullBodyHorizontal == 0.0)
        return;
      Vector3 bodyOffset = this.GetBodyOffset();
      this.pullBodyOffset = V3Tools.ExtractVertical(bodyOffset, this.root.up, this.pullBodyVertical) + V3Tools.ExtractHorizontal(bodyOffset, this.root.up, this.pullBodyHorizontal);
      this.bodyEffector.positionOffset += this.pullBodyOffset;
    }

    private Vector3 GetBodyOffset()
    {
      Vector3 offset = Vector3.zero + this.GetHandBodyPull(this.leftHandEffector, this.leftArmChain, Vector3.zero) * Mathf.Clamp(this.leftHandEffector.positionWeight, 0.0f, 1f);
      return offset + this.GetHandBodyPull(this.rightHandEffector, this.rightArmChain, offset) * Mathf.Clamp(this.rightHandEffector.positionWeight, 0.0f, 1f);
    }

    private Vector3 GetHandBodyPull(IKEffector effector, FBIKChain arm, Vector3 offset)
    {
      Vector3 vector3 = effector.position - (arm.nodes[0].transform.position + offset);
      float num1 = arm.nodes[0].length + arm.nodes[1].length;
      float magnitude = vector3.magnitude;
      if ((double) magnitude < (double) num1)
        return Vector3.zero;
      float num2 = magnitude - num1;
      return vector3 / magnitude * num2;
    }

    protected override void ApplyBendConstraints()
    {
      if (this.iterations > 0)
      {
        this.chain[1].bendConstraint.rotationOffset = this.leftHandEffector.planeRotationOffset;
        this.chain[2].bendConstraint.rotationOffset = this.rightHandEffector.planeRotationOffset;
        this.chain[3].bendConstraint.rotationOffset = this.leftFootEffector.planeRotationOffset;
        this.chain[4].bendConstraint.rotationOffset = this.rightFootEffector.planeRotationOffset;
      }
      else
      {
        this.offset = Vector3.Lerp(this.effectors[0].positionOffset, this.effectors[0].position - (this.effectors[0].bone.position + this.effectors[0].positionOffset), this.effectors[0].positionWeight);
        for (int index = 0; index < 5; ++index)
        {
          IKSolver.Node node = this.effectors[index].GetNode((IKSolverFullBody) this);
          node.solverPosition = node.solverPosition + this.offset;
        }
      }
      base.ApplyBendConstraints();
    }

    protected override void WritePose()
    {
      if (this.iterations == 0)
        this.spineMapping.spineBones[0].position += this.offset;
      base.WritePose();
    }
  }
}
