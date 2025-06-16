// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.UiEffectVisibleNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class UiEffectVisibleNode : FlowControlNode
  {
    private ValueInput<bool> visibleInput;
    private ValueInput<float> timeInput;
    private ValueInput<UiEffectType> typeInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        UiEffectType type = this.typeInput.value;
        if (type == UiEffectType.None)
          type = UiEffectType.Fade;
        MonoBehaviourInstance<UiEffectsController>.Instance.SetVisible(this.visibleInput.value, type, this.timeInput.value, (Action) (() => output.Call()));
      }));
      this.visibleInput = this.AddValueInput<bool>("Visible");
      this.timeInput = this.AddValueInput<float>("Time");
      this.typeInput = this.AddValueInput<UiEffectType>("Type");
    }
  }
}
