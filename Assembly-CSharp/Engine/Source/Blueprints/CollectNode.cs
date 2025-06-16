using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CollectNode : FlowControlNode
  {
    private ValueInput<CollectControllerComponent> controllerInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        this.controllerInput.value?.Collect();
        output.Call();
      }));
      this.controllerInput = this.AddValueInput<CollectControllerComponent>("Controller");
    }
  }
}
