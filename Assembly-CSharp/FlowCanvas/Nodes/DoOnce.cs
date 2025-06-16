using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Filters")]
  [Description("Filters Out to be called only once. After the first call, Out is no longer called until Reset is called")]
  public class DoOnce : FlowControlNode
  {
    private bool called;

    public override void OnGraphStarted() => this.called = false;

    protected override void RegisterPorts()
    {
      FlowOutput o = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        if (this.called)
          return;
        this.called = true;
        o.Call();
      }));
      this.AddFlowInput("Reset", (FlowHandler) (() => this.called = false));
    }
  }
}
