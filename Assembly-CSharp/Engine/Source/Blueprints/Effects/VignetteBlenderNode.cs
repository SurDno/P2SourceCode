using System.Collections.Generic;
using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class VignetteBlenderNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<IList<IParameter<IntensityParameter<Color>>>> intensityParametersInput;

    [Port("Value")]
    private IntensityParameter<Color> Value()
    {
      float num1 = 0.0f;
      float num2 = 0.0f;
      Color black = Color.black;
      IList<IParameter<IntensityParameter<Color>>> parameterList = intensityParametersInput.value;
      if (parameterList != null)
      {
        for (int index = 0; index < parameterList.Count; ++index)
        {
          float intensity = parameterList[index].Value.Intensity;
          if (num1 < (double) intensity)
            num1 = intensity;
          num2 += intensity;
        }
        if (num2 == 0.0)
          return new IntensityParameter<Color> {
            Intensity = 0.0f,
            Value = black
          };
        for (int index = 0; index < parameterList.Count; ++index)
        {
          IParameter<IntensityParameter<Color>> parameter = parameterList[index];
          float intensity = parameter.Value.Intensity;
          Color color = parameter.Value.Value;
          black += color * intensity / num2;
        }
      }
      return new IntensityParameter<Color> {
        Intensity = num1,
        Value = black
      };
    }
  }
}
