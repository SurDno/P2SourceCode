namespace PLVirtualMachine.Common;

public interface ICondition : IObject, IEditorBaseTemplate, IOrderedChild, IStaticUpdateable {
	string Name { get; }

	bool IsPart();
}