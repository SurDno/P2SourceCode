using System;
using System.Collections.Generic;
using System.Reflection;
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

namespace PLVirtualMachine;

public static class CreateEntityUtility {
	public static VMBaseEntity CreateObject(IBlueprint templateObject, VMBaseEntity parentEntity) {
		var flag1 = false;
		var vmEntity = new VMEntity();
		if (flag1)
			vmEntity.Initialize(templateObject, parentEntity);
		else
			vmEntity.Initialize(templateObject);
		IEntity spawnMilestoneRealEntity = null;
		var flag2 = typeof(VMWorldObject).IsAssignableFrom(templateObject.GetType());
		if (flag2 && vmEntity.IsWorldEntity) {
			if (vmEntity.IsPlayerControllable())
				vmEntity.Enabled = false;
			var vmWorldObject = (VMWorldObject)templateObject;
			if (vmWorldObject.IsPhysic) {
				var parent = flag1 ? ServiceCache.Simulation.Hierarchy : ServiceCache.Simulation.Objects;
				var flag3 = true;
				if (vmEntity.Instance == null) {
					Logger.AddError(string.Format(
						"Cannot add created entity {0} to simulation: entity engine instance is null at {1}!",
						templateObject.Name, EngineAPIManager.Instance.CurrentFSMStateInfo));
					flag3 = false;
				}

				if (flag1 && parentEntity != null) {
					if (parentEntity.Instance == null) {
						Logger.AddError(string.Format(
							"Cannot add created entity {0} to hierarchy in simulation: parent entity instance is null at {1}!",
							templateObject.Name, EngineAPIManager.Instance.CurrentFSMStateInfo));
						flag3 = false;
					} else
						parent = parentEntity.Instance;
				}

				if (flag3) {
					try {
						if ((parentEntity != null) & flag1)
							parentEntity.AddChildEntity(vmEntity);
						else if (!flag1)
							VirtualMachine.Instance.GameRootEntity.AddChildEntity(vmEntity);
						ServiceCache.Simulation.Add(vmEntity.Instance, parent);
					} catch (Exception ex) {
						Logger.AddError(string.Format("Cannot add created entity {0} to simulation: {1} at {2}",
							templateObject.Name, ex, EngineAPIManager.Instance.CurrentFSMStateInfo));
					}

					var flag4 = true;
					if (!flag1 && ((VMLogicObject)templateObject).Static &&
					    !((VMWorldObject)templateObject).Instantiated)
						flag4 = false;
					if (flag4)
						vmEntity.Instantiated = true;
					if (vmEntity.Instantiated && !flag1) {
						if (parentEntity != null && parentEntity.IsWorldEntity)
							spawnMilestoneRealEntity = parentEntity.Instance;
						if (spawnMilestoneRealEntity == null)
							spawnMilestoneRealEntity =
								CreateHierarchyHelper.GetEngineMilestoneEntityByPositionID(vmWorldObject
									.WorldPositionGuid);
					}
				}
			}
		} else if (VirtualMachine.Instance.GameRootEntity != null)
			VirtualMachine.Instance.GameRootEntity.AddChildEntity(vmEntity);
		else
			Logger.AddError(string.Format(
				"Entity {0} creation error: this entity must be creating as game object (not hierarchy)!",
				vmEntity.Name));

		vmEntity.OnCreate();
		var flag5 = ((VMLogicObject)templateObject).StateGraph != null;
		if (flag5 || (flag2 && vmEntity != null)) {
			if (vmEntity.NeedCreateFSM) {
				var entityFsm = DynamicFSM.CreateEntityFSM(vmEntity);
				if (flag5 & flag1)
					entityFsm.Active = true;
				else if (vmEntity.Instantiated)
					InitializeObject(vmEntity, (VMLogicObject)templateObject, entityFsm);
			}

			if (flag2 && vmEntity.IsWorldEntity && vmEntity.Instantiated)
				VirtualMachine.Instance.SpawnObject(vmEntity, spawnMilestoneRealEntity);
			return vmEntity;
		}

		InitializeObject(vmEntity, (VMLogicObject)templateObject);
		return null;
	}

	public static void InitializeObject(
		VMEntity entity,
		VMLogicObject templateObject,
		DynamicFSM fsm = null) {
		try {
			if (fsm != null) {
				if (fsm.PropertyInitialized)
					return;
				fsm.PropertyInitialized = true;
			}

			if (entity == null)
				return;
			if (!entity.Instantiated)
				Logger.AddError(string.Format(
					"{0} initialize error: Initializing not instantiated objects not allowed at {1}!",
					templateObject.Name, EngineAPIManager.Instance.CurrentFSMStateInfo));
			else if (typeof(IWorldObject).IsAssignableFrom(templateObject.GetType()) && entity.Instance == null)
				Logger.AddError(string.Format(" Object {0} hasn't engine instance at {1}", entity.Name,
					EngineAPIManager.Instance.CurrentFSMStateInfo));
			else {
				var flag = false;
				if (entity != null && entity.Instance != null)
					flag = entity.IsEngineRoot;
				if (flag)
					return;
				IBlueprint template = null;
				if (templateObject.IsVirtual)
					template = VirtualMachine.Instance.GetVirtualObjectBaseTemplate(entity);
				foreach (var component in entity.Components)
					ComputeComponent(entity, templateObject, fsm, template, component);
			}
		} catch (Exception ex) {
			Logger.AddError(string.Format("Initialization object {0}  error: {1} at {2} !", entity.Name, ex,
				EngineAPIManager.Instance.CurrentFSMStateInfo));
		}
	}

	private static void ComputeComponent(
		VMEntity entity,
		VMLogicObject templateObject,
		DynamicFSM fsm,
		IBlueprint template,
		VMComponent component) {
		var functionalComponents = templateObject.FunctionalComponents;
		IFunctionalComponent staticComponent = null;
		foreach (var keyValuePair in functionalComponents) {
			var functionalComponent = keyValuePair.Value;
			if (((VMFunctionalComponent)functionalComponent).Inited &&
			    ((VMFunctionalComponent)functionalComponent).ComponentType.IsAssignableFrom(component.GetType())) {
				staticComponent = functionalComponent;
				break;
			}
		}

		if (staticComponent == null)
			return;
		ComputeProperties(entity, templateObject, fsm, template, component, staticComponent);
	}

	private static void ComputeProperties(
		VMEntity entity,
		VMLogicObject templateObject,
		DynamicFSM fsm,
		IBlueprint template,
		VMComponent component,
		IFunctionalComponent staticComponent) {
		component.GetType();
		var functionalComponentByName = EngineAPIManager.GetFunctionalComponentByName(component.Name);
		for (var index = 0; index < functionalComponentByName.Properties.Count; ++index) {
			var property = functionalComponentByName.Properties[index];
			ComputeProperty(entity, templateObject, fsm, property.Attribute, property.PropertyInfo, staticComponent,
				template, component);
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
		VMComponent component) {
		var isVirtual = templateObject.IsVirtual;
		if (isVirtual && !attribute.InitialInHierarchy)
			return;
		if ((isVirtual ? attribute.InitialInHierarchy ? 1 : 0 : attribute.Initial ? 1 : 0) != 0) {
			var property = ((IBlueprint)templateObject).GetProperty(staticComponent.Name, propertyInfo.Name);
			if (property == null)
				return;
			if (property.Value == null) {
				if (template != null)
					property = template.GetProperty(staticComponent.Name, propertyInfo.Name);
			} else if (typeof(IRef).IsAssignableFrom(property.Value.GetType()) && ((IRef)property.Value).Empty &&
			           template != null)
				property = template.GetProperty(staticComponent.Name, propertyInfo.Name);

			if (property == null || property.Value == null)
				return;
			if (!propertyInfo.CanWrite)
				return;
			try {
				var obj = CreateHierarchyHelper.ConvertEditorTypeToEngineType(property.Value, propertyInfo.PropertyType,
					property.Type);
				if (VMGlobalStorageManager.Instance != null) {
					var fieldKey = templateObject.BaseGuid + property.Name;
					if (VMGlobalStorageManager.Instance.TemplateRTDataUpdated) {
						var templateRtFieldValue = VMGlobalStorageManager.Instance.GetTemplateRTFieldValue(fieldKey);
						if (templateRtFieldValue != null)
							obj = templateRtFieldValue;
					}
				}

				try {
					if (obj == null)
						return;
					propertyInfo.SetValue(component, obj, null);
				} catch (Exception ex) {
					Logger.AddError(string.Format("Object {0} property {1} set error: {2} at {3}", entity.Name,
						propertyInfo.Name, ex, EngineAPIManager.Instance.CurrentFSMStateInfo));
				}
			} catch (Exception ex) {
				Logger.AddError(string.Format("Object {0} property {1} init error: {2} at {3}", entity.Name,
					propertyInfo.Name, ex, EngineAPIManager.Instance.CurrentFSMStateInfo));
			}
		} else {
			if (fsm == null)
				return;
			try {
				((DynamicParameter)fsm.GetContextParam(propertyInfo.Name))?.UpdateStandartValueByPhysicalComponent();
			} catch (Exception ex) {
				Logger.AddError(string.Format("Object {0} property {1} set error: {2} at {3}", entity.Name,
					propertyInfo.Name, ex, EngineAPIManager.Instance.CurrentFSMStateInfo));
			}
		}
	}
}