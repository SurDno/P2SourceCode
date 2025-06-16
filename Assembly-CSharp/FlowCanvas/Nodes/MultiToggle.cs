using System.Collections.Generic;
using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Name("Toggle (Multi)")]
  [Description("Whenever any input is called the current output is called as well. Calling '+' or '-' also changes the current output")]
  [Category("Flow Controllers/Togglers")]
  [ContextDefinedOutputs(typeof (int))]
  public class MultiToggle : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 4;
    private int current;

    public int portCount
    {
      get => _portCount;
      set => _portCount = value;
    }

    public override string name
    {
      get => base.name + " " + string.Format("[{0}]", current.ToString());
    }

    public override void OnGraphStarted() => current = 0;

    public override void OnGraphStoped() => current = 0;

    protected override void RegisterPorts()
    {
      List<FlowOutput> outs = new List<FlowOutput>();
      for (int index = 0; index < portCount; ++index)
        outs.Add(AddFlowOutput(index.ToString()));
      AddFlowInput("In", () => outs[current].Call());
      AddFlowInput("+", () =>
      {
        current = (int) Mathf.Repeat(current + 1, portCount);
        outs[current].Call();
      });
      AddFlowInput("-", () =>
      {
        current = (int) Mathf.Repeat(current - 1, portCount);
        outs[current].Call();
      });
      AddValueOutput("Current", () => current);
    }
  }
}
