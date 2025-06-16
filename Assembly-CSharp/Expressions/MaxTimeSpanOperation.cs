using Engine.Common.Generator;
using Inspectors;
using System;

namespace Expressions
{
  [TypeName(TypeName = "[max(a, b)] : Time", MenuItem = "max(a, b)/Time")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MaxTimeSpanOperation : BinaryOperation<TimeSpan>
  {
    protected override TimeSpan Compute(TimeSpan a, TimeSpan b) => a > b ? a : b;

    public override string ValueView
    {
      get
      {
        return "max(" + (this.a != null ? this.a.ValueView : "null") + ", " + (this.b != null ? this.b.ValueView : "null") + ")";
      }
    }
  }
}
