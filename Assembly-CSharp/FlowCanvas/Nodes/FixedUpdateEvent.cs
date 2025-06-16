using ParadoxNotion.Design;
using ParadoxNotion.Services;

namespace FlowCanvas.Nodes
{
  [Name("On Fixed Update")]
  [Category("Events/Graph")]
  [Description("Called every fixed framerate frame, which should be used when dealing with Physics")]
  public class FixedUpdateEvent : EventNode
  {
    private FlowOutput fixedUpdate;

    protected override void RegisterPorts() => fixedUpdate = AddFlowOutput("Out");

    public override void OnGraphStarted()
    {
      BlueprintManager.current.onFixedUpdate += FixedUpdate;
    }

    public override void OnGraphStoped()
    {
      BlueprintManager.current.onFixedUpdate -= FixedUpdate;
    }

    private void FixedUpdate() => fixedUpdate.Call();
  }
}
