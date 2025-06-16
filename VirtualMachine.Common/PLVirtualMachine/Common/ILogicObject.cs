namespace PLVirtualMachine.Common;

public interface ILogicObject : IContext, INamed {
	IBlueprint Blueprint { get; }

	bool Static { get; }

	IVariable GetSelf();
}