using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class DelayNode : FlowControlNode
  {
    [Port("StartDelay")]
    private ValueInput<float> startDelayInput;
    [Port("EndDelay")]
    private ValueInput<float> endDelayInput;
    [Port("Value")]
    private ValueInput<bool> valueInput;
    private bool prevValue;
    private float accumulate;

    [Port("Value")]
    private bool Value() => Compute(valueInput.value);

    public bool Compute(bool value)
    {
      if (value)
      {
        if (prevValue)
        {
          accumulate = 0.0f;
        }
        else
        {
          accumulate += Time.deltaTime;
          if (accumulate >= (double) startDelayInput.value)
          {
            prevValue = true;
            accumulate = 0.0f;
          }
        }
      }
      else if (prevValue)
      {
        accumulate += Time.deltaTime;
        if (accumulate >= (double) endDelayInput.value)
        {
          prevValue = false;
          accumulate = 0.0f;
        }
      }
      else
        accumulate = 0.0f;
      return prevValue;
    }
  }
}
