using System;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[max(a, b)] : Time", MenuItem = "max(a, b)/Time")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class MaxTimeSpanOperation : BinaryOperation<TimeSpan> {
	protected override TimeSpan Compute(TimeSpan a, TimeSpan b) {
		return a > b ? a : b;
	}

	public override string ValueView =>
		"max(" + (a != null ? a.ValueView : "null") + ", " + (b != null ? b.ValueView : "null") + ")";
}