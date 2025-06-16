using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[a != b] : BlockType", MenuItem = "a != b/BlockType")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NotEqualBlockTypeOperations : ComparisonOperation<BlockTypeEnum>
  {
    protected override bool Compute(BlockTypeEnum a, BlockTypeEnum b) => a != b;

    protected override string OperatorView() => "!=";
  }
}
