using System;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Services.Utilities;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class OpenSleepWindowNode : FlowControlNode
  {
    private ValueInput<IEntity> targetInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () => UIServiceUtility.PushWindow(output, (Action<ISleepWindow>) (window =>
      {
        window.Actor = ServiceLocator.GetService<ISimulation>().Player;
        window.Target = targetInput.value;
      })));
      targetInput = AddValueInput<IEntity>("Target");
    }
  }
}
