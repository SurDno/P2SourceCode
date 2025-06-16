using System.Collections;
using Engine.Common.Services;
using Engine.Services.Engine.Assets;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class WaitingAllAssetsLoadedNode : FlowControlNode {
	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => StartCoroutine(WaitingPlayerCanControlling(output)));
	}

	private IEnumerator WaitingPlayerCanControlling(FlowOutput output) {
		var assetLoader = ServiceLocator.GetService<AssetLoader>();
		while (!assetLoader.IsEmpty)
			yield return null;
		output.Call();
	}
}