using ParadoxNotion.Design;
using System.Collections.Generic;

namespace FlowCanvas.Nodes
{
  [Description("Enumerate a value (usualy a list or array) for each of it's elements")]
  [Category("Flow Controllers/Iterators")]
  public class ForEach<T> : FlowControlNode
  {
    private T current;
    private bool broken;

    protected override void RegisterPorts()
    {
      ValueInput<IEnumerable<T>> list = this.AddValueInput<IEnumerable<T>>("Value");
      this.AddValueOutput<T>("Current", (ValueHandler<T>) (() => this.current));
      FlowOutput fCurrent = this.AddFlowOutput("Do");
      FlowOutput fFinish = this.AddFlowOutput("Done");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IEnumerable<T> objs = list.value;
        if (objs == null)
        {
          fFinish.Call();
        }
        else
        {
          this.broken = false;
          foreach (T obj in objs)
          {
            if (!this.broken)
            {
              this.current = obj;
              fCurrent.Call();
            }
            else
              break;
          }
          fFinish.Call();
        }
      }));
      this.AddFlowInput("Break", (FlowHandler) (() => this.broken = true));
    }
  }
}
