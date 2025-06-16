using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common;

public interface IRef : IVariable, INamed, IVMStringSerializable {
	bool Empty { get; }

	bool Exist { get; }

	IObject StaticInstance { get; }

	ulong BaseGuid { get; }
}