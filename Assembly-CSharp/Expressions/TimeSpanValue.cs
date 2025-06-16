using Engine.Common.Generator;
using Inspectors;
using System;

namespace Expressions
{
  [TypeName(TypeName = "[value] : Time", MenuItem = "value/Time")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class TimeSpanValue : ConstValue<TimeSpan>
  {
  }
}
