using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class DummyNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<bool> valueInput;

    [Port("Value")]
    private bool Value() => this.valueInput.value;
  }
}
