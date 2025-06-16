using System.Collections;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Blueprints.Utility;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class WaitingLoadingLocationNode : FlowControlNode {
	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => StartCoroutine(WaitingLoading(output)));
	}

	private IEnumerator WaitingLoading(FlowOutput output) {
		WaitingLoadingUtility.Logs.Clear();
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player == null) {
			Debug.LogError("player == null");
			output.Call();
		} else {
			var locationItem = player.GetComponent<LocationItemComponent>();
			if (locationItem == null) {
				Debug.LogError("locationItem == null");
				output.Call();
			} else {
				do {
					yield return null;
				} while (locationItem.IsHibernation);

				output.Call();
			}
		}
	}
}