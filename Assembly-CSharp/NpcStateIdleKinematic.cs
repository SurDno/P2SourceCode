using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings.External;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateIdleKinematic : INpcState {
	private EngineBehavior behavior;
	private NavMeshAgent agent;
	private Rigidbody rigidbody;
	private Pivot pivot;
	private Animator animator;
	private NpcState npcState;
	private NPCWeaponService weaponService;
	private bool agentWasEnabled;
	private bool rigidbodyWasKinematic;
	private AnimatorUpdateMode initialAnimatorUpdateMode;
	private AnimatorCullingMode initialAnimatorCullingMode;
	private AnimatorState45 animatorState;
	private bool inited;
	private bool failed;
	private bool sayReplics;
	private float timeToNextReplic;

	public GameObject GameObject { get; private set; }

	[Inspected] public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

	private bool TryInit() {
		if (inited)
			return true;
		behavior = pivot.GetBehavior();
		rigidbody = pivot.GetRigidbody();
		weaponService = pivot.GetNpcWeaponService();
		agent = pivot.GetAgent();
		animator = pivot.GetAnimator();
		if (animator == null) {
			Debug.LogError("Null animator " + GameObject.name, GameObject);
			Debug.LogError("Null animator " + GameObject.GetFullName());
			failed = true;
			return false;
		}

		animatorState = AnimatorState45.GetAnimatorState(animator);
		failed = false;
		inited = true;
		return true;
	}

	public NpcStateIdleKinematic(NpcState npcState, Pivot pivot) {
		this.npcState = npcState;
		this.pivot = pivot;
		GameObject = npcState.gameObject;
	}

	public void Activate(float primaryIdleProbability, bool makeObstacle = false) {
		if (!TryInit())
			return;
		MovementControllerUtility.SetRandomAnimation(animator, pivot.SecondaryIdleAnimationCount,
			pivot.SecondaryLowIdleAnimationCount);
		if (agent != null) {
			agentWasEnabled = agent.enabled;
			agent.enabled = false;
		}

		if (rigidbody != null) {
			rigidbodyWasKinematic = rigidbody.isKinematic;
			rigidbody.isKinematic = true;
		}

		animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
		animatorState.PrimaryIdleProbability = primaryIdleProbability;
		initialAnimatorUpdateMode = animator.updateMode;
		animator.updateMode = AnimatorUpdateMode.Normal;
		initialAnimatorCullingMode = animator.cullingMode;
		animator.cullingMode = AnimatorCullingMode.CullCompletely;
		if (makeObstacle)
			CreateObstacle();
		if (npcState.Owner != null) {
			var component = npcState.Owner.GetComponent<ParametersComponent>();
			if (component != null) {
				var byName = component.GetByName<bool>(ParameterNameEnum.SayReplicsInIdle);
				if (byName != null) {
					sayReplics = byName.Value;
					if (sayReplics)
						timeToNextReplic =
							Random.Range(
								ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMin,
								ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMax);
				}
			}
		}

		if (!(weaponService != null))
			return;
		weaponService.Weapon = WeaponEnum.Unknown;
	}

	private void CreateObstacle() {
		agent.enabled = false;
		var obstacle = pivot.GetObstacle();
		if (!(obstacle != null))
			return;
		obstacle.enabled = true;
		obstacle.carving = true;
		obstacle.radius = agent.radius;
	}

	public void Shutdown() {
		if (failed)
			return;
		if (agent != null)
			agent.enabled = agentWasEnabled;
		if ((bool)(Object)rigidbody)
			rigidbody.isKinematic = rigidbodyWasKinematic;
		animator.updateMode = initialAnimatorUpdateMode;
		animator.cullingMode = initialAnimatorCullingMode;
		var obstacle = pivot.GetObstacle();
		if (obstacle != null)
			obstacle.enabled = false;
		if (!(weaponService != null))
			return;
		weaponService.Weapon = npcState.Weapon;
	}

	public void OnAnimatorMove() {
		if (failed)
			return;
		var num = animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;
		GameObject.transform.position += animator.deltaPosition;
		GameObject.transform.rotation *= Quaternion.AngleAxis(57.29578f * animator.angularVelocity.y * num, Vector3.up);
	}

	public void OnAnimatorEventEvent(string obj) {
		if (!failed)
			;
	}

	public void Update() {
		if (failed)
			return;
		if (animatorState.ControlMovableState != AnimatorState45.MovableState45.Idle)
			animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !sayReplics)
			return;
		timeToNextReplic -= Time.deltaTime;
		if (timeToNextReplic <= 0.0) {
			timeToNextReplic =
				Random.Range(ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMin,
					ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMax);
			NPCStateHelper.SayIdleReplic(npcState.Owner);
		}
	}

	public void OnLodStateChanged(bool inLodState) {
		npcState.AnimatorEnabled = !inLodState;
		var component = npcState.Owner?.GetComponent<EffectsComponent>();
		if (component == null)
			return;
		component.Disabled = inLodState;
	}
}