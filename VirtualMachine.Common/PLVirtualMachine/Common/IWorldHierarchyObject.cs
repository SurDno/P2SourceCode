using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public interface IWorldHierarchyObject :
	ILogicObject,
	IContext,
	INamed,
	IWorldObject,
	IEngineTemplated,
	IHierarchyObject,
	IEngineInstanced {
	void Initialize(IWorldBlueprint templateObject);

	IEnumerable<IHierarchyObject> SimpleChilds { get; }

	bool IsPhantom { get; }

	IWorldHierarchyObject Parent { get; set; }

	void ClearHierarchy();
}