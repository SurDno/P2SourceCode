using Engine.Common.Components.Detectors;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[condition ? true : false] : DetectType", MenuItem = "condition ? true : false/DetectType")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ConditionDetectTypeOperation : ConditionOperation<DetectType>
  {
  }
}
