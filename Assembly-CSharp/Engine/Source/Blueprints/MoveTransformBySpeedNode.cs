using System.Collections;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class MoveTransformBySpeedNode : FlowControlNode {
	private ValueInput<Transform> fromInput;
	private ValueInput<Transform> toInput;
	private ValueInput<Transform> targetInput;
	private ValueInput<float> speedInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var target = targetInput.value;
			if (!(target != null))
				return;
			var from = fromInput.value;
			if (from != null) {
				var to = toInput.value;
				if (to != null) {
					target.position = from.position;
					target.rotation = from.rotation;
					StartCoroutine(Move(target, from, to, speedInput.value, output));
				}
			}
		});
		fromInput = AddValueInput<Transform>("From");
		toInput = AddValueInput<Transform>("To");
		targetInput = AddValueInput<Transform>("Target");
		speedInput = AddValueInput<float>("Speed");
	}

	private IEnumerator Move(
		Transform target,
		Transform from,
		Transform to,
		float speed,
		FlowOutput output) {
		var progress = 0.0f;
		var distance = (to.position - from.position).magnitude;
		while (true) {
			yield return null;
			progress += Time.deltaTime * speed;
			if (progress < (double)distance) {
				target.position = Vector3.Lerp(from.position, to.position, progress / distance);
				target.rotation = Quaternion.Lerp(from.rotation, to.rotation, progress / distance);
			} else
				break;
		}

		target.position = to.position;
		target.rotation = to.rotation;
		output.Call();
	}
}