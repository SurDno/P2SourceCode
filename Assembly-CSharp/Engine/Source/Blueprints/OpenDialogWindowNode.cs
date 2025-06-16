using System;
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
  public class OpenDialogWindowNode : FlowControlNode
  {
    private ValueInput<ISpeakingComponent> targetInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        ISpeakingComponent target = targetInput.value;
        if (target == null)
          return;
        if (target.SpeakAvailable)
        {
          UIServiceUtility.PushWindow(output, (Action<IDialogWindow>) (window =>
          {
            window.Target = target;
            window.Actor = ServiceLocator.GetService<ISimulation>().Player.GetComponent<ISpeakingComponent>();
          }));
        }
        else
        {
          Debug.LogError((object) ("Speak is not available : " + target.Owner.GetInfo()));
          output.Call();
        }
      });
      targetInput = AddValueInput<ISpeakingComponent>("Speaking");
    }
  }
}
