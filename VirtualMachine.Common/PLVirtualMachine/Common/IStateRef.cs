using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common;

[VMType("IStateRef")]
public interface IStateRef : IRef, IVariable, INamed, IVMStringSerializable {
	IState State { get; }

	void Initialize(ulong baseGuid);
}