namespace FlowCanvas.Nodes
{
  public abstract class CallableActionNode : CallableActionNodeBase
  {
    public abstract void Invoke();

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      FlowOutput o = node.AddFlowOutput(" ");
      node.AddFlowInput(" ", (FlowHandler) (() =>
      {
        this.Invoke();
        o.Call();
      }));
    }
  }
}
