using System.Collections;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
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
      FlowOutput output = AddFlowOutput("Out");
      frames = AddValueInput<int>("Frames");
      AddFlowInput("In", () => StartCoroutine(Delay(frames.value, output)));
    }

    private IEnumerator Delay(float frames, FlowOutput output)
    {
      for (int index = 0; index < (double) frames; ++index)
        yield return new WaitForEndOfFrame();
      output.Call();
    }
  }
}
