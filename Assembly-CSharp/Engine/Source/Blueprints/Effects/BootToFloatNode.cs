using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class BootToFloatNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<bool> valueInput;
    [Port("Min")]
    private ValueInput<float> minInput;
    [Port("Max")]
    private ValueInput<float> maxInput;

    [Port("Value")]
    private float Value() => valueInput.value ? maxInput.value : minInput.value;
  }
}
