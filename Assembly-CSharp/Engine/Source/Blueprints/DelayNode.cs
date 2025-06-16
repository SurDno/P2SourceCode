using System.Collections;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

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
      AddFlowInput("In", () => StartCoroutine(Delay(delay.value, output)));
    }

    private IEnumerator Delay(float delay, FlowOutput output)
    {
      yield return new WaitForSeconds(delay);
      if (graphAgent != null)
        output.Call();
    }
  }
}
