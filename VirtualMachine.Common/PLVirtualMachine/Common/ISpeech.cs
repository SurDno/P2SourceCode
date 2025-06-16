using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public interface ISpeech :
	IState,
	IGraphObject,
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable,
	ILocalContext {
	IGameString Text { get; }

	IParam TextParam { get; }

	bool OnlyOnce { get; }

	bool IsTrade { get; }

	List<ISpeechReply> Replies { get; }

	IObjRef Author { get; }

	IActionLine ActionLine { get; }
}