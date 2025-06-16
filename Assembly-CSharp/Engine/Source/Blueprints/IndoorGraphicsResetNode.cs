using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class IndoorGraphicsResetNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        PlayerIndoorCheck.ResetOverride();
        PlayerIsolatedIndoorCheck.ResetOverride();
        CutsceneIndoorCheck.Set(false);
        output.Call();
      }));
    }
  }
}
