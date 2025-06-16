// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.FloatSumParametersNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;

#nullable disable
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
