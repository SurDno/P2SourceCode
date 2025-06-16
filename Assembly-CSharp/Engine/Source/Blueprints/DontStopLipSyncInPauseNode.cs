using Engine.Source.Commons;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class DontStopLipSyncInPauseNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        InstanceByRequest<EngineApplication>.Instance.DontStopLipSyncInPause = true;
        output.Call();
      }));
    }
  }
}
