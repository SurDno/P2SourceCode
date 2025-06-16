using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[value] : BlockType", MenuItem = "value/BlockType")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class BlockTypeValue : ConstValue<BlockTypeEnum>
  {
  }
}
