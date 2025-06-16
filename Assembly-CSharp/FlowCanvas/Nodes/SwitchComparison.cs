using ParadoxNotion.Design;
using System;

namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Switchers")]
  [Description("Branch the Flow based on a comparison between two comparable objects")]
  [ContextDefinedInputs(new Type[] {typeof (IComparable)})]
  public class SwitchComparison : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      FlowOutput equal = this.AddFlowOutput("==");
      FlowOutput notEqual = this.AddFlowOutput("!=");
      FlowOutput greater = this.AddFlowOutput(">");
      FlowOutput less = this.AddFlowOutput("<");
      ValueInput<IComparable> a = this.AddValueInput<IComparable>("A");
      ValueInput<IComparable> b = this.AddValueInput<IComparable>("B");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IComparable comparable1 = a.value;
        IComparable comparable2 = b.value;
        if (comparable1 == null || comparable2 == null)
        {
          if (comparable1 == comparable2)
            equal.Call();
          if (comparable1 == comparable2)
            return;
          notEqual.Call();
        }
        else
        {
          if (comparable1.CompareTo((object) comparable2) == 0)
            equal.Call();
          else
            notEqual.Call();
          if (comparable1.CompareTo((object) comparable2) == 1)
            greater.Call();
          if (comparable1.CompareTo((object) comparable2) == -1)
            less.Call();
        }
      }));
    }
  }
}
