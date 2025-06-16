using System.ComponentModel;
using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class ShakeCameraNode : FlowControlNode, IUpdatable {
	[Port("Value")] private ValueInput<float> valueInput;

	public void Update() {
		GameCamera.Instance.Camera.transform.localPosition = Vector3.one * Random.value * valueInput.value;
	}
}