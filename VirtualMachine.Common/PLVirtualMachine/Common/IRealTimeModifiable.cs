namespace PLVirtualMachine.Common;

public interface IRealTimeModifiable {
	void OnModify();

	bool Modified { get; }

	IRealTimeModifiable ModifiableParent { get; }
}