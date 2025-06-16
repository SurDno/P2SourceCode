using Engine.Source.Effects.Values;

namespace Engine.Source.Commons.Abilities
{
  public interface IAbilityValueContainer
  {
    IAbilityValue<T> GetAbilityValue<T>(AbilityValueNameEnum name) where T : struct;
  }
}
