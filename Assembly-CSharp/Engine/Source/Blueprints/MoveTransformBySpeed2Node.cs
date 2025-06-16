using System.Collections;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class MoveTransformBySpeed2Node : FlowControlNode {
	private ValueInput<Transform> fromInput;
	private ValueInput<Transform> toInput;
	private ValueInput<Transform> targetInput;
	private ValueInput<float> speedInput;
	private ValueInput<AnimationCurve> curveInput;
	private ValueInput<Vector3> characterSpeedInput;
	private ValueInput<float> speedOutInput;
	private const float minTime = 0.3f;

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
					StartCoroutine(Move(target, from, to, speedInput.value, output, characterSpeedInput.value,
						curveInput.value, speedOutInput.value));
				}
			}
		});
		fromInput = AddValueInput<Transform>("From");
		toInput = AddValueInput<Transform>("To");
		targetInput = AddValueInput<Transform>("Target");
		speedInput = AddValueInput<float>("Speed");
		curveInput = AddValueInput<AnimationCurve>("Curve");
		characterSpeedInput = AddValueInput<Vector3>("Character Speed");
		speedOutInput = AddValueInput<float>("Speed Out");
	}

	private Vector3 EvaluateCubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		return (float)((1.0 - t) * (1.0 - t) * (1.0 - t)) * p0 + (float)(3.0 * t * (1.0 - t) * (1.0 - t)) * p1 +
		       (float)(3.0 * t * t * (1.0 - t)) * p2 + t * t * t * p3;
	}

	private Vector3 EvaluateCubicBezierDerivative(
		Vector3 p0,
		Vector3 p1,
		Vector3 p2,
		Vector3 p3,
		float t) {
		return (float)(-3.0 * (1.0 - t) * (1.0 - t)) * p0 +
		       (float)(3.0 * (1.0 - t) * (1.0 - t) - 6.0 * (1.0 - t) * t) * p1 +
		       (float)(6.0 * (1.0 - t) * t - 3.0 * t * t) * p2 + 3f * t * t * p3;
	}

	private Vector3 EvaluateCubicBezierDerivative2(
		Vector3 p0,
		Vector3 p1,
		Vector3 p2,
		Vector3 p3,
		float t) {
		return (float)(6.0 * (1.0 - t)) * p0 + (float)(-6.0 * (1.0 - t) - 6.0 * (1.0 - 2.0 * t)) * p1 +
		       (float)(6.0 * (1.0 - 2.0 * t) - 6.0 * t) * p2 + 6f * t * p3;
	}

	private Vector3 EvaluateQuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		return (float)((1.0 - t) * (1.0 - t)) * p0 + (float)(2.0 * t * (1.0 - t)) * p1 + t * t * p2;
	}

	private Vector3 EvaluateQuadraticBezierDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		return (float)(-2.0 * (1.0 - t)) * p0 + (float)(2.0 * (1.0 - 2.0 * t)) * p1 + 2f * t * p2;
	}

	private IEnumerator Move(
		Transform target,
		Transform from,
		Transform to,
		float speed,
		FlowOutput output,
		Vector3 characterSpeedInput,
		AnimationCurve curve,
		float speedOut) {
		var progress = 0.0f;
		var vector3 = to.position - from.position;
		var distance = vector3.magnitude;
		var enterTime = distance / speed;
		if (enterTime < 0.30000001192092896)
			enterTime = 0.3f;
		enterTime *= 5f;
		var p0 = from.transform.position;
		var p1 = p0 + from.transform.forward * 0.3f * distance;
		var p3 = to.transform.position;
		var p2 = p3 - to.transform.forward * 0.3f * distance;
		vector3 = EvaluateCubicBezierDerivative(p0, p1, p2, p3, 0.0f);
		var derivative0 = vector3.magnitude;
		var scale = characterSpeedInput.magnitude / derivative0;
		var currentSpeed = characterSpeedInput.magnitude;
		while (true) {
			yield return null;
			var derivative = EvaluateCubicBezierDerivative(p0, p1, p2, p3, progress).magnitude;
			currentSpeed += 5f * Time.deltaTime;
			progress += Time.deltaTime / enterTime;
			if (progress < 1.0) {
				target.position = EvaluateCubicBezier(p0, p1, p2, p3, progress);
				target.rotation = Quaternion.LerpUnclamped(from.rotation, to.rotation, progress);
			} else
				break;
		}

		target.position = to.position;
		target.rotation = to.rotation;
		output.Call();
	}
}