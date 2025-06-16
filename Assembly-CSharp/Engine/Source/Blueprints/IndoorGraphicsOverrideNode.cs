using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class IndoorGraphicsOverrideNode : FlowControlNode
  {
    private ValueInput<bool> insideIndoorInput;
    private ValueInput<bool> isolatedInput;
    private ValueInput<bool> cutsceneIsolatedInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        bool flag = this.insideIndoorInput.value;
        PlayerIndoorCheck.Override(flag);
        PlayerIsolatedIndoorCheck.Override(flag && this.isolatedInput.value);
        CutsceneIndoorCheck.Set(this.cutsceneIsolatedInput.value);
        output.Call();
      }));
      this.insideIndoorInput = this.AddValueInput<bool>("InsideIndoor");
      this.isolatedInput = this.AddValueInput<bool>("Isolated");
      this.cutsceneIsolatedInput = this.AddValueInput<bool>("CutsceneIsolated");
    }
  }
}
