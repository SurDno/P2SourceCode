using Engine.Source.Utility;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class WaitingPlayerCanControllingNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() => this.StartCoroutine(this.WaitingPlayerCanControlling(output))));
    }

    private IEnumerator WaitingPlayerCanControlling(FlowOutput output)
    {
      while (!PlayerUtility.IsPlayerCanControlling)
        yield return (object) null;
      output.Call();
    }
  }
}
