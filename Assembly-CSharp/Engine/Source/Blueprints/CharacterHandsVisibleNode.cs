using Engine.Source.Utility;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CharacterHandsVisibleNode : FlowControlNode
  {
    private ValueInput<bool> valueInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        PlayerUtility.ShowPlayerHands(this.valueInput.value);
        output.Call();
      }));
      this.valueInput = this.AddValueInput<bool>("Visible");
    }
  }
}
