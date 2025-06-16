using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class TeleportByTransformNode : FlowControlNode {
	private ValueInput<IEntity> who;
	private ValueInput<ILocationComponent> targetLocation;
	private ValueInput<Transform> targetTransform;
	private FlowOutput output;
	private bool wait;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			if (who.value == null)
				Debug.LogError(typeof(TeleportByTransformNode) + " who.value == null");
			else if (targetTransform.value == null)
				Debug.LogError(typeof(TeleportByTransformNode) + " targetTransform.value == null");
			else if (targetLocation.value == null)
				Debug.LogError(typeof(TeleportByTransformNode) + " targetLocation.value == null");
			else {
				var component = who.value.GetComponent<NavigationComponent>();
				if (component == null)
					Debug.LogError(typeof(TeleportByTransformNode) + " navigation == null");
				else if (wait)
					Debug.LogError(typeof(TeleportByTransformNode) + " is waiting");
				else {
					wait = true;
					component.OnTeleport += OnTeleport;
					component.TeleportTo(targetLocation.value, targetTransform.value.position,
						targetTransform.value.rotation);
				}
			}
		});
		who = AddValueInput<IEntity>("Who");
		targetLocation = AddValueInput<ILocationComponent>("Location");
		targetTransform = AddValueInput<Transform>("Transform");
	}

	private void OnTeleport(INavigationComponent owner, IEntity target) {
		if (!wait)
			Debug.LogError("OnTeleport event is not wait");
		wait = false;
		owner.OnTeleport -= OnTeleport;
		output.Call();
	}
}