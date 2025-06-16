using ParadoxNotion;

namespace FlowCanvas.Nodes
{
  public abstract class PureFunctionNode<TResult, T1, T2> : PureFunctionNodeBase
  {
    public abstract TResult Invoke(T1 a, T2 b);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      ValueInput<T1> p1 = node.AddValueInput<T1>(this.parameters[0].Name.SplitCamelCase());
      ValueInput<T2> p2 = node.AddValueInput<T2>(this.parameters[1].Name.SplitCamelCase());
      node.AddValueOutput<TResult>("Value", (ValueHandler<TResult>) (() => this.Invoke(p1.value, p2.value)));
    }
  }
}
