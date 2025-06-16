using ParadoxNotion.Design;
using System;

namespace FlowCanvas.Nodes
{
  [Name("Switch Condition")]
  [Category("Flow Controllers/Switchers")]
  [Description("Branch the Flow based on a conditional boolean value")]
  [ContextDefinedInputs(new Type[] {typeof (bool)})]
  public class SwitchBool : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      ValueInput<bool> c = this.AddValueInput<bool>("Condition");
      FlowOutput tOut = this.AddFlowOutput("True");
      FlowOutput fOut = this.AddFlowOutput("False");
      this.AddFlowInput("In", (FlowHandler) (() => this.Call(c.value ? tOut : fOut)));
    }
  }
}
