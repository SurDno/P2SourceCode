using Engine.Common.Components.Detectors;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[a == b] : DetectType", MenuItem = "a == b/DetectType")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class EqualDetectTypeOperations : ComparisonOperation<DetectType> {
	protected override bool Compute(DetectType a, DetectType b) {
		return a == b;
	}

	protected override string OperatorView() {
		return "==";
	}
}