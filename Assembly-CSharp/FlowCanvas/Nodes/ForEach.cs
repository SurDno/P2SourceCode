using System.Collections;
using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;

namespace FlowCanvas.Nodes
{
  [Description("Enumerate a value (usualy a list or array) for each of it's elements")]
  [Category("Flow Controllers/Iterators")]
  [ContextDefinedInputs(typeof (IEnumerable))]
  [ContextDefinedOutputs(typeof (object))]
  public class ForEach : FlowControlNode
  {
    private object current;
    private bool broken;

    protected override void RegisterPorts()
    {
      ValueInput<IEnumerable> list = AddValueInput<IEnumerable>("Value");
      AddValueOutput("Current", () => current);
      FlowOutput fCurrent = AddFlowOutput("Do");
      FlowOutput fFinish = AddFlowOutput("Done");
      AddFlowInput("In", () =>
      {
        IEnumerable enumerable = list.value;
        if (enumerable == null)
        {
          fFinish.Call();
        }
        else
        {
          broken = false;
          foreach (object obj in enumerable)
          {
            if (!broken)
            {
              current = obj;
              fCurrent.Call();
            }
            else
              break;
          }
          fFinish.Call();
        }
      });
      AddFlowInput("Break", () => broken = true);
    }
  }
}
