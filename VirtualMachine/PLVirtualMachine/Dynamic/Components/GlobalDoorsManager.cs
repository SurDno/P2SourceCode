using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Cofe.Proxies;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMGlobalDoorsManager))]
  public class GlobalDoorsManager : 
    VMGlobalDoorsManager,
    IInitialiseComponentFromHierarchy,
    IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (GlobalDoorsManager);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }

    public override void Initialize(VMBaseEntity parent)
    {
      base.Initialize(parent);
      if (Instance == null)
        Instance = this;
      else
        Logger.AddError("Global Market Manager component creation dublicate!");
    }

    public static void ResetInstance()
    {
      Instance = null;
    }

    public override void OpenDoor(IObjRef gateObj)
    {
      if (gateObj == null)
      {
        Logger.AddError(string.Format("Gate object for opening not defined at {0}", DynamicFSM.CurrentStateInfo));
      }
      else
      {
        VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(gateObj.EngineGuid);
        if (entityByEngineGuid == null)
          Logger.AddError(string.Format("Gate object {0} not found at {1}", gateObj.Name, DynamicFSM.CurrentStateInfo));
        else if (entityByEngineGuid != null)
        {
          VMDoor componentByName = (VMDoor) entityByEngineGuid.GetComponentByName("Gate");
          if (componentByName != null)
            componentByName.Component.LockState.Value = LockState.Unlocked;
          else
            Logger.AddError(string.Format("Object {0} isn't gate at {1}", entityByEngineGuid.Name, DynamicFSM.CurrentStateInfo));
        }
        else
          Logger.AddError(string.Format("Gate object for opening not defined at {0}", DynamicFSM.CurrentStateInfo));
      }
    }

    public override void UnlockDoor(IObjRef gateObj)
    {
      if (gateObj == null)
      {
        Logger.AddError(string.Format("Gate object for unlocking not defined at {0}", DynamicFSM.CurrentStateInfo));
      }
      else
      {
        VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(gateObj.EngineGuid);
        if (entityByEngineGuid == null)
          Logger.AddError(string.Format("Gate object {0} not found at {1}", gateObj.Name, DynamicFSM.CurrentStateInfo));
        else if (entityByEngineGuid != null)
        {
          VMDoor componentByName = (VMDoor) entityByEngineGuid.GetComponentByName("Gate");
          if (componentByName != null)
            componentByName.Component.LockState.Value = LockState.Unlocked;
          else
            Logger.AddError(string.Format("Object {0} isn't gate at {1}", entityByEngineGuid.Name, DynamicFSM.CurrentStateInfo));
        }
        else
          Logger.AddError(string.Format("Gate object for unlocking not defined at {0}", DynamicFSM.CurrentStateInfo));
      }
    }

    public override bool IsDoorOpen(IObjRef gateObj)
    {
      if (gateObj == null)
      {
        Logger.AddError(string.Format("Gate object for opening not defined at {0}", DynamicFSM.CurrentStateInfo));
        return false;
      }
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(gateObj.EngineGuid);
      if (entityByEngineGuid == null)
      {
        Logger.AddError(string.Format("Gate object {0} not found at {1}", gateObj.Name, DynamicFSM.CurrentStateInfo));
        return false;
      }
      if (entityByEngineGuid != null)
      {
        VMDoor componentByName = (VMDoor) entityByEngineGuid.GetComponentByName("Gate");
        if (componentByName != null)
          return componentByName.Component.Opened.Value;
        Logger.AddError(string.Format("Object {0} isn't gate at {1}", entityByEngineGuid.Name, DynamicFSM.CurrentStateInfo));
      }
      else
        Logger.AddError(string.Format("Gate object for opening not defined at {0}", DynamicFSM.CurrentStateInfo));
      return false;
    }

    public override bool IsDoorUnlocked(IObjRef gateObj)
    {
      if (gateObj == null)
      {
        Logger.AddError(string.Format("Gate object for opening not defined at {0}", DynamicFSM.CurrentStateInfo));
        return false;
      }
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(gateObj.EngineGuid);
      if (entityByEngineGuid == null)
      {
        Logger.AddError(string.Format("Gate object {0} not found at {1}", gateObj.Name, DynamicFSM.CurrentStateInfo));
        return false;
      }
      if (entityByEngineGuid != null)
      {
        VMDoor componentByName = (VMDoor) entityByEngineGuid.GetComponentByName("Gate");
        if (componentByName != null)
          return componentByName.Component.LockState.Value == LockState.Unlocked;
        Logger.AddError(string.Format("Object {0} isn't gate at {1}", entityByEngineGuid.Name, DynamicFSM.CurrentStateInfo));
      }
      else
        Logger.AddError(string.Format("Gate object for opening not defined at {0}", DynamicFSM.CurrentStateInfo));
      return false;
    }

    public override void SetGatesOpeningState(
      string gatesRootInfo,
      GateState gateOpeningState,
      string operationVolumeStr,
      PriorityParameterEnum priority)
    {
      float operationVolume = GameComponent.ReadContextFloatParamValue(VMEngineAPIManager.LastMethodExecInitiator, operationVolumeStr) / 100f;
      SetGatesParams(gatesRootInfo, operationVolume, (int) gateOpeningState, -1, priority);
    }

    public override void SetGatesLockState(
      string gatesRootInfo,
      LockState gateLockState,
      string operationVolumeStr,
      PriorityParameterEnum priority)
    {
      float operationVolume = GameComponent.ReadContextFloatParamValue(VMEngineAPIManager.LastMethodExecInitiator, operationVolumeStr) / 100f;
      SetGatesParams(gatesRootInfo, operationVolume, -1, (int) gateLockState, priority);
    }

    public override void SetGatesAllStates(
      string gatesRootInfo,
      GateState gateState,
      LockState gateLockState,
      string gateStatuses,
      string operationVolumeStr,
      PriorityParameterEnum priority)
    {
      try
      {
        float operationVolume = GameComponent.ReadContextFloatParamValue(VMEngineAPIManager.LastMethodExecInitiator, operationVolumeStr) / 100f;
        SetGatesParams(gatesRootInfo, operationVolume, (int) gateState, (int) gateLockState, priority, gateStatuses);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Set gates all states operation error: {0} at {1}", ex, DynamicFSM.CurrentStateInfo));
      }
    }

    private void SetGatesParams(
      string storagesRootInfo,
      float operationVolume,
      int gateOpeningState,
      int gateLockState,
      PriorityParameterEnum priority,
      string gateStatuses = "")
    {
      try
      {
        Dictionary<string, bool> statusesParamInfo = GameComponent.GetBoolStatusesParamInfo(gateStatuses);
        List<VMEntity> entityListByRootInfo = GameComponent.GetEntityListByRootInfo(storagesRootInfo, "Gate");
        int num1 = (int) Math.Round(operationVolume * (double) entityListByRootInfo.Count);
        int num2 = 0;
        List<VMEntity> vmEntityList = [];
        for (int index = 0; index < entityListByRootInfo.Count; ++index)
        {
          if (num1 < entityListByRootInfo.Count && VMMath.GetRandomDouble() > operationVolume)
          {
            vmEntityList.Add(entityListByRootInfo[index]);
          }
          else
          {
            SetGateParams(entityListByRootInfo[index], gateOpeningState, gateLockState, statusesParamInfo, priority);
            ++num2;
            if (num2 >= num1)
              break;
          }
        }
        if (num2 >= num1)
          return;
        for (int index = 0; index < num1 - num2; ++index)
        {
          if (index < vmEntityList.Count)
            SetGateParams(vmEntityList[index], gateOpeningState, gateLockState, statusesParamInfo, priority);
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Set gates states operation error: {0} at {1}", ex, DynamicFSM.CurrentStateInfo));
      }
    }

    private void SetGateParams(
      VMEntity gateEntity,
      int gateOpeningState,
      int gateLockState,
      Dictionary<string, bool> statusParamsInfo,
      PriorityParameterEnum priority)
    {
      try
      {
        if (gateEntity == null || !gateEntity.IsWorldEntity)
          return;
        if (gateEntity.Instance == null)
        {
          Logger.AddError(string.Format("Gate entity not found, cannot process gate operation at {0}!", DynamicFSM.CurrentStateInfo));
        }
        else
        {
          VMDoor componentByName = (VMDoor) gateEntity.GetComponentByName("Gate");
          if (gateOpeningState > -1)
            componentByName.SetOpenedValue(priority, gateOpeningState == 1);
          if (gateLockState > -1)
            componentByName.SetLockStateValue(priority, (LockState) gateLockState);
          componentByName.SetDefaultPriority(priority);
          GameComponent.SetComponentStatuses(componentByName, statusParamsInfo);
          componentByName.SetDefaultPriority(PriorityParameterEnum.Default);
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Set gate {0} opening state operation error: {1} at {2}", gateEntity.EditorTemplate.Name, ex, DynamicFSM.CurrentStateInfo));
      }
    }
  }
}
