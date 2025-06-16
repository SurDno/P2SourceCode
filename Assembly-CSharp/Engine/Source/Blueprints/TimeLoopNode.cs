using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
      UpdateValue();
      return value;
    }

    private void UpdateValue()
    {
      float time = Time.time;
      if (prevTime == (double) time)
        return;
      if (lengthInput.value != 0.0)
        value = Mathf.Repeat(value + (time - prevTime) / lengthInput.value, 1f);
      prevTime = time;
    }
  }
}
