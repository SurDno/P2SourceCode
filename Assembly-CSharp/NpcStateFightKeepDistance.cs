using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateFightKeepDistance : INpcState {
	private NpcState npcState;
	private Pivot pivot;
	private NPCEnemy enemy;
	private NavMeshAgent agent;
	private Animator animator;
	private AnimatorState45 animatorState;
	private NPCWeaponService weaponService;
	private IKController ikController;
	private int prevAreaMask;
	private bool agentWasEnabled;
	private float keepDistance;
	private bool strafe;
	private bool inited;
	private bool failed;

	public GameObject GameObject { get; private set; }

	[Inspected] public NpcStateStatusEnum Status { get; protected set; }

	private bool TryInit() {
		if (inited)
			return true;
		enemy = pivot.GetNpcEnemy();
		agent = pivot.GetAgent();
		animator = pivot.GetAnimator();
		weaponService = pivot.GetNpcWeaponService();
		ikController = GameObject.GetComponent<IKController>();
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

	private bool IsEnemyRunningAway() {
		return enemy.Enemy.Velocity.magnitude >= 0.5 && Vector3.Dot(enemy.transform.forward,
			(enemy.Enemy.transform.position - enemy.transform.position).normalized) > 0.25;
	}

	public NpcStateFightKeepDistance(NpcState npcState, Pivot pivot) {
		GameObject = npcState.gameObject;
		this.pivot = pivot;
		this.npcState = npcState;
	}

	public void Activate(float keepDistance, bool strafe, bool aim) {
		if (!TryInit())
			return;
		prevAreaMask = agent.areaMask;
		agentWasEnabled = agent.enabled;
		this.keepDistance = keepDistance;
		this.strafe = strafe;
		weaponService.Weapon = npcState.Weapon;
		npcState.WeaponChangeEvent += State_WeaponChangeEvent;
		Status = NpcStateStatusEnum.Running;
		var component = (LocationItemComponent)npcState.Owner.GetComponent<ILocationItemComponent>();
		if (component == null) {
			Debug.LogWarning(GameObject.name + ": location component not found");
			Status = NpcStateStatusEnum.Failed;
		} else {
			var isIndoor = component.IsIndoor;
			NPCStateHelper.SetAgentAreaMask(agent, isIndoor);
			agent.enabled = true;
			if (!agent.isOnNavMesh) {
				var position = GameObject.transform.position;
				if (NavMeshUtility.SampleRaycastPosition(ref position, isIndoor ? 1f : 5f, isIndoor ? 2f : 10f,
					    agent.areaMask))
					agent.Warp(position);
				else {
					Status = NpcStateStatusEnum.Failed;
					return;
				}
			}

			if (!((ikController != null) & aim) || !(enemy != null) || !(enemy.Enemy != null))
				return;
			ikController.WeaponTarget = enemy.Enemy.transform;
		}
	}

	private void State_WeaponChangeEvent(WeaponEnum weapon) {
		weaponService.Weapon = weapon;
	}

	public void Shutdown() {
		if (failed)
			return;
		npcState.WeaponChangeEvent -= State_WeaponChangeEvent;
		agent.areaMask = prevAreaMask;
		agent.enabled = agentWasEnabled;
		if (!(ikController != null))
			return;
		ikController.WeaponTarget = null;
	}

	public void OnAnimatorMove() {
		if (failed)
			return;
		enemy?.OnExternalAnimatorMove();
	}

	public void OnAnimatorEventEvent(string obj) {
		if (!failed)
			;
	}

	public void Update() {
		if (failed)
			return;
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
			agent.nextPosition = GameObject.transform.position;
		else {
			enemy.RotationTarget = null;
			enemy.RotateByPath = false;
			enemy.RetreatAngle = new float?();
			if (enemy.Enemy == null)
				return;
			var forward = enemy.Enemy.transform.position - enemy.transform.position;
			var magnitude = forward.magnitude;
			forward.Normalize();
			if (enemy.IsReacting) {
				if (enemy.IsAttacking || enemy.IsContrReacting)
					enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation,
						Quaternion.AngleAxis(0.0f, Vector3.up) * Quaternion.LookRotation(forward),
						270f * Time.deltaTime);
				Status = NpcStateStatusEnum.Running;
			} else {
				float num1;
				if (magnitude < (double)keepDistance) {
					if (false) {
						var retreatDirection2 =
							PathfindingHelper.FindBestRetreatDirection2(enemy.transform, enemy.Enemy.transform);
						if (!retreatDirection2.HasValue) {
							num1 = 0.0f;
							enemy.RotationTarget = enemy.Enemy.transform;
						} else {
							var quaternion = Quaternion.LookRotation(forward) *
							                 Quaternion.AngleAxis(retreatDirection2.Value, Vector3.up);
							num1 = PathfindingHelper.IsFreeSpace(enemy.transform.position,
								enemy.transform.position - enemy.transform.forward * 1f)
								? -2f
								: 0.0f;
							enemy.RotationTarget = enemy.Enemy.transform;
						}
					}

					var flag = strafe && magnitude < 2.0;
					var distance =
						PathfindingHelper.FindDistance(enemy.transform.position, -enemy.transform.forward, 10f);
					var num2 = flag
						? PathfindingHelper.FindDistance(enemy.transform.position, -enemy.transform.right, 2.5f)
						: 0.0f;
					var num3 = flag
						? PathfindingHelper.FindDistance(enemy.transform.position, enemy.transform.right, 2.5f)
						: 0.0f;
					if (num2 > (double)distance && num2 > (double)num3) {
						num1 = 0.0f;
						enemy.RotationTarget = enemy.Enemy.transform;
						animatorState.SetTrigger("Fight.Triggers/StepLeft");
					} else if (num3 > (double)distance && num3 > (double)num2) {
						num1 = 0.0f;
						enemy.RotationTarget = enemy.Enemy.transform;
						animatorState.SetTrigger("Fight.Triggers/StepRight");
					} else {
						Quaternion.LookRotation(forward);
						num1 = distance > 1.0 ? -2f : 0.0f;
						enemy.RotationTarget = enemy.Enemy.transform;
					}
				} else if (magnitude < keepDistance + 1.0) {
					num1 = 0.0f;
					enemy.RotationTarget = enemy.Enemy.transform;
				} else {
					num1 = 1f;
					enemy.RotationTarget = enemy.Enemy.transform;
				}

				enemy.DesiredWalkSpeed = num1;
				agent.nextPosition = animator.rootPosition;
				Status = NpcStateStatusEnum.Running;
			}
		}
	}

	public void OnLodStateChanged(bool enabled) { }
}