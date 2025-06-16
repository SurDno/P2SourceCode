using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Name("OR")]
  [Category("Flow Controllers/Flow Merge")]
  [Description("Calls Out when either input is called")]
  public class ORMerge : FlowControlNode, IMultiPortNode
  {
    private FlowOutput fOut;
    [SerializeField]
    private int _portCount = 2;

    public int portCount
    {
      get => this._portCount;
      set => this._portCount = value;
    }

    protected override void RegisterPorts()
    {
      this.fOut = this.AddFlowOutput("Out");
      for (int index = 0; index < this.portCount; ++index)
        this.AddFlowInput(index.ToString(), (FlowHandler) (() => this.fOut.Call()));
    }
  }
}
