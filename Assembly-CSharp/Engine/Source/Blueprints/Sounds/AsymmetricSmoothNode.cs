using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
      UpdateValue();
      return prevValue;
    }

    private void UpdateValue()
    {
      float num = valueInput.value;
      if (num == (double) prevValue)
      {
        time = Time.time;
      }
      else
      {
        if (time == (double) Time.time)
          return;
        if (num > (double) prevValue)
        {
          prevValue += (Time.time - time) / increaseTimeInput.value;
          if (prevValue > (double) num)
            prevValue = num;
        }
        else if (num < (double) prevValue)
        {
          prevValue -= (Time.time - time) / decreaseTimeInput.value;
          if (prevValue < (double) num)
            prevValue = num;
        }
        time = Time.time;
      }
    }
  }
}
