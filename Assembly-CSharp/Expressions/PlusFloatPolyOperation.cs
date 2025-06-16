using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[a + ... + z] : float", MenuItem = "a + ... + z/float")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class PlusFloatPolyOperation : PolyOperation<float> {
	protected override float Compute(float a, float b) {
		return a + b;
	}

	protected override string OperatorView() {
		return "+";
	}
}