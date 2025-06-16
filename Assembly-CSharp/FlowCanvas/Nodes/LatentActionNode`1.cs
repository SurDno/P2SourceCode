using System.Collections;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
  public abstract class LatentActionNode<T1> : LatentActionNodeBase
  {
    public abstract IEnumerator Invoke(T1 a);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      base.OnRegisterPorts(node);
      ValueInput<T1> p1 = node.AddValueInput<T1>(parameters[0].Name.SplitCamelCase());
      node.AddFlowInput("In", () => Begin(Invoke(p1.value)));
      node.AddFlowInput("Break", () => Break());
    }
  }
}
