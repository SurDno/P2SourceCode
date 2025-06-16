using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[value] : float", MenuItem = "value/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class FloatValue : ConstValue<float>
  {
  }
}
