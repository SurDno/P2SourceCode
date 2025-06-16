using System.Collections;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class MoveTransformByTimeNode : FlowControlNode {
	private ValueInput<Transform> fromInput;
	private ValueInput<Transform> toInput;
	private ValueInput<Transform> targetInput;
	private ValueInput<float> timeInput;
	private ValueInput<AnimationCurve> curveInput;

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
					StartCoroutine(Move(target, from, to, timeInput.value, curveInput.value, output));
				}
			}
		});
		fromInput = AddValueInput<Transform>("From");
		toInput = AddValueInput<Transform>("To");
		targetInput = AddValueInput<Transform>("Target");
		timeInput = AddValueInput<float>("Time");
		curveInput = AddValueInput<AnimationCurve>("Curve");
	}

	private IEnumerator Move(
		Transform target,
		Transform from,
		Transform to,
		float time,
		AnimationCurve curve,
		FlowOutput output) {
		var progress = 0.0f;
		while (true) {
			yield return null;
			if (!(target == null)) {
				if (!(from == null)) {
					if (!(to == null)) {
						progress += Time.deltaTime;
						if (progress < (double)time) {
							var value = progress / time;
							if (curve != null && curve.keys != null && curve.keys.Length > 1)
								value = curve.Evaluate(value);
							target.position = Vector3.LerpUnclamped(from.position, to.position, value);
							target.rotation = Quaternion.LerpUnclamped(from.rotation, to.rotation, value);
						} else
							goto label_7;
					} else
						goto label_5;
				} else
					goto label_3;
			} else
				break;
		}

		Debug.LogError("target == null");
		goto label_12;
		label_3:
		Debug.LogError("from == null");
		goto label_12;
		label_5:
		Debug.LogError("to == null");
		goto label_12;
		label_7:
		target.position = to.position;
		target.rotation = to.rotation;
		label_12:
		output.Call();
	}
}