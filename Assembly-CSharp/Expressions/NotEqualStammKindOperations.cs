using Engine.Common.Commons;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[a != b] : StammKind", MenuItem = "a != b/StammKind")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class NotEqualStammKindOperations : ComparisonOperation<StammKind> {
	protected override bool Compute(StammKind a, StammKind b) {
		return a != b;
	}

	protected override string OperatorView() {
		return "!=";
	}
}