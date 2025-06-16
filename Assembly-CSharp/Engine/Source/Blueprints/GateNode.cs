using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GateNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> gateInput;
    private ValueInput<bool> openInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        IDoorComponent doorComponent = gateInput.value;
        if (doorComponent != null)
        {
          if (openInput.value)
            doorComponent.Opened.Value = true;
          else
            doorComponent.Opened.Value = false;
        }
        output.Call();
      });
      gateInput = AddValueInput<IDoorComponent>("Gate");
      openInput = AddValueInput<bool>("Open");
    }
  }
}
