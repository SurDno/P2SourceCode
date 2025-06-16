using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;

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
