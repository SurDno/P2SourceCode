using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
