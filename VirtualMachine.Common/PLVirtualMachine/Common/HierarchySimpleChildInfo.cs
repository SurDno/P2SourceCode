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
      simpeChildTemplate = simpleChiTemplate;
    }

    public ulong BaseGuid
    {
      get => simpeChildTemplate != null ? simpeChildTemplate.BaseGuid : 0UL;
    }

    public string Name => simpeChildTemplate.Name;

    public Guid EngineGuid => engineGuid;

    public Guid EngineTemplateGuid => simpeChildTemplate.EngineTemplateGuid;

    public HierarchyGuid HierarchyGuid => hierarchyGuid;

    public IBlueprint EditorTemplate => simpeChildTemplate;

    public void SetParent(IWorldHierarchyObject parent)
    {
      hierarchyParent = parent;
      hierarchyGuid = new HierarchyGuid(parent.HierarchyGuid, BaseGuid);
    }

    public void InitInstanceGuid(Guid instanceGuid) => engineGuid = instanceGuid;

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
      simpeChildTemplate = null;
      hierarchyParent = null;
    }
  }
}
