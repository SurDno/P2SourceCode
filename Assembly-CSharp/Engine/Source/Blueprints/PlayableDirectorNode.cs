using System;
using FlowCanvas;
using FlowCanvas.Nodes;
using InputServices;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.Playables;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class PlayableDirectorNode : FlowControlNode, IUpdatable {
	private ValueInput<PlayableDirector> directorInput;
	private ValueInput<bool> interruptibleInput;
	private PlayableDirector director;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			director = !(director != null) ? directorInput.value : throw new Exception();
			if (director == null)
				output.Call();
			else {
				Action<PlayableDirector> stopped = null;
				stopped = tmp => {
					director.stopped -= stopped;
					director = null;
					output.Call();
				};
				director.stopped += stopped;
				director.Play();
			}
		});
		directorInput = AddValueInput<PlayableDirector>("Director");
		interruptibleInput = AddValueInput<bool>("Interruptible");
	}

	public void Update() {
		if (!interruptibleInput.value || director == null ||
		    (!Input.GetKeyDown(KeyCode.Escape) && !InputService.Instance.GetButtonDown("B", false)))
			return;
		director.Stop();
	}
}