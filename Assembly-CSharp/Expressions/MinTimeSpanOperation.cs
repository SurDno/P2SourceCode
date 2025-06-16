using System;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[min(a, b)] : Time", MenuItem = "min(a, b)/Time")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class MinTimeSpanOperation : BinaryOperation<TimeSpan> {
	protected override TimeSpan Compute(TimeSpan a, TimeSpan b) {
		return a < b ? a : b;
	}

	public override string ValueView =>
		"min(" + (a != null ? a.ValueView : "null") + ", " + (b != null ? b.ValueView : "null") + ")";
}