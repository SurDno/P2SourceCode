// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.GlobalDoorsManager
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Cofe.Proxies;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;
using System.Collections.Generic;

#nullable disable
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
      if (VMGlobalDoorsManager.Instance == null)
        VMGlobalDoorsManager.Instance = (VMGlobalDoorsManager) this;
      else
        Logger.AddError(string.Format("Global Market Manager component creation dublicate!"));
    }

    public static void ResetInstance()
    {
      VMGlobalDoorsManager.Instance = (VMGlobalDoorsManager) null;
    }

    public override void OpenDoor(IObjRef gateObj)
    {
      if (gateObj == null)
      {
        Logger.AddError(string.Format("Gate object for opening not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
      }
      else
      {
        VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(gateObj.EngineGuid);
        if (entityByEngineGuid == null)
          Logger.AddError(string.Format("Gate object {0} not found at {1}", (object) gateObj.Name, (object) DynamicFSM.CurrentStateInfo));
        else if (entityByEngineGuid != null)
        {
          VMDoor componentByName = (VMDoor) entityByEngineGuid.GetComponentByName("Gate");
          if (componentByName != null)
            componentByName.Component.LockState.Value = LockState.Unlocked;
          else
            Logger.AddError(string.Format("Object {0} isn't gate at {1}", (object) entityByEngineGuid.Name, (object) DynamicFSM.CurrentStateInfo));
        }
        else
          Logger.AddError(string.Format("Gate object for opening not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
      }
    }

    public override void UnlockDoor(IObjRef gateObj)
    {
      if (gateObj == null)
      {
        Logger.AddError(string.Format("Gate object for unlocking not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
      }
      else
      {
        VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(gateObj.EngineGuid);
        if (entityByEngineGuid == null)
          Logger.AddError(string.Format("Gate object {0} not found at {1}", (object) gateObj.Name, (object) DynamicFSM.CurrentStateInfo));
        else if (entityByEngineGuid != null)
        {
          VMDoor componentByName = (VMDoor) entityByEngineGuid.GetComponentByName("Gate");
          if (componentByName != null)
            componentByName.Component.LockState.Value = LockState.Unlocked;
          else
            Logger.AddError(string.Format("Object {0} isn't gate at {1}", (object) entityByEngineGuid.Name, (object) DynamicFSM.CurrentStateInfo));
        }
        else
          Logger.AddError(string.Format("Gate object for unlocking not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
      }
    }

    public override bool IsDoorOpen(IObjRef gateObj)
    {
      if (gateObj == null)
      {
        Logger.AddError(string.Format("Gate object for opening not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(gateObj.EngineGuid);
      if (entityByEngineGuid == null)
      {
        Logger.AddError(string.Format("Gate object {0} not found at {1}", (object) gateObj.Name, (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      if (entityByEngineGuid != null)
      {
        VMDoor componentByName = (VMDoor) entityByEngineGuid.GetComponentByName("Gate");
        if (componentByName != null)
          return componentByName.Component.Opened.Value;
        Logger.AddError(string.Format("Object {0} isn't gate at {1}", (object) entityByEngineGuid.Name, (object) DynamicFSM.CurrentStateInfo));
      }
      else
        Logger.AddError(string.Format("Gate object for opening not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
      return false;
    }

    public override bool IsDoorUnlocked(IObjRef gateObj)
    {
      if (gateObj == null)
      {
        Logger.AddError(string.Format("Gate object for opening not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(gateObj.EngineGuid);
      if (entityByEngineGuid == null)
      {
        Logger.AddError(string.Format("Gate object {0} not found at {1}", (object) gateObj.Name, (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      if (entityByEngineGuid != null)
      {
        VMDoor componentByName = (VMDoor) entityByEngineGuid.GetComponentByName("Gate");
        if (componentByName != null)
          return componentByName.Component.LockState.Value == LockState.Unlocked;
        Logger.AddError(string.Format("Object {0} isn't gate at {1}", (object) entityByEngineGuid.Name, (object) DynamicFSM.CurrentStateInfo));
      }
      else
        Logger.AddError(string.Format("Gate object for opening not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
      return false;
    }

    public override void SetGatesOpeningState(
      string gatesRootInfo,
      GateState gateOpeningState,
      string operationVolumeStr,
      PriorityParameterEnum priority)
    {
      float operationVolume = GameComponent.ReadContextFloatParamValue(VMEngineAPIManager.LastMethodExecInitiator, operationVolumeStr) / 100f;
      this.SetGatesParams(gatesRootInfo, operationVolume, (int) gateOpeningState, -1, priority);
    }

    public override void SetGatesLockState(
      string gatesRootInfo,
      LockState gateLockState,
      string operationVolumeStr,
      PriorityParameterEnum priority)
    {
      float operationVolume = GameComponent.ReadContextFloatParamValue(VMEngineAPIManager.LastMethodExecInitiator, operationVolumeStr) / 100f;
      this.SetGatesParams(gatesRootInfo, operationVolume, -1, (int) gateLockState, priority);
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
        this.SetGatesParams(gatesRootInfo, operationVolume, (int) gateState, (int) gateLockState, priority, gateStatuses);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Set gates all states operation error: {0} at {1}", (object) ex, (object) DynamicFSM.CurrentStateInfo));
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
        int num1 = (int) Math.Round((double) operationVolume * (double) entityListByRootInfo.Count);
        int num2 = 0;
        List<VMEntity> vmEntityList = new List<VMEntity>();
        for (int index = 0; index < entityListByRootInfo.Count; ++index)
        {
          if (num1 < entityListByRootInfo.Count && VMMath.GetRandomDouble() > (double) operationVolume)
          {
            vmEntityList.Add(entityListByRootInfo[index]);
          }
          else
          {
            this.SetGateParams(entityListByRootInfo[index], gateOpeningState, gateLockState, statusesParamInfo, priority);
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
            this.SetGateParams(vmEntityList[index], gateOpeningState, gateLockState, statusesParamInfo, priority);
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Set gates states operation error: {0} at {1}", (object) ex, (object) DynamicFSM.CurrentStateInfo));
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
          Logger.AddError(string.Format("Gate entity not found, cannot process gate operation at {0}!", (object) DynamicFSM.CurrentStateInfo));
        }
        else
        {
          VMDoor componentByName = (VMDoor) gateEntity.GetComponentByName("Gate");
          if (gateOpeningState > -1)
            componentByName.SetOpenedValue(priority, gateOpeningState == 1);
          if (gateLockState > -1)
            componentByName.SetLockStateValue(priority, (LockState) gateLockState);
          componentByName.SetDefaultPriority(priority);
          GameComponent.SetComponentStatuses((VMComponent) componentByName, statusParamsInfo);
          componentByName.SetDefaultPriority(PriorityParameterEnum.Default);
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Set gate {0} opening state operation error: {1} at {2}", (object) gateEntity.EditorTemplate.Name, (object) ex, (object) DynamicFSM.CurrentStateInfo));
      }
    }
  }
}
