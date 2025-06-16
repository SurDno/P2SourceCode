namespace PLVirtualMachine.Common;

public interface IActionLoop {
	CommonVariable LoopListParam { get; }

	object StartIndexParam { get; }

	object EndIndexParam { get; }

	bool LoopRandomIndexing { get; }

	IVariable LoopListParamInstance { get; }
}