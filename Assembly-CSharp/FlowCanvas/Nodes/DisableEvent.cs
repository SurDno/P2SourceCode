using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("On Disable")]
  [Category("Events/Graph")]
  [Description("Called when the Graph is Disabled")]
  public class DisableEvent : EventNode
  {
    private FlowOutput disable;

    public override void OnGraphStoped() => this.disable.Call();

    protected override void RegisterPorts() => this.disable = this.AddFlowOutput("Out");
  }
}
