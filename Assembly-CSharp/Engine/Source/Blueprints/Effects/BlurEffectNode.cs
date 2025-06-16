// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.BlurEffectNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
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
