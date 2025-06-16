using Engine.Behaviours.Components;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateMoveCloud : INpcState {
	private Pivot pivot;
	private NpcState npcState;
	[Inspected] private StateEnum state = StateEnum.Moving;
	[Inspected] private NavMeshAgent agent;
	[Inspected] private Vector3 destination;
	private int prevAreaMask;
	private bool agentWasEnabled;
	private float speed = 2f;

	public GameObject GameObject { get; private set; }

	[Inspected] public NpcStateStatusEnum Status { get; private set; }

	public NpcStateMoveCloud(NpcState npcState, Pivot pivot) {
		this.npcState = npcState;
		this.pivot = pivot;
		GameObject = npcState.gameObject;
	}

	public void Activate(Vector3 destination) {
		Status = NpcStateStatusEnum.Running;
		agent = pivot.GetAgent();
		if (agent != null) {
			speed = agent.speed;
			agentWasEnabled = agent.enabled;
			agent.enabled = false;
		}

		this.destination = destination;
		state = StateEnum.Moving;
	}

	public void Shutdown() {
		if (!(agent != null))
			return;
		agent.enabled = agentWasEnabled;
	}

	public void OnAnimatorMove() { }

	public void OnAnimatorEventEvent(string obj) { }

	public void Update() {
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
			return;
		if (state == StateEnum.Moving)
			OnUpdateMove();
		if (state != StateEnum.Done)
			return;
		Status = NpcStateStatusEnum.Success;
	}

	public void OnUpdateMove() {
		var vector3 = destination - GameObject.transform.position;
		var magnitude = vector3.magnitude;
		var num = 0.2f;
		GameObject.transform.position += vector3.normalized * speed * Time.deltaTime;
		if (magnitude >= (double)num)
			return;
		state = StateEnum.Done;
	}

	public void OnLodStateChanged(bool enabled) { }

	private enum StateEnum {
		Moving,
		Done
	}
}