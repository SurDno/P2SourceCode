using Engine.Common;
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
  public class OpenSleepWindowNode : FlowControlNode
  {
    private ValueInput<IEntity> targetInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() => UIServiceUtility.PushWindow<ISleepWindow>(output, (Action<ISleepWindow>) (window =>
      {
        window.Actor = ServiceLocator.GetService<ISimulation>().Player;
        window.Target = this.targetInput.value;
      }))));
      this.targetInput = this.AddValueInput<IEntity>("Target");
    }
  }
}
