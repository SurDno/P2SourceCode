using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Description("Stops and cease execution of the FlowSript")]
  public class Finish : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      ValueInput<bool> c = this.AddValueInput<bool>("Success");
      this.AddFlowInput("In", (FlowHandler) (() => this.graph.Stop(c.value)));
    }
  }
}
