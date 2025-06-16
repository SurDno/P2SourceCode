using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("Dummy")]
  [Description("Use for organization")]
  [Color("808080")]
  public class Dummy : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      FlowOutput fOut = AddFlowOutput("Out");
      AddFlowInput("In", () => fOut.Call());
    }
  }
}
