using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;
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
      FlowOutput output = this.AddFlowOutput("Out");
      this.delay = this.AddValueInput<float>("Delay");
      this.AddFlowInput("In", (FlowHandler) (() => this.StartCoroutine(this.Delay(this.delay.value, output))));
    }

    private IEnumerator Delay(float delay, FlowOutput output)
    {
      yield return (object) new WaitForSeconds(delay);
      if ((Object) this.graphAgent != (Object) null)
        output.Call();
    }
  }
}
