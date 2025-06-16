using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class PlayerActionNode : FlowControlNode
  {
    private ValueInput<ActionEnum> actionInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        ServiceLocator.GetService<ISimulation>().Player?.GetComponent<PlayerControllerComponent>()?.ComputeAction(this.actionInput.value);
        output.Call();
      }));
      this.actionInput = this.AddValueInput<ActionEnum>("Action");
    }
  }
}
