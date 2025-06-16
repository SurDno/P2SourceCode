using Engine.Behaviours.Components;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateMove : INpcState {
	private NpcState npcState;
	private Pivot pivot;
	private Animator animator;
	private NPCWeaponService weaponService;
	private Rigidbody rigidbody;
	private AnimatorUpdateMode initialAnimatorUpdateMode;
	private AnimatorCullingMode initialAnimatorCullingMode;
	private bool agentWasEnabled;
	private bool rigidbodyWasKinematic;
	private bool rigidbodyWasGravity;
	[Inspected] private StateEnum state = StateEnum.WaitingPath;
	[Inspected] private bool failed;
	[Inspected] private NavMeshAgent agent;
	[Inspected] private bool failOnPartialPath;
	[Inspected] private bool pathIsPartial;
	[Inspected] private Vector3 destination;
	private EngineBehavior behavior;
	private int prevAreaMask;
	private bool inited;

	public GameObject GameObject { get; private set; }

	[Inspected] private NavMeshAgentWrapper agentWrapper => new(agent);

	[Inspected] public NpcStateStatusEnum Status { get; private set; }

	private bool TryInit() {
		if (inited)
			return true;
		behavior = pivot.GetBehavior();
		agent = pivot.GetAgent();
		animator = pivot.GetAnimator();
		weaponService = pivot.GetNpcWeaponService();
		rigidbody = pivot.GetRigidbody();
		inited = true;
		return true;
	}

	public NpcStateMove(NpcState npcState, Pivot pivot) {
		GameObject = npcState.gameObject;
		this.pivot = pivot;
		this.npcState = npcState;
	}

	public void Activate(Vector3 destination, bool failOnPartialPath = false) {
		failed = false;
		Status = NpcStateStatusEnum.Running;
		if (!TryInit())
			return;
		this.failOnPartialPath = failOnPartialPath;
		this.destination = destination;
		state = StateEnum.WaitingPath;
		prevAreaMask = agent.areaMask;
		agentWasEnabled = agent.enabled;
		pathIsPartial = false;
		var indoor = true;
		if (npcState.Owner != null) {
			var component = (LocationItemComponent)npcState.Owner.GetComponent<ILocationItemComponent>();
			if (component == null) {
				Debug.LogWarning(GameObject.name + ": location component not found");
				Status = NpcStateStatusEnum.Failed;
				return;
			}

			if (component != null)
				indoor = component.IsIndoor;
		}

		if (rigidbody != null) {
			rigidbodyWasKinematic = rigidbody.isKinematic;
			rigidbodyWasGravity = rigidbody.useGravity;
			rigidbody.useGravity = false;
			rigidbody.isKinematic = false;
		}

		NPCStateHelper.SetAgentAreaMask(agent, indoor);
		agent.enabled = true;
		if (!agent.isOnNavMesh) {
			var position = GameObject.transform.position;
			if (NavMeshUtility.SampleRaycastPosition(ref position, indoor ? 1f : 5f, indoor ? 2f : 10f, agent.areaMask))
				agent.Warp(position);
			else {
				Status = NpcStateStatusEnum.Failed;
				return;
			}
		}

		agent.SetDestination(destination);
		if (animator != null) {
			initialAnimatorUpdateMode = animator.updateMode;
			animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
			initialAnimatorCullingMode = animator.cullingMode;
			animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
		}

		if (!(weaponService != null))
			return;
		weaponService.Weapon = WeaponEnum.Unknown;
	}

	public void Shutdown() {
		if (failed)
			return;
		if (animator != null) {
			animator.updateMode = initialAnimatorUpdateMode;
			animator.cullingMode = initialAnimatorCullingMode;
		}

		if (rigidbody != null) {
			rigidbody.isKinematic = rigidbodyWasKinematic;
			rigidbody.useGravity = rigidbodyWasGravity;
		}

		agent.areaMask = prevAreaMask;
		agent.enabled = agentWasEnabled;
		if (!(weaponService != null))
			return;
		weaponService.Weapon = npcState.Weapon;
	}

	public void OnAnimatorMove() {
		if (failed)
			return;
		behavior.OnExternalAnimatorMove();
	}

	public void OnAnimatorEventEvent(string obj) {
		if (!failed)
			;
	}

	public void Update() {
		if (!inited)
			return;
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused) {
			if (!(agent != null))
				return;
			agent.velocity = Vector3.zero;
		} else {
			if (state == StateEnum.WaitingPath)
				OnUpdateWaitPath();
			if (state == StateEnum.Moving)
				OnUpdateMove();
			if (state == StateEnum.Stopping)
				OnUpdateStopMovement();
			if (state == StateEnum.StopAndRestartPath)
				OnUpdateStopAndRestartPath();
			if (state != StateEnum.Done)
				return;
			Status = failed ? NpcStateStatusEnum.Failed : NpcStateStatusEnum.Success;
		}
	}

	public void OnUpdateWaitPath() {
		if (agent.pathPending)
			return;
		if (Random.value < Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(agent)) {
			Debug.Log(
				ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ")
					.GetInfo(npcState.Owner), GameObject);
			var destination = agent.destination;
			agent.ResetPath();
			agent.SetDestination(destination);
			state = StateEnum.WaitingPath;
		} else {
			pathIsPartial = agent.pathStatus == NavMeshPathStatus.PathPartial;
			if (pathIsPartial && failOnPartialPath)
				CompleteTask(true);
			else if (agent.pathStatus == NavMeshPathStatus.PathInvalid) {
				var destination = agent.destination;
				agent.ResetPath();
				agent.SetDestination(destination);
				state = StateEnum.WaitingPath;
			} else if (!NavMeshUtility.HasPathNoGarbage(agent) ||
			           (Random.value < Time.deltaTime / 0.5 && !NavMeshUtility.HasPathWithGarbage(agent))) {
				Debug.LogWarningFormat("{0} : agent.path.corners.Length == 0, distance to destination = {1}",
					GameObject.name, (GameObject.transform.position - agent.destination).magnitude);
				CompleteTask(false);
			} else {
				var stoppingDistance = agent.stoppingDistance;
				if (agent.desiredVelocity.magnitude < 0.0099999997764825821) {
					if (agent.remainingDistance < (double)stoppingDistance)
						CompleteTask(false);
					agent.ResetPath();
					agent.SetDestination(destination);
				} else {
					state = StateEnum.Moving;
					behavior.StartMovement(agent.desiredVelocity.normalized);
				}
			}
		}
	}

	public void OnUpdateMove() {
		if (Random.value < Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(agent)) {
			Debug.Log(
				ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ")
					.GetInfo(npcState.Owner), GameObject);
			state = StateEnum.StopAndRestartPath;
		} else {
			var num = agent.stoppingDistance * 3f;
			if (!agent.hasPath || !behavior.Move(agent.desiredVelocity, agent.remainingDistance) ||
			    agent.remainingDistance >= (double)num)
				return;
			agent.ResetPath();
			state = StateEnum.Stopping;
		}
	}

	public void OnUpdateStopAndRestartPath() {
		if (!behavior.Move(agent.desiredVelocity, 0.0f))
			return;
		var destination = agent.destination;
		agent.ResetPath();
		agent.SetDestination(destination);
		state = StateEnum.WaitingPath;
	}

	public void OnUpdateStopMovement() {
		if (!behavior.Move(agent.desiredVelocity, 0.0f))
			return;
		state = StateEnum.Done;
	}

	private void CompleteTask(bool failed) {
		if (!this.failed)
			this.failed = failed;
		if (state == StateEnum.Moving) {
			behavior.Move(agent.desiredVelocity, 0.0f);
			state = StateEnum.Stopping;
		} else
			state = StateEnum.Done;
	}

	public void OnLodStateChanged(bool inLodState) {
		npcState.AnimatorEnabled = !inLodState;
		var component = npcState.Owner?.GetComponent<EffectsComponent>();
		if (component != null)
			component.Disabled = inLodState;
		agent.updatePosition = inLodState;
		agent.updateRotation = inLodState;
		agent.speed = behavior.Gait == EngineBehavior.GaitType.Walk ? 1.5f : 3f;
		agent.acceleration = 10f;
	}

	private enum StateEnum {
		WaitingPath,
		Moving,
		Stopping,
		StopAndRestartPath,
		Done
	}
}