using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[a != b] : bool", MenuItem = "a != b/bool")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class NotEqualBoolOperations : ComparisonOperation<bool> {
	protected override bool Compute(bool a, bool b) {
		return a != b;
	}

	protected override string OperatorView() {
		return "!=";
	}
}