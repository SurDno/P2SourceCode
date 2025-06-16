using ParadoxNotion;

namespace FlowCanvas.Nodes
{
  public abstract class CallableActionNode<T1, T2, T3, T4, T5, T6> : CallableActionNodeBase
  {
    public abstract void Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      FlowOutput o = node.AddFlowOutput(" ");
      ValueInput<T1> p1 = node.AddValueInput<T1>(this.parameters[0].Name.SplitCamelCase());
      ValueInput<T2> p2 = node.AddValueInput<T2>(this.parameters[1].Name.SplitCamelCase());
      ValueInput<T3> p3 = node.AddValueInput<T3>(this.parameters[2].Name.SplitCamelCase());
      ValueInput<T4> p4 = node.AddValueInput<T4>(this.parameters[3].Name.SplitCamelCase());
      ValueInput<T5> p5 = node.AddValueInput<T5>(this.parameters[4].Name.SplitCamelCase());
      ValueInput<T6> p6 = node.AddValueInput<T6>(this.parameters[5].Name.SplitCamelCase());
      node.AddFlowInput(" ", (FlowHandler) (() =>
      {
        this.Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value);
        o.Call();
      }));
    }
  }
}
