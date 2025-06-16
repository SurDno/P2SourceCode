namespace PLVirtualMachine.Common;

public interface IEntryPoint : IObject, IEditorBaseTemplate {
	IActionLine ActionLine { get; }
}