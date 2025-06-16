using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[parameter] : BlockType", MenuItem = "parameter/BlockType")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextBlockTypeValue : EffectContextValue<BlockTypeEnum>
  {
  }
}
