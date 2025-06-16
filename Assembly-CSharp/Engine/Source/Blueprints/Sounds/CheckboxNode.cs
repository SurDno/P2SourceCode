using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class CheckboxNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<bool> valueInput;

    [Port("Value")]
    private float Value() => valueInput.value ? 1f : 0.0f;
  }
}
