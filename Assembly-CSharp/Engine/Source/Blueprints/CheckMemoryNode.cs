using Engine.Source.Otimizations;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;

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
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() => this.StartCoroutine(this.Compute(output))));
    }

    private IEnumerator Compute(FlowOutput output)
    {
      yield return (object) MemoryStrategy.Instance.Compute(this.contextInput.value);
      output.Call();
    }
  }
}
