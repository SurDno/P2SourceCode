// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.VignetteBlenderNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
      IList<IParameter<IntensityParameter<Color>>> parameterList = this.intensityParametersInput.value;
      if (parameterList != null)
      {
        for (int index = 0; index < parameterList.Count; ++index)
        {
          float intensity = parameterList[index].Value.Intensity;
          if ((double) num1 < (double) intensity)
            num1 = intensity;
          num2 += intensity;
        }
        if ((double) num2 == 0.0)
          return new IntensityParameter<Color>()
          {
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
      return new IntensityParameter<Color>()
      {
        Intensity = num1,
        Value = black
      };
    }
  }
}
