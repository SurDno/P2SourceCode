// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.HierarchySimpleChildInfo
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common
{
  public class HierarchySimpleChildInfo : 
    IHierarchyObject,
    IEngineInstanced,
    IEngineTemplated,
    INamed
  {
    private IWorldBlueprint simpeChildTemplate;
    private HierarchyGuid hierarchyGuid = HierarchyGuid.Empty;
    private IWorldHierarchyObject hierarchyParent;
    private Guid engineGuid = Guid.Empty;

    public HierarchySimpleChildInfo(IWorldBlueprint simpleChiTemplate)
    {
      this.simpeChildTemplate = simpleChiTemplate;
    }

    public ulong BaseGuid
    {
      get => this.simpeChildTemplate != null ? this.simpeChildTemplate.BaseGuid : 0UL;
    }

    public string Name => this.simpeChildTemplate.Name;

    public Guid EngineGuid => this.engineGuid;

    public Guid EngineTemplateGuid => this.simpeChildTemplate.EngineTemplateGuid;

    public HierarchyGuid HierarchyGuid => this.hierarchyGuid;

    public IBlueprint EditorTemplate => (IBlueprint) this.simpeChildTemplate;

    public void SetParent(IWorldHierarchyObject parent)
    {
      this.hierarchyParent = parent;
      this.hierarchyGuid = new HierarchyGuid(parent.HierarchyGuid, this.BaseGuid);
    }

    public void InitInstanceGuid(Guid instanceGuid) => this.engineGuid = instanceGuid;

    public IEnumerable<IHierarchyObject> HierarchyChilds
    {
      get
      {
        yield break;
      }
    }

    public void AddHierarchyChild(IHierarchyObject child)
    {
    }

    public void ClearHierarchy()
    {
      this.simpeChildTemplate = (IWorldBlueprint) null;
      this.hierarchyParent = (IWorldHierarchyObject) null;
    }
  }
}
