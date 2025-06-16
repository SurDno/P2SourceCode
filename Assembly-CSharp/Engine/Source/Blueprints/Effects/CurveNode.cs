using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class CurveNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<float> valueInput;
    [Port("Curve")]
    private ValueInput<AnimationCurve> curveInput;

    [Port("Value")]
    private float Value()
    {
      AnimationCurve animationCurve = this.curveInput.value;
      return animationCurve != null ? animationCurve.Evaluate(this.valueInput.value) : this.valueInput.value;
    }
  }
}
