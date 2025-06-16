using Engine.Common.Generator;
using Inspectors;
using System;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[parameter] : Time", MenuItem = "parameter/Time")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextTimeSpanValue : EffectContextValue<TimeSpan>
  {
  }
}
