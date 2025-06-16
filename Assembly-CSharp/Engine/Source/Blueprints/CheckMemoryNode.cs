using System.Collections;
using Engine.Source.Otimizations;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CheckMemoryNode : FlowControlNode
  {
    [Port("Context")]
    private ValueInput<MemoryStrategyContextEnum> contextInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", (FlowHandler) (() => StartCoroutine(Compute(output))));
    }

    private IEnumerator Compute(FlowOutput output)
    {
      yield return MemoryStrategy.Instance.Compute(contextInput.value);
      output.Call();
    }
  }
}
