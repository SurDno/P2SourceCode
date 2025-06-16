using System.Collections;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class DelayNode : FlowControlNode
  {
    private ValueInput<float> delay;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      delay = AddValueInput<float>("Delay");
      AddFlowInput("In", (FlowHandler) (() => StartCoroutine(Delay(delay.value, output))));
    }

    private IEnumerator Delay(float delay, FlowOutput output)
    {
      yield return (object) new WaitForSeconds(delay);
      if ((Object) graphAgent != (Object) null)
        output.Call();
    }
  }
}
