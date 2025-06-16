using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[a + ... + z] : int", MenuItem = "a + ... + z/int")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class PlusIntPolyOperation : PolyOperation<int> {
	protected override int Compute(int a, int b) {
		return a + b;
	}

	protected override string OperatorView() {
		return "+";
	}
}