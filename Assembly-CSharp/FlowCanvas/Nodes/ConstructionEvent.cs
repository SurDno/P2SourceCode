using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("On Awake")]
  [Category("Events/Graph")]
  [Description("Called only once and the first time the Graph is enabled.\nUse this only for initialization of this graph.")]
  public class ConstructionEvent : EventNode
  {
    private FlowOutput awake;
    private bool called;

    public override void OnGraphStarted()
    {
      if (called)
        return;
      called = true;
      awake.Call();
    }

    protected override void RegisterPorts() => awake = AddFlowOutput("Once");
  }
}
