// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.StatsVisibleNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class StatsVisibleNode : FlowControlNode
  {
    private ValueInput<bool> valueInput;
    private ValueInput<bool> ignoreTextNotifications;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        ServiceLocator.GetService<UIService>().Get<IHudWindow>().SetVisibility(this.valueInput.value, this.ignoreTextNotifications.value);
        output.Call();
      }));
      this.valueInput = this.AddValueInput<bool>("Visible");
      this.ignoreTextNotifications = this.AddValueInput<bool>("Ignore Text Notifications");
    }
  }
}
