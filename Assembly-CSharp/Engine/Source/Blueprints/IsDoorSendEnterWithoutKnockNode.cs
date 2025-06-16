// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.IsDoorSendEnterWithoutKnockNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class IsDoorSendEnterWithoutKnockNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> doorInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.AddValueOutput<bool>("SendEnterWithoutKnock", (ValueHandler<bool>) (() =>
      {
        IDoorComponent doorComponent = this.doorInput.value;
        return doorComponent != null && doorComponent.SendEnterWithoutKnock.Value;
      }));
      this.doorInput = this.AddValueInput<IDoorComponent>("Door");
    }
  }
}
