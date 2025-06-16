using Engine.Behaviours.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  [Description("НЕ ИСПОЛЬЗОВАТЬ, ЗАМЕНИТЬ НА IsPlayer2Node")]
  [Color("FF0000")]
  public class IsPlayerNode : FlowControlNode
  {
    [Port("Target")]
    private ValueInput<GameObject> inputValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput trueOut = AddFlowOutput("True");
      FlowOutput falseOut = AddFlowOutput("False");
      AddFlowInput("In", () =>
      {
        GameObject gameObject = inputValue.value;
        if (gameObject == null)
          falseOut.Call();
        else if (gameObject.GetComponent<PivotPlayer>() == null)
          falseOut.Call();
        else
          trueOut.Call();
      });
    }
  }
}
