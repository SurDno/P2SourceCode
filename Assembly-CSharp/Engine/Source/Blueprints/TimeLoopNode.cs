using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class TimeLoopNode : FlowControlNode
  {
    [Port("Length")]
    private ValueInput<float> lengthInput;
    private float prevTime;
    private float value;

    [Port("Value")]
    private float Value()
    {
      this.UpdateValue();
      return this.value;
    }

    private void UpdateValue()
    {
      float time = Time.time;
      if ((double) this.prevTime == (double) time)
        return;
      if ((double) this.lengthInput.value != 0.0)
        this.value = Mathf.Repeat(this.value + (time - this.prevTime) / this.lengthInput.value, 1f);
      this.prevTime = time;
    }
  }
}
