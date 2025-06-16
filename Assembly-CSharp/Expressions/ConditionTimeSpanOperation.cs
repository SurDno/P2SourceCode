using System;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[condition ? true : false] : Time", MenuItem = "condition ? true : false/Time")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ConditionTimeSpanOperation : ConditionOperation<TimeSpan>
  {
  }
}
