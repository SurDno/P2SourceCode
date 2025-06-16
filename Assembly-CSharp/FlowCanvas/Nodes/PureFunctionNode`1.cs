namespace FlowCanvas.Nodes
{
  public abstract class PureFunctionNode<TResult> : PureFunctionNodeBase
  {
    public abstract TResult Invoke();

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      node.AddValueOutput("Value", () => Invoke());
    }
  }
}
