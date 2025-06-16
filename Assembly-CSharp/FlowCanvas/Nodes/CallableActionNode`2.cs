using ParadoxNotion;

namespace FlowCanvas.Nodes
{
  public abstract class CallableActionNode<T1, T2> : CallableActionNodeBase
  {
    public abstract void Invoke(T1 a, T2 b);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      FlowOutput o = node.AddFlowOutput(" ");
      ValueInput<T1> p1 = node.AddValueInput<T1>(this.parameters[0].Name.SplitCamelCase());
      ValueInput<T2> p2 = node.AddValueInput<T2>(this.parameters[1].Name.SplitCamelCase());
      node.AddFlowInput(" ", (FlowHandler) (() =>
      {
        this.Invoke(p1.value, p2.value);
        o.Call();
      }));
    }
  }
}
