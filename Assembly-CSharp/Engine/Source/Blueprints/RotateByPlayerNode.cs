using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class RotateByPlayerNode : FlowControlNode {
	private ValueInput<Transform> targetValue;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var transform = targetValue.value;
			if (transform != null) {
				var player = ServiceLocator.GetService<ISimulation>().Player;
				if (player != null) {
					var gameObject = ((IEntityView)player).GameObject;
					if (gameObject != null && Mathf.Sign(Vector3.Dot(transform.parent.rotation * Vector3.forward,
						    gameObject.transform.rotation * Vector3.forward)) == -1.0) {
						var eulerAngles = transform.rotation.eulerAngles;
						eulerAngles.y += 180f;
						transform.rotation = Quaternion.Euler(eulerAngles);
					}
				}
			}

			output.Call();
		});
		targetValue = AddValueInput<Transform>("Target");
	}
}