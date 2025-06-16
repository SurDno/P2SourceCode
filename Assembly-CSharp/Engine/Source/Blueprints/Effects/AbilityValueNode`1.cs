// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.AbilityValueNode`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons.Abilities;
using Engine.Source.Effects.Values;
using FlowCanvas;
using FlowCanvas.Nodes;

#nullable disable
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
      IAbilityValueContainer abilityValueContainer = this.abilityControllerInput.value;
      if (abilityValueContainer == null)
        return default (T);
      IAbilityValue<T> abilityValue = abilityValueContainer.GetAbilityValue<T>(this.valueNameInput.value);
      return abilityValue == null ? default (T) : abilityValue.Value;
    }
  }
}
