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
      ValueInput<T1> p1 = node.AddValueInput<T1>(this.parameters[0].Name.SplitCamelCase());
      node.AddValueOutput<TResult>("Value", (ValueHandler<TResult>) (() => this.result));
      node.AddFlowInput(" ", (FlowHandler) (() =>
      {
        this.result = this.Invoke(p1.value);
        o.Call();
      }));
    }
  }
}
