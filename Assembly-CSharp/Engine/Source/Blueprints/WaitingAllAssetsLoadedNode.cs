using Engine.Common.Services;
using Engine.Services.Engine.Assets;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class WaitingAllAssetsLoadedNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() => this.StartCoroutine(this.WaitingPlayerCanControlling(output))));
    }

    private IEnumerator WaitingPlayerCanControlling(FlowOutput output)
    {
      AssetLoader assetLoader = ServiceLocator.GetService<AssetLoader>();
      while (!assetLoader.IsEmpty)
        yield return (object) null;
      output.Call();
    }
  }
}
