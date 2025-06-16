// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.ParameterNode`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;

#nullable disable
namespace Engine.Source.Blueprints
{
  public abstract class ParameterNode<T> : FlowControlNode where T : struct
  {
    [Port("Parameters")]
    private ValueInput<ParametersComponent> parametersInput;
    [Port("Name")]
    private ValueInput<ParameterNameEnum> nameInput;

    [Port("Value")]
    private T Value()
    {
      ParametersComponent parametersComponent = this.parametersInput.value;
      if (parametersComponent != null)
      {
        IParameter<T> byName = parametersComponent.GetByName<T>(this.nameInput.value);
        if (byName != null)
          return byName.Value;
      }
      return default (T);
    }
  }
}
