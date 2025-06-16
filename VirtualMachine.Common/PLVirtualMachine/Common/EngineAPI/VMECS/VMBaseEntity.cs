using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Components.Movable;
using Engine.Common.Services;
using Engine.Common.Types;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

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
        Logger.AddError(string.Format("Creating object {0} must be world object", templateObj.Name));
      }
      else
      {
        editorTemplate = templateObj;
        if (typeof (IHierarchyObject).IsAssignableFrom(templateObj.GetType()))
          InstantiateEngineEntity(((IEngineInstanced) templateObj).EngineGuid);
        else
          InstantiateEngineEntity();
      }
    }

    public virtual void Initialize(ILogicObject templateObj, Guid engineGuid = default (Guid))
    {
      if (templateObj == null)
        return;
      editorTemplate = templateObj;
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
        InstantiateEngineEntity(engineGuid);
        if (engineEntity == null)
        {
          string str = "unknown";
          if (editorTemplate != null)
            str = editorTemplate.Name;
          Logger.AddError(string.Format("Cannot create object {0}, engine instance creation failed", str));
          return;
        }
        Instantiated = true;
        if (typeof (IEntity).IsAssignableFrom(engineEntity.GetType()))
        {
          LoadComponentsFromEntityInstance(engineEntity);
          AddNewComponent("Common");
        }
        else
          LoadComponentsFromVMTemplate(templateObj.Blueprint);
        MakeEngineData();
      }
      else
      {
        engineEntityTemplate = null;
        engineEntity = !(engineGuid != Guid.Empty) ? ServiceCache.Factory.Create<IEntity>() : ServiceCache.Factory.Create<IEntity>(engineGuid);
        Instantiated = true;
        LoadComponentsFromVMTemplate(templateObj.Blueprint);
      }
      if (engineEntity == null)
        return;
      engineEntity.Name = templateObj.Name;
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
        Logger.AddError(string.Format("Editor template for entity {0} initialization not defined", str));
      }
      else
      {
        engineEntityTemplate = (IEntity) engInstance.Template;
        engineEntity = engInstance;
        Instantiated = true;
        isDynamicChild = bDynamicChild;
        editorTemplate = templateObj;
        if (engineEntity == null)
          Logger.AddError(string.Format("Entity for editor template {0} not found in engine", templateObj.Name));
        else
          LoadComponentsFromEntityInstance(engineEntity);
      }
    }

    public virtual void InitSimpleChild(IHierarchyObject simpleChildTemplate)
    {
      editorTemplate = simpleChildTemplate.EditorTemplate;
      InstantiateEngineEntity(simpleChildTemplate.EngineGuid);
      Instantiated = true;
      isSimple = true;
    }

    public bool IsDynamicChild => isDynamicChild;

    public bool IsSimple => isSimple;

    public ulong BaseGuid
    {
      get => editorTemplate != null ? editorTemplate.Blueprint.BaseGuid : 0UL;
    }

    public Guid EngineGuid => engineEntity != null ? engineEntity.Id : Guid.Empty;

    public Guid EngineTemplateGuid
    {
      get => engineEntityTemplate == null ? Guid.Empty : engineEntityTemplate.Id;
    }

    public IEntity Instance => engineEntity;

    public IBlueprint EditorTemplate => editorTemplate.Blueprint;

    public bool IsWorldEntity
    {
      get
      {
        return engineEntity != null && typeof (IEntity).IsAssignableFrom(engineEntity.GetType());
      }
    }

    public bool IsHierarchy
    {
      get
      {
        if (IsSimple)
          return true;
        return editorTemplate != null && typeof (IHierarchyObject).IsAssignableFrom(editorTemplate.GetType()) && ((IHierarchyObject) editorTemplate).HierarchyGuid.IsHierarchy;
      }
    }

    public HierarchyGuid HierarchyGuid
    {
      get
      {
        return editorTemplate != null && typeof (IHierarchyObject).IsAssignableFrom(editorTemplate.GetType()) ? ((IHierarchyObject) editorTemplate).HierarchyGuid : HierarchyGuid.Empty;
      }
    }

    public string Name => engineEntity == null ? "" : engineEntity.Name;

    public List<VMComponent> Components => components;

    public VMComponent GetComponentByName(string componentName)
    {
      foreach (VMComponent component in components)
      {
        if (component.Name == componentName)
          return component;
      }
      return null;
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
        Type componentTypeByName = GetComponentTypeByName(componentName);
        if (componentTypeByName != null)
        {
          VMComponent instance = (VMComponent) Activator.CreateInstance(componentTypeByName);
          instance.Initialize(this);
          DoAddComponent(componentName, instance);
        }
        else
          Logger.AddError(string.Format("Entity {0} component {1} creation error", Name, componentName));
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Entity {0} component {1} creation error: {2}", Name, componentName, ex));
      }
    }

    private void AddNewComponentFromEngine(IComponent component)
    {
      string componentName = GetComponentName(component);
      Type componentTypeByName = GetComponentTypeByName(componentName);
      if (null == componentTypeByName)
        return;
      try
      {
        VMComponent newComponent = (VMComponent) ProxyFactory.Create(componentTypeByName);
        newComponent.Initialize(this, component);
        DoAddComponent(componentName, newComponent);
      }
      catch (Exception ex)
      {
        string str = null;
        if (component == null)
          return;
        string name = component.GetType().Name;
        if (component.Owner != null)
          str = component.Owner.Name;
        Logger.AddError(string.Format("Component {0} in entity {1} creation error: {2}", name, str, ex));
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
        if (idDisposed)
        {
          Logger.AddWarning(string.Format("You try to remove already removed object: {0} at {1}", Name, EngineAPIManager.Instance.CurrentFSMStateInfo));
        }
        else
        {
          ((VMCommon) GetComponentByName("Common")).OnRemove();
          if (EngineAPIManager.ObjectCreationExtraDebugInfoMode)
            Logger.AddWarning(string.Format("Extra debug info: removing object: {0} name = {1}", Instance.Id, Name));
          DisposeInstance();
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Object id={0} removing error: {1}", Name, ex));
      }
    }

    public virtual void DisposeInstance(bool clear = false)
    {
      try
      {
        foreach (VMComponent component in Components)
          component.Clear();
        if (engineEntity != null)
          engineEntity.Dispose();
        engineEntity = null;
        Instantiated = false;
        idDisposed = true;
        if (childs != null)
        {
          for (int index = 0; index < childs.Count; ++index)
            childs[index].DisposeInstance(clear);
        }
        if (clear)
        {
          if (childs != null)
            childs.Clear();
          if (allChilds != null)
            allChilds.Clear();
        }
        else if (childs != null)
        {
          for (int index = 0; index < dynamicChilds.Count; ++index)
            childs.Remove(dynamicChilds[index]);
        }
        if (dynamicChilds == null)
          return;
        dynamicChilds.Clear();
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Object id={0} removing error: {1}", Name, ex));
      }
    }

    public bool IsDisposed => idDisposed;

    public bool Instantiated { get; set; }

    public bool IsEngineRoot => isEngineRoot;

    public bool IsFunctionalSupport(string componentName)
    {
      return GetComponentByName(componentName) != null;
    }

    public bool IsFunctionalSupport(IEnumerable<string> functionals)
    {
      foreach (string functional in functionals)
      {
        if (GetComponentByName(functional) == null)
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
        TryAddComponent(stringList[index]);
    }

    private void TryAddComponent(string name)
    {
      foreach (VMComponent component in components)
      {
        if (component.Name == name)
          return;
      }
      AddNewComponent(name);
    }

    public void LoadComponentsFromEntityInstance(IEntity entity)
    {
      foreach (IComponent component in entity.Components)
      {
        if (component != null)
        {
          try
          {
            AddNewComponentFromEngine(component);
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
      if (childs == null)
        childs = new List<VMBaseEntity>();
      childs.Add(childEntity);
      childEntity.ParentEntity = this;
      if (!childEntity.IsDynamicChild)
      {
        if (childEntity.IsSimple)
          return;
        AddAggregateChild(childEntity);
      }
      else
      {
        if (dynamicChilds == null)
          dynamicChilds = new List<VMBaseEntity>();
        dynamicChilds.Add(childEntity);
      }
    }

    public VMBaseEntity ParentEntity
    {
      get => parentEntity;
      set => parentEntity = value;
    }

    public List<VMBaseEntity> GetAllChildEntities() => allChilds;

    public List<VMBaseEntity> GetChildEntities() => childs;

    public virtual void OnCreate(bool afterLoad = false)
    {
      foreach (VMComponent component in components)
        component.OnCreate();
    }

    public void AfterCreate()
    {
      if (childs != null)
      {
        for (int index = 0; index < childs.Count; ++index)
        {
          if (typeof (VMBaseEntity).IsAssignableFrom(childs[index].GetType()))
            childs[index].AfterCreate();
        }
      }
      foreach (VMComponent component in Components)
        component.AfterCreate();
    }

    public string GetCustomTag()
    {
      VMComponent componentByName = GetComponentByName("Common");
      return componentByName != null ? ((VMCommon) componentByName).CustomTag : "";
    }

    public void SetCustomTag(string sTag)
    {
      VMComponent componentByName = GetComponentByName("Common");
      if (componentByName == null)
        return;
      ((VMCommon) componentByName).CustomTag = sTag;
    }

    public VMBaseEntity GetNearestOwnerByFunctional(VMType type)
    {
      if (IsFunctionalSupport(type.GetFunctionalParts()))
        return this;
      return ParentEntity != null ? ParentEntity.GetNearestOwnerByFunctional(type) : null;
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
      if (editorTemplate == null)
        return;
      if (typeof (IWorldObject).IsAssignableFrom(editorTemplate.GetType()))
      {
        try
        {
          Guid id = ((IEngineTemplated) editorTemplate).EngineTemplateGuid;
          if (id == Guid.Empty)
            id = ((IWorldObject) editorTemplate).EngineBaseTemplateGuid;
          engineEntityTemplate = ServiceCache.TemplateService.GetTemplate<IEntity>(id);
          if (engineEntityTemplate == null)
          {
            Logger.AddError(string.Format("Object by template {0} creation failed!, Template with engine guid {1} not found in engine", editorTemplate.Name, id));
          }
          else
          {
            engineEntity = !(engDynGuid == Guid.Empty) ? ServiceCache.Factory.Instantiate(engineEntityTemplate, engDynGuid) : ServiceCache.Factory.Instantiate(engineEntityTemplate);
            idDisposed = false;
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Object {0} with id {1} engine instancing error: {2}", Name, engDynGuid, ex.ToString()));
        }
      }
      else
      {
        try
        {
          engineEntity = ServiceCache.Factory.Create<IEntity>(engDynGuid);
          idDisposed = false;
          engineEntity.Name = editorTemplate.Name;
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Object {0} with id {1} engine instancing error: {2}", Name, engDynGuid, ex.ToString()));
        }
      }
    }

    protected virtual void AddAggregateChild(VMBaseEntity childEntity)
    {
      if (allChilds == null)
        allChilds = new List<VMBaseEntity>();
      allChilds.Add(childEntity);
      if (ParentEntity == null)
        return;
      ParentEntity.AddAggregateChild(childEntity);
    }

    protected void CreateEntityByEngineTemplate(IEntity engTemplate)
    {
      if (engTemplate == null)
      {
        Logger.AddError("Cannot create entity by engine template: engine template is null !!!");
      }
      else
      {
        engineEntityTemplate = engTemplate;
        LoadComponentsFromEntityInstance(engineEntityTemplate);
        AddNewComponent("Common");
        MakeEngineData();
      }
    }

    private void DoAddComponent(string componentName, VMComponent newComponent)
    {
      if (newComponent.Name == "Storage")
        EntityStorageComponent = (VMStorage) newComponent;
      else if (newComponent.Name == "Common")
        EntityCommonComponent = (VMCommon) newComponent;
      else if (newComponent.Name == "CrowdItemComponent")
        isCrowdItem = true;
      components.Add(newComponent);
    }
  }
}
