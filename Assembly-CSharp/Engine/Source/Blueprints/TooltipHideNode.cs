using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class TooltipHideNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        ServiceLocator.GetService<NotificationService>().RemoveNotify(NotificationEnum.Tooltip);
        output.Call();
      });
    }
  }
}
