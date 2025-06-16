using ParadoxNotion;

namespace FlowCanvas.Nodes
{
  public abstract class PureFunctionNode<TResult, T1, T2, T3> : PureFunctionNodeBase
  {
    public abstract TResult Invoke(T1 a, T2 b, T3 c);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      ValueInput<T1> p1 = node.AddValueInput<T1>(parameters[0].Name.SplitCamelCase());
      ValueInput<T2> p2 = node.AddValueInput<T2>(parameters[1].Name.SplitCamelCase());
      ValueInput<T3> p3 = node.AddValueInput<T3>(parameters[2].Name.SplitCamelCase());
      node.AddValueOutput("Value", () => Invoke(p1.value, p2.value, p3.value));
    }
  }
}
