using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class FloatClampedSumParametersNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<IList<IParameter<float>>> parametersInput;

    [Port("Value")]
    private float Value()
    {
      float num1 = 0.0f;
      float num2 = 0.0f;
      float num3 = 0.0f;
      IList<IParameter<float>> parameterList = this.parametersInput.value;
      if (parameterList != null)
      {
        for (int index = 0; index < parameterList.Count; ++index)
        {
          float b = parameterList[index].Value;
          num1 += b;
          num2 = Mathf.Min(num2, b);
          num3 = Mathf.Max(num3, b);
        }
      }
      return Mathf.Clamp(num1, num2, num3);
    }
  }
}
