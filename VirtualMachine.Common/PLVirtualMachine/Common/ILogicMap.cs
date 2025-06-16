using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public interface ILogicMap :
	IGraph,
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable {
	List<IGraphObject> Nodes { get; }

	ELogicMapType LogicMapType { get; }

	IGameMode GameTimeContext { get; }

	IGameString Title { get; }
}