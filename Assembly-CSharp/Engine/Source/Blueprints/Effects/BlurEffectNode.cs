using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class BlurEffectNode : FlowControlNode, IUpdatable
  {
    [Port("Value")]
    private ValueInput<float> valueInput;
    private MotionTrail motionTrail;

    public void Update()
    {
      if ((Object) this.motionTrail == (Object) null)
        this.motionTrail = GameCamera.Instance.Camera.GetComponent<MotionTrail>();
      if ((Object) this.motionTrail == (Object) null)
        return;
      this.motionTrail.enabled = (double) this.valueInput.value > 1.0 / 256.0;
      this.motionTrail.Strength = this.valueInput.value;
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (Object) this.motionTrail)
        return;
      this.motionTrail.enabled = false;
    }
  }
}
