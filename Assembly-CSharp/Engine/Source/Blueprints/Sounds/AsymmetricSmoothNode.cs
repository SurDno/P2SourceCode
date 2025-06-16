using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class AsymmetricSmoothNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<float> valueInput;
    [Port("IncreaseTime")]
    private ValueInput<float> increaseTimeInput;
    [Port("DecreaseTime")]
    private ValueInput<float> decreaseTimeInput;
    private float time;
    private float prevValue;

    [Port("Value")]
    private float Value()
    {
      this.UpdateValue();
      return this.prevValue;
    }

    private void UpdateValue()
    {
      float num = this.valueInput.value;
      if ((double) num == (double) this.prevValue)
      {
        this.time = Time.time;
      }
      else
      {
        if ((double) this.time == (double) Time.time)
          return;
        if ((double) num > (double) this.prevValue)
        {
          this.prevValue += (Time.time - this.time) / this.increaseTimeInput.value;
          if ((double) this.prevValue > (double) num)
            this.prevValue = num;
        }
        else if ((double) num < (double) this.prevValue)
        {
          this.prevValue -= (Time.time - this.time) / this.decreaseTimeInput.value;
          if ((double) this.prevValue < (double) num)
            this.prevValue = num;
        }
        this.time = Time.time;
      }
    }
  }
}
