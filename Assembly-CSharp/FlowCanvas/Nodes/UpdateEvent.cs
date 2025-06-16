using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("On Update")]
  [Category("Events/Graph")]
  [Description("Called per-frame")]
  public class UpdateEvent : EventNode, IUpdatable
  {
    private FlowOutput update;

    protected override void RegisterPorts() => this.update = this.AddFlowOutput("Out");

    public void Update() => this.update.Call();
  }
}
