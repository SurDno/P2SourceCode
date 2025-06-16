using Engine.Source.Commons.Abilities;
using Engine.Source.Effects.Values;
using FlowCanvas;
using FlowCanvas.Nodes;

namespace Engine.Source.Blueprints.Effects
{
  public class AbilityValueNode<T> : FlowControlNode where T : struct
  {
    [Port("Controller")]
    private ValueInput<IAbilityValueContainer> abilityControllerInput;
    [Port("Name")]
    private ValueInput<AbilityValueNameEnum> valueNameInput;

    [Port("Value")]
    private T Value()
    {
      IAbilityValueContainer abilityValueContainer = abilityControllerInput.value;
      if (abilityValueContainer == null)
        return default (T);
      IAbilityValue<T> abilityValue = abilityValueContainer.GetAbilityValue<T>(valueNameInput.value);
      return abilityValue == null ? default (T) : abilityValue.Value;
    }
  }
}
