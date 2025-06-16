using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Types;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using IObject = PLVirtualMachine.Common.IObject;

namespace PLVirtualMachine.Objects
{
  public class VMWorldObject : 
    VMBlueprint,
    IWorldBlueprint,
    IBlueprint,
    IGameObjectContext,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    IContext,
    ILogicObject,
    IWorldObject,
    IEngineTemplated
  {
    [FieldData("WorldPositionGuid")]
    protected HierarchyGuid worldPositionGuid = HierarchyGuid.Empty;
    [FieldData("EngineTemplateID")]
    protected Guid engineTemplateID = Guid.Empty;
    [FieldData("EngineBaseTemplateID")]
    protected Guid engineBaseTemplateID = Guid.Empty;
    [FieldData("Instantiated")]
    protected bool isInstantiated;
    private IWorldBlueprint engineBaseTemplate;
    private bool vmInited;
    private bool directEngineCreated;

    public VMWorldObject(ulong guid)
      : base(guid)
    {
    }

    public Guid EngineTemplateGuid => engineTemplateID;

    public Guid EngineBaseTemplateGuid => engineBaseTemplateID;

    public override string GuidStr
    {
      get
      {
        return EngineTemplateGuid != Guid.Empty ? GuidUtility.GetGuidString(EngineTemplateGuid) : base.GuidStr;
      }
    }

    public override bool IsEqual(IObject other)
    {
      return other != null && typeof (VMWorldObject).IsAssignableFrom(other.GetType()) && base.IsEqual(other);
    }

    public HierarchyGuid WorldPositionGuid => worldPositionGuid;

    public override IFiniteStateMachine StateGraph
    {
      get
      {
        if (base.StateGraph != null)
          return base.StateGraph;
        return engineBaseTemplate != null && (long) engineBaseTemplate.BaseGuid != (long) BaseGuid ? engineBaseTemplate.StateGraph : null;
      }
    }

    public override bool IsDerivedFrom(ulong iBlueprintGuid, bool bWithSelf = false)
    {
      return base.IsDerivedFrom(iBlueprintGuid, bWithSelf);
    }

    public override void OnAfterLoad()
    {
      if (engineBaseTemplateID != Guid.Empty && engineBaseTemplate == null)
        engineBaseTemplate = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(engineBaseTemplateID);
      base.OnAfterLoad();
      if (EngineTemplateGuid != Guid.Empty && !vmInited)
      {
        WorldEntityUtility.RegistrStaticWorldTemplate(this);
        vmInited = true;
      }
      if (!IsEngineRoot)
        return;
      VMWorldHierarchyObject hierarchyRootObject = new VMWorldHierarchyObject();
      hierarchyRootObject.Initialize(this);
      HierarchyManager.RegistrGameHierarchyRoot(hierarchyRootObject);
    }

    public virtual bool IsPhysic => true;

    public bool Instantiated => isInstantiated;

    public bool IsEngineRoot => worldPositionGuid.TemplateGuid == ulong.MaxValue;

    public override bool DirectEngineCreated => directEngineCreated;

    public void InitTemplateFromEngineDirect(IEntity engEntity)
    {
      if (!((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).WorldObjectSaveOptimizedMode)
        Logger.AddError(string.Format("Invalid world object '{0}' enginedirect template creation in not optimized xml saving mode", Name));
      if (engEntity == null || engineTemplateID != Guid.Empty)
        return;
      if (!IsEngineRoot)
      {
        isInstantiated = false;
        isStatic = false;
        name = engEntity.Name;
        engineTemplateID = engEntity.Id;
        IEntity baseTemplate = HierarchyManager.GetBaseTemplate(engEntity);
        if (baseTemplate != null)
          engineBaseTemplateID = baseTemplate.Id;
        directEngineCreated = true;
      }
      Update();
    }

    public bool IsPhantom
    {
      get
      {
        return engineTemplateID == Guid.Empty && engineBaseTemplateID == Guid.Empty && name == "";
      }
    }

    protected override void MakeInheritanceMapping()
    {
      List<IBlueprint> list = GetClassHierarchy().ToList();
      if (engineBaseTemplate != null && (long) engineBaseTemplate.BaseGuid != (long) BaseGuid)
        list.Add(engineBaseTemplate);
      MakeInheritanceMapping(list);
    }
  }
}
