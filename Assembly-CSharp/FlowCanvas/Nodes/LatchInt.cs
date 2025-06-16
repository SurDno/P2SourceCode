using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Name("Latch Integer")]
  [Category("Flow Controllers/Flow Convert")]
  [Description("Convert a Flow signal to an integer value")]
  [ContextDefinedInputs(new System.Type[] {typeof (int)})]
  public class LatchInt : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 4;
    private int latched;

    public int portCount
    {
      get => this._portCount;
      set => this._portCount = value;
    }

    protected override void RegisterPorts()
    {
      FlowOutput o = this.AddFlowOutput("Out");
      for (int index = 0; index < this.portCount; ++index)
      {
        int i = index;
        this.AddFlowInput(i.ToString(), (FlowHandler) (() =>
        {
          this.latched = i;
          o.Call();
        }));
      }
      this.AddValueOutput<int>("Value", (ValueHandler<int>) (() => this.latched));
    }
  }
}
