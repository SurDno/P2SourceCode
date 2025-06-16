using System.Collections;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
      AddFlowInput("In", (FlowHandler) (() => StartCoroutine(Delay(frames.value, output))));
    }

    private IEnumerator Delay(float frames, FlowOutput output)
    {
      for (int index = 0; index < (double) frames; ++index)
        yield return (object) new WaitForEndOfFrame();
      output.Call();
    }
  }
}
