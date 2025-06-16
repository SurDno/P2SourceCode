// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Objects.VMWorldObject
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Types;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace PLVirtualMachine.Objects
{
  public class VMWorldObject : 
    VMBlueprint,
    IWorldBlueprint,
    IBlueprint,
    IGameObjectContext,
    IContainer,
    PLVirtualMachine.Common.IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    IContext,
    ILogicObject,
    IWorldObject,
    IEngineTemplated
  {
    [FieldData("WorldPositionGuid", DataFieldType.None)]
    protected HierarchyGuid worldPositionGuid = HierarchyGuid.Empty;
    [FieldData("EngineTemplateID", DataFieldType.None)]
    protected Guid engineTemplateID = Guid.Empty;
    [FieldData("EngineBaseTemplateID", DataFieldType.None)]
    protected Guid engineBaseTemplateID = Guid.Empty;
    [FieldData("Instantiated", DataFieldType.None)]
    protected bool isInstantiated;
    private IWorldBlueprint engineBaseTemplate;
    private bool vmInited;
    private bool directEngineCreated;

    public VMWorldObject(ulong guid)
      : base(guid)
    {
    }

    public Guid EngineTemplateGuid => this.engineTemplateID;

    public Guid EngineBaseTemplateGuid => this.engineBaseTemplateID;

    public override string GuidStr
    {
      get
      {
        return this.EngineTemplateGuid != Guid.Empty ? GuidUtility.GetGuidString(this.EngineTemplateGuid) : base.GuidStr;
      }
    }

    public override bool IsEqual(PLVirtualMachine.Common.IObject other)
    {
      return other != null && typeof (VMWorldObject).IsAssignableFrom(other.GetType()) && base.IsEqual(other);
    }

    public HierarchyGuid WorldPositionGuid => this.worldPositionGuid;

    public override IFiniteStateMachine StateGraph
    {
      get
      {
        if (base.StateGraph != null)
          return base.StateGraph;
        return this.engineBaseTemplate != null && (long) this.engineBaseTemplate.BaseGuid != (long) this.BaseGuid ? this.engineBaseTemplate.StateGraph : (IFiniteStateMachine) null;
      }
    }

    public override bool IsDerivedFrom(ulong iBlueprintGuid, bool bWithSelf = false)
    {
      return base.IsDerivedFrom(iBlueprintGuid, bWithSelf);
    }

    public override void OnAfterLoad()
    {
      if (this.engineBaseTemplateID != Guid.Empty && this.engineBaseTemplate == null)
        this.engineBaseTemplate = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(this.engineBaseTemplateID);
      base.OnAfterLoad();
      if (this.EngineTemplateGuid != Guid.Empty && !this.vmInited)
      {
        WorldEntityUtility.RegistrStaticWorldTemplate(this);
        this.vmInited = true;
      }
      if (!this.IsEngineRoot)
        return;
      VMWorldHierarchyObject hierarchyRootObject = new VMWorldHierarchyObject();
      hierarchyRootObject.Initialize((IWorldBlueprint) this);
      HierarchyManager.RegistrGameHierarchyRoot((IWorldHierarchyObject) hierarchyRootObject);
    }

    public virtual bool IsPhysic => true;

    public bool Instantiated => this.isInstantiated;

    public bool IsEngineRoot => this.worldPositionGuid.TemplateGuid == ulong.MaxValue;

    public override bool DirectEngineCreated => this.directEngineCreated;

    public void InitTemplateFromEngineDirect(IEntity engEntity)
    {
      if (!((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).WorldObjectSaveOptimizedMode)
        Logger.AddError(string.Format("Invalid world object '{0}' enginedirect template creation in not optimized xml saving mode", (object) this.Name));
      if (engEntity == null || this.engineTemplateID != Guid.Empty)
        return;
      if (!this.IsEngineRoot)
      {
        this.isInstantiated = false;
        this.isStatic = false;
        this.name = engEntity.Name;
        this.engineTemplateID = engEntity.Id;
        IEntity baseTemplate = HierarchyManager.GetBaseTemplate(engEntity);
        if (baseTemplate != null)
          this.engineBaseTemplateID = baseTemplate.Id;
        this.directEngineCreated = true;
      }
      this.Update();
    }

    public bool IsPhantom
    {
      get
      {
        return this.engineTemplateID == Guid.Empty && this.engineBaseTemplateID == Guid.Empty && this.name == "";
      }
    }

    protected override void MakeInheritanceMapping()
    {
      List<IBlueprint> list = this.GetClassHierarchy().ToList<IBlueprint>();
      if (this.engineBaseTemplate != null && (long) this.engineBaseTemplate.BaseGuid != (long) this.BaseGuid)
        list.Add((IBlueprint) this.engineBaseTemplate);
      this.MakeInheritanceMapping((IEnumerable<IBlueprint>) list);
    }
  }
}
