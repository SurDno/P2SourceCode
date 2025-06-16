using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[a - b] : int", MenuItem = "a - b/int")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class MinusIntOperation : BinaryOperation<int> {
	protected override int Compute(int a, int b) {
		return a - b;
	}

	protected override string OperatorView() {
		return "-";
	}
}