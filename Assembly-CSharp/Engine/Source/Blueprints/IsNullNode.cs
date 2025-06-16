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
      FlowOutput trueOut = this.AddFlowOutput("True");
      FlowOutput falseOut = this.AddFlowOutput("False");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        if (this.inputValue.value == null)
          trueOut.Call();
        else
          falseOut.Call();
      }));
      this.inputValue = this.AddValueInput<object>("Target");
    }
  }
}
