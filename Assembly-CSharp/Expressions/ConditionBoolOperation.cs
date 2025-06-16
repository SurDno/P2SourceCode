using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[condition ? true : false] : bool", MenuItem = "condition ? true : false/bool")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ConditionBoolOperation : ConditionOperation<bool>
  {
  }
}
