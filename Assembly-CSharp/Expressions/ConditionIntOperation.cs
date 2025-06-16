using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[condition ? true : false] : int", MenuItem = "condition ? true : false/int")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ConditionIntOperation : ConditionOperation<int>
  {
  }
}
