using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
  [CreateAssetMenu(fileName = "PuppetMaster Humanoid Config", menuName = "PuppetMaster/Humanoid Config", order = 1)]
  public class PuppetMasterHumanoidConfig : ScriptableObject
  {
    [LargeHeader("Simulation")]
    public PuppetMaster.State state;
    public PuppetMaster.StateSettings stateSettings = PuppetMaster.StateSettings.Default;
    public PuppetMaster.Mode mode;
    public float blendTime = 0.1f;
    public bool fixTargetTransforms = true;
    public int solverIterationCount = 6;
    public bool visualizeTargetPose = true;
    [LargeHeader("Master Weights")]
    [Range(0.0f, 1f)]
    public float mappingWeight = 1f;
    [Range(0.0f, 1f)]
    public float pinWeight = 1f;
    [Range(0.0f, 1f)]
    public float muscleWeight = 1f;
    [LargeHeader("Joint and Muscle Settings")]
    public float muscleSpring = 100f;
    public float muscleDamper = 0.0f;
    [Range(1f, 8f)]
    public float pinPow = 4f;
    [Range(0.0f, 100f)]
    public float pinDistanceFalloff = 5f;
    public bool updateJointAnchors = true;
    public bool supportTranslationAnimation;
    public bool angularLimits;
    public bool internalCollisions;
    [LargeHeader("Individual Muscle Settings")]
    public PuppetMasterHumanoidConfig.HumanoidMuscle[] muscles = new PuppetMasterHumanoidConfig.HumanoidMuscle[0];

    public void ApplyTo(PuppetMaster p)
    {
      if ((UnityEngine.Object) p.targetRoot == (UnityEngine.Object) null)
        Debug.LogWarning((object) "Please assign 'Target Root' for PuppetMaster using a Humanoid Config.", (UnityEngine.Object) p.transform);
      else if ((UnityEngine.Object) p.targetAnimator == (UnityEngine.Object) null)
        Debug.LogError((object) "PuppetMaster 'Target Root' does not have an Animator component. Can not use Humanoid Config.", (UnityEngine.Object) p.transform);
      else if (!p.targetAnimator.isHuman)
      {
        Debug.LogError((object) "PuppetMaster target is not a Humanoid. Can not use Humanoid Config.", (UnityEngine.Object) p.transform);
      }
      else
      {
        p.state = this.state;
        p.stateSettings = this.stateSettings;
        p.mode = this.mode;
        p.blendTime = this.blendTime;
        p.fixTargetTransforms = this.fixTargetTransforms;
        p.solverIterationCount = this.solverIterationCount;
        p.visualizeTargetPose = this.visualizeTargetPose;
        p.mappingWeight = this.mappingWeight;
        p.pinWeight = this.pinWeight;
        p.muscleWeight = this.muscleWeight;
        p.muscleSpring = this.muscleSpring;
        p.muscleDamper = this.muscleDamper;
        p.pinPow = this.pinPow;
        p.pinDistanceFalloff = this.pinDistanceFalloff;
        p.updateJointAnchors = this.updateJointAnchors;
        p.supportTranslationAnimation = this.supportTranslationAnimation;
        p.angularLimits = this.angularLimits;
        p.internalCollisions = this.internalCollisions;
        foreach (PuppetMasterHumanoidConfig.HumanoidMuscle muscle1 in this.muscles)
        {
          Muscle muscle2 = this.GetMuscle(muscle1.bone, p.targetAnimator, p);
          if (muscle2 != null)
          {
            muscle2.props.group = muscle1.props.group;
            muscle2.props.mappingWeight = muscle1.props.mappingWeight;
            muscle2.props.mapPosition = muscle1.props.mapPosition;
            muscle2.props.muscleDamper = muscle1.props.muscleDamper;
            muscle2.props.muscleWeight = muscle1.props.muscleWeight;
            muscle2.props.pinWeight = muscle1.props.pinWeight;
          }
        }
      }
    }

    private Muscle GetMuscle(HumanBodyBones boneId, Animator animator, PuppetMaster puppetMaster)
    {
      Transform boneTransform = animator.GetBoneTransform(boneId);
      if ((UnityEngine.Object) boneTransform == (UnityEngine.Object) null)
        return (Muscle) null;
      foreach (Muscle muscle in puppetMaster.muscles)
      {
        if ((UnityEngine.Object) muscle.target == (UnityEngine.Object) boneTransform)
          return muscle;
      }
      return (Muscle) null;
    }

    [Serializable]
    public class HumanoidMuscle
    {
      [SerializeField]
      [HideInInspector]
      public string name;
      public HumanBodyBones bone;
      public Muscle.Props props;
    }
  }
}
