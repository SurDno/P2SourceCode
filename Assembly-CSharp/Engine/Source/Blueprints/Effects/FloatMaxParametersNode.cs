using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class FloatMaxParametersNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<IList<IParameter<float>>> parametersInput;

    [Port("Value")]
    private float Value()
    {
      float num1 = 0.0f;
      IList<IParameter<float>> parameterList = this.parametersInput.value;
      if (parameterList != null)
      {
        for (int index = 0; index < parameterList.Count; ++index)
        {
          float num2 = parameterList[index].Value;
          if ((double) num1 < (double) num2)
            num1 = num2;
        }
      }
      return num1;
    }
  }
}
