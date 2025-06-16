using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class SmoothNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<float> valueInput;
    [Port("Time")]
    private ValueInput<float> timeInput;
    private float time;
    private float prevValue;

    [Port("Value")]
    private float Value()
    {
      this.UpdateValue();
      return this.prevValue;
    }

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      this.time = Time.time;
    }

    private void UpdateValue()
    {
      float num1 = this.valueInput.value;
      if ((double) num1 == (double) this.prevValue)
      {
        this.time = Time.time;
      }
      else
      {
        if ((double) this.time == (double) Time.time)
          return;
        float num2 = (Time.time - this.time) / this.timeInput.value;
        if ((double) num1 > (double) this.prevValue)
        {
          this.prevValue += num2;
          if ((double) this.prevValue > (double) num1)
            this.prevValue = num1;
        }
        else
        {
          this.prevValue -= num2;
          if ((double) this.prevValue < (double) num1)
            this.prevValue = num1;
        }
        this.time = Time.time;
      }
    }
  }
}
