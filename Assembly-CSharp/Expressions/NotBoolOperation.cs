using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[not value] : int", MenuItem = "not value/int")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class NotBoolOperation : UnaryOperation<bool> {
	protected override bool Compute(bool value) {
		return !value;
	}

	protected override string OperatorView() {
		return "!";
	}
}