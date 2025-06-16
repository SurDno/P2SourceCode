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
      float num = this.valueInput.value;
      if ((double) this.prevValue == (double) num)
        return;
      this.prevValue = num;
      GameCamera.Instance.AdditionalFov = num;
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      GameCamera.Instance.AdditionalFov = 0.0f;
    }
  }
}
