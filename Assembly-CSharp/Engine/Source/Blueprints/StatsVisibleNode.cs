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
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        ServiceLocator.GetService<UIService>().Get<IHudWindow>().SetVisibility(valueInput.value, ignoreTextNotifications.value);
        output.Call();
      });
      valueInput = AddValueInput<bool>("Visible");
      ignoreTextNotifications = AddValueInput<bool>("Ignore Text Notifications");
    }
  }
}
