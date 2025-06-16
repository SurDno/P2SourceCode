using System;
using UnityEngine;

namespace RootMotion.Dynamics;

[CreateAssetMenu(fileName = "PuppetMaster Humanoid Config", menuName = "PuppetMaster/Humanoid Config", order = 1)]
public class PuppetMasterHumanoidConfig : ScriptableObject {
	[LargeHeader("Simulation")] public PuppetMaster.State state;
	public PuppetMaster.StateSettings stateSettings = PuppetMaster.StateSettings.Default;
	public PuppetMaster.Mode mode;
	public float blendTime = 0.1f;
	public bool fixTargetTransforms = true;
	public int solverIterationCount = 6;
	public bool visualizeTargetPose = true;

	[LargeHeader("Master Weights")] [Range(0.0f, 1f)]
	public float mappingWeight = 1f;

	[Range(0.0f, 1f)] public float pinWeight = 1f;
	[Range(0.0f, 1f)] public float muscleWeight = 1f;

	[LargeHeader("Joint and Muscle Settings")]
	public float muscleSpring = 100f;

	public float muscleDamper;
	[Range(1f, 8f)] public float pinPow = 4f;
	[Range(0.0f, 100f)] public float pinDistanceFalloff = 5f;
	public bool updateJointAnchors = true;
	public bool supportTranslationAnimation;
	public bool angularLimits;
	public bool internalCollisions;

	[LargeHeader("Individual Muscle Settings")]
	public HumanoidMuscle[] muscles = new HumanoidMuscle[0];

	public void ApplyTo(PuppetMaster p) {
		if (p.targetRoot == null)
			Debug.LogWarning("Please assign 'Target Root' for PuppetMaster using a Humanoid Config.", p.transform);
		else if (p.targetAnimator == null)
			Debug.LogError(
				"PuppetMaster 'Target Root' does not have an Animator component. Can not use Humanoid Config.",
				p.transform);
		else if (!p.targetAnimator.isHuman)
			Debug.LogError("PuppetMaster target is not a Humanoid. Can not use Humanoid Config.", p.transform);
		else {
			p.state = state;
			p.stateSettings = stateSettings;
			p.mode = mode;
			p.blendTime = blendTime;
			p.fixTargetTransforms = fixTargetTransforms;
			p.solverIterationCount = solverIterationCount;
			p.visualizeTargetPose = visualizeTargetPose;
			p.mappingWeight = mappingWeight;
			p.pinWeight = pinWeight;
			p.muscleWeight = muscleWeight;
			p.muscleSpring = muscleSpring;
			p.muscleDamper = muscleDamper;
			p.pinPow = pinPow;
			p.pinDistanceFalloff = pinDistanceFalloff;
			p.updateJointAnchors = updateJointAnchors;
			p.supportTranslationAnimation = supportTranslationAnimation;
			p.angularLimits = angularLimits;
			p.internalCollisions = internalCollisions;
			foreach (var muscle1 in muscles) {
				var muscle2 = GetMuscle(muscle1.bone, p.targetAnimator, p);
				if (muscle2 != null) {
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

	private Muscle GetMuscle(HumanBodyBones boneId, Animator animator, PuppetMaster puppetMaster) {
		var boneTransform = animator.GetBoneTransform(boneId);
		if (boneTransform == null)
			return null;
		foreach (var muscle in puppetMaster.muscles)
			if (muscle.target == boneTransform)
				return muscle;
		return null;
	}

	[Serializable]
	public class HumanoidMuscle {
		[SerializeField] [HideInInspector] public string name;
		public HumanBodyBones bone;
		public Muscle.Props props;
	}
}