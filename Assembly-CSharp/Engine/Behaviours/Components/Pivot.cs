using System;
using Engine.Behaviours.Engines.Controllers;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Engine.Behaviours.Components;

[DisallowMultipleComponent]
public class Pivot : MonoBehaviour, IEntityAttachable {
	public HierarhyStructureEnum HierarhyStructure = HierarhyStructureEnum.Ordinary;
	public GameObject AnchorCameraFPS;

	[Tooltip("Camera прикрпленная к голове с правильно расположенными осями (z-вперед, y-вверх")]
	public GameObject AnchorCameraOnHead;

	public DialogLightingProfile DialogLightingProfile;
	public GameObject DialogDOFTarget;
	public Transform RootBone;
	[Header("Body parts")] public GameObject Head;
	public GameObject Chest;
	public GameObject Belly;
	public GameObject Foot;
	public GameObject Hands;
	[Header("Weapon Aim")] public AimWeaponType AimType = AimWeaponType.Head;

	[Tooltip("Этим объектом происходит прицеливание")]
	public GameObject AimTransform;

	[Tooltip("из этого объекта расчитывается направление выстрела")]
	public GameObject ShootStart;

	[Tooltip("Aim Axis")] public Vector3 AimAxis = Vector3.forward;
	[Tooltip("Pole Axis")] public Vector3 PoleAxis = Vector3.up;

	[Header("Weapon transform")] [Tooltip("Эта трансформация копируется в WeaponTransform")]
	public GameObject WeaponDummyTransform;

	[Tooltip("Оружие")] public GameObject WeaponTransform;

	[Header("Ragdoll")] [SerializeField] [FormerlySerializedAs("Ragdoll_Pelvis")]
	private GameObject ragdollPelvis;

	[SerializeField] [FormerlySerializedAs("Ragdoll_LeftHips")]
	private GameObject ragdollLeftHips;

	[SerializeField] [FormerlySerializedAs("Ragdoll_LeftKnee")]
	private GameObject ragdollLeftKnee;

	[SerializeField] [FormerlySerializedAs("Ragdoll_RightHips")]
	private GameObject ragdollRightHips;

	[SerializeField] [FormerlySerializedAs("Ragdoll_RightKnee")]
	private GameObject ragdollRightKnee;

	[SerializeField] [FormerlySerializedAs("Ragdoll_LeftArm")]
	private GameObject ragdollLeftArm;

	[SerializeField] [FormerlySerializedAs("Ragdoll_LeftElbow")]
	private GameObject ragdollLeftElbow;

	[SerializeField] [FormerlySerializedAs("Ragdoll_RightArm")]
	private GameObject ragdollRightArm;

	[SerializeField] [FormerlySerializedAs("Ragdoll_RightElbow")]
	private GameObject ragdollRightElbow;

	[SerializeField] [FormerlySerializedAs("Ragdoll_MiddleSpine")]
	private GameObject ragdollMiddleSpine;

	[SerializeField] [FormerlySerializedAs("Ragdoll_Head")]
	private GameObject ragdollHead;

	[SerializeField] [FormerlySerializedAs("Ragdoll_Weapon")]
	private GameObject ragdollWeapon;

	[Header("Sound")] [SerializeField] private NPCSoundBankDescription soundBank;

	[Header("You can use ComputeNpc to fill this!")] [SerializeField]
	private Animator animator;

	[SerializeField] private AnimatorEventProxy animatorEventProxy;
	[SerializeField] private NavMeshAgent agent;
	[SerializeField] private PuppetMaster puppet;
	[SerializeField] private NavMeshObstacle obstacle;
	[SerializeField] private Rigidbody rigidbody;
	[SerializeField] private NPCEnemy npcEnemy;
	[SerializeField] private NPCWeaponService npcWeaponService;
	[SerializeField] private EngineBehavior behavior;
	[SerializeField] public int SecondaryIdleAnimationCount;
	[SerializeField] public int SecondaryLowIdleAnimationCount;
	[SerializeField] public int DialogIdleAnimationCount;
	[SerializeField] public CombatStyleEnum DefaultCombatStyle;
	[SerializeField] public GameObject HidingOuterWeapon;
	private Vector3 headImpulseValue;
	private float ragdollWeight;
	private float actualRagdollWeight;
	private bool ragdollInternalCollisions = true;

	[EnumFlag(typeof(POIAnimationEnum))] [SerializeField]
	private POIAnimationEnum supportedFastPOIExits;

	[SerializeField] private bool supportsFastPOIExits;
	[SerializeField] public bool CollidersSetForSharpshooting;

	[Inspected] public IEntity Owner { get; private set; }

	[Inspected] public bool Indoor { get; private set; }

	public event Action<bool> IndoorChangedEvent;

	public Animator GetAnimator() {
		return animator;
	}

	public AnimatorEventProxy GetAnimatorEventProxy() {
		return animatorEventProxy;
	}

	public PuppetMaster GetPuppet() {
		return puppet;
	}

	public NavMeshObstacle GetObstacle() {
		return obstacle;
	}

	public NavMeshAgent GetAgent() {
		return agent;
	}

	public Rigidbody GetRigidbody() {
		return rigidbody;
	}

	public NPCEnemy GetNpcEnemy() {
		return npcEnemy;
	}

	public NPCWeaponService GetNpcWeaponService() {
		return npcWeaponService;
	}

	public EngineBehavior GetBehavior() {
		return behavior;
	}

	public NPCSoundBankDescription SoundBank => soundBank;

	private void Awake() {
		if (animator == null)
			Debug.LogWarning(gameObject.name + " doesn't contain " + typeof(Animator).Name + " unity component.");
		if (!(agent == null))
			return;
		Debug.LogWarning(gameObject.name + " doesn't contain " + typeof(NavMeshAgent).Name + " unity component.");
	}

	private void Start() {
		RagdollWeight = 0.0f;
		ActualRagdollWeight = 0.0f;
	}

	public void ResetMovable() {
		AnimatorState45.GetAnimatorState(animator).ResetMovable();
	}

	private bool PlayRandomSound(
		AudioMixerGroup mixer,
		AudioClip[] sounds,
		Transform transform,
		float volume,
		float spatialBlend) {
		if (sounds == null || sounds.Length == 0 || transform == null)
			return false;
		var sound = sounds[Random.Range(0, sounds.Length)];
		if (spatialBlend > 0.5)
			SoundUtility.PlayAudioClip3D(Chest.transform, sound, mixer, volume, 1f, 2.5f, true, 0.0f,
				context: gameObject.GetFullName());
		else
			SoundUtility.PlayAudioClip2D(sound, mixer, volume, 0.0f, context: gameObject.GetFullName());
		return true;
	}

	public void PlaySound(SoundEnum soundEnum, float volumeScale = 1f, bool protagonist = false) {
		if (soundBank == null)
			return;
		AudioMixerGroup mixer;
		float spatialBlend;
		if (protagonist) {
			mixer = ScriptableObjectInstance<GameSettingsData>.Instance.ProtagonistHitsMixer;
			spatialBlend = 0.0f;
		} else {
			mixer = Indoor
				? ScriptableObjectInstance<GameSettingsData>.Instance.NpcClothesIndoorMixer
				: ScriptableObjectInstance<GameSettingsData>.Instance.NpcClothesOutdoorMixer;
			spatialBlend = 1f;
		}

		switch (soundEnum) {
			case SoundEnum.RagdollFall:
				PlayRandomSound(mixer, soundBank.RagdollFallSounds, Chest.transform, volumeScale, spatialBlend);
				break;
			case SoundEnum.HittedVocal:
				PlayRandomSound(mixer, soundBank.HittedVocalSounds, Head.transform, volumeScale, spatialBlend);
				break;
			case SoundEnum.HittedLowStaminaVocal:
				PlayRandomSound(mixer, soundBank.HittedLowStaminaVocalSounds, Head.transform, volumeScale,
					spatialBlend);
				break;
			case SoundEnum.HittedDodgeVocal:
				PlayRandomSound(mixer, soundBank.HittedDodgeVocalSounds, Head.transform, volumeScale, spatialBlend);
				break;
			case SoundEnum.StaggerVocal:
				PlayRandomSound(mixer, soundBank.StaggerVocalSounds, Head.transform, volumeScale, spatialBlend);
				break;
			case SoundEnum.StaggerNonVocal:
				PlayRandomSound(mixer, soundBank.StaggerNonVocalSounds, Head.transform, volumeScale, spatialBlend);
				break;
			case SoundEnum.StrafeLeft:
				PlayRandomSound(mixer, soundBank.StrafeLeftSounds, Chest.transform, volumeScale, spatialBlend);
				break;
			case SoundEnum.StrafeRight:
				PlayRandomSound(mixer, soundBank.StrafeRightSounds, Chest.transform, volumeScale, spatialBlend);
				break;
			case SoundEnum.StepBack:
				PlayRandomSound(mixer, soundBank.StepBackSounds, Chest.transform, volumeScale, spatialBlend);
				break;
			case SoundEnum.BlockHitted:
				PlayRandomSound(mixer, soundBank.BlockHittedSounds, Chest.transform, volumeScale, spatialBlend);
				break;
			case SoundEnum.FaceHitted:
				PlayRandomSound(mixer, soundBank.FaceHittedSounds, Head.transform, volumeScale, spatialBlend);
				break;
			case SoundEnum.Stab:
				PlayRandomSound(mixer, soundBank.StabSounds, Chest.transform, volumeScale, spatialBlend);
				break;
			case SoundEnum.StabBlock:
				PlayRandomSound(mixer, soundBank.StabBlockSounds, Chest.transform, volumeScale, spatialBlend);
				break;
			case SoundEnum.BulletHit:
				PlayRandomSound(mixer, soundBank.BulletHitSounds, Chest.transform, volumeScale, spatialBlend);
				break;
		}
	}

	public Transform GetAimTransform(AimWeaponType aimType) {
		switch (aimType) {
			case AimWeaponType.Head:
				return Head != null ? Head.transform : null;
			case AimWeaponType.Chest:
				return Chest != null ? Chest.transform : null;
			case AimWeaponType.Belly:
				return Belly != null ? Belly.transform : null;
			case AimWeaponType.Foot:
				return Foot == null ? transform : Foot.transform;
			default:
				throw new NotImplementedException();
		}
	}

	public void RagdollApplyImpulseToHead(Vector3 impulseWorldSpace) {
		headImpulseValue = impulseWorldSpace;
	}

	public float RagdollWeight {
		get => ragdollWeight;
		set => ragdollWeight = value;
	}

	public float ActualRagdollWeight {
		get => actualRagdollWeight;
		set {
			if (actualRagdollWeight < 0.949999988079071 && value >= 0.949999988079071)
				PlaySound(SoundEnum.RagdollFall);
			actualRagdollWeight = value;
			var component1 = GetComponent<IKController>();
			if (component1 != null)
				component1.enabled = value == 0.0;
			if (HierarhyStructure == HierarhyStructureEnum.Ordinary) {
				var enable = value > 0.0;
				EnableRagdool(ragdollPelvis, enable);
				EnableRagdool(ragdollLeftHips, enable);
				EnableRagdool(ragdollRightHips, enable);
				EnableRagdool(ragdollLeftKnee, enable);
				EnableRagdool(ragdollRightKnee, enable);
				EnableRagdool(ragdollLeftArm, enable);
				EnableRagdool(ragdollRightArm, enable);
				EnableRagdool(ragdollLeftElbow, enable);
				EnableRagdool(ragdollRightElbow, enable);
				EnableRagdool(ragdollMiddleSpine, enable);
				EnableRagdool(ragdollHead, enable);
				if (ragdollHead != null && enable && headImpulseValue.sqrMagnitude > 0.10000000149011612) {
					var component2 = ragdollHead.GetComponent<Rigidbody>();
					if (component2 != null) {
						component2.AddForceAtPosition(headImpulseValue, ragdollHead.transform.position,
							ForceMode.Impulse);
						headImpulseValue = Vector3.zero;
					}
				}

				EnableRagdool(ragdollWeapon, enable);
			} else {
				if (HierarhyStructure != HierarhyStructureEnum.PuppetMaster || puppet == null)
					return;
				var a = 0.0f;
				if (value < 0.029999999329447746) {
					puppet.pinWeight = 1f;
					puppet.muscleWeight = 1f;
					puppet.mode = PuppetMaster.Mode.Kinematic;
					puppet.state = PuppetMaster.State.Alive;
					if (puppet.gameObject.activeSelf)
						puppet.gameObject.SetActive(false);
				} else if (value > 0.949999988079071) {
					if (!puppet.gameObject.activeSelf)
						puppet.gameObject.SetActive(true);
					puppet.pinWeight = 0.0f;
					puppet.muscleWeight = a;
					puppet.mode = PuppetMaster.Mode.Active;
					puppet.state = PuppetMaster.State.Dead;
				} else {
					if (!puppet.gameObject.activeSelf)
						puppet.gameObject.SetActive(true);
					puppet.pinWeight = 1f - value;
					puppet.muscleWeight = Mathf.Max(a, 1f - value);
					puppet.mode = PuppetMaster.Mode.Active;
					puppet.state = PuppetMaster.State.Alive;
				}
			}
		}
	}

	public bool RgdollInternalCollisions {
		get => ragdollInternalCollisions;
		set {
			ragdollInternalCollisions = value;
			if (HierarhyStructure != HierarhyStructureEnum.PuppetMaster || !(puppet != null))
				return;
			puppet.internalCollisions = ragdollInternalCollisions;
		}
	}

	private void EnableRagdool(GameObject go, bool enable) {
		if (!(go != null))
			return;
		go.layer = enable
			? ScriptableObjectInstance<GameSettingsData>.Instance.RagdollLayer.GetIndex()
			: ScriptableObjectInstance<GameSettingsData>.Instance.DynamicLayer.GetIndex();
		var component1 = go.GetComponent<Rigidbody>();
		if (component1 != null) {
			component1.isKinematic = !enable;
			component1.useGravity = enable;
		}

		var component2 = go.GetComponent<Collider>();
		if (component2 != null)
			component2.enabled = enable;
	}

	void IEntityAttachable.Attach(IEntity owner) {
		Owner = owner;
		var component = Owner.GetComponent<NavigationComponent>();
		if (component == null)
			return;
		component.OnTeleport += NavigationComponent_OnTeleport;
		NavigationComponent_OnTeleport(null, null);
	}

	void IEntityAttachable.Detach() {
		var component = Owner.GetComponent<NavigationComponent>();
		if (component != null)
			component.OnTeleport -= NavigationComponent_OnTeleport;
		Owner = null;
	}

	private void NavigationComponent_OnTeleport(
		INavigationComponent navigationComponent,
		IEntity entity) {
		var component = Owner?.GetComponent<LocationItemComponent>();
		Indoor = component != null && component.IsIndoor;
		var indoorChangedEvent = IndoorChangedEvent;
		if (indoorChangedEvent == null)
			return;
		indoorChangedEvent(Indoor);
	}

	public bool HasFastPOIExit(POIAnimationEnum poiType) {
		return supportsFastPOIExits;
	}

	private void Update() {
		if (Mathf.Approximately(ActualRagdollWeight, RagdollWeight))
			return;
		ActualRagdollWeight = RagdollWeight;
	}

	public enum HierarhyStructureEnum {
		Ordinary,
		PuppetMaster
	}

	public enum AimWeaponType {
		Unknown,
		Head,
		Chest,
		Belly,
		Foot
	}

	public enum SoundEnum {
		Unknown,
		RagdollFall,
		HittedVocal,
		HittedLowStaminaVocal,
		HittedDodgeVocal,
		StaggerVocal,
		StaggerNonVocal,
		StrafeLeft,
		StrafeRight,
		StepBack,
		BlockHitted,
		FaceHitted,
		Stab,
		StabBlock,
		BulletHit
	}
}