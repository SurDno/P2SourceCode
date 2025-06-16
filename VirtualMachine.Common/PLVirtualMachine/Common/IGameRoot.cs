using System;

namespace PLVirtualMachine.Common;

public interface IGameRoot : IEditorBaseTemplate, IContext, INamed {
	Guid GetEngineTemplateGuidByBaseGuid(ulong baseGuid);

	ulong GetBaseGuidByEngineTemplateGuid(Guid engGuid);

	IBlueprint GetBlueprintByGuid(ulong bpGuid);

	HierarchyGuid GetHierarchyGuidByHierarchyPath(string path);

	IWorldBlueprint GetEngineTemplateByGuid(ulong editorTemplateGuid);

	IWorldHierarchyObject GetWorldHierarhyObjectByGuid(HierarchyGuid hGuid);
}