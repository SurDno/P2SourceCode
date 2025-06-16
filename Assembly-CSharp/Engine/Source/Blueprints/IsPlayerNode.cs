using Engine.Behaviours.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
        if ((Object) gameObject == (Object) null)
          falseOut.Call();
        else if ((Object) gameObject.GetComponent<PivotPlayer>() == (Object) null)
          falseOut.Call();
        else
          trueOut.Call();
      });
    }
  }
}
