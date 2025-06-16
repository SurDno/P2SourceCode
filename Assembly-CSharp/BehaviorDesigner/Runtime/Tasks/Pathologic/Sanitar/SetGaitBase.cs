using Engine.Behaviours.Components;
using Engine.Source.Components;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic.Sanitar;

public abstract class SetGaitBase : Action {
	protected EngineBehavior behavior;

	public abstract EngineBehavior.GaitType GetGait();

	public override TaskStatus OnUpdate() {
		if (behavior == null) {
			behavior = gameObject.GetComponent<EngineBehavior>();
			if (behavior == null) {
				Debug.LogWarning(
					gameObject.name + ": doesn't contain " + typeof(BehaviorComponent).Name + " engine component",
					gameObject);
				return TaskStatus.Failure;
			}
		}

		behavior.Gait = GetGait();
		return TaskStatus.Success;
	}
}