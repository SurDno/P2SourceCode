using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public interface IActionLine :
	IGameAction,
	IOrderedChild,
	IContextElement,
	IObject,
	IEditorBaseTemplate,
	INamed,
	IBaseAction,
	IStaticUpdateable {
	List<IGameAction> Actions { get; }

	EActionLineType ActionLineType { get; }
}