using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class POISetup : MonoBehaviour {
	private static HashSet<string> clipNames = new();

	[EnumFlag(typeof(POIAnimationEnum))] [SerializeField] [FormerlySerializedAs("SupportedAnimations")]
	private POIAnimationEnum _supportedAnimations;

	[SerializeField] [FormerlySerializedAs("S_SitAtDesk_Setup")]
	private POIAnimationSetupSlow _s_SitAtDesk_Setup = new();

	[SerializeField] private POIAnimationSetupSlow s_SitAtDesk_Setup = new();

	[SerializeField] [FormerlySerializedAs("S_SitOnBench_Setup")]
	private POIAnimationSetupSlow _s_SitOnBench_Setup = new();

	[SerializeField] private POIAnimationSetupSlow s_SitOnBench_Setup = new();

	[SerializeField] [FormerlySerializedAs("S_LeanOnWall_Setup")]
	private POIAnimationSetupSlow _s_LeanOnWall_Setup = new();

	[SerializeField] private POIAnimationSetupSlow s_LeanOnWall_Setup = new();

	[SerializeField] [FormerlySerializedAs("S_LeanOnTable_Setup")]
	private POIAnimationSetupSlow _s_LeanOnTable_Setup = new();

	[SerializeField] private POIAnimationSetupSlow s_LeanOnTable_Setup = new();

	[SerializeField] [FormerlySerializedAs("S_SitNearWall_Setup")]
	private POIAnimationSetupSlow _s_SitNearWall_Setup = new();

	[SerializeField] private POIAnimationSetupSlow s_SitNearWall_Setup = new();

	[SerializeField] [FormerlySerializedAs("S_LieOnBed_Setup")]
	private POIAnimationSetupSlow _s_LieOnBed_Setup = new();

	[SerializeField] private POIAnimationSetupSlow s_LieOnBed_Setup = new();

	[SerializeField] [FormerlySerializedAs("S_NearFire_Setup")]
	private POIAnimationSetupSlow _s_NearFire_Setup = new();

	[SerializeField] private POIAnimationSetupSlow s_NearFire_Setup = new();

	[SerializeField] [FormerlySerializedAs("Q_ViewPoster_Setup")]
	private POIAnimationSetupBase _q_ViewPoster_Setup = new();

	[SerializeField] private POIAnimationSetupQuick q_ViewPoster_Setup = new();

	[SerializeField] [FormerlySerializedAs("Q_LookOutOfTheWindow_Setup")]
	private POIAnimationSetupBase _q_LookOutOfTheWindow_Setup = new();

	[SerializeField] private POIAnimationSetupQuick q_LookOutOfTheWindow_Setup = new();

	[SerializeField] [FormerlySerializedAs("Q_LookUnder_Setup")]
	private POIAnimationSetupBase _q_LookUnder_Setup = new();

	[SerializeField] private POIAnimationSetupQuick q_LookUnder_Setup = new();

	[SerializeField] [FormerlySerializedAs("Q_LookIntoTheWindow_Setup")]
	private POIAnimationSetupBase _q_LookIntoTheWindow_Setup = new();

	[SerializeField] private POIAnimationSetupQuick q_LookIntoTheWindow_Setup = new();

	[SerializeField] [FormerlySerializedAs("Q_ActionWithWall_Setup")]
	private POIAnimationSetupBase _q_ActionWithWall_Setup = new();

	[SerializeField] private POIAnimationSetupQuick q_ActionWithWall_Setup = new();

	[SerializeField] [FormerlySerializedAs("Q_ActionWithTable_Setup")]
	private POIAnimationSetupBase _q_ActionWithTable_Setup = new();

	[SerializeField] private POIAnimationSetupQuick q_ActionWithTable_Setup = new();

	[SerializeField] [FormerlySerializedAs("Q_ActionWithWardrobe_Setup")]
	private POIAnimationSetupBase _q_ActionWithWardrobe_Setup = new();

	[SerializeField] private POIAnimationSetupQuick q_ActionWithWardrobe_Setup = new();

	[SerializeField] [FormerlySerializedAs("Q_ActionWithShelves_Setup")]
	private POIAnimationSetupBase _q_ActionWithShelves_Setup = new();

	[SerializeField] private POIAnimationSetupQuick q_ActionWithShelves_Setup = new();

	[SerializeField] [FormerlySerializedAs("Q_ActionWithNightstand_Setup")]
	private POIAnimationSetupBase _q_ActionWithNightstand_Setup = new();

	[SerializeField] private POIAnimationSetupQuick q_ActionWithNightstand_Setup = new();

	[SerializeField] [FormerlySerializedAs("Q_ActionOnFloor_Setup")]
	private POIAnimationSetupBase _q_ActionOnFloor_Setup = new();

	[SerializeField] private POIAnimationSetupQuick q_ActionOnFloor_Setup = new();
	[SerializeField] private POIAnimationSetupSlow _s_ActionOnFloor_Setup = new();
	[SerializeField] private POIAnimationSetupSlow s_ActionOnFloor_Setup = new();
	[SerializeField] private POIAnimationSetupBase _q_Idle_Setup = new();
	[SerializeField] private POIAnimationSetupQuick q_Idle_Setup = new();
	[SerializeField] private POIAnimationSetupQuick q_NearFire_Setup = new();
	[SerializeField] private POIAnimationSetupSlow s_Dialog_Setup = new();
	[SerializeField] private POIAnimationSetupSlow s_Loot_Setup = new();
	[SerializeField] private POIAnimationSetupQuick q_PlaygroundPlay_Setup = new();
	[SerializeField] private POIAnimationSetupSlow s_PlaygroundSandbox_Setup = new();
	[SerializeField] private POIAnimationSetupSlow s_PlaygroundCooperative_Setup = new();
	[SerializeField] private POIAnimationSetupQuick q_PlaygroundCooperative_Setup = new();
	[SerializeField] private POIAnimationSetupSlow s_SitAtDeskRight_Setup = new();

	public POIAnimationEnum SupportedAnimations {
		get => _supportedAnimations;
		set => _supportedAnimations = value;
	}

	private void Start() { }

	public POIAnimationSetupBase GetAnimationSetupOld(POIAnimationEnum animation) {
		switch (animation) {
			case POIAnimationEnum.S_SitAtDesk:
				return _s_SitAtDesk_Setup;
			case POIAnimationEnum.S_SitOnBench:
				return _s_SitOnBench_Setup;
			case POIAnimationEnum.S_LeanOnWall:
				return _s_LeanOnWall_Setup;
			case POIAnimationEnum.S_LeanOnTable:
				return _s_LeanOnTable_Setup;
			case POIAnimationEnum.S_SitNearWall:
				return _s_SitNearWall_Setup;
			case POIAnimationEnum.S_LieOnBed:
				return _s_LieOnBed_Setup;
			case POIAnimationEnum.S_NearFire:
				return _s_NearFire_Setup;
			case POIAnimationEnum.Q_ViewPoster:
				return _q_ViewPoster_Setup;
			case POIAnimationEnum.Q_LookOutOfTheWindow:
				return _q_LookOutOfTheWindow_Setup;
			case POIAnimationEnum.Q_LookUnder:
				return _q_LookUnder_Setup;
			case POIAnimationEnum.Q_LookIntoTheWindow:
				return _q_LookIntoTheWindow_Setup;
			case POIAnimationEnum.Q_ActionWithWall:
				return _q_ActionWithWall_Setup;
			case POIAnimationEnum.Q_ActionWithTable:
				return _q_ActionWithTable_Setup;
			case POIAnimationEnum.Q_ActionWithWardrobe:
				return _q_ActionWithWardrobe_Setup;
			case POIAnimationEnum.Q_ActionWithShelves:
				return _q_ActionWithShelves_Setup;
			case POIAnimationEnum.Q_ActionWithNightstand:
				return _q_ActionWithNightstand_Setup;
			case POIAnimationEnum.Q_ActionOnFloor:
				return _q_ActionOnFloor_Setup;
			case POIAnimationEnum.S_ActionOnFloor:
				return _s_ActionOnFloor_Setup;
			case POIAnimationEnum.Q_Idle:
				return _q_Idle_Setup;
			case POIAnimationEnum.Q_NearFire:
				return _q_Idle_Setup;
			case POIAnimationEnum.S_Dialog:
				return _q_Idle_Setup;
			case POIAnimationEnum.S_Loot:
				return _q_Idle_Setup;
			case POIAnimationEnum.Q_PlaygroundPlay:
				return q_PlaygroundPlay_Setup;
			case POIAnimationEnum.S_PlaygroundSandbox:
				return s_PlaygroundSandbox_Setup;
			case POIAnimationEnum.S_PlaygroundCooperative:
				return s_PlaygroundCooperative_Setup;
			case POIAnimationEnum.Q_PlaygroundCooperative:
				return q_PlaygroundCooperative_Setup;
			case POIAnimationEnum.S_SitAtDeskRight:
				return s_SitAtDeskRight_Setup;
			default:
				return null;
		}
	}

	public POIAnimationSetupBase GetAnimationSetup(POIAnimationEnum animation) {
		switch (animation) {
			case POIAnimationEnum.S_SitAtDesk:
				return s_SitAtDesk_Setup;
			case POIAnimationEnum.S_SitOnBench:
				return s_SitOnBench_Setup;
			case POIAnimationEnum.S_LeanOnWall:
				return s_LeanOnWall_Setup;
			case POIAnimationEnum.S_LeanOnTable:
				return s_LeanOnTable_Setup;
			case POIAnimationEnum.S_SitNearWall:
				return s_SitNearWall_Setup;
			case POIAnimationEnum.S_LieOnBed:
				return s_LieOnBed_Setup;
			case POIAnimationEnum.S_NearFire:
				return s_NearFire_Setup;
			case POIAnimationEnum.Q_ViewPoster:
				return q_ViewPoster_Setup;
			case POIAnimationEnum.Q_LookOutOfTheWindow:
				return q_LookOutOfTheWindow_Setup;
			case POIAnimationEnum.Q_LookUnder:
				return q_LookUnder_Setup;
			case POIAnimationEnum.Q_LookIntoTheWindow:
				return q_LookIntoTheWindow_Setup;
			case POIAnimationEnum.Q_ActionWithWall:
				return q_ActionWithWall_Setup;
			case POIAnimationEnum.Q_ActionWithTable:
				return q_ActionWithTable_Setup;
			case POIAnimationEnum.Q_ActionWithWardrobe:
				return q_ActionWithWardrobe_Setup;
			case POIAnimationEnum.Q_ActionWithShelves:
				return q_ActionWithShelves_Setup;
			case POIAnimationEnum.Q_ActionWithNightstand:
				return q_ActionWithNightstand_Setup;
			case POIAnimationEnum.Q_ActionOnFloor:
				return q_ActionOnFloor_Setup;
			case POIAnimationEnum.S_ActionOnFloor:
				return s_ActionOnFloor_Setup;
			case POIAnimationEnum.Q_Idle:
				return q_Idle_Setup;
			case POIAnimationEnum.Q_NearFire:
				return q_NearFire_Setup;
			case POIAnimationEnum.S_Dialog:
				return s_Dialog_Setup;
			case POIAnimationEnum.S_Loot:
				return s_Loot_Setup;
			case POIAnimationEnum.Q_PlaygroundPlay:
				return q_PlaygroundPlay_Setup;
			case POIAnimationEnum.S_PlaygroundSandbox:
				return s_PlaygroundSandbox_Setup;
			case POIAnimationEnum.S_PlaygroundCooperative:
				return s_PlaygroundCooperative_Setup;
			case POIAnimationEnum.Q_PlaygroundCooperative:
				return q_PlaygroundCooperative_Setup;
			case POIAnimationEnum.S_SitAtDeskRight:
				return s_SitAtDeskRight_Setup;
			default:
				return null;
		}
	}

	public Vector3 GetAnimationOffset(POIAnimationEnum animation, int animationIndex) {
		var animationSetup = GetAnimationSetup(animation);
		return animationSetup != null && animationSetup.Elements.Count > animationIndex &&
		       animationSetup.Elements[animationIndex] != null
			? animationSetup.Elements[animationIndex].Offset
			: Vector3.zero;
	}

	public bool GetAnimationPlayOnce(POIAnimationEnum animation, int animationIndex) {
		var animationSetup = GetAnimationSetup(animation);
		if (animationSetup == null || animationSetup.Elements.Count <= animationIndex ||
		    animationSetup.Elements[animationIndex] == null)
			return false;
		return animationSetup is POIAnimationSetupQuick || animationSetup.Elements[animationIndex].PlayOnce;
	}

	public bool AnimationIsQuick(POIAnimationEnum animation) {
		return GetAnimationSetup(animation) is POIAnimationSetupQuick;
	}

	public bool GetNeedSynchronizeAnimation(POIAnimationEnum animation, int animationIndex) {
		var animationSetup = GetAnimationSetup(animation);
		if (animationSetup == null || animationSetup.Elements.Count <= animationIndex ||
		    animationSetup.Elements[animationIndex] == null)
			return false;
		return animationSetup.Elements[animationIndex].Offset != Vector3.zero ||
		       animationSetup.Elements[animationIndex].NeedSynchroniseAnimation;
	}

	public float GetAnimationSyncDelay(POIAnimationEnum animation, int animationIndex) {
		var animationSetup = GetAnimationSetup(animation);
		return animationSetup != null && animationSetup.Elements.Count > animationIndex &&
		       animationSetup.Elements[animationIndex] != null
			? animationSetup.Elements[animationIndex].AnimationSyncDelay
			: 0.0f;
	}

	private void SetNewClip(AnimationClipOverrides clips, string name, AnimationClip newClip) {
		clips[name] = newClip;
	}
}