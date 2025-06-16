namespace FlowCanvas.Nodes
{
  public abstract class CallableFunctionNode<TResult> : CallableFunctionNodeBase
  {
    private TResult result;

    public abstract TResult Invoke();

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      FlowOutput o = node.AddFlowOutput(" ");
      node.AddValueOutput<TResult>("Value", (ValueHandler<TResult>) (() => this.result));
      node.AddFlowInput(" ", (FlowHandler) (() =>
      {
        this.result = this.Invoke();
        o.Call();
      }));
    }
  }
}
