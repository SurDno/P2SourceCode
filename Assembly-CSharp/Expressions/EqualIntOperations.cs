using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[a == b] : int", MenuItem = "a == b/int")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class EqualIntOperations : ComparisonOperation<int> {
	protected override bool Compute(int left, int right) {
		return left == right;
	}

	protected override string OperatorView() {
		return "==";
	}
}