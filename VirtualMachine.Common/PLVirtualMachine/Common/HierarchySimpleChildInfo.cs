using System;
using System.Collections.Generic;

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
