using Engine.Common.Components.Detectors;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[value] : DetectType", MenuItem = "value/DetectType")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class DetectTypeValue : ConstValue<DetectType>
  {
  }
}
