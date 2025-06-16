using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("On Enable")]
  [Category("Events/Graph")]
  [Description("Called when the Graph is enabled")]
  public class EnableEvent : EventNode
  {
    private FlowOutput enable;

    public override void OnGraphStarted() => this.enable.Call();

    protected override void RegisterPorts() => this.enable = this.AddFlowOutput("Out");
  }
}
