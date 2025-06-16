using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
