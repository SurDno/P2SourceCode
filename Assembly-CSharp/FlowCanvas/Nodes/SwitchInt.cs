using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Name("Switch Integer")]
  [Category("Flow Controllers/Switchers")]
  [Description("Branch the Flow based on an integer value. The Default output is called when the Index value is out of range.")]
  [ContextDefinedInputs(new System.Type[] {typeof (int)})]
  public class SwitchInt : FlowControlNode, IMultiPortNode
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
      ValueInput<int> index = this.AddValueInput<int>("Index");
      List<FlowOutput> outs = new List<FlowOutput>();
      for (int index1 = 0; index1 < this.portCount; ++index1)
        outs.Add(this.AddFlowOutput(index1.ToString()));
      FlowOutput def = this.AddFlowOutput("Default");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        int index2 = index.value;
        if (index2 >= 0 && index2 < outs.Count)
          outs[index2].Call();
        else
          def.Call();
      }));
    }
  }
}
