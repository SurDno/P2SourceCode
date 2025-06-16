using ParadoxNotion;

namespace FlowCanvas.Nodes
{
  public abstract class CallableActionNode<T1> : CallableActionNodeBase
  {
    public abstract void Invoke(T1 a);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      FlowOutput o = node.AddFlowOutput(" ");
      ValueInput<T1> p1 = node.AddValueInput<T1>(this.parameters[0].Name.SplitCamelCase());
      node.AddFlowInput(" ", (FlowHandler) (() =>
      {
        this.Invoke(p1.value);
        o.Call();
      }));
    }
  }
}
