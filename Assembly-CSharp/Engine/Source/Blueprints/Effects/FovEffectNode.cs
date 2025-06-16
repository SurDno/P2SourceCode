using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class FovEffectNode : FlowControlNode, IUpdatable
  {
    [Port("Value")]
    private ValueInput<float> valueInput;
    private float prevValue;

    public void Update()
    {
      float num = valueInput.value;
      if (prevValue == (double) num)
        return;
      prevValue = num;
      GameCamera.Instance.AdditionalFov = num;
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      GameCamera.Instance.AdditionalFov = 0.0f;
    }
  }
}
