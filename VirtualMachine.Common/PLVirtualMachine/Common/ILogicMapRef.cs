using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common;

[VMType("ILogicMapRef")]
public interface ILogicMapRef : IRef, IVariable, INamed, IVMStringSerializable {
	ILogicMap LogicMap { get; }
}