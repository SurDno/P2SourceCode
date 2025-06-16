using Engine.Common;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects.Engine;

namespace Engine.Source.Effects
{
  public static class ExpressionEffectUtility
  {
    public static IEntity GetEntity(EffectContextEnum effectContext, IEffect context)
    {
      switch (effectContext)
      {
        case EffectContextEnum.Self:
          return context.AbilityItem.Self;
        case EffectContextEnum.Item:
          return context.AbilityItem.Item;
        case EffectContextEnum.Target:
          return context.Target;
        default:
          return (IEntity) null;
      }
    }
  }
}
