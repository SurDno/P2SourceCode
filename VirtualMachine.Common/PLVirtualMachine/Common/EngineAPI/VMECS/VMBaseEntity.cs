// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMBaseEntity
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Components.Movable;
using Engine.Common.Services;
using Engine.Common.Types;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;
using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  public class VMBaseEntity : 
    IEngineRTInstance,
    IEngineInstanced,
    IEngineTemplated,
    IEditorBaseTemplate
  {
    protected IEntity engineEntityTemplate;
    protected IEntity engineEntity;
    protected ILogicObject editorTemplate;
    protected bool idDisposed;
    protected bool isEngineRoot;
    protected bool isDynamicChild;
    protected bool isSimple;
    protected bool isCrowdItem;
    protected List<VMBaseEntity> childs;
    protected List<VMBaseEntity> dynamicChilds;
    private List<VMBaseEntity> allChilds;
    private VMBaseEntity parentEntity;
    private List<VMComponent> components = new List<VMComponent>();

    public virtual void Initialize(
      ILogicObject templateObj,
      VMBaseEntity parentEntity,
      bool loading = false)
    {
      if (!typeof (IWorldObject).IsAssignableFrom(templateObj.GetType()))
      {
        Logger.AddError(string.Format("Creating object {0} must be world object", (object) templateObj.Name));
      }
      else
      {
        this.editorTemplate = templateObj;
        if (typeof (IHierarchyObject).IsAssignableFrom(templateObj.GetType()))
          this.InstantiateEngineEntity(((IEngineInstanced) templateObj).EngineGuid);
        else
          this.InstantiateEngineEntity();
      }
    }

    public virtual void Initialize(ILogicObject templateObj, Guid engineGuid = default (Guid))
    {
      if (templateObj == null)
        return;
      this.editorTemplate = templateObj;
      bool flag1 = false;
      bool flag2 = false;
      if (typeof (IWorldObject).IsAssignableFrom(templateObj.GetType()))
      {
        flag1 = ((IWorldObject) templateObj).DirectEngineCreated;
        flag2 = ((IWorldObject) templateObj).EngineBaseTemplateGuid != Guid.Empty;
        if (!flag2)
          flag2 = ((IEngineTemplated) templateObj).EngineTemplateGuid != Guid.Empty;
      }
      if (flag1)
        Logger.AddError("!!! Такого быть не должно !!!");
      else if (flag2)
      {
        this.InstantiateEngineEntity(engineGuid);
        if (this.engineEntity == null)
        {
          string str = "unknown";
          if (this.editorTemplate != null)
            str = this.editorTemplate.Name;
          Logger.AddError(string.Format("Cannot create object {0}, engine instance creation failed", (object) str));
          return;
        }
        this.Instantiated = true;
        if (typeof (IEntity).IsAssignableFrom(this.engineEntity.GetType()))
        {
          this.LoadComponentsFromEntityInstance(this.engineEntity);
          this.AddNewComponent("Common");
        }
        else
          this.LoadComponentsFromVMTemplate(templateObj.Blueprint);
        this.MakeEngineData();
      }
      else
      {
        this.engineEntityTemplate = (IEntity) null;
        this.engineEntity = !(engineGuid != Guid.Empty) ? ServiceCache.Factory.Create<IEntity>() : ServiceCache.Factory.Create<IEntity>(engineGuid);
        this.Instantiated = true;
        this.LoadComponentsFromVMTemplate(templateObj.Blueprint);
      }
      if (this.engineEntity == null)
        return;
      this.engineEntity.Name = templateObj.Name;
    }

    public virtual void Initialize(
      ILogicObject templateObj,
      IEntity engInstance,
      bool bDynamicChild = false)
    {
      if (templateObj == null)
      {
        string str = "none";
        if (engInstance != null)
          str = engInstance.Name;
        Logger.AddError(string.Format("Editor template for entity {0} initialization not defined", (object) str));
      }
      else
      {
        this.engineEntityTemplate = (IEntity) engInstance.Template;
        this.engineEntity = engInstance;
        this.Instantiated = true;
        this.isDynamicChild = bDynamicChild;
        this.editorTemplate = templateObj;
        if (this.engineEntity == null)
          Logger.AddError(string.Format("Entity for editor template {0} not found in engine", (object) templateObj.Name));
        else
          this.LoadComponentsFromEntityInstance(this.engineEntity);
      }
    }

    public virtual void InitSimpleChild(IHierarchyObject simpleChildTemplate)
    {
      this.editorTemplate = (ILogicObject) simpleChildTemplate.EditorTemplate;
      this.InstantiateEngineEntity(simpleChildTemplate.EngineGuid);
      this.Instantiated = true;
      this.isSimple = true;
    }

    public bool IsDynamicChild => this.isDynamicChild;

    public bool IsSimple => this.isSimple;

    public ulong BaseGuid
    {
      get => this.editorTemplate != null ? this.editorTemplate.Blueprint.BaseGuid : 0UL;
    }

    public Guid EngineGuid => this.engineEntity != null ? this.engineEntity.Id : Guid.Empty;

    public Guid EngineTemplateGuid
    {
      get => this.engineEntityTemplate == null ? Guid.Empty : this.engineEntityTemplate.Id;
    }

    public IEntity Instance => this.engineEntity;

    public IBlueprint EditorTemplate => this.editorTemplate.Blueprint;

    public bool IsWorldEntity
    {
      get
      {
        return this.engineEntity != null && typeof (IEntity).IsAssignableFrom(this.engineEntity.GetType());
      }
    }

    public bool IsHierarchy
    {
      get
      {
        if (this.IsSimple)
          return true;
        return this.editorTemplate != null && typeof (IHierarchyObject).IsAssignableFrom(this.editorTemplate.GetType()) && ((IHierarchyObject) this.editorTemplate).HierarchyGuid.IsHierarchy;
      }
    }

    public HierarchyGuid HierarchyGuid
    {
      get
      {
        return this.editorTemplate != null && typeof (IHierarchyObject).IsAssignableFrom(this.editorTemplate.GetType()) ? ((IHierarchyObject) this.editorTemplate).HierarchyGuid : HierarchyGuid.Empty;
      }
    }

    public string Name => this.engineEntity == null ? "" : this.engineEntity.Name;

    public List<VMComponent> Components => this.components;

    public VMComponent GetComponentByName(string componentName)
    {
      foreach (VMComponent component in this.components)
      {
        if (component.Name == componentName)
          return component;
      }
      return (VMComponent) null;
    }

    public static string GetComponentName(IComponent engComponent)
    {
      foreach (Type type in engComponent.GetType().GetInterfaces())
      {
        string componentName = InfoAttribute.GetComponentName(type);
        if (componentName != null)
          return componentName;
      }
      return "";
    }

    public void AddNewComponent(string componentName)
    {
      try
      {
        Type componentTypeByName = this.GetComponentTypeByName(componentName);
        if (componentTypeByName != (Type) null)
        {
          VMComponent instance = (VMComponent) Activator.CreateInstance(componentTypeByName);
          instance.Initialize(this);
          this.DoAddComponent(componentName, instance);
        }
        else
          Logger.AddError(string.Format("Entity {0} component {1} creation error", (object) this.Name, (object) componentName));
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Entity {0} component {1} creation error: {2}", (object) this.Name, (object) componentName, (object) ex));
      }
    }

    private void AddNewComponentFromEngine(IComponent component)
    {
      string componentName = VMBaseEntity.GetComponentName(component);
      Type componentTypeByName = this.GetComponentTypeByName(componentName);
      if ((Type) null == componentTypeByName)
        return;
      try
      {
        VMComponent newComponent = (VMComponent) ProxyFactory.Create(componentTypeByName);
        newComponent.Initialize(this, component);
        this.DoAddComponent(componentName, newComponent);
      }
      catch (Exception ex)
      {
        string str = (string) null;
        if (component == null)
          return;
        string name = component.GetType().Name;
        if (component.Owner != null)
          str = component.Owner.Name;
        Logger.AddError(string.Format("Component {0} in entity {1} creation error: {2}", (object) name, (object) str, (object) ex));
      }
    }

    public virtual void SetPosition(IEntity positionMileStoneEngEntity, AreaEnum areaType = AreaEnum.Unknown)
    {
    }

    public string EngineBaseTemplateGuid
    {
      get
      {
        IEntity engineEntityTemplate = this.engineEntityTemplate;
        if (engineEntityTemplate == null)
          return "";
        Engine.Common.IObject template = engineEntityTemplate.Template;
        return template == null ? "" : GuidUtility.GetGuidString(template.Id);
      }
    }

    public virtual void Remove()
    {
      try
      {
        if (this.idDisposed)
        {
          Logger.AddWarning(string.Format("You try to remove already removed object: {0} at {1}", (object) this.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        }
        else
        {
          ((VMCommon) this.GetComponentByName("Common")).OnRemove();
          if (EngineAPIManager.ObjectCreationExtraDebugInfoMode)
            Logger.AddWarning(string.Format("Extra debug info: removing object: {0} name = {1}", (object) this.Instance.Id, (object) this.Name));
          this.DisposeInstance();
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Object id={0} removing error: {1}", (object) this.Name, (object) ex));
      }
    }

    public virtual void DisposeInstance(bool clear = false)
    {
      try
      {
        foreach (VMComponent component in this.Components)
          component.Clear();
        if (this.engineEntity != null)
          this.engineEntity.Dispose();
        this.engineEntity = (IEntity) null;
        this.Instantiated = false;
        this.idDisposed = true;
        if (this.childs != null)
        {
          for (int index = 0; index < this.childs.Count; ++index)
            this.childs[index].DisposeInstance(clear);
        }
        if (clear)
        {
          if (this.childs != null)
            this.childs.Clear();
          if (this.allChilds != null)
            this.allChilds.Clear();
        }
        else if (this.childs != null)
        {
          for (int index = 0; index < this.dynamicChilds.Count; ++index)
            this.childs.Remove(this.dynamicChilds[index]);
        }
        if (this.dynamicChilds == null)
          return;
        this.dynamicChilds.Clear();
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Object id={0} removing error: {1}", (object) this.Name, (object) ex));
      }
    }

    public bool IsDisposed => this.idDisposed;

    public bool Instantiated { get; set; }

    public bool IsEngineRoot => this.isEngineRoot;

    public bool IsFunctionalSupport(string componentName)
    {
      return this.GetComponentByName(componentName) != null;
    }

    public bool IsFunctionalSupport(IEnumerable<string> functionals)
    {
      foreach (string functional in functionals)
      {
        if (this.GetComponentByName(functional) == null)
          return false;
      }
      return true;
    }

    public void LoadComponentsFromVMTemplate(IBlueprint template)
    {
      Dictionary<string, IFunctionalComponent> functionalComponents = template.FunctionalComponents;
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, IFunctionalComponent> keyValuePair in functionalComponents)
      {
        if (keyValuePair.Value.DependedComponentName == "")
          stringList.Add(keyValuePair.Key);
      }
      for (int index = 0; index < stringList.Count; ++index)
        this.TryAddComponent(stringList[index]);
    }

    private void TryAddComponent(string name)
    {
      foreach (VMComponent component in this.components)
      {
        if (component.Name == name)
          return;
      }
      this.AddNewComponent(name);
    }

    public void LoadComponentsFromEntityInstance(IEntity entity)
    {
      foreach (IComponent component in entity.Components)
      {
        if (component != null)
        {
          try
          {
            this.AddNewComponentFromEngine(component);
          }
          catch (Exception ex)
          {
            Logger.AddError("Load components from instance error: " + ex.Message);
          }
        }
      }
    }

    public void AddChildEntity(VMBaseEntity childEntity)
    {
      if (this.childs == null)
        this.childs = new List<VMBaseEntity>();
      this.childs.Add(childEntity);
      childEntity.ParentEntity = this;
      if (!childEntity.IsDynamicChild)
      {
        if (childEntity.IsSimple)
          return;
        this.AddAggregateChild(childEntity);
      }
      else
      {
        if (this.dynamicChilds == null)
          this.dynamicChilds = new List<VMBaseEntity>();
        this.dynamicChilds.Add(childEntity);
      }
    }

    public VMBaseEntity ParentEntity
    {
      get => this.parentEntity;
      set => this.parentEntity = value;
    }

    public List<VMBaseEntity> GetAllChildEntities() => this.allChilds;

    public List<VMBaseEntity> GetChildEntities() => this.childs;

    public virtual void OnCreate(bool afterLoad = false)
    {
      foreach (VMComponent component in this.components)
        component.OnCreate();
    }

    public void AfterCreate()
    {
      if (this.childs != null)
      {
        for (int index = 0; index < this.childs.Count; ++index)
        {
          if (typeof (VMBaseEntity).IsAssignableFrom(this.childs[index].GetType()))
            this.childs[index].AfterCreate();
        }
      }
      foreach (VMComponent component in this.Components)
        component.AfterCreate();
    }

    public string GetCustomTag()
    {
      VMComponent componentByName = this.GetComponentByName("Common");
      return componentByName != null ? ((VMCommon) componentByName).CustomTag : "";
    }

    public void SetCustomTag(string sTag)
    {
      VMComponent componentByName = this.GetComponentByName("Common");
      if (componentByName == null)
        return;
      ((VMCommon) componentByName).CustomTag = sTag;
    }

    public VMBaseEntity GetNearestOwnerByFunctional(VMType type)
    {
      if (this.IsFunctionalSupport(type.GetFunctionalParts()))
        return this;
      return this.ParentEntity != null ? this.ParentEntity.GetNearestOwnerByFunctional(type) : (VMBaseEntity) null;
    }

    public VMStorage EntityStorageComponent { get; protected set; }

    public VMCommon EntityCommonComponent { get; protected set; }

    protected virtual Type GetComponentTypeByName(string componentName)
    {
      return EngineAPIManager.GetComponentTypeByName(componentName);
    }

    protected virtual void MakeEngineData()
    {
    }

    protected void InstantiateEngineEntity(Guid engDynGuid = default (Guid))
    {
      if (this.editorTemplate == null)
        return;
      if (typeof (IWorldObject).IsAssignableFrom(this.editorTemplate.GetType()))
      {
        try
        {
          Guid id = ((IEngineTemplated) this.editorTemplate).EngineTemplateGuid;
          if (id == Guid.Empty)
            id = ((IWorldObject) this.editorTemplate).EngineBaseTemplateGuid;
          this.engineEntityTemplate = ServiceCache.TemplateService.GetTemplate<IEntity>(id);
          if (this.engineEntityTemplate == null)
          {
            Logger.AddError(string.Format("Object by template {0} creation failed!, Template with engine guid {1} not found in engine", (object) this.editorTemplate.Name, (object) id));
          }
          else
          {
            this.engineEntity = !(engDynGuid == Guid.Empty) ? ServiceCache.Factory.Instantiate<IEntity>(this.engineEntityTemplate, engDynGuid) : ServiceCache.Factory.Instantiate<IEntity>(this.engineEntityTemplate);
            this.idDisposed = false;
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Object {0} with id {1} engine instancing error: {2}", (object) this.Name, (object) engDynGuid, (object) ex.ToString()));
        }
      }
      else
      {
        try
        {
          this.engineEntity = ServiceCache.Factory.Create<IEntity>(engDynGuid);
          this.idDisposed = false;
          this.engineEntity.Name = this.editorTemplate.Name;
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Object {0} with id {1} engine instancing error: {2}", (object) this.Name, (object) engDynGuid, (object) ex.ToString()));
        }
      }
    }

    protected virtual void AddAggregateChild(VMBaseEntity childEntity)
    {
      if (this.allChilds == null)
        this.allChilds = new List<VMBaseEntity>();
      this.allChilds.Add(childEntity);
      if (this.ParentEntity == null)
        return;
      this.ParentEntity.AddAggregateChild(childEntity);
    }

    protected void CreateEntityByEngineTemplate(IEntity engTemplate)
    {
      if (engTemplate == null)
      {
        Logger.AddError(string.Format("Cannot create entity by engine template: engine template is null !!!"));
      }
      else
      {
        this.engineEntityTemplate = engTemplate;
        this.LoadComponentsFromEntityInstance(this.engineEntityTemplate);
        this.AddNewComponent("Common");
        this.MakeEngineData();
      }
    }

    private void DoAddComponent(string componentName, VMComponent newComponent)
    {
      if (newComponent.Name == "Storage")
        this.EntityStorageComponent = (VMStorage) newComponent;
      else if (newComponent.Name == "Common")
        this.EntityCommonComponent = (VMCommon) newComponent;
      else if (newComponent.Name == "CrowdItemComponent")
        this.isCrowdItem = true;
      this.components.Add(newComponent);
    }
  }
}
