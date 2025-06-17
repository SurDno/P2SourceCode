using System;
using Engine.Common.Types;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.GameLogic
{
  [VMType("IObjRef")]
  [VMFactory(typeof (IObjRef))]
  public class VMObjRef : 
    BaseRef,
    IObjRef,
    IRef,
    IVariable,
    INamed,
    IVMStringSerializable,
    IEngineInstanced,
    IEngineTemplated
  {
    private HierarchyGuid hierarchyGuid = HierarchyGuid.Empty;
    private Guid engineGuid = Guid.Empty;
    private IEngineRTInstance engineInstance;

    public void Initialize(Guid engineGuid)
    {
      this.engineGuid = engineGuid;
      Load();
    }

    public void Initialize(HierarchyGuid hGuid)
    {
      hierarchyGuid = hGuid;
      BaseGuid = hierarchyGuid.TemplateGuid;
      Load();
    }

    public void Initialize(IBlueprint obj) => LoadStaticInstance(obj);

    public void InitializeInstance(IEngineRTInstance instance)
    {
      engineInstance = instance;
      LoadStaticInstance(instance.EditorTemplate);
    }

    public override EContextVariableCategory Category => Object == null || Object.GetCategory() != EObjectCategory.OBJECT_CATEGORY_GAME ? EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_OBJECT : EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GAME;

    public Guid EngineTemplateGuid
    {
      get
      {
        if (engineInstance != null)
          return engineInstance.EngineTemplateGuid;
        return Object != null && typeof (IWorldObject).IsAssignableFrom(Object.GetType()) ? ((IEngineTemplated) Object).EngineTemplateGuid : Guid.Empty;
      }
    }

    public Guid EngineGuid => EngineInstance != null ? EngineInstance.EngineGuid : engineGuid;

    public HierarchyGuid HierarchyGuid => hierarchyGuid;

    public IBlueprint Object
    {
      get
      {
        if (StaticInstance == null && (BaseGuid != 0UL || hierarchyGuid != HierarchyGuid.Empty))
          Load();
        return (IBlueprint) StaticInstance;
      }
    }

    public IEngineInstanced EngineInstance
    {
      get
      {
        if (engineInstance == null)
          LoadDynamic();
        if (engineInstance != null && engineInstance.IsDisposed)
          LoadDynamic();
        return engineInstance;
      }
    }

    public IBlueprint EditorTemplate => Object;

    public override VMType Type => StaticInstance == null ? new VMType(typeof (IObjRef)) : new VMType(typeof (IObjRef), StaticInstance.BaseGuid.ToString());

    public override bool IsEqual(IVariable other)
    {
      if (other == null)
        return false;
      if (typeof (IObjRef).IsAssignableFrom(other.GetType()))
      {
        if (EngineInstance != null && ((IObjRef) other).EngineInstance != null)
          return EngineInstance.EngineGuid == ((IObjRef) other).EngineInstance.EngineGuid;
        if (engineGuid != Guid.Empty || ((VMObjRef) other).EngineGuid != Guid.Empty)
          return engineGuid == ((VMObjRef) other).EngineGuid;
        if (!hierarchyGuid.IsEmpty || !((VMObjRef) other).hierarchyGuid.IsEmpty)
          return hierarchyGuid == ((VMObjRef) other).hierarchyGuid;
      }
      return base.IsEqual(other);
    }

    public override bool Empty => !Static && EngineInstance == null && Guid.Empty == engineGuid;

    public override bool Exist => EngineInstance != null && ((VMBaseEntity) EngineInstance).Instantiated;

    public bool Static
    {
      get
      {
        if (StaticInstance != null)
          return ((ILogicObject) StaticInstance).Static;
        return engineInstance != null && hierarchyGuid.IsHierarchy;
      }
    }

    public override bool IsDynamic => engineInstance != null || engineGuid != Guid.Empty;

    public bool IsHierarchy => !hierarchyGuid.IsEmpty;

    public override string Write()
    {
      if (!HierarchyGuid.IsEmpty)
        return HierarchyGuid.Write();
      return EngineInstance != null ? GuidUtility.GetGuidString(EngineInstance.EngineGuid) : base.Write();
    }

    public override void Read(string data)
    {
      switch (GuidUtility.GetGuidFormat(data))
      {
        case EGuidFormat.GT_HIERARCHY:
          hierarchyGuid = new HierarchyGuid(data);
          Load();
          break;
        case EGuidFormat.GT_ENGINE:
          engineGuid = new Guid(data);
          Load();
          break;
        default:
          base.Read(data);
          break;
      }
    }

    public IBlueprint TypeTemplate => Object;

    protected override void Load()
    {
      if (!hierarchyGuid.IsEmpty)
      {
        ILogicObject hierarhyObjectByGuid = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetWorldHierarhyObjectByGuid(hierarchyGuid);
        if (hierarhyObjectByGuid != null)
          LoadStaticInstance(hierarhyObjectByGuid.Blueprint);
      }
      else if (Guid.Empty != engineGuid && StaticInstance == null)
      {
        IBlueprint templateByEngineGuid = WorldEntityUtility.GetEditorTemplateByEngineGuid(engineGuid);
        if (templateByEngineGuid != null)
          LoadStaticInstance(templateByEngineGuid);
      }
      else
        base.Load();
      LoadDynamic();
    }

    public override string Name
    {
      get
      {
        if (engineInstance != null)
          return GuidUtility.GetGuidString(engineInstance.EngineGuid);
        return !hierarchyGuid.IsEmpty ? hierarchyGuid.Write() : base.Name;
      }
    }

    protected override Type NeedInstanceType => typeof (IBlueprint);

    private void LoadDynamic()
    {
      engineInstance = null;
      if (Guid.Empty != engineGuid)
        engineInstance = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(engineGuid);
      if (engineInstance == null && !hierarchyGuid.IsEmpty)
        engineInstance = WorldEntityUtility.GetDynamicObjectEntityByHierarchyGuid(hierarchyGuid);
      if (engineInstance != null || !Static)
        return;
      engineInstance = WorldEntityUtility.GetDynamicObjectEntityByStaticGuid(StaticInstance.BaseGuid);
    }
  }
}
