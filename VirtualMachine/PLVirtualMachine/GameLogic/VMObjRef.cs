using Engine.Common.Types;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

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
      this.Load();
    }

    public void Initialize(HierarchyGuid hGuid)
    {
      this.hierarchyGuid = hGuid;
      this.BaseGuid = this.hierarchyGuid.TemplateGuid;
      this.Load();
    }

    public void Initialize(IBlueprint obj) => this.LoadStaticInstance((IObject) obj);

    public void InitializeInstance(IEngineRTInstance instance)
    {
      this.engineInstance = instance;
      this.LoadStaticInstance((IObject) instance.EditorTemplate);
    }

    public override EContextVariableCategory Category
    {
      get
      {
        return this.Object == null || this.Object.GetCategory() != EObjectCategory.OBJECT_CATEGORY_GAME ? EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_OBJECT : EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GAME;
      }
    }

    public Guid EngineTemplateGuid
    {
      get
      {
        if (this.engineInstance != null)
          return this.engineInstance.EngineTemplateGuid;
        return this.Object != null && typeof (IWorldObject).IsAssignableFrom(this.Object.GetType()) ? ((IEngineTemplated) this.Object).EngineTemplateGuid : Guid.Empty;
      }
    }

    public Guid EngineGuid
    {
      get => this.EngineInstance != null ? this.EngineInstance.EngineGuid : this.engineGuid;
    }

    public HierarchyGuid HierarchyGuid => this.hierarchyGuid;

    public IBlueprint Object
    {
      get
      {
        if (this.StaticInstance == null && (this.BaseGuid != 0UL || this.hierarchyGuid != HierarchyGuid.Empty))
          this.Load();
        return (IBlueprint) this.StaticInstance;
      }
    }

    public IEngineInstanced EngineInstance
    {
      get
      {
        if (this.engineInstance == null)
          this.LoadDynamic();
        if (this.engineInstance != null && this.engineInstance.IsDisposed)
          this.LoadDynamic();
        return (IEngineInstanced) this.engineInstance;
      }
    }

    public IBlueprint EditorTemplate => this.Object;

    public override VMType Type
    {
      get
      {
        return this.StaticInstance == null ? new VMType(typeof (IObjRef)) : new VMType(typeof (IObjRef), this.StaticInstance.BaseGuid.ToString());
      }
    }

    public override bool IsEqual(IVariable other)
    {
      if (other == null)
        return false;
      if (typeof (IObjRef).IsAssignableFrom(other.GetType()))
      {
        if (this.EngineInstance != null && ((IObjRef) other).EngineInstance != null)
          return this.EngineInstance.EngineGuid == ((IObjRef) other).EngineInstance.EngineGuid;
        if (this.engineGuid != Guid.Empty || ((VMObjRef) other).EngineGuid != Guid.Empty)
          return this.engineGuid == ((VMObjRef) other).EngineGuid;
        if (!this.hierarchyGuid.IsEmpty || !((VMObjRef) other).hierarchyGuid.IsEmpty)
          return this.hierarchyGuid == ((VMObjRef) other).hierarchyGuid;
      }
      return base.IsEqual(other);
    }

    public override bool Empty
    {
      get => !this.Static && this.EngineInstance == null && Guid.Empty == this.engineGuid;
    }

    public override bool Exist
    {
      get => this.EngineInstance != null && ((VMBaseEntity) this.EngineInstance).Instantiated;
    }

    public bool Static
    {
      get
      {
        if (this.StaticInstance != null)
          return ((ILogicObject) this.StaticInstance).Static;
        return this.engineInstance != null && this.hierarchyGuid.IsHierarchy;
      }
    }

    public override bool IsDynamic => this.engineInstance != null || this.engineGuid != Guid.Empty;

    public bool IsHierarchy => !this.hierarchyGuid.IsEmpty;

    public override string Write()
    {
      if (!this.HierarchyGuid.IsEmpty)
        return this.HierarchyGuid.Write();
      return this.EngineInstance != null ? GuidUtility.GetGuidString(this.EngineInstance.EngineGuid) : base.Write();
    }

    public override void Read(string data)
    {
      switch (GuidUtility.GetGuidFormat(data))
      {
        case EGuidFormat.GT_HIERARCHY:
          this.hierarchyGuid = new HierarchyGuid(data);
          this.Load();
          break;
        case EGuidFormat.GT_ENGINE:
          this.engineGuid = new Guid(data);
          this.Load();
          break;
        default:
          base.Read(data);
          break;
      }
    }

    public IBlueprint TypeTemplate => this.Object;

    protected override void Load()
    {
      if (!this.hierarchyGuid.IsEmpty)
      {
        ILogicObject hierarhyObjectByGuid = (ILogicObject) ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetWorldHierarhyObjectByGuid(this.hierarchyGuid);
        if (hierarhyObjectByGuid != null)
          this.LoadStaticInstance((IObject) hierarhyObjectByGuid.Blueprint);
      }
      else if (Guid.Empty != this.engineGuid && this.StaticInstance == null)
      {
        IBlueprint templateByEngineGuid = WorldEntityUtility.GetEditorTemplateByEngineGuid(this.engineGuid);
        if (templateByEngineGuid != null)
          this.LoadStaticInstance((IObject) templateByEngineGuid);
      }
      else
        base.Load();
      this.LoadDynamic();
    }

    public override string Name
    {
      get
      {
        if (this.engineInstance != null)
          return GuidUtility.GetGuidString(this.engineInstance.EngineGuid);
        return !this.hierarchyGuid.IsEmpty ? this.hierarchyGuid.Write() : base.Name;
      }
    }

    protected override System.Type NeedInstanceType => typeof (IBlueprint);

    private void LoadDynamic()
    {
      this.engineInstance = (IEngineRTInstance) null;
      if (Guid.Empty != this.engineGuid)
        this.engineInstance = (IEngineRTInstance) WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(this.engineGuid);
      if (this.engineInstance == null && !this.hierarchyGuid.IsEmpty)
        this.engineInstance = (IEngineRTInstance) WorldEntityUtility.GetDynamicObjectEntityByHierarchyGuid(this.hierarchyGuid);
      if (this.engineInstance != null || !this.Static)
        return;
      this.engineInstance = (IEngineRTInstance) WorldEntityUtility.GetDynamicObjectEntityByStaticGuid(this.StaticInstance.BaseGuid);
    }
  }
}
