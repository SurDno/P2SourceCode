using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Services;
using Engine.Impl.Tasks;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStatePointOfInterest : INpcState, INpcStateNeedSyncBack {
	private NPCEnemy enemy;
	private NpcState npcState;
	private Pivot pivot;
	private EngineBehavior behavior;
	private POISetup poiSetup;
	private Animator animator;
	private Rigidbody rigidbody;
	private NavMeshAgent agent;
	private NPCWeaponService weaponService;
	private AnimatorState45 animatorState;
	private FightAnimatorBehavior.AnimatorState fightAnimatorState;
	private Vector3 _poiDeltaPos;
	private Vector3 _poiDeltaPosPassed;
	private float _poiDeltaAngle;
	private bool _poiDeltaFinished;
	private float _poiDeltaSyncDelay;
	private float _poiSyncPositionTimeLeft;
	private float _poiSyncRotationTimeLeft;
	private POIBase _poi;
	private POIService poiService;
	private bool frozenForDialog;
	private bool animationIsQuick;
	[Inspected] private GameObject poiTarget;
	[Inspected] private StateEnum state;
	[Inspected] private float timeLeft;
	private Vector3 deltaPosition;
	private Vector3 poiStartPosition;
	[Inspected] private POIAnimationEnum animation;
	[Inspected] private int animationIndex;
	[Inspected] private int animationsCount;
	private bool initiallyKinematic;
	[Inspected] private bool inited;
	[Inspected] private bool failed;
	private bool dialogActivityChecked;
	private Vector3 enterPoint;
	private float timeToNextRandomAnimationSet;
	private float timeToNextRandomAnimationSetMax = 2f;
	private bool syncBackInited;
	private bool couldPlayReactionAnimation;
	private bool neededExtraExitPOI;
	private Vector3 poiBackPosition;

	public GameObject GameObject { get; private set; }

	private bool TryInit() {
		if (inited)
			return true;
		behavior = pivot.GetBehavior();
		poiSetup = GameObject.GetComponent<POISetup>();
		agent = pivot.GetAgent();
		rigidbody = pivot.GetRigidbody();
		weaponService = pivot.GetNpcWeaponService();
		animator = pivot.GetAnimator();
		enemy = pivot.GetNpcEnemy();
		if (animator == null) {
			Debug.LogError("Null animator " + GameObject.name, GameObject);
			Debug.LogError("Null animator " + GameObject.GetFullName());
			failed = true;
			return false;
		}

		animatorState = AnimatorState45.GetAnimatorState(animator);
		fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(animator);
		syncBackInited = false;
		if (poiSetup == null) {
			failed = true;
			return false;
		}

		failed = false;
		inited = true;
		return true;
	}

	public NpcStatePointOfInterest(NpcState npcState, Pivot pivot) {
		GameObject = npcState.gameObject;
		this.pivot = pivot;
		this.npcState = npcState;
	}

	public NpcStateStatusEnum Status =>
		state == StateEnum.End ? NpcStateStatusEnum.Success : NpcStateStatusEnum.Running;

	public void Activate(
		float poiTime,
		POIBase poi,
		POIAnimationEnum animation,
		int animationIndex,
		int animationsCount) {
		if (!TryInit())
			return;
		poiStartPosition = GameObject.transform.position;
		this.animation = animation;
		this.animationIndex = animationIndex;
		this.animationsCount = animationsCount;
		if (enemy != null)
			couldPlayReactionAnimation = enemy.CanPlayReactionAnimation;
		neededExtraExitPOI = npcState.NeedExtraExitPOI;
		if ((bool)(Object)rigidbody) {
			initiallyKinematic = rigidbody.isKinematic;
			rigidbody.isKinematic = true;
		}

		animatorState.ControlMovableState = AnimatorState45.MovableState45.POI;
		animatorState.ControlPOIAnimationIndex = animationIndex;
		animatorState.ControlPOIMiddleAnimationsCount = animationsCount;
		animatorState.ControlPOIStartFromMiddle = false;
		animatorState.MovableStop = false;
		npcState.NeedExtraExitPOI = false;
		switch (animation) {
			case POIAnimationEnum.S_SitAtDesk:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitAtDesk;
				break;
			case POIAnimationEnum.S_SitOnBench:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitOnBench;
				if (enemy != null)
					enemy.CanPlayReactionAnimation = false;
				npcState.NeedExtraExitPOI = true;
				break;
			case POIAnimationEnum.S_LeanOnWall:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_LeanOnWall;
				break;
			case POIAnimationEnum.S_LeanOnTable:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_LeanOnTable;
				break;
			case POIAnimationEnum.S_SitNearWall:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitNearWall;
				if (enemy != null)
					enemy.CanPlayReactionAnimation = false;
				npcState.NeedExtraExitPOI = true;
				break;
			case POIAnimationEnum.S_LieOnBed:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_LieOnBed;
				break;
			case POIAnimationEnum.S_NearFire:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_NearFire;
				break;
			case POIAnimationEnum.Q_ViewPoster:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ViewPoster;
				break;
			case POIAnimationEnum.Q_LookOutOfTheWindow:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_LookOutOfTheWindow;
				break;
			case POIAnimationEnum.Q_LookUnder:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_LookUnder;
				break;
			case POIAnimationEnum.Q_LookIntoTheWindow:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_LookIntoTheWindow;
				break;
			case POIAnimationEnum.Q_ActionWithWall:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithWall;
				break;
			case POIAnimationEnum.Q_ActionWithTable:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithTable;
				break;
			case POIAnimationEnum.Q_ActionWithWardrobe:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithWardrobe;
				break;
			case POIAnimationEnum.Q_ActionWithShelves:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithShelves;
				break;
			case POIAnimationEnum.Q_ActionWithNightstand:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithNightstand;
				break;
			case POIAnimationEnum.Q_ActionOnFloor:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionOnFloor;
				break;
			case POIAnimationEnum.S_ActionOnFloor:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_ActionOnFloor;
				break;
			case POIAnimationEnum.Q_Idle:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_Idle;
				break;
			case POIAnimationEnum.Q_NearFire:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_NearFire;
				break;
			case POIAnimationEnum.S_Dialog:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_Dialog;
				break;
			case POIAnimationEnum.S_Loot:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_Loot;
				break;
			case POIAnimationEnum.Q_PlaygroundPlay:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_PlaygroundPlay;
				break;
			case POIAnimationEnum.S_PlaygroundSandbox:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_PlaygroundSandbox;
				break;
			case POIAnimationEnum.S_PlaygroundCooperative:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_PlaygroundCooperative;
				break;
			case POIAnimationEnum.Q_PlaygroundCooperative:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_PlaygroundCooperative;
				break;
			case POIAnimationEnum.S_SitAtDeskRight:
				animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitAtDeskRight;
				break;
		}

		timeLeft = !poiSetup.GetAnimationPlayOnce(animation, animationIndex) ? poiTime : 0.0f;
		poiService = ServiceLocator.GetService<POIService>();
		state = StateEnum.Prepare;
		_poi = poi;
		poiTarget = null;
		dialogActivityChecked = false;
		frozenForDialog = false;
		animationIsQuick = poiSetup.AnimationIsQuick(animation);
		if (poiSetup.GetNeedSynchronizeAnimation(animation, animationIndex))
			SynchronizeAnimation();
		SetRandomNextAnimation();
		if (!(weaponService != null))
			return;
		weaponService.Weapon = WeaponEnum.Unknown;
	}

	public void ActivateLoot(float poiTime, GameObject target) {
		if (!TryInit())
			return;
		_poi = null;
		poiTarget = target;
		animation = POIAnimationEnum.S_Loot;
		animationIndex = 0;
		animationsCount = 1;
		if ((bool)(Object)rigidbody) {
			initiallyKinematic = rigidbody.isKinematic;
			rigidbody.isKinematic = true;
		}

		if (animatorState != null) {
			animatorState.ControlMovableState = AnimatorState45.MovableState45.POI;
			animatorState.ControlPOIAnimationIndex = animationIndex;
			animatorState.ControlPOIMiddleAnimationsCount = animationsCount;
			animatorState.ControlPOIStartFromMiddle = false;
			animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_Loot;
		}

		timeLeft = !poiSetup.GetAnimationPlayOnce(animation, animationIndex) ? poiTime : 0.0f;
		state = StateEnum.Prepare;
		poiStartPosition = GameObject.transform.position;
		SetRandomNextAnimation();
	}

	public void Shutdown() {
		if (failed)
			return;
		if (Status != NpcStateStatusEnum.Success)
			SetPOIDeltaParams(poiStartPosition - GameObject.transform.position, 0.0f, 0.3f, 0.0f);
		deltaPosition = Vector3.zero;
		if ((bool)(Object)rigidbody)
			rigidbody.isKinematic = initiallyKinematic;
		if (poiService != null)
			poiService.RemoveCharacterAsDialogTarget(GameObject);
		if (weaponService != null)
			weaponService.Weapon = npcState.Weapon;
		if (enemy != null)
			enemy.CanPlayReactionAnimation = couldPlayReactionAnimation;
		npcState.NeedExtraExitPOI = neededExtraExitPOI;
	}

	public void OnAnimatorMove() {
		if (failed)
			return;
		var deltaTime = animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;
		if (animatorState.IsPOI) {
			agent.nextPosition = animator.rootPosition;
			GameObject.transform.position += animator.deltaPosition;
			GameObject.transform.rotation *= animator.deltaRotation;
			if (_poiDeltaFinished)
				return;
			AddPOIDelta(deltaTime);
		} else {
			if (!_poiDeltaFinished)
				AddPOIDelta(deltaTime);
			if (agent.isActiveAndEnabled && agent.isOnNavMesh) {
				var vector3 = GameObject.transform.position + animator.deltaPosition;
				vector3.y = Mathf.MoveTowards(vector3.y, agent.nextPosition.y, deltaTime * 0.1f);
				agent.nextPosition = vector3;
				GameObject.transform.position = agent.nextPosition;
			}

			GameObject.transform.rotation *=
				Quaternion.AngleAxis(57.29578f * animator.angularVelocity.y * deltaTime, Vector3.up);
		}
	}

	private void AddPOIDelta(float deltaTime) {
		if (_poiSyncRotationTimeLeft > 0.0) {
			GameObject.transform.rotation *=
				Quaternion.AngleAxis(_poiDeltaAngle * Mathf.Min(_poiSyncRotationTimeLeft, deltaTime), Vector3.up);
			_poiSyncRotationTimeLeft -= deltaTime;
		}

		if (_poiDeltaSyncDelay > 0.0)
			_poiDeltaSyncDelay -= deltaTime;
		if (_poiDeltaSyncDelay <= 0.0 && _poiSyncPositionTimeLeft > 0.0) {
			var num = Mathf.Min(_poiSyncPositionTimeLeft, deltaTime);
			GameObject.transform.position += _poiDeltaPos * num;
			_poiSyncPositionTimeLeft -= num;
		}

		if (_poiSyncRotationTimeLeft > 0.0 || _poiSyncPositionTimeLeft > 0.0)
			return;
		_poiDeltaFinished = true;
	}

	private void SynchronizeAnimation() {
		Vector3 closestTargetPosition;
		Quaternion closestTargetRotation;
		if (_poi != null) {
			_poi.GetClosestTargetPoint(animation, animationIndex, poiSetup, GameObject.transform.position,
				out closestTargetPosition, out closestTargetRotation);
			poiBackPosition = closestTargetPosition - poiSetup.GetAnimationOffset(animation, animationIndex);
		} else {
			closestTargetPosition = poiTarget.transform.position;
			closestTargetRotation = poiTarget.transform.rotation;
			poiBackPosition = closestTargetPosition;
		}

		var deltaPos = closestTargetPosition - GameObject.transform.position;
		var vector3 = GameObject.transform.InverseTransformVector(closestTargetRotation * Vector3.forward);
		var deltaAngle = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
		var animationSyncDelay = poiSetup.GetAnimationSyncDelay(animation, animationIndex);
		deltaPosition = deltaPos;
		var syncTime = 1f;
		SetPOIDeltaParams(deltaPos, deltaAngle, syncTime, animationSyncDelay);
	}

	public void SetPOIDeltaParams(
		Vector3 deltaPos,
		float deltaAngle,
		float syncTime,
		float syncDelay) {
		_poiDeltaPos = deltaPos / syncTime;
		_poiDeltaAngle = deltaAngle / syncTime;
		_poiDeltaPosPassed = Vector3.zero;
		_poiDeltaSyncDelay = syncDelay;
		_poiDeltaFinished = false;
		_poiSyncPositionTimeLeft = syncTime;
		_poiSyncRotationTimeLeft = syncTime;
	}

	public void OnAnimatorEventEvent(string obj) {
		if (!failed)
			;
	}

	public void Update() {
		if (failed || InstanceByRequest<EngineApplication>.Instance.IsPaused ||
		    (poiTarget == null && (_poi == null || _poi.gameObject == null)))
			return;
		if (state == StateEnum.Prepare) {
			if (!animatorState.IsPOI)
				return;
			state = StateEnum.POI;
		} else if (state == StateEnum.POI) {
			if (!dialogActivityChecked) {
				dialogActivityChecked = true;
				enterPoint = GameObject.transform.position;
				if (_poi != null && _poi.SupportsDialog && poiService != null && timeLeft > 0.0)
					poiService.AddCharacterAsDialogTarget(GameObject, this);
			}

			if (!UpdateDuringPOI(_poi))
				return;
			animatorState.MovableStop = true;
			state = !animatorState.IsPOIExit
				? !animationIsQuick ? StateEnum.WaitForPOIExitStart : StateEnum.WaitForPOIEnd
				: StateEnum.WaitForPOIEnd;
		} else if (state == StateEnum.WaitForPOIExitStart) {
			if (animatorState.IsPOIExit || !animatorState.IsPOI) {
				state = StateEnum.WaitForPOIEnd;
				SetSyncBack();
			}
		} else if (state == StateEnum.WaitForPOIEnd && UpdateWaitForPOIEnd())
			state = StateEnum.End;

		if (fightAnimatorState == null || !fightAnimatorState.IsReaction || syncBackInited)
			return;
		SetSyncBack();
		animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
		if (weaponService != null)
			weaponService.Weapon = npcState.Weapon;
	}

	private bool UpdatDuringeStopping() {
		return animatorState.IsPOI;
	}

	private bool UpdateDuringPOI(POIBase poi) {
		timeToNextRandomAnimationSet -= Time.deltaTime;
		if (timeToNextRandomAnimationSet <= 0.0) {
			timeToNextRandomAnimationSet = timeToNextRandomAnimationSetMax;
			SetRandomNextAnimation();
		}

		if (frozenForDialog)
			return false;
		timeLeft -= Time.deltaTime;
		return timeLeft < 0.0;
	}

	private bool UpdateWaitForPOIEnd() {
		return !animatorState.IsPOI;
	}

	private void SetSyncBack() {
		syncBackInited = true;
		SetPOIDeltaParams(new Vector3(0.0f, -deltaPosition.y, 0.0f), 0.0f, 0.5f, 0.0f);
	}

	public void SetDialogFreeze(bool freeze) {
		frozenForDialog = freeze;
	}

	public Vector3 GetEnterPoint() {
		return enterPoint;
	}

	public POIBase GetPOI() {
		return _poi;
	}

	public void LookAt(GameObject target) {
		if (GameObject == null)
			return;
		if (target == null)
			Debug.LogError(
				"target == null, Такого быть недолжно, воспроизвести, разобраться в чем дело и пофиксить нормально");
		else {
			var component = GameObject.GetComponent<IKController>();
			if (!(component != null))
				return;
			component.enabled = true;
			component.LookTarget = target.transform;
			component.LookEyeContactOnly = false;
			component.StopIfOutOfLimits = true;
		}
	}

	private void SetRandomNextAnimation() {
		animator.SetInteger("Movable.POI.AnimationIndex2",
			Random.Range(0, animator.GetInteger("Movable.POI.MiddleAnimationsCount")));
	}

	public Vector3 GetSyncBackPosition() {
		return poiBackPosition;
	}

	public POIAnimationEnum GetPoiType() {
		return animation;
	}

	public void OnLodStateChanged(bool enabled) { }

	private enum StateEnum {
		Prepare,
		POI,
		WaitForPOIExitStart,
		WaitForPOIEnd,
		End
	}
}