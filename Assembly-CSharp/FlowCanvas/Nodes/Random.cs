using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using System.Collections.Generic;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Description("Calls one random output each time In is called")]
  [ContextDefinedOutputs(new System.Type[] {typeof (int)})]
  public class Random : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 4;
    private int current;

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
        this.current = UnityEngine.Random.Range(0, this.portCount);
        outs[this.current].Call();
      }));
      this.AddValueOutput<int>("Current", (ValueHandler<int>) (() => this.current));
    }
  }
}
