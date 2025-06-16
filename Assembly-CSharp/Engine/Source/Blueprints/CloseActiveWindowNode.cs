using System.Collections;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CloseActiveWindowNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () => CoroutineService.Instance.Route(Route(output)));
    }

    private IEnumerator Route(FlowOutput output)
    {
      UIService uiService = ServiceLocator.GetService<UIService>();
      while ((Object) uiService.Active != (Object) null && !(uiService.Active is IHudWindow) && !(uiService.Active is IDialogWindow))
      {
        uiService.Pop();
        while (uiService.IsTransition)
          yield return null;
      }
      output.Call();
    }
  }
}
