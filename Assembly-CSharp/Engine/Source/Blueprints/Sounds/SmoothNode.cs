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
      UpdateValue();
      return prevValue;
    }

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      time = Time.time;
    }

    private void UpdateValue()
    {
      float num1 = valueInput.value;
      if (num1 == (double) prevValue)
      {
        time = Time.time;
      }
      else
      {
        if (time == (double) Time.time)
          return;
        float num2 = (Time.time - time) / timeInput.value;
        if (num1 > (double) prevValue)
        {
          prevValue += num2;
          if (prevValue > (double) num1)
            prevValue = num1;
        }
        else
        {
          prevValue -= num2;
          if (prevValue < (double) num1)
            prevValue = num1;
        }
        time = Time.time;
      }
    }
  }
}
