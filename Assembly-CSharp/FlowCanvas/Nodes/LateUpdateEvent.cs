using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System;

namespace FlowCanvas.Nodes
{
  [Name("On Late Update")]
  [Category("Events/Graph")]
  [Description("Called per-frame, but after normal Update")]
  public class LateUpdateEvent : EventNode
  {
    private FlowOutput lateUpdate;

    protected override void RegisterPorts() => this.lateUpdate = this.AddFlowOutput("Out");

    public override void OnGraphStarted()
    {
      BlueprintManager.current.onLateUpdate += new Action(this.LateUpdate);
    }

    public override void OnGraphStoped()
    {
      BlueprintManager.current.onLateUpdate -= new Action(this.LateUpdate);
    }

    private void LateUpdate() => this.lateUpdate.Call();
  }
}
