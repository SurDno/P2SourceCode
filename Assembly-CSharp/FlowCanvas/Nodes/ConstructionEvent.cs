using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("On Awake")]
  [Category("Events/Graph")]
  [Description("Called only once and the first time the Graph is enabled.\nUse this only for initialization of this graph.")]
  public class ConstructionEvent : EventNode
  {
    private FlowOutput awake;
    private bool called = false;

    public override void OnGraphStarted()
    {
      if (this.called)
        return;
      this.called = true;
      this.awake.Call();
    }

    protected override void RegisterPorts() => this.awake = this.AddFlowOutput("Once");
  }
}
