using Engine.Common.Generator;
using Inspectors;
using System;

namespace Expressions
{
  [TypeName(TypeName = "[a != b] : Time", MenuItem = "a != b/Time")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NotEqualTimeSpanOperations : ComparisonOperation<TimeSpan>
  {
    protected override bool Compute(TimeSpan a, TimeSpan b) => a != b;

    protected override string OperatorView() => "!=";
  }
}
