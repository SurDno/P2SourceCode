using Engine.Source.Commons;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class UnityApplicationNode : FlowControlNode
  {
    private ValueInput<bool> inputValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        InstanceByRequest<EngineApplication>.Instance.IsPaused = this.inputValue.value;
        output.Call();
      }));
      this.inputValue = this.AddValueInput<bool>("IsPause");
    }
  }
}
