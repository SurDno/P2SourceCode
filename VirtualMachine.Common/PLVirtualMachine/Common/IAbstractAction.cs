namespace PLVirtualMachine.Common;

public interface IAbstractAction : IBaseAction, IFunctionalPoint, IStaticUpdateable {
	EActionType ActionType { get; }

	EMathOperationType MathOperationType { get; }

	IParam SourceConstant { get; }

	string TargetEvent { get; }
}