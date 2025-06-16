using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine;

public static class CreateHierarchyEntityUtility {
	public static VMBaseEntity CreateObject(
		IWorldHierarchyObject staticObject,
		VMBaseEntity parentEntity) {
		var vmEntity = new VMEntity();
		if (typeof(IWorldHierarchyObject).IsAssignableFrom(staticObject.GetType()) && staticObject.IsPhantom)
			return null;
		vmEntity.Initialize(staticObject, parentEntity);
		if (vmEntity.Instance == null)
			return null;
		var parent = ServiceCache.Simulation.Hierarchy;
		if (parentEntity != null) {
			if (parentEntity.Instance != null)
				parent = parentEntity.Instance;
			parentEntity.AddChildEntity(vmEntity);
		}

		ServiceCache.Simulation.Add(vmEntity.Instance, parent);
		vmEntity.Instantiated = true;
		vmEntity.OnCreate();
		if (vmEntity.NeedCreateFSM) {
			var entityFsm = DynamicFSM.CreateEntityFSM(vmEntity);
			if (staticObject.Blueprint.StateGraph != null)
				entityFsm.Active = true;
			else {
				entityFsm.PropertyInitialized = true;
				if (!vmEntity.IsEngineRoot)
					InitializeObject(vmEntity, (VMLogicObject)staticObject.Blueprint);
			}
		}

		return vmEntity;
	}

	private static void InitializeObject(VMEntity entity, VMLogicObject templateObject) {
		foreach (var component in entity.Components)
			ComputeComponent(entity, templateObject, component);
	}

	private static void ComputeComponent(
		VMEntity entity,
		VMLogicObject templateObject,
		VMComponent component) {
		if (component is IInitialiseComponentFromHierarchy componentFromHierarchy)
			componentFromHierarchy.InitiliseComponentFromHierarchy(entity, templateObject);
		else
			Logger.AddError("Component : " + component.GetType().Name + " is not IInitialiseComponentFromHierarchy");
	}
}