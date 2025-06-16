using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using System;

namespace FlowCanvas.Nodes
{
  [Name("Latch Condition")]
  [Category("Flow Controllers/Flow Convert")]
  [Description("Convert a Flow signal to boolean value")]
  [ContextDefinedOutputs(new Type[] {typeof (bool)})]
  public class LatchBool : FlowControlNode
  {
    private bool latched;

    protected override void RegisterPorts()
    {
      FlowOutput o = this.AddFlowOutput("Out");
      this.AddFlowInput("True", (FlowHandler) (() =>
      {
        this.latched = true;
        o.Call();
      }));
      this.AddFlowInput("False", (FlowHandler) (() =>
      {
        this.latched = false;
        o.Call();
      }));
      this.AddValueOutput<bool>("Value", (ValueHandler<bool>) (() => this.latched));
    }
  }
}
