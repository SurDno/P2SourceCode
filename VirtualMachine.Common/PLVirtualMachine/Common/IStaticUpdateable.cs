namespace PLVirtualMachine.Common;

public interface IStaticUpdateable {
	void Update();

	bool IsUpdated { get; }
}