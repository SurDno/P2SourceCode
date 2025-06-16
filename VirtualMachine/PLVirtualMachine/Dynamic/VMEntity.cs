// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.VMEntity
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

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
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
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
        Logger.AddError(string.Format("Creating object template not defined !"));
      }
      else
      {
        base.Initialize(templateObj, parentEntity, loading);
        if (typeof (IWorldObject).IsAssignableFrom(templateObj.GetType()))
        {
          IWorldObject worldObject = (IWorldObject) templateObj;
          if (worldObject.IsPhysic)
            this.Instantiated = worldObject.Instantiated;
          if (worldObject.IsEngineRoot)
            this.RegistrAsEngineRoot();
        }
        this.FinishCreateObject();
      }
    }

    public override void Initialize(ILogicObject templateObj, Guid engineGuid = default (Guid))
    {
      base.Initialize(templateObj, engineGuid);
      if (!typeof (VMWorldObject).IsAssignableFrom(templateObj.GetType()))
        return;
      if (((VMWorldObject) templateObj).IsPhysic)
        this.Instantiated = ((VMWorldObject) templateObj).Instantiated;
      if (!((VMWorldObject) templateObj).IsEngineRoot)
        return;
      this.RegistrAsEngineRoot();
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
      this.modified = true;
      bool flag = false;
      XmlElement xmlNode1 = (XmlElement) null;
      XmlElement xmlElement = (XmlElement) null;
      XmlElement listNode = (XmlElement) null;
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode.Name == "Instantiated")
          this.Instantiated = VMSaveLoadManager.ReadBool((XmlNode) childNode);
        else if (childNode.Name == "Positioned")
          this.positioned = VMSaveLoadManager.ReadBool((XmlNode) childNode);
        else if (childNode.Name == "Removed")
          flag = VMSaveLoadManager.ReadBool((XmlNode) childNode);
        else if (childNode.Name == "Components")
          xmlElement = childNode;
        else if (childNode.Name == "StateMachine")
          xmlNode1 = childNode;
        else if (childNode.Name == "DynamicChildList")
          listNode = childNode;
        else if (childNode.Name == "TemplateId")
          this.engineEntityTemplate = ServiceCache.TemplateService.GetTemplate<IEntity>(VMSaveLoadManager.ReadGuid((XmlNode) childNode));
        else if (childNode.Name == "InstanceId")
        {
          this.Instantiated = true;
          this.isDynamicChild = true;
          this.engineEntity = ServiceCache.Factory.Instantiate<IEntity>(this.engineEntityTemplate, VMSaveLoadManager.ReadGuid((XmlNode) childNode));
        }
      }
      this.saveLoadInstantiated = true;
      if (flag || this.isDynamicChild)
        return;
      if (xmlElement != null)
      {
        foreach (XmlElement childNode in xmlElement.ChildNodes)
          this.GetComponentByName(childNode.FirstChild.InnerText)?.LoadFromXML(childNode);
      }
      if (typeof (IWorldObject).IsAssignableFrom(this.editorTemplate.GetType()))
      {
        try
        {
          IEntity objects = ServiceCache.Simulation.Objects;
          if (!typeof (IWorldHierarchyObject).IsAssignableFrom(this.editorTemplate.GetType()))
            ServiceCache.Simulation.Add(this.Instance, objects);
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Saveload error: adding engine instance of {0} to simulation failed: {1}", (object) this.Name, (object) ex.ToString()));
        }
      }
      if (!this.IsSimple)
      {
        if (this.FSM == null)
        {
          if (this.GetComponentByName("Speaking") != null)
            this.FSM = (DynamicFSM) new DynamicTalkingFSM(this, (VMLogicObject) this.editorTemplate);
          else if (xmlNode1 != null || this.NeedCreateFSM)
            this.FSM = new DynamicFSM(this, (VMLogicObject) this.editorTemplate);
          if (xmlNode1 != null)
            this.FSM.LoadFromXML(xmlNode1);
        }
        else if (xmlNode1 != null)
          this.FSM.LoadFromXML(xmlNode1);
      }
      if (listNode == null || listNode.ChildNodes.Count <= 0)
        return;
      List<VMEntity> list = new List<VMEntity>();
      VMSaveLoadManager.LoadDynamiSerializableList<VMEntity>(listNode, list);
      VMStorage componentByName = (VMStorage) this.GetComponentByName("Storage");
      for (int index = 0; index < list.Count; ++index)
        this.OnLoadDynamicChildEntity(list[index], componentByName);
      componentByName?.CheckInventoryTagsConsistensy();
    }

    public bool Enabled
    {
      get => ((VMCommon) this.GetComponentByName("Common")).ObjectEnabled;
      set => ((VMCommon) this.GetComponentByName("Common")).ObjectEnabled = value;
    }

    public override void DisposeInstance(bool bClear = false)
    {
      if (this.FSM != null)
        this.FSM.Clear();
      base.DisposeInstance(bClear);
    }

    public void StateSave(IDataWriter writer)
    {
      if (this.IsDisposed)
        return;
      if (this.Instance == null)
        Logger.AddError(string.Format("Saveload error: cannot save entity {0}, because it's engine instance not created!", (object) this.Name));
      else if (this.IsDynamicChild)
      {
        SaveManagerUtility.Save(writer, "TemplateId", this.Instance.Template.Id);
        SaveManagerUtility.Save(writer, "InstanceId", this.Instance.Id);
      }
      else
      {
        string saveLoadKeyGuid = this.GetSaveLoadKeyGuid();
        SaveManagerUtility.Save(writer, "Id", saveLoadKeyGuid);
        ulong baseGuid = this.editorTemplate.Blueprint.BaseGuid;
        SaveManagerUtility.Save(writer, "StaticId", baseGuid);
        SaveManagerUtility.Save(writer, "EngineId", this.Instance.Id);
        SaveManagerUtility.Save(writer, "Name", this.Name);
        SaveManagerUtility.Save(writer, "Removed", this.Instance.IsDisposed);
        SaveManagerUtility.Save(writer, "Instantiated", this.Instantiated);
        SaveManagerUtility.Save(writer, "Positioned", this.positioned);
        if (this.FSM != null && this.FSM.NeedSave)
          SaveManagerUtility.SaveDynamicSerializable(writer, "StateMachine", (ISerializeStateSave) this.FSM);
        SaveManagerUtility.SaveDynamicSerializableList<VMComponent>(writer, "Components", this.Components);
        if (this.dynamicChilds == null)
          return;
        SaveManagerUtility.SaveDynamicSerializableList<VMBaseEntity>(writer, "DynamicChildList", this.dynamicChilds);
      }
    }

    public override void OnCreate(bool afterLoad = false)
    {
      if (!afterLoad && typeof (IWorldHierarchyObject).IsAssignableFrom(this.editorTemplate.GetType()))
      {
        foreach (IHierarchyObject simpleChild in ((IWorldHierarchyObject) this.editorTemplate).SimpleChilds)
          this.CreateSimpleChild(simpleChild);
      }
      base.OnCreate();
      if (WorldEntityUtility.IsDynamicGuidExist(this.EngineGuid))
        return;
      VirtualMachine.Instance.RegisterDynamicObject(this, this.editorTemplate);
    }

    public void AfterSaveLoading()
    {
      if (this.FSM != null && this.Instantiated)
        this.FSM.AfterSaveLoading();
      if (this.childs != null)
      {
        for (int index = 0; index < this.childs.Count; ++index)
          ((VMEntity) this.childs[index]).AfterSaveLoading();
      }
      foreach (VMComponent component in this.Components)
        component.AfterSaveLoading();
    }

    public void InitFSM(DynamicFSM fsm) => this.FSM = fsm;

    public bool FSMExist => this.FSM != null;

    public DynamicFSM GetFSM()
    {
      if (this.FSM == null)
        this.FSM = DynamicFSM.CreateEntityFSM(this);
      return this.FSM;
    }

    protected DynamicFSM FSM { get; set; }

    public bool NeedCreateFSM
    {
      get
      {
        return VirtualMachine.Instance.ForceCreateFSM || this.editorTemplate.Blueprint.StateGraph != null || typeof (IWorldObject).IsAssignableFrom(this.editorTemplate.GetType()) && !((IWorldObject) this.editorTemplate).DirectEngineCreated;
      }
    }

    public void RegistrAsEngineRoot()
    {
      this.isEngineRoot = true;
      if (VMEntityUtility.RegisteredEngineRoot == null)
        VMEntityUtility.RegisteredEngineRoot = this;
      else
        Logger.AddError(string.Format("Engine root {0} already registered !", (object) VMEntityUtility.RegisteredEngineRoot.Name));
    }

    public override void SetPosition(IEntity positionMileStoneEngEntity, AreaEnum NOT_USED_areaType = AreaEnum.Unknown)
    {
      try
      {
        INavigationComponent component = this.Instance.GetComponent<INavigationComponent>();
        if (component != null)
          component.TeleportTo(positionMileStoneEngEntity);
        else
          Logger.AddError(typeof (INavigationComponent).ToString() + " not found in " + this.Instance.Name);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Entity {0} set position error: {1}", (object) this.Name, (object) ex.ToString()));
      }
    }

    public string DebugContextInfo
    {
      get
      {
        IEntity instance = this.Instance;
        return instance != null ? instance.Context : "";
      }
      set
      {
        IEntity instance = this.Instance;
        if (instance == null)
          return;
        instance.Context = value;
      }
    }

    public override void Remove()
    {
      base.Remove();
      if (this.FSM == null || !this.FSM.Active)
        return;
      Logger.AddError(string.Format("Invalid object {0} removing: FSM is active !!!", (object) this.Name));
    }

    public string GetSaveLoadKeyGuid()
    {
      try
      {
        return this.IsHierarchy ? this.HierarchyGuid.Write() : GuidUtility.GetGuidString(this.EngineGuid);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Saveload key guid get error at {0} Entity: {1}", (object) this.Name, (object) ex.ToString()));
      }
      return "";
    }

    public void PreLoading()
    {
      if (this.FSM == null)
        return;
      this.FSM.PreLoading();
    }

    public bool SaveLoadInstantiated => this.saveLoadInstantiated;

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
        ServiceCache.Simulation.Add(ServiceCache.Factory.Instantiate<IEntity>(ServiceCache.TemplateService.GetTemplate<IEntity>(freeEntities[index].TemplateId), freeEntities[index].InstanceId), ServiceCache.Simulation.Storables);
    }

    public bool IsPlayerControllable(bool bMain = false)
    {
      if (this.GetComponentByName("PlayerControllerComponent") != null)
      {
        if (!bMain)
          return true;
        if (this.editorTemplate != null)
          return GameTimeManager.MainContextPlayingCharacter == null || (long) this.editorTemplate.Blueprint.BaseGuid == (long) GameTimeManager.MainContextPlayingCharacter.BaseGuid;
      }
      return false;
    }

    public bool Positioned
    {
      get => this.positioned;
      set => this.positioned = value;
    }

    public bool IsCrowdItem => this.isCrowdItem;

    public bool Modified => this.modified;

    public void OnModify() => this.modified = true;

    public void CreateSimpleChild(IHierarchyObject simpleChildTemplate)
    {
      if (this.Instance == null)
      {
        Logger.AddError(string.Format("Cannot create simple child entity: owner engine instance not defined in {0}", (object) this.Name));
      }
      else
      {
        VMEntity childEntity = new VMEntity();
        childEntity.InitSimpleChild(simpleChildTemplate);
        if (childEntity.Instance == null)
        {
          Logger.AddError(string.Format("Cannot create simple child entity by template id={0}: engine instance not created", (object) simpleChildTemplate.EngineTemplateGuid));
        }
        else
        {
          this.AddChildEntity((VMBaseEntity) childEntity);
          ServiceCache.Simulation.Add(childEntity.Instance, this.Instance);
        }
      }
    }

    public IRealTimeModifiable ModifiableParent => (IRealTimeModifiable) this.ParentEntity;

    public bool NeedSave
    {
      get
      {
        if (this.IsDisposed)
          return false;
        if (this.IsHierarchy)
          return this.Modified;
        return this.positioned || this.editorTemplate != null && this.editorTemplate.Static || !this.IsCrowdItem;
      }
    }

    public void TestHierarchyConsistency()
    {
      if (this.Instance != null && this.editorTemplate != null && !this.IsSimple && this.editorTemplate.Name != this.Instance.Name)
        VirtualMachine.SetFatalError(string.Format("SAVE LOAD CRITICAL ERROR: HIERARCHY INCONSISTENCY AT OBJECT {0}, ENGINE_GUID={1}, ENGINE_NAME={2} !!!", (object) this.editorTemplate.Name, (object) this.Instance.Id, (object) this.Instance.Name));
      if (this.childs == null)
        return;
      for (int index = 0; index < this.childs.Count; ++index)
        ((VMEntity) this.childs[index]).TestHierarchyConsistency();
    }

    protected override void AddAggregateChild(VMBaseEntity childEntity)
    {
      if (this.EngineGuid == VirtualMachine.Instance.WorldHierarchyRootEntity.EngineGuid && !childEntity.IsHierarchy)
        VirtualMachine.SetFatalError(string.Format("UNUSUAL HIERARCHY SYSTEM ERROR: INCORRECT OBJECT {0} WORLD ADDING !!!", (object) childEntity.Name));
      base.AddAggregateChild(childEntity);
    }

    protected override Type GetComponentTypeByName(string componentName)
    {
      Type componentTypeByName = EngineAPIManager.GetComponentTypeByName(componentName);
      return componentTypeByName != (Type) null ? ProxyFactory.GetProxyType(componentTypeByName) : (Type) null;
    }

    protected override void MakeEngineData()
    {
      if (!this.IsWorldEntity)
        return;
      if (this.engineEntityTemplate == null)
      {
        if (this.Instance != null)
        {
          this.engineEntityTemplate = (IEntity) this.Instance.Template;
        }
        else
        {
          Logger.AddError(string.Format("Cannot get engine data for entity with editor template id = {0}: Engine entity and template are not defined", (object) this.EngineBaseTemplateGuid));
          return;
        }
      }
      base.MakeEngineData();
    }

    protected void FinishCreateObject(bool bWorldObject = true)
    {
      if (bWorldObject)
        this.CreateEntityByEngineTemplate(this.engineEntity);
      else
        this.AddNewComponent("Common");
      this.LoadComponentsFromVMTemplate(this.editorTemplate.Blueprint);
      if (this.engineEntity == null)
        return;
      this.engineEntity.Name = this.editorTemplate.Name;
    }

    private void OnLoadDynamicChildEntity(VMEntity dynamicChild, VMStorage storageContainer)
    {
      IWorldBlueprint engineTemplateByGuid = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(dynamicChild.EngineTemplateGuid);
      if (engineTemplateByGuid == null)
        Logger.AddError(string.Format("Saveload error: dynamic child template with engine guid = {0} not registered", (object) GuidUtility.GetGuidString(dynamicChild.EngineTemplateGuid)));
      else if (dynamicChild.Instance == null)
      {
        Logger.AddError(string.Format("SaveLoad error: Cannot load dynamic child with template {0}, instance not created", (object) dynamicChild.EngineTemplateGuid));
      }
      else
      {
        IEntity instance1 = dynamicChild.Instance;
        IEntity instance2 = this.Instance;
        ServiceCache.Simulation.Add(instance1, instance2);
        dynamicChild.Initialize((ILogicObject) engineTemplateByGuid, dynamicChild.Instance, true);
        WorldEntityUtility.OnCreateWorldTemplateDynamicChildInstance(dynamicChild, (IBlueprint) engineTemplateByGuid);
        this.AddChildEntity((VMBaseEntity) dynamicChild);
        if (storageContainer == null || dynamicChild.GetComponentByName("Inventory") == null)
          return;
        storageContainer.OnLoadInventory(instance1);
      }
    }
  }
}
