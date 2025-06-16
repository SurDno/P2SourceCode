using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class FloatSumParametersNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<IList<IParameter<float>>> parametersInput;

    [Port("Value")]
    private float Value()
    {
      float num = 0.0f;
      IList<IParameter<float>> parameterList = this.parametersInput.value;
      if (parameterList != null)
      {
        for (int index = 0; index < parameterList.Count; ++index)
        {
          IParameter<float> parameter = parameterList[index];
          num += parameter.Value;
        }
      }
      return num;
    }
  }
}
