using System;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[a - b] : Time", MenuItem = "a - b/Time")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class MinusTimeSpanOperation : BinaryOperation<TimeSpan> {
	protected override TimeSpan Compute(TimeSpan a, TimeSpan b) {
		return a - b;
	}

	protected override string OperatorView() {
		return "-";
	}
}