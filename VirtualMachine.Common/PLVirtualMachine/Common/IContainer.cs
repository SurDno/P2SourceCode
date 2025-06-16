namespace PLVirtualMachine.Common;

public interface IContainer :
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable {
	EObjectCategory GetCategory();

	IContainer Owner { get; }

	void Clear();
}