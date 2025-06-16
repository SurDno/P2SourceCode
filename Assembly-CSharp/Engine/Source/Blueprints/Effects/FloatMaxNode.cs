using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class FloatMaxNode : FlowControlNode
  {
    [Port("A")]
    private ValueInput<float> aInput;
    [Port("B")]
    private ValueInput<float> bInput;

    [Port("Value")]
    private float Value() => Mathf.Max(aInput.value, bInput.value);
  }
}
