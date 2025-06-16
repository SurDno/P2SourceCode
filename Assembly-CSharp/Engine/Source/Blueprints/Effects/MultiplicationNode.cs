using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class MultiplicationNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<float> valueInput;
    [Port("Multiplier")]
    private ValueInput<float> multiplierInput;

    [Port("Value")]
    private float Value() => this.valueInput.value * this.multiplierInput.value;
  }
}
