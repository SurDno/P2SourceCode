namespace PLVirtualMachine.Common;

public interface IBranch :
	IState,
	IGraphObject,
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable,
	ILocalContext {
	ICondition GetBranchCondition(int exitPntIndex);
}