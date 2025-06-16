using Engine.Common.Commons;
using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[parameter] : StammKind", MenuItem = "parameter/StammKind")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextStammKindValue : EffectContextValue<StammKind>
  {
  }
}
