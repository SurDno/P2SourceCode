using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public interface IGameObjectContext :
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable,
	IContext,
	ILogicObject {
	IEnumerable<IStateRef> GetObjectStates();
}