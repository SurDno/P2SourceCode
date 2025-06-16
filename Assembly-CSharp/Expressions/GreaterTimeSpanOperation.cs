using System;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[a > b] : Time", MenuItem = "a > b/Time")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class GreaterTimeSpanOperation : ComparisonOperation<TimeSpan>
  {
    protected override bool Compute(TimeSpan a, TimeSpan b) => a > b;

    protected override string OperatorView() => ">";
  }
}
