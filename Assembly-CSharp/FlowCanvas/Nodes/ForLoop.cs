using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;

namespace FlowCanvas.Nodes
{
  [Description("Perform a for loop")]
  [Category("Flow Controllers/Iterators")]
  [ContextDefinedInputs(typeof (int))]
  [ContextDefinedOutputs(typeof (int))]
  public class ForLoop : FlowControlNode
  {
    private int current;
    private bool broken;

    protected override void RegisterPorts()
    {
      ValueInput<int> n = AddValueInput<int>("Loops");
      AddValueOutput("Index", () => current);
      FlowOutput fCurrent = AddFlowOutput("Do");
      FlowOutput fFinish = AddFlowOutput("Done");
      AddFlowInput("In", () =>
      {
        current = 0;
        broken = false;
        for (int index = 0; index < n.value && !broken; ++index)
        {
          current = index;
          fCurrent.Call();
        }
        fFinish.Call();
      });
      AddFlowInput("Break", () => broken = true);
    }
  }
}
