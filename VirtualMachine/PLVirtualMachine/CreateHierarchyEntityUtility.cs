// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.CreateHierarchyEntityUtility
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.Objects;

#nullable disable
namespace PLVirtualMachine
{
  public static class CreateHierarchyEntityUtility
  {
    public static VMBaseEntity CreateObject(
      IWorldHierarchyObject staticObject,
      VMBaseEntity parentEntity)
    {
      VMEntity vmEntity = new VMEntity();
      if (typeof (IWorldHierarchyObject).IsAssignableFrom(staticObject.GetType()) && staticObject.IsPhantom)
        return (VMBaseEntity) null;
      vmEntity.Initialize((ILogicObject) staticObject, parentEntity, false);
      if (vmEntity.Instance == null)
        return (VMBaseEntity) null;
      IEntity parent = ServiceCache.Simulation.Hierarchy;
      if (parentEntity != null)
      {
        if (parentEntity.Instance != null)
          parent = parentEntity.Instance;
        parentEntity.AddChildEntity((VMBaseEntity) vmEntity);
      }
      ServiceCache.Simulation.Add(vmEntity.Instance, parent);
      vmEntity.Instantiated = true;
      vmEntity.OnCreate(false);
      if (vmEntity.NeedCreateFSM)
      {
        DynamicFSM entityFsm = DynamicFSM.CreateEntityFSM(vmEntity);
        if (staticObject.Blueprint.StateGraph != null)
        {
          entityFsm.Active = true;
        }
        else
        {
          entityFsm.PropertyInitialized = true;
          if (!vmEntity.IsEngineRoot)
            CreateHierarchyEntityUtility.InitializeObject(vmEntity, (VMLogicObject) staticObject.Blueprint);
        }
      }
      return (VMBaseEntity) vmEntity;
    }

    private static void InitializeObject(VMEntity entity, VMLogicObject templateObject)
    {
      foreach (VMComponent component in entity.Components)
        CreateHierarchyEntityUtility.ComputeComponent(entity, templateObject, component);
    }

    private static void ComputeComponent(
      VMEntity entity,
      VMLogicObject templateObject,
      VMComponent component)
    {
      if (component is IInitialiseComponentFromHierarchy componentFromHierarchy)
        componentFromHierarchy.InitiliseComponentFromHierarchy(entity, templateObject);
      else
        Logger.AddError("Component : " + component.GetType().Name + " is not IInitialiseComponentFromHierarchy");
    }
  }
}
