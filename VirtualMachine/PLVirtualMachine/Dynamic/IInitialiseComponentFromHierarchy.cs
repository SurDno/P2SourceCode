using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic;

public interface IInitialiseComponentFromHierarchy {
	void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject);
}