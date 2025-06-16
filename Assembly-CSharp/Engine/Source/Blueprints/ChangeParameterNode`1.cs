using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;

namespace Engine.Source.Blueprints
{
  public abstract class ChangeParameterNode<T> : FlowControlNode where T : struct
  {
    [Port("Parameters")]
    private ValueInput<ParametersComponent> parametersInput;
    [Port("Name")]
    private ValueInput<ParameterNameEnum> nameInput;
    [Port("Value")]
    private ValueInput<T> valueInput;
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      ParametersComponent parametersComponent = parametersInput.value;
      if (parametersComponent != null)
      {
        IParameter<T> byName = parametersComponent.GetByName<T>(nameInput.value);
        if (byName != null)
          byName.Value = valueInput.value;
      }
      output.Call();
    }
  }
}
