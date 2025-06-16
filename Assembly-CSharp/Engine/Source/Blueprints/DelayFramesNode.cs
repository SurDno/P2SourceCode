using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class DelayFramesNode : FlowControlNode
  {
    private ValueInput<int> frames;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.frames = this.AddValueInput<int>("Frames");
      this.AddFlowInput("In", (FlowHandler) (() => this.StartCoroutine(this.Delay((float) this.frames.value, output))));
    }

    private IEnumerator Delay(float frames, FlowOutput output)
    {
      for (int index = 0; (double) index < (double) frames; ++index)
        yield return (object) new WaitForEndOfFrame();
      output.Call();
    }
  }
}
