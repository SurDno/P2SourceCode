// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.IGameRoot
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System;

#nullable disable
namespace PLVirtualMachine.Common
{
  public interface IGameRoot : IEditorBaseTemplate, IContext, INamed
  {
    Guid GetEngineTemplateGuidByBaseGuid(ulong baseGuid);

    ulong GetBaseGuidByEngineTemplateGuid(Guid engGuid);

    IBlueprint GetBlueprintByGuid(ulong bpGuid);

    HierarchyGuid GetHierarchyGuidByHierarchyPath(string path);

    IWorldBlueprint GetEngineTemplateByGuid(ulong editorTemplateGuid);

    IWorldHierarchyObject GetWorldHierarhyObjectByGuid(HierarchyGuid hGuid);
  }
}
