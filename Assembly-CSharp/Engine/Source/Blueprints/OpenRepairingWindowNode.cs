using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Services.Utilities;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class OpenRepairingWindowNode : FlowControlNode
  {
    private ValueInput<IEntity> targetInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IEntity target = this.targetInput.value;
        if (target == null)
          return;
        UIServiceUtility.PushWindow<IRepairingWindow>(output, (Action<IRepairingWindow>) (window =>
        {
          window.Actor = ServiceLocator.GetService<ISimulation>().Player.GetComponent<IStorageComponent>();
          window.Target = target;
        }));
      }));
      this.targetInput = this.AddValueInput<IEntity>("Target");
    }
  }
}
