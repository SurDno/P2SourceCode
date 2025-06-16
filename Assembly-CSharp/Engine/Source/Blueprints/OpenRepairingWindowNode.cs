using System;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Services.Utilities;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class OpenRepairingWindowNode : FlowControlNode
  {
    private ValueInput<IEntity> targetInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        IEntity target = targetInput.value;
        if (target == null)
          return;
        UIServiceUtility.PushWindow(output, (Action<IRepairingWindow>) (window =>
        {
          window.Actor = ServiceLocator.GetService<ISimulation>().Player.GetComponent<IStorageComponent>();
          window.Target = target;
        }));
      });
      targetInput = AddValueInput<IEntity>("Target");
    }
  }
}
