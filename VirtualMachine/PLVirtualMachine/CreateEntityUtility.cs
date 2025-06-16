using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PLVirtualMachine
{
  public static class CreateEntityUtility
  {
    public static VMBaseEntity CreateObject(IBlueprint templateObject, VMBaseEntity parentEntity)
    {
      bool flag1 = false;
      VMEntity vmEntity = new VMEntity();
      if (flag1)
        vmEntity.Initialize((ILogicObject) templateObject, parentEntity, false);
      else
        vmEntity.Initialize((ILogicObject) templateObject, new Guid());
      IEntity spawnMilestoneRealEntity = (IEntity) null;
      bool flag2 = typeof (VMWorldObject).IsAssignableFrom(templateObject.GetType());
      if (flag2 && vmEntity.IsWorldEntity)
      {
        if (vmEntity.IsPlayerControllable())
          vmEntity.Enabled = false;
        VMWorldObject vmWorldObject = (VMWorldObject) templateObject;
        if (vmWorldObject.IsPhysic)
        {
          IEntity parent = flag1 ? ServiceCache.Simulation.Hierarchy : ServiceCache.Simulation.Objects;
          bool flag3 = true;
          if (vmEntity.Instance == null)
          {
            Logger.AddError(string.Format("Cannot add created entity {0} to simulation: entity engine instance is null at {1}!", (object) templateObject.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
            flag3 = false;
          }
          if (flag1 && parentEntity != null)
          {
            if (parentEntity.Instance == null)
            {
              Logger.AddError(string.Format("Cannot add created entity {0} to hierarchy in simulation: parent entity instance is null at {1}!", (object) templateObject.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
              flag3 = false;
            }
            else
              parent = parentEntity.Instance;
          }
          if (flag3)
          {
            try
            {
              if (parentEntity != null & flag1)
                parentEntity.AddChildEntity((VMBaseEntity) vmEntity);
              else if (!flag1)
                VirtualMachine.Instance.GameRootEntity.AddChildEntity((VMBaseEntity) vmEntity);
              ServiceCache.Simulation.Add(vmEntity.Instance, parent);
            }
            catch (Exception ex)
            {
              Logger.AddError(string.Format("Cannot add created entity {0} to simulation: {1} at {2}", (object) templateObject.Name, (object) ex.ToString(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
            }
            bool flag4 = true;
            if (!flag1 && ((VMLogicObject) templateObject).Static && !((VMWorldObject) templateObject).Instantiated)
              flag4 = false;
            if (flag4)
              vmEntity.Instantiated = true;
            if (vmEntity.Instantiated && !flag1)
            {
              if (parentEntity != null && parentEntity.IsWorldEntity)
                spawnMilestoneRealEntity = parentEntity.Instance;
              if (spawnMilestoneRealEntity == null)
                spawnMilestoneRealEntity = CreateHierarchyHelper.GetEngineMilestoneEntityByPositionID(vmWorldObject.WorldPositionGuid);
            }
          }
        }
      }
      else if (VirtualMachine.Instance.GameRootEntity != null)
        VirtualMachine.Instance.GameRootEntity.AddChildEntity((VMBaseEntity) vmEntity);
      else
        Logger.AddError(string.Format("Entity {0} creation error: this entity must be creating as game object (not hierarchy)!", (object) vmEntity.Name));
      vmEntity.OnCreate(false);
      bool flag5 = ((VMLogicObject) templateObject).StateGraph != null;
      if (flag5 || flag2 && vmEntity != null)
      {
        if (vmEntity.NeedCreateFSM)
        {
          DynamicFSM entityFsm = DynamicFSM.CreateEntityFSM(vmEntity);
          if (flag5 & flag1)
            entityFsm.Active = true;
          else if (vmEntity.Instantiated)
            CreateEntityUtility.InitializeObject(vmEntity, (VMLogicObject) templateObject, entityFsm);
        }
        if (flag2 && vmEntity.IsWorldEntity && vmEntity.Instantiated)
          VirtualMachine.Instance.SpawnObject(vmEntity, spawnMilestoneRealEntity);
        return (VMBaseEntity) vmEntity;
      }
      CreateEntityUtility.InitializeObject(vmEntity, (VMLogicObject) templateObject);
      return (VMBaseEntity) null;
    }

    public static void InitializeObject(
      VMEntity entity,
      VMLogicObject templateObject,
      DynamicFSM fsm = null)
    {
      try
      {
        if (fsm != null)
        {
          if (fsm.PropertyInitialized)
            return;
          fsm.PropertyInitialized = true;
        }
        if (entity == null)
          return;
        if (!entity.Instantiated)
          Logger.AddError(string.Format("{0} initialize error: Initializing not instantiated objects not allowed at {1}!", (object) templateObject.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        else if (typeof (IWorldObject).IsAssignableFrom(templateObject.GetType()) && entity.Instance == null)
        {
          Logger.AddError(string.Format(" Object {0} hasn't engine instance at {1}", (object) entity.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        }
        else
        {
          bool flag = false;
          if (entity != null && entity.Instance != null)
            flag = entity.IsEngineRoot;
          if (flag)
            return;
          IBlueprint template = (IBlueprint) null;
          if (templateObject.IsVirtual)
            template = VirtualMachine.Instance.GetVirtualObjectBaseTemplate(entity);
          foreach (VMComponent component in entity.Components)
            CreateEntityUtility.ComputeComponent(entity, templateObject, fsm, template, component);
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Initialization object {0}  error: {1} at {2} !", (object) entity.Name, (object) ex, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
    }

    private static void ComputeComponent(
      VMEntity entity,
      VMLogicObject templateObject,
      DynamicFSM fsm,
      IBlueprint template,
      VMComponent component)
    {
      Dictionary<string, IFunctionalComponent> functionalComponents = templateObject.FunctionalComponents;
      IFunctionalComponent staticComponent = (IFunctionalComponent) null;
      foreach (KeyValuePair<string, IFunctionalComponent> keyValuePair in functionalComponents)
      {
        IFunctionalComponent functionalComponent = keyValuePair.Value;
        if (((VMFunctionalComponent) functionalComponent).Inited && ((VMFunctionalComponent) functionalComponent).ComponentType.IsAssignableFrom(component.GetType()))
        {
          staticComponent = functionalComponent;
          break;
        }
      }
      if (staticComponent == null)
        return;
      CreateEntityUtility.ComputeProperties(entity, templateObject, fsm, template, component, staticComponent);
    }

    private static void ComputeProperties(
      VMEntity entity,
      VMLogicObject templateObject,
      DynamicFSM fsm,
      IBlueprint template,
      VMComponent component,
      IFunctionalComponent staticComponent)
    {
      component.GetType();
      ComponentInfo functionalComponentByName = EngineAPIManager.GetFunctionalComponentByName(component.Name);
      for (int index = 0; index < functionalComponentByName.Properties.Count; ++index)
      {
        APIPropertyInfo property = functionalComponentByName.Properties[index];
        CreateEntityUtility.ComputeProperty(entity, templateObject, fsm, property.Attribute, property.PropertyInfo, staticComponent, template, component);
      }
    }

    private static void ComputeProperty(
      VMEntity entity,
      VMLogicObject templateObject,
      DynamicFSM fsm,
      PropertyAttribute attribute,
      PropertyInfo propertyInfo,
      IFunctionalComponent staticComponent,
      IBlueprint template,
      VMComponent component)
    {
      bool isVirtual = templateObject.IsVirtual;
      if (isVirtual && !attribute.InitialInHierarchy)
        return;
      if ((isVirtual ? (attribute.InitialInHierarchy ? 1 : 0) : (attribute.Initial ? 1 : 0)) != 0)
      {
        IParam property = ((IBlueprint) templateObject).GetProperty(staticComponent.Name, propertyInfo.Name);
        if (property == null)
          return;
        if (property.Value == null)
        {
          if (template != null)
            property = template.GetProperty(staticComponent.Name, propertyInfo.Name);
        }
        else if (typeof (IRef).IsAssignableFrom(property.Value.GetType()) && ((IRef) property.Value).Empty && template != null)
          property = template.GetProperty(staticComponent.Name, propertyInfo.Name);
        if (property == null || property.Value == null)
          return;
        if (!propertyInfo.CanWrite)
          return;
        try
        {
          object obj = CreateHierarchyHelper.ConvertEditorTypeToEngineType(property.Value, propertyInfo.PropertyType, property.Type);
          if (VMGlobalStorageManager.Instance != null)
          {
            string fieldKey = templateObject.BaseGuid.ToString() + property.Name;
            if (VMGlobalStorageManager.Instance.TemplateRTDataUpdated)
            {
              ITextRef templateRtFieldValue = VMGlobalStorageManager.Instance.GetTemplateRTFieldValue(fieldKey);
              if (templateRtFieldValue != null)
                obj = (object) templateRtFieldValue;
            }
          }
          try
          {
            if (obj == null)
              return;
            propertyInfo.SetValue((object) component, obj, (object[]) null);
          }
          catch (Exception ex)
          {
            Logger.AddError(string.Format("Object {0} property {1} set error: {2} at {3}", (object) entity.Name, (object) propertyInfo.Name, (object) ex, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Object {0} property {1} init error: {2} at {3}", (object) entity.Name, (object) propertyInfo.Name, (object) ex, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        }
      }
      else
      {
        if (fsm == null)
          return;
        try
        {
          ((DynamicParameter) fsm.GetContextParam(propertyInfo.Name))?.UpdateStandartValueByPhysicalComponent();
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Object {0} property {1} set error: {2} at {3}", (object) entity.Name, (object) propertyInfo.Name, (object) ex, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        }
      }
    }
  }
}
