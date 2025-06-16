// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.UnlockAchievementNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class UnlockAchievementNode : FlowControlNode
  {
    private ValueInput<string> nameInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.AddFlowInput("In", (FlowHandler) (() => ServiceLocator.GetService<IAchievementService>().Unlock(this.nameInput.value)));
      this.nameInput = this.AddValueInput<string>("Name");
    }
  }
}
