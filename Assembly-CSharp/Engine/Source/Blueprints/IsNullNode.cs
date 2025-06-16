using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class IsNullNode : FlowControlNode
  {
    private ValueInput<object> inputValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput trueOut = AddFlowOutput("True");
      FlowOutput falseOut = AddFlowOutput("False");
      AddFlowInput("In", () =>
      {
        if (inputValue.value == null)
          trueOut.Call();
        else
          falseOut.Call();
      });
      inputValue = AddValueInput<object>("Target");
    }
  }
}
