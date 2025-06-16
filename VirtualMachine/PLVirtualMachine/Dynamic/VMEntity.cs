using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Movable;
using Engine.Common.Services;
using Engine.Common.Types;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.Objects;
using PLVirtualMachine.Time;

namespace PLVirtualMachine.Dynamic
{
  public class VMEntity : 
    VMBaseEntity,
    IRealTimeModifiable,
    ISerializeStateSave,
    IDynamicLoadSerializable,
    INeedSave
  {
    private bool saveLoadInstantiated;
    private bool positioned;
    private bool modified;

    public override void Initialize(
      ILogicObject templateObj,
      VMBaseEntity parentEntity,
      bool loading = false)
    {
      if (templateObj == null)
      {
        Logger.AddError("Creating object template not defined !");
      }
      else
      {
        base.Initialize(templateObj, parentEntity, loading);
        if (typeof (IWorldObject).IsAssignableFrom(templateObj.GetType()))
        {
          IWorldObject worldObject = (IWorldObject) templateObj;
          if (worldObject.IsPhysic)
            Instantiated = worldObject.Instantiated;
          if (worldObject.IsEngineRoot)
            RegistrAsEngineRoot();
        }
        FinishCreateObject();
      }
    }

    public override void Initialize(ILogicObject templateObj, Guid engineGuid = default (Guid))
    {
      base.Initialize(templateObj, engineGuid);
      if (!typeof (VMWorldObject).IsAssignableFrom(templateObj.GetType()))
        return;
      if (((VMWorldObject) templateObj).IsPhysic)
        Instantiated = ((VMWorldObject) templateObj).Instantiated;
      if (!((VMWorldObject) templateObj).IsEngineRoot)
        return;
      RegistrAsEngineRoot();
    }

    public override void Initialize(
      ILogicObject templateObj,
      IEntity engInstance,
      bool bDynamicChild = false)
    {
      base.Initialize(templateObj, engInstance, bDynamicChild);
    }

    public override void InitSimpleChild(IHierarchyObject simpleChildTemplate)
    {
      base.InitSimpleChild(simpleChildTemplate);
    }

    public virtual void LoadFromXML(XmlElement xmlNode)
    {
      modified = true;
      bool flag = false;
      XmlElement xmlNode1 = null;
      XmlElement xmlElement = null;
      XmlElement listNode = null;
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode.Name == "Instantiated")
          Instantiated = VMSaveLoadManager.ReadBool(childNode);
        else if (childNode.Name == "Positioned")
          positioned = VMSaveLoadManager.ReadBool(childNode);
        else if (childNode.Name == "Removed")
          flag = VMSaveLoadManager.ReadBool(childNode);
        else if (childNode.Name == "Components")
          xmlElement = childNode;
        else if (childNode.Name == "StateMachine")
          xmlNode1 = childNode;
        else if (childNode.Name == "DynamicChildList")
          listNode = childNode;
        else if (childNode.Name == "TemplateId")
          engineEntityTemplate = ServiceCache.TemplateService.GetTemplate<IEntity>(VMSaveLoadManager.ReadGuid(childNode));
        else if (childNode.Name == "InstanceId")
        {
          Instantiated = true;
          isDynamicChild = true;
          engineEntity = ServiceCache.Factory.Instantiate(engineEntityTemplate, VMSaveLoadManager.ReadGuid(childNode));
        }
      }
      saveLoadInstantiated = true;
      if (flag || isDynamicChild)
        return;
      if (xmlElement != null)
      {
        foreach (XmlElement childNode in xmlElement.ChildNodes)
          GetComponentByName(childNode.FirstChild.InnerText)?.LoadFromXML(childNode);
      }
      if (typeof (IWorldObject).IsAssignableFrom(editorTemplate.GetType()))
      {
        try
        {
          IEntity objects = ServiceCache.Simulation.Objects;
          if (!typeof (IWorldHierarchyObject).IsAssignableFrom(editorTemplate.GetType()))
            ServiceCache.Simulation.Add(Instance, objects);
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Saveload error: adding engine instance of {0} to simulation failed: {1}", Name, ex));
        }
      }
      if (!IsSimple)
      {
        if (FSM == null)
        {
          if (GetComponentByName("Speaking") != null)
            FSM = new DynamicTalkingFSM(this, (VMLogicObject) editorTemplate);
          else if (xmlNode1 != null || NeedCreateFSM)
            FSM = new DynamicFSM(this, (VMLogicObject) editorTemplate);
          if (xmlNode1 != null)
            FSM.LoadFromXML(xmlNode1);
        }
        else if (xmlNode1 != null)
          FSM.LoadFromXML(xmlNode1);
      }
      if (listNode == null || listNode.ChildNodes.Count <= 0)
        return;
      List<VMEntity> list = new List<VMEntity>();
      VMSaveLoadManager.LoadDynamiSerializableList(listNode, list);
      VMStorage componentByName = (VMStorage) GetComponentByName("Storage");
      for (int index = 0; index < list.Count; ++index)
        OnLoadDynamicChildEntity(list[index], componentByName);
      componentByName?.CheckInventoryTagsConsistensy();
    }

    public bool Enabled
    {
      get => ((VMCommon) GetComponentByName("Common")).ObjectEnabled;
      set => ((VMCommon) GetComponentByName("Common")).ObjectEnabled = value;
    }

    public override void DisposeInstance(bool bClear = false)
    {
      if (FSM != null)
        FSM.Clear();
      base.DisposeInstance(bClear);
    }

    public void StateSave(IDataWriter writer)
    {
      if (IsDisposed)
        return;
      if (Instance == null)
        Logger.AddError(string.Format("Saveload error: cannot save entity {0}, because it's engine instance not created!", Name));
      else if (IsDynamicChild)
      {
        SaveManagerUtility.Save(writer, "TemplateId", Instance.Template.Id);
        SaveManagerUtility.Save(writer, "InstanceId", Instance.Id);
      }
      else
      {
        string saveLoadKeyGuid = GetSaveLoadKeyGuid();
        SaveManagerUtility.Save(writer, "Id", saveLoadKeyGuid);
        ulong baseGuid = editorTemplate.Blueprint.BaseGuid;
        SaveManagerUtility.Save(writer, "StaticId", baseGuid);
        SaveManagerUtility.Save(writer, "EngineId", Instance.Id);
        SaveManagerUtility.Save(writer, "Name", Name);
        SaveManagerUtility.Save(writer, "Removed", Instance.IsDisposed);
        SaveManagerUtility.Save(writer, "Instantiated", Instantiated);
        SaveManagerUtility.Save(writer, "Positioned", positioned);
        if (FSM != null && FSM.NeedSave)
          SaveManagerUtility.SaveDynamicSerializable(writer, "StateMachine", FSM);
        SaveManagerUtility.SaveDynamicSerializableList(writer, "Components", Components);
        if (dynamicChilds == null)
          return;
        SaveManagerUtility.SaveDynamicSerializableList(writer, "DynamicChildList", dynamicChilds);
      }
    }

    public override void OnCreate(bool afterLoad = false)
    {
      if (!afterLoad && typeof (IWorldHierarchyObject).IsAssignableFrom(editorTemplate.GetType()))
      {
        foreach (IHierarchyObject simpleChild in ((IWorldHierarchyObject) editorTemplate).SimpleChilds)
          CreateSimpleChild(simpleChild);
      }
      base.OnCreate();
      if (WorldEntityUtility.IsDynamicGuidExist(EngineGuid))
        return;
      VirtualMachine.Instance.RegisterDynamicObject(this, editorTemplate);
    }

    public void AfterSaveLoading()
    {
      if (FSM != null && Instantiated)
        FSM.AfterSaveLoading();
      if (childs != null)
      {
        for (int index = 0; index < childs.Count; ++index)
          ((VMEntity) childs[index]).AfterSaveLoading();
      }
      foreach (VMComponent component in Components)
        component.AfterSaveLoading();
    }

    public void InitFSM(DynamicFSM fsm) => FSM = fsm;

    public bool FSMExist => FSM != null;

    public DynamicFSM GetFSM()
    {
      if (FSM == null)
        FSM = DynamicFSM.CreateEntityFSM(this);
      return FSM;
    }

    protected DynamicFSM FSM { get; set; }

    public bool NeedCreateFSM
    {
      get
      {
        return VirtualMachine.Instance.ForceCreateFSM || editorTemplate.Blueprint.StateGraph != null || typeof (IWorldObject).IsAssignableFrom(editorTemplate.GetType()) && !((IWorldObject) editorTemplate).DirectEngineCreated;
      }
    }

    public void RegistrAsEngineRoot()
    {
      isEngineRoot = true;
      if (VMEntityUtility.RegisteredEngineRoot == null)
        VMEntityUtility.RegisteredEngineRoot = this;
      else
        Logger.AddError(string.Format("Engine root {0} already registered !", VMEntityUtility.RegisteredEngineRoot.Name));
    }

    public override void SetPosition(IEntity positionMileStoneEngEntity, AreaEnum NOT_USED_areaType = AreaEnum.Unknown)
    {
      try
      {
        INavigationComponent component = Instance.GetComponent<INavigationComponent>();
        if (component != null)
          component.TeleportTo(positionMileStoneEngEntity);
        else
          Logger.AddError(typeof (INavigationComponent) + " not found in " + Instance.Name);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Entity {0} set position error: {1}", Name, ex));
      }
    }

    public string DebugContextInfo
    {
      get
      {
        IEntity instance = Instance;
        return instance != null ? instance.Context : "";
      }
      set
      {
        IEntity instance = Instance;
        if (instance == null)
          return;
        instance.Context = value;
      }
    }

    public override void Remove()
    {
      base.Remove();
      if (FSM == null || !FSM.Active)
        return;
      Logger.AddError(string.Format("Invalid object {0} removing: FSM is active !!!", Name));
    }

    public string GetSaveLoadKeyGuid()
    {
      try
      {
        return IsHierarchy ? HierarchyGuid.Write() : GuidUtility.GetGuidString(EngineGuid);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Saveload key guid get error at {0} Entity: {1}", Name, ex));
      }
      return "";
    }

    public void PreLoading()
    {
      if (FSM == null)
        return;
      FSM.PreLoading();
    }

    public bool SaveLoadInstantiated => saveLoadInstantiated;

    public static IEnumerable<FreeEntityInfo> GetFreeEntities()
    {
      IEntity storables = ServiceCache.Simulation.Storables;
      if (storables != null && storables.Childs != null)
      {
        foreach (IEntity child in storables.Childs)
        {
          if (!child.DontSave)
            yield return new FreeEntityInfo(child.Template.Id, child.Id);
        }
      }
    }

    public static void LoadFreeEntities(List<FreeEntityInfo> freeEntities)
    {
      for (int index = 0; index < freeEntities.Count; ++index)
        ServiceCache.Simulation.Add(ServiceCache.Factory.Instantiate(ServiceCache.TemplateService.GetTemplate<IEntity>(freeEntities[index].TemplateId), freeEntities[index].InstanceId), ServiceCache.Simulation.Storables);
    }

    public bool IsPlayerControllable(bool bMain = false)
    {
      if (GetComponentByName("PlayerControllerComponent") != null)
      {
        if (!bMain)
          return true;
        if (editorTemplate != null)
          return GameTimeManager.MainContextPlayingCharacter == null || (long) editorTemplate.Blueprint.BaseGuid == (long) GameTimeManager.MainContextPlayingCharacter.BaseGuid;
      }
      return false;
    }

    public bool Positioned
    {
      get => positioned;
      set => positioned = value;
    }

    public bool IsCrowdItem => isCrowdItem;

    public bool Modified => modified;

    public void OnModify() => modified = true;

    public void CreateSimpleChild(IHierarchyObject simpleChildTemplate)
    {
      if (Instance == null)
      {
        Logger.AddError(string.Format("Cannot create simple child entity: owner engine instance not defined in {0}", Name));
      }
      else
      {
        VMEntity childEntity = new VMEntity();
        childEntity.InitSimpleChild(simpleChildTemplate);
        if (childEntity.Instance == null)
        {
          Logger.AddError(string.Format("Cannot create simple child entity by template id={0}: engine instance not created", simpleChildTemplate.EngineTemplateGuid));
        }
        else
        {
          AddChildEntity(childEntity);
          ServiceCache.Simulation.Add(childEntity.Instance, Instance);
        }
      }
    }

    public IRealTimeModifiable ModifiableParent => (IRealTimeModifiable) ParentEntity;

    public bool NeedSave
    {
      get
      {
        if (IsDisposed)
          return false;
        if (IsHierarchy)
          return Modified;
        return positioned || editorTemplate != null && editorTemplate.Static || !IsCrowdItem;
      }
    }

    public void TestHierarchyConsistency()
    {
      if (Instance != null && editorTemplate != null && !IsSimple && editorTemplate.Name != Instance.Name)
        VirtualMachine.SetFatalError(string.Format("SAVE LOAD CRITICAL ERROR: HIERARCHY INCONSISTENCY AT OBJECT {0}, ENGINE_GUID={1}, ENGINE_NAME={2} !!!", editorTemplate.Name, Instance.Id, Instance.Name));
      if (childs == null)
        return;
      for (int index = 0; index < childs.Count; ++index)
        ((VMEntity) childs[index]).TestHierarchyConsistency();
    }

    protected override void AddAggregateChild(VMBaseEntity childEntity)
    {
      if (EngineGuid == VirtualMachine.Instance.WorldHierarchyRootEntity.EngineGuid && !childEntity.IsHierarchy)
        VirtualMachine.SetFatalError(string.Format("UNUSUAL HIERARCHY SYSTEM ERROR: INCORRECT OBJECT {0} WORLD ADDING !!!", childEntity.Name));
      base.AddAggregateChild(childEntity);
    }

    protected override Type GetComponentTypeByName(string componentName)
    {
      Type componentTypeByName = EngineAPIManager.GetComponentTypeByName(componentName);
      return componentTypeByName != null ? ProxyFactory.GetProxyType(componentTypeByName) : null;
    }

    protected override void MakeEngineData()
    {
      if (!IsWorldEntity)
        return;
      if (engineEntityTemplate == null)
      {
        if (Instance != null)
        {
          engineEntityTemplate = (IEntity) Instance.Template;
        }
        else
        {
          Logger.AddError(string.Format("Cannot get engine data for entity with editor template id = {0}: Engine entity and template are not defined", EngineBaseTemplateGuid));
          return;
        }
      }
      base.MakeEngineData();
    }

    protected void FinishCreateObject(bool bWorldObject = true)
    {
      if (bWorldObject)
        CreateEntityByEngineTemplate(engineEntity);
      else
        AddNewComponent("Common");
      LoadComponentsFromVMTemplate(editorTemplate.Blueprint);
      if (engineEntity == null)
        return;
      engineEntity.Name = editorTemplate.Name;
    }

    private void OnLoadDynamicChildEntity(VMEntity dynamicChild, VMStorage storageContainer)
    {
      IWorldBlueprint engineTemplateByGuid = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(dynamicChild.EngineTemplateGuid);
      if (engineTemplateByGuid == null)
        Logger.AddError(string.Format("Saveload error: dynamic child template with engine guid = {0} not registered", GuidUtility.GetGuidString(dynamicChild.EngineTemplateGuid)));
      else if (dynamicChild.Instance == null)
      {
        Logger.AddError(string.Format("SaveLoad error: Cannot load dynamic child with template {0}, instance not created", dynamicChild.EngineTemplateGuid));
      }
      else
      {
        IEntity instance1 = dynamicChild.Instance;
        IEntity instance2 = Instance;
        ServiceCache.Simulation.Add(instance1, instance2);
        dynamicChild.Initialize(engineTemplateByGuid, dynamicChild.Instance, true);
        WorldEntityUtility.OnCreateWorldTemplateDynamicChildInstance(dynamicChild, engineTemplateByGuid);
        AddChildEntity(dynamicChild);
        if (storageContainer == null || dynamicChild.GetComponentByName("Inventory") == null)
          return;
        storageContainer.OnLoadInventory(instance1);
      }
    }
  }
}
