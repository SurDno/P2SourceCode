// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.ExpressionEffectUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects.Engine;

#nullable disable
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
