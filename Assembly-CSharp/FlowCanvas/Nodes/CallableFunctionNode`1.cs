namespace FlowCanvas.Nodes
{
  public abstract class CallableFunctionNode<TResult> : CallableFunctionNodeBase
  {
    private TResult result;

    public abstract TResult Invoke();

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      FlowOutput o = node.AddFlowOutput(" ");
      node.AddValueOutput("Value", () => result);
      node.AddFlowInput(" ", () =>
      {
        result = Invoke();
        o.Call();
      });
    }
  }
}
