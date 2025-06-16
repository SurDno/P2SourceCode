// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.FovEffectNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

#nullable disable
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
