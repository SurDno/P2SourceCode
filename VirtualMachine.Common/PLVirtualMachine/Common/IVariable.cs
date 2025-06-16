using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common;

public interface IVariable : INamed {
	EContextVariableCategory Category { get; }

	VMType Type { get; }

	bool IsEqual(IVariable otherVar);
}