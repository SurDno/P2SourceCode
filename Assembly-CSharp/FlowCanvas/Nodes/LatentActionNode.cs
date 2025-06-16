using System.Collections;

namespace FlowCanvas.Nodes
{
  public abstract class LatentActionNode : LatentActionNodeBase
  {
    public abstract IEnumerator Invoke();

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      base.OnRegisterPorts(node);
      node.AddFlowInput("In", (FlowHandler) (() => this.Begin(this.Invoke())));
      node.AddFlowInput("Break", (FlowHandler) (() => this.Break()));
    }
  }
}
