using ParadoxNotion;

namespace FlowCanvas.Nodes
{
  public abstract class CallableFunctionNode<TResult, T1> : CallableFunctionNodeBase
  {
    private TResult result;

    public abstract TResult Invoke(T1 a);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      FlowOutput o = node.AddFlowOutput(" ");
      ValueInput<T1> p1 = node.AddValueInput<T1>(parameters[0].Name.SplitCamelCase());
      node.AddValueOutput("Value", () => result);
      node.AddFlowInput(" ", () =>
      {
        result = Invoke(p1.value);
        o.Call();
      });
    }
  }
}
