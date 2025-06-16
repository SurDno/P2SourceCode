// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.TooltipShowNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class TooltipShowNode : FlowControlNode
  {
    private ValueInput<string> localizationTag;
    private ValueInput<float> timeout;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        string text = this.localizationTag.value;
        if (!string.IsNullOrEmpty(text))
          text = ServiceLocator.GetService<LocalizationService>().GetText(text);
        ServiceLocator.GetService<NotificationService>().AddNotify(NotificationEnum.Tooltip, new object[2]
        {
          (object) text,
          (object) this.timeout.value
        });
        output.Call();
      }));
      this.localizationTag = this.AddValueInput<string>("Localization Tag");
      this.timeout = this.AddValueInput<float>("Timeout");
    }
  }
}
