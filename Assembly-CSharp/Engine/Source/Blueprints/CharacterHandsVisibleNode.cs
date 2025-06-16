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
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        PlayerUtility.ShowPlayerHands(valueInput.value);
        output.Call();
      });
      valueInput = AddValueInput<bool>("Visible");
    }
  }
}
