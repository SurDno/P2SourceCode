namespace PLVirtualMachine.Common;

public interface ITalkingGraph :
	IFiniteStateMachine,
	IState,
	IGraphObject,
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable,
	ILocalContext,
	IGraph {
	bool OnlyOnce { get; }
}