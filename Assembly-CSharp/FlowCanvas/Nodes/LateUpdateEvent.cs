using ParadoxNotion.Design;
using ParadoxNotion.Services;

namespace FlowCanvas.Nodes
{
  [Name("On Late Update")]
  [Category("Events/Graph")]
  [Description("Called per-frame, but after normal Update")]
  public class LateUpdateEvent : EventNode
  {
    private FlowOutput lateUpdate;

    protected override void RegisterPorts() => lateUpdate = AddFlowOutput("Out");

    public override void OnGraphStarted()
    {
      BlueprintManager.current.onLateUpdate += LateUpdate;
    }

    public override void OnGraphStoped()
    {
      BlueprintManager.current.onLateUpdate -= LateUpdate;
    }

    private void LateUpdate() => lateUpdate.Call();
  }
}
