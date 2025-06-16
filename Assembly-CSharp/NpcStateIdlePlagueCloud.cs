using Engine.Behaviours.Components;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateIdlePlagueCloud : INpcState {
	private EngineBehavior behavior;
	private NavMeshAgent agent;
	private Pivot pivot;
	private NpcState npcState;
	private bool agentWasEnabled;
	private bool inited;
	private bool failed;

	public GameObject GameObject { get; private set; }

	[Inspected] public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

	private bool TryInit() {
		if (inited)
			return true;
		behavior = pivot.GetBehavior();
		agent = pivot.GetAgent();
		failed = false;
		inited = true;
		return true;
	}

	public NpcStateIdlePlagueCloud(NpcState npcState, Pivot pivot) {
		this.npcState = npcState;
		this.pivot = pivot;
		GameObject = npcState.gameObject;
	}

	public void Activate() {
		if (!TryInit() || !(bool)(Object)agent)
			return;
		agentWasEnabled = agent.enabled;
		agent.enabled = true;
	}

	public void Shutdown() {
		if (failed || !(bool)(Object)agent)
			return;
		agent.enabled = agentWasEnabled;
	}

	public void OnAnimatorMove() {
		if (!failed)
			;
	}

	public void OnAnimatorEventEvent(string obj) {
		if (!failed)
			;
	}

	public void Update() {
		if (failed || !InstanceByRequest<EngineApplication>.Instance.IsPaused)
			;
	}

	public void OnLodStateChanged(bool enabled) { }
}