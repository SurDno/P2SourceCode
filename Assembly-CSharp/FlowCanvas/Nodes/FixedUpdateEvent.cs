using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System;

namespace FlowCanvas.Nodes
{
  [Name("On Fixed Update")]
  [Category("Events/Graph")]
  [Description("Called every fixed framerate frame, which should be used when dealing with Physics")]
  public class FixedUpdateEvent : EventNode
  {
    private FlowOutput fixedUpdate;

    protected override void RegisterPorts() => this.fixedUpdate = this.AddFlowOutput("Out");

    public override void OnGraphStarted()
    {
      BlueprintManager.current.onFixedUpdate += new Action(this.FixedUpdate);
    }

    public override void OnGraphStoped()
    {
      BlueprintManager.current.onFixedUpdate -= new Action(this.FixedUpdate);
    }

    private void FixedUpdate() => this.fixedUpdate.Call();
  }
}
