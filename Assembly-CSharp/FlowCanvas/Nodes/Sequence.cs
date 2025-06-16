using System.Collections.Generic;
using Engine.Common.Generator;
using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;

namespace FlowCanvas.Nodes
{
  [Name("Flip Flop (Multi)")]
  [Category("Flow Controllers/Togglers")]
  [Description("Each time input is signaled, the next output in order is called. After the last output, the order loops from the start.\nReset, resets the current index to zero.")]
  [ContextDefinedOutputs(typeof (int))]
  public class Sequence : FlowControlNode, IMultiPortNode
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    private int _portCount = 4;
    private int current;

    public int portCount
    {
      get => _portCount;
      set => _portCount = value;
    }

    public override void OnGraphStarted() => current = 0;

    public override void OnGraphStoped() => current = 0;

    public override string name
    {
      get => base.name + " " + string.Format("[{0}]", current.ToString());
    }

    protected override void RegisterPorts()
    {
      List<FlowOutput> outs = new List<FlowOutput>();
      for (int index = 0; index < portCount; ++index)
        outs.Add(AddFlowOutput(index.ToString()));
      AddFlowInput("In", () =>
      {
        outs[current].Call();
        current = (int) Mathf.Repeat((float) (current + 1), (float) portCount);
      });
      AddFlowInput("Reset", () => current = 0);
      AddValueOutput("Current", () => current);
    }
  }
}
