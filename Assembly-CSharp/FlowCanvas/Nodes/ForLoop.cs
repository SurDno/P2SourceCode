using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using System;

namespace FlowCanvas.Nodes
{
  [Description("Perform a for loop")]
  [Category("Flow Controllers/Iterators")]
  [ContextDefinedInputs(new Type[] {typeof (int)})]
  [ContextDefinedOutputs(new Type[] {typeof (int)})]
  public class ForLoop : FlowControlNode
  {
    private int current;
    private bool broken;

    protected override void RegisterPorts()
    {
      ValueInput<int> n = this.AddValueInput<int>("Loops");
      this.AddValueOutput<int>("Index", (ValueHandler<int>) (() => this.current));
      FlowOutput fCurrent = this.AddFlowOutput("Do");
      FlowOutput fFinish = this.AddFlowOutput("Done");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        this.current = 0;
        this.broken = false;
        for (int index = 0; index < n.value && !this.broken; ++index)
        {
          this.current = index;
          fCurrent.Call();
        }
        fFinish.Call();
      }));
      this.AddFlowInput("Break", (FlowHandler) (() => this.broken = true));
    }
  }
}
