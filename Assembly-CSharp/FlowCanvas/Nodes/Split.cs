using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Description("Split the Flow in multiple directions. Calls all outputs in the same frame but in order")]
  public class Split : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 4;

    public int portCount
    {
      get => this._portCount;
      set => this._portCount = value;
    }

    protected override void RegisterPorts()
    {
      List<FlowOutput> outs = new List<FlowOutput>();
      for (int index = 0; index < this.portCount; ++index)
        outs.Add(this.AddFlowOutput(index.ToString()));
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        for (int index = 0; index < this.portCount; ++index)
          outs[index].Call();
      }));
    }
  }
}
