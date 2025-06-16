using Engine.Behaviours.Components;
using UnityEngine;

public class NpcStateMoveFollow : NpcStateMoveBase {
	private Transform target;
	private float followDistance;
	private Vector3 lastDestination;
	private bool completed;

	public NpcStateMoveFollow(NpcState npcState, Pivot pivot)
		: base(npcState, pivot, false) { }

	public void Activate(Transform target, float followDistance) {
		if (!Activate())
			return;
		this.target = target;
		this.followDistance = followDistance;
		completed = false;
		if (target == null) {
			Debug.LogWarning(GameObject.name + ": target is null");
			Status = NpcStateStatusEnum.Failed;
		} else {
			lastDestination = target.position;
			if ((target.position - GameObject.transform.position).magnitude < 1.1000000238418579 * followDistance)
				Status = NpcStateStatusEnum.Success;
			else
				agent.SetDestination(lastDestination);
		}
	}

	public override void DoUpdate() {
		if (target == null) {
			Debug.LogWarning(GameObject.name + ": target is null");
			Status = NpcStateStatusEnum.Failed;
		} else {
			if (completed)
				return;
			if (agent.hasPath && agent.remainingDistance < (double)followDistance) {
				CompleteTask(false);
				completed = true;
			}

			var vector3 = lastDestination - target.position;
			var magnitude = vector3.magnitude;
			vector3 = target.position - GameObject.transform.position;
			if (vector3.magnitude > followDistance * 5.0) {
				if (magnitude > (double)followDistance) {
					lastDestination = target.position;
					agent.SetDestination(lastDestination);
				}
			} else if (magnitude > 0.30000001192092896 * followDistance) {
				lastDestination = target.position;
				agent.SetDestination(lastDestination);
			}
		}
	}
}