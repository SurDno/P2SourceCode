using Cofe.Loggers;
using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.Regions;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Objects;
using PLVirtualMachine.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMGameComponent))]
  public class GameComponent : VMGameComponent, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public static int MaxGetEntityListByRootInfoInnerEntityesCount = 10000;
    public static List<VMEntity> GroupOperationInnerEntityesByRootList = new List<VMEntity>(GameComponent.MaxGetEntityListByRootInfoInnerEntityesCount);
    public static double totalGetComponentByNameIndexTime = 0.0;
    public static double totalGetEntityListByRootInfoTime = 0.0;
    public static double totalInnerEntityesListAddTime = 0.0;

    public override string GetComponentTypeName() => nameof (GameComponent);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
      {
        case 311972042:
          if (!(name == "OnCommonConsoleEvent"))
            break;
          this.OnCommonConsoleEvent += (Action<string>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case 462744480:
          if (!(name == "OnRegionLoadedOnce"))
            break;
          this.OnRegionLoadedOnce += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case 856205441:
          if (!(name == "OnTimer"))
            break;
          this.OnTimer += (Action<ulong>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case 896158278:
          if (!(name == "OnLoadGame"))
            break;
          this.OnLoadGame += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case 995394406:
          if (!(name == "OnStartGame"))
            break;
          this.OnStartGame += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case 1566732495:
          if (!(name == "OnTemplateEntityLogicEvent"))
            break;
          this.OnTemplateEntityLogicEvent += (VMGameComponent.OnEntityEventHandler) ((p1, p2) => target.RaiseFromEngineImpl((object) p1, (object) p2));
          break;
        case 2206040734:
          if (!(name == "NeedCreateDropBagEvent"))
            break;
          this.NeedCreateDropBagEvent += (VMGameComponent.NeedCreateDropBagEventType) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case 2488324245:
          if (!(name == "NeedDeleteDropBagEvent"))
            break;
          this.NeedDeleteDropBagEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case 2596678185:
          if (!(name == "OnEndStateSleep"))
            break;
          this.OnEndStateSleep += (Action<ulong>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case 2852118553:
          if (!(name == "OnRegionLoaded"))
            break;
          this.OnRegionLoaded += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case 2866581911:
          if (!(name == "OnRegionReputationChangedEvent"))
            break;
          this.OnRegionReputationChangedEvent += (Action<IEntity, float>) ((p1, p2) => target.RaiseFromEngineImpl((object) p1, (object) p2));
          break;
        case 2894496429:
          if (!(name == "OnEntityLogicEvent"))
            break;
          this.OnEntityLogicEvent += (Action<string, IEntity>) ((p1, p2) => target.RaiseFromEngineImpl((object) p1, (object) p2));
          break;
        case 3108488871:
          if (!(name == "OnGameModeChanged"))
            break;
          this.OnGameModeChanged += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case 3544009596:
          if (!(name == "OnRegionDiseaseLevelChangedEvent"))
            break;
          this.OnRegionDiseaseLevelChangedEvent += (Action<IEntity, int>) ((p1, p2) => target.RaiseFromEngineImpl((object) p1, (object) p2));
          break;
        case 3566377774:
          if (!(name == "OnFurnitureLoadedOnce"))
            break;
          this.OnFurnitureLoadedOnce += (Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum>) ((p1, p2, p3, p4) => target.RaiseFromEngineImpl((object) p1, (object) p2, (object) p3, (object) p4));
          break;
        case 3807770123:
          if (!(name == "OnEndCutsceneEvent"))
            break;
          this.OnEndCutsceneEvent += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case 4042290315:
          if (!(name == "OnFurnitureLoaded"))
            break;
          this.OnFurnitureLoaded += (Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum>) ((p1, p2, p3, p4) => target.RaiseFromEngineImpl((object) p1, (object) p2, (object) p3, (object) p4));
          break;
        case 4200346183:
          if (!(name == "OnValueLogicEvent"))
            break;
          this.OnValueLogicEvent += (Action<string, string>) ((p1, p2) => target.RaiseFromEngineImpl((object) p1, (object) p2));
          break;
      }
    }

    public override void Initialize(VMBaseEntity parent)
    {
      base.Initialize(parent);
      if (VMGameComponent.instance == null)
        VMGameComponent.instance = (VMGameComponent) this;
      else
        Logger.AddError(string.Format("Game component creation dublicate!"));
    }

    public static void ResetInstance() => VMGameComponent.instance = (VMGameComponent) null;

    public override void SetCurrentGameTimeContext(IGameModeRef gameMode)
    {
      GameTimeManager.SetCurrentGameTimeContext(gameMode.GameMode.Name);
    }

    public override IGameModeRef GetCurrentGameTimeContext()
    {
      IGameMode staticGameMode = GameTimeManager.CurrentGameTimeContext.StaticGameMode;
      VMGameModeRef currentGameTimeContext = new VMGameModeRef();
      currentGameTimeContext.Initialize(staticGameMode);
      return (IGameModeRef) currentGameTimeContext;
    }

    public override GameTime GetCurrGameTime() => GameTimeManager.GetCurrentGameTime();

    public override GameTime GetCurrDayTime() => GameTimeManager.GetCurrentGameDayTime();

    public override void SetCurrGameTime(GameTime currTime, bool sendTimerEvents)
    {
      GameComponent.SetCurrentGameTime(currTime, sendTimerEvents);
    }

    public static void SetCurrentGameTime(GameTime currTime, bool sendTimerEvents)
    {
      GameTimeManager.SetCurrentGameTime(currTime, sendTimerEvents);
    }

    public override void AddTime(GameTime addingTime) => GameTimeManager.AddTime(addingTime);

    public override void SetCurrSolarTime(GameTime currTime)
    {
      GameTimeManager.SetCurrentSolarTime(currTime);
    }

    public override void SetCurrGeneralTime(GameTime currTime, bool sendTimerEvents)
    {
      GameTimeManager.SetCurrentGameTime(currTime, sendTimerEvents);
      GameTimeManager.SetCurrentSolarTime(currTime);
    }

    public override void SetGameTimeSpeedFactor(float timeSpeed)
    {
      GameTimeManager.SetCurrentGameTimeSpeed(timeSpeed);
    }

    public override void SetSolarTimeSpeedFactor(float timeSpeed)
    {
      GameTimeManager.SetCurrentSolarTimeSpeed(timeSpeed);
    }

    public override void SetGeneralTimeSpeedFactor(float timeSpeed)
    {
      GameTimeManager.SetCurrentGameTimeSpeed(timeSpeed);
      GameTimeManager.SetCurrentSolarTimeSpeed(timeSpeed);
    }

    public override IObjRef CreateObject(IBlueprintRef staticObj)
    {
      return this.DoCreateObject(staticObj, (IObjRef) null);
    }

    public override void RemoveObject(IObjRef remObj)
    {
      if (remObj == null)
      {
        if (EngineAPIManager.ObjectCreationExtraDebugInfoMode)
          Logger.AddError(string.Format("Cannot remove not existing object"));
        else
          Logger.AddWarning(string.Format("Cannot remove not existing object"));
      }
      else
      {
        if (remObj == null)
          return;
        VirtualMachine.Instance.RemoveDynamicObject(remObj.EngineGuid);
      }
    }

    public override bool IsObjectExist(IObjRef checkObj)
    {
      if (checkObj != null && !checkObj.Empty && WorldEntityUtility.IsDynamicGuidExist(checkObj.EngineGuid))
      {
        VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(checkObj.EngineGuid);
        if (entityByEngineGuid != null)
          return entityByEngineGuid.Instantiated;
      }
      return false;
    }

    public override void RemoveObjectsGroup(VMCommonList objList)
    {
      for (int objIndex = 0; objIndex < objList.ObjectsCount; ++objIndex)
      {
        object obj = objList.GetObject(objIndex);
        if (obj != null)
        {
          if (typeof (IObjRef).IsAssignableFrom(obj.GetType()))
            VirtualMachine.Instance.RemoveDynamicObject(((IEngineInstanced) obj).EngineGuid);
          else
            Logger.AddWarning(string.Format("Error group object removing: list memeber at index {0} isn't game object at {1}", (object) objIndex, (object) DynamicFSM.CurrentStateInfo));
        }
        else
          Logger.AddError(string.Format("Error group object removing: list memeber at index {0} is null at {1}", (object) objIndex, (object) DynamicFSM.CurrentStateInfo));
      }
    }

    public override void EnableObject(IObjRef obj, bool enable)
    {
      if (obj == null)
      {
        Logger.AddError(string.Format("Object for enable operation not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
      }
      else
      {
        VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(obj.EngineGuid);
        if (entityByEngineGuid == null)
          Logger.AddError(string.Format("Object {0} for enable operation not found at {1}", (object) obj.Name, (object) DynamicFSM.CurrentStateInfo));
        else if (!entityByEngineGuid.IsWorldEntity)
          Logger.AddError(string.Format("Object {0} for enable operation isn't engine entity at {1}", (object) obj.Name, (object) DynamicFSM.CurrentStateInfo));
        else if (entityByEngineGuid.Instance == null)
          Logger.AddError(string.Format("Object {0} for enable operation hasn't engine instance at {1}", (object) obj.Name, (object) DynamicFSM.CurrentStateInfo));
        else
          entityByEngineGuid.Instance.IsEnabled = enable;
      }
    }

    public override void EnableObjectsGroup(VMCommonList objList, bool enable)
    {
      for (int objIndex = 0; objIndex < objList.ObjectsCount; ++objIndex)
      {
        object obj = objList.GetObject(objIndex);
        if (typeof (IObjRef).IsAssignableFrom(obj.GetType()))
        {
          IObjRef objRef = (IObjRef) obj;
          VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(objRef.EngineGuid);
          if (entityByEngineGuid == null)
          {
            Logger.AddError(string.Format("Object {0} for enable operation not found at {1}", (object) objRef.Name, (object) DynamicFSM.CurrentStateInfo));
            break;
          }
          if (!entityByEngineGuid.IsWorldEntity)
          {
            Logger.AddError(string.Format("Object {0} for enable operation isn't engine entity at {1}", (object) entityByEngineGuid.Name, (object) DynamicFSM.CurrentStateInfo));
            break;
          }
          if (entityByEngineGuid.Instance == null)
          {
            Logger.AddError(string.Format("Object {0} for enable operation hasn't engine instance at {1}", (object) objRef.Name, (object) DynamicFSM.CurrentStateInfo));
            break;
          }
          entityByEngineGuid.Instance.IsEnabled = enable;
        }
        else
          Logger.AddError(string.Format("Error group object removing: list memeber at index {0} isn't game object at {1}", (object) objIndex, (object) DynamicFSM.CurrentStateInfo));
      }
    }

    public override IObjRef CreateObjectTo(IBlueprintRef staticObj, IObjRef milestone)
    {
      return this.DoCreateObject(staticObj, milestone);
    }

    private IObjRef DoCreateObject(IBlueprintRef staticObj, IObjRef milestone)
    {
      try
      {
        bool flag = milestone != null;
        VMObjRef vmObjRef1 = (VMObjRef) milestone;
        VMEntity parentEntity = (VMEntity) null;
        if (vmObjRef1 != null)
        {
          parentEntity = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(vmObjRef1.EngineGuid);
          if (parentEntity == null && milestone != null)
            Logger.AddError(string.Format("Creation position entity param not defined"));
        }
        VMEntity vmEntity = (VMEntity) null;
        IBlueprint blueprint = staticObj.Blueprint;
        if (blueprint != null)
        {
          vmEntity = (VMEntity) CreateEntityUtility.CreateObject(blueprint, (VMBaseEntity) parentEntity);
          vmEntity.Positioned = flag;
        }
        else
          Logger.AddError(string.Format("Cannot create object, because template with id={0} not found", (object) staticObj.StaticInstance.BaseGuid));
        VMObjRef vmObjRef2 = (VMObjRef) null;
        if (vmEntity != null)
        {
          vmObjRef2 = new VMObjRef();
          vmObjRef2.Initialize(vmEntity.EngineGuid);
          vmEntity.AfterCreate();
          if (vmObjRef2 != null && VMEngineAPIManager.LastMethodExecInitiator != null && typeof (DynamicFSM).IsAssignableFrom(VMEngineAPIManager.LastMethodExecInitiator.GetType()))
            ((DynamicFSM) VMEngineAPIManager.LastMethodExecInitiator).OnAddChildDynamicObject(vmEntity.GetFSM());
        }
        return (IObjRef) vmObjRef2;
      }
      catch (Exception ex)
      {
        string str1 = "null";
        if (staticObj != null)
          str1 = staticObj.Name;
        string str2 = "null";
        if (milestone != null)
          str2 = milestone.Name;
        Logger.AddError(string.Format("Creation object {0} in point {1} error: {2} at {3}!", (object) str1, (object) str2, (object) ex, (object) DynamicFSM.CurrentStateInfo));
      }
      return (IObjRef) null;
    }

    public override ulong SleepState(GameTime interval)
    {
      IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
      return GameTimeManager.StartTimer(EGameTimerType.GAME_TIMER_TYPE_RELATIVE_LOCAL, methodExecInitiator.Entity.EngineGuid, methodExecInitiator.CurrentState.BaseGuid, interval, false, "").TimerGuid;
    }

    public override ulong StartGameTimer(GameTime interval)
    {
      IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
      IGameMode gameTimeContext = methodExecInitiator.GameTimeContext;
      return GameTimeManager.StartTimer(EGameTimerType.GAME_TIMER_TYPE_RELATIVE_GLOBAL, methodExecInitiator.Entity.EngineGuid, methodExecInitiator.CurrentState.BaseGuid, interval, false, gameTimeContext.Name).TimerGuid;
    }

    public override ulong StartLoopGameTimer(GameTime interval)
    {
      IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
      IGameMode gameTimeContext = methodExecInitiator.GameTimeContext;
      return GameTimeManager.StartTimer(EGameTimerType.GAME_TIMER_TYPE_RELATIVE_GLOBAL, methodExecInitiator.Entity.EngineGuid, methodExecInitiator.CurrentState.BaseGuid, interval, true, gameTimeContext.Name).TimerGuid;
    }

    public override ulong StartGameTimerAtContext(GameTime interval, IGameModeRef gameMode)
    {
      IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
      IGameMode gameMode1 = methodExecInitiator.GameTimeContext;
      if (gameMode != null && gameMode.GameMode != null)
        gameMode1 = gameMode.GameMode;
      return GameTimeManager.StartTimer(EGameTimerType.GAME_TIMER_TYPE_RELATIVE_GLOBAL, methodExecInitiator.Entity.EngineGuid, methodExecInitiator.CurrentState.BaseGuid, interval, false, gameMode1.Name).TimerGuid;
    }

    public override ulong StartLoopGameTimerAtContext(GameTime interval, IGameModeRef gameMode)
    {
      IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
      IGameMode gameMode1 = methodExecInitiator.GameTimeContext;
      if (gameMode != null && gameMode.GameMode != null)
        gameMode1 = gameMode.GameMode;
      return GameTimeManager.StartTimer(EGameTimerType.GAME_TIMER_TYPE_RELATIVE_GLOBAL, methodExecInitiator.Entity.EngineGuid, methodExecInitiator.CurrentState.BaseGuid, interval, true, gameMode1.Name).TimerGuid;
    }

    public override void StopTimer(ulong iTimerID)
    {
      IGameMode gameTimeContext = VMEngineAPIManager.LastMethodExecInitiator.GameTimeContext;
      GameTimeManager.StopTimer(iTimerID, gameTimeContext.Name);
    }

    public override void StopTimerAtContext(ulong timerIndex, IGameModeRef gameMode)
    {
      IGameMode gameMode1 = VMEngineAPIManager.LastMethodExecInitiator.GameTimeContext;
      if (gameMode != null && gameMode.GameMode != null)
        gameMode1 = gameMode.GameMode;
      GameTimeManager.StopTimer(timerIndex, gameMode1.Name);
    }

    public override ulong SleepStateRT(float interval)
    {
      double totalSeconds = Math.Floor((double) interval);
      return this.SleepState(new GameTime((ulong) totalSeconds, (double) interval - totalSeconds));
    }

    public override ulong StartTimer(float interval)
    {
      double totalSeconds = Math.Floor((double) interval);
      return this.StartGameTimer(new GameTime((ulong) totalSeconds, (double) interval - totalSeconds));
    }

    public override ulong StartLoopTimer(float interval)
    {
      double totalSeconds = Math.Floor((double) interval);
      return this.StartLoopGameTimer(new GameTime((ulong) totalSeconds, (double) interval - totalSeconds));
    }

    public override void ResetCustomTags(string storagesRootInfo)
    {
      this.DoResetTags(storagesRootInfo);
    }

    public void DoResetTags(string rootInfo, string funcComponentName = "")
    {
      List<VMEntity> vmEntityList = !(funcComponentName == "Storage") ? GameComponent.GetEntityListByRootInfo(rootInfo, funcComponentName) : GameComponent.GetStorageEntityListByRootInfo(rootInfo);
      for (int index = 0; index < vmEntityList.Count; ++index)
      {
        VMEntity vmEntity = vmEntityList[index];
        if (vmEntity.Instance == null)
        {
          Logger.AddError(string.Format("Group operation processing: entity {0} not inited in engine at {1}", (object) vmEntity.Name, (object) DynamicFSM.CurrentStateInfo));
          break;
        }
        if (vmEntity != null)
        {
          vmEntity.SetCustomTag(VMCommon.defaultTag);
          vmEntity.EntityStorageComponent?.ResetInnerContainersTags();
        }
      }
    }

    public override void SetCustomTagsDistribution(
      string storagesRootInfo,
      string storageTagDistributionInfo,
      string funcComponentName)
    {
      if (storagesRootInfo == "0")
        Logger.AddWarning(string.Format("Set custom tags distribution: empty group operation root info at {0}", (object) DynamicFSM.CurrentStateInfo));
      else
        this.DoSetTagsDistribution(GameComponent.GetEntityListByRootInfo(storagesRootInfo, funcComponentName), storageTagDistributionInfo);
    }

    public void DoSetTagsDistribution(List<VMEntity> dObjects, string storageTagDistributionInfo)
    {
      Dictionary<int, VMCommon> dictionary = new Dictionary<int, VMCommon>();
      for (int index = 0; index < dObjects.Count; ++index)
      {
        VMEntity dObject = dObjects[index];
        if (dObject != null)
        {
          if (dObject.Instance == null)
          {
            Logger.AddError(string.Format("Group operation processing: entity {0} not inited in engine at {1}", (object) dObject.Name, (object) DynamicFSM.CurrentStateInfo));
            return;
          }
          VMCommon componentByName = (VMCommon) dObject.GetComponentByName("Common");
          if (componentByName == null)
          {
            Logger.AddError(string.Format("Cannot process tags distribution in object {0}, because common component not found at {1}", (object) dObject.Name, (object) DynamicFSM.CurrentStateInfo));
          }
          else
          {
            bool flag = false;
            do
            {
              int randomInt = VMMath.GetRandomInt();
              if (!dictionary.ContainsKey(randomInt))
              {
                dictionary.Add(randomInt, componentByName);
                flag = true;
              }
            }
            while (!flag);
          }
        }
      }
      List<int> list = dictionary.Keys.ToList<int>();
      list.Sort();
      TagDistributionInfo distributionInfo = new TagDistributionInfo();
      distributionInfo.Read(storageTagDistributionInfo);
      float num1 = 0.01f;
      if (!distributionInfo.DistribInPercentage)
      {
        int num2 = 0;
        for (int index = 0; index < distributionInfo.TagInfoList.Count; ++index)
          num2 += distributionInfo.TagInfoList[index].Percentage;
        if (num2 == 0)
        {
          Logger.AddError(string.Format("Distributing tags summ is zero at {0}!", (object) DynamicFSM.CurrentStateInfo));
          return;
        }
        num1 = 1f / (float) num2;
      }
      List<int> intList = new List<int>();
      for (int index = 0; index < distributionInfo.TagInfoList.Count; ++index)
        intList.Add(0);
      for (int index1 = 0; index1 < list.Count; ++index1)
      {
        float num3 = (0.5f + (float) index1) / (float) list.Count;
        float num4 = 0.0f;
        for (int index2 = 0; index2 < distributionInfo.TagInfoList.Count; ++index2)
        {
          float num5 = num4 + num1 * (float) distributionInfo.TagInfoList[index2].Percentage;
          if ((double) num3 > (double) num4 && (double) num3 <= (double) num5)
          {
            if (!distributionInfo.DistribInPercentage)
            {
              if (intList[index2] < distributionInfo.TagInfoList[index2].Percentage)
                dictionary[list[index1]].CustomTag = distributionInfo.TagInfoList[index2].Tag;
              intList[index2]++;
              break;
            }
            dictionary[list[index1]].CustomTag = distributionInfo.TagInfoList[index2].Tag;
            break;
          }
          num4 = num5;
        }
      }
    }

    public override void ProcessCustomGroupObjectAction(string storagesRootInfo, string actionInfo)
    {
      if (storagesRootInfo == "0")
      {
        Logger.AddWarning(string.Format("Process custom group action statuses: empty operation root info at {0}", (object) DynamicFSM.CurrentStateInfo));
      }
      else
      {
        IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
        ILocalContext currentState = (ILocalContext) methodExecInitiator.CurrentState;
        ActionDataStruct action = new ActionDataStruct(actionInfo, currentState);
        List<VMEntity> entityListByRootInfo = GameComponent.GetEntityListByRootInfo(storagesRootInfo);
        for (int index = 0; index < entityListByRootInfo.Count; ++index)
        {
          VMEntity vmEntity = entityListByRootInfo[index];
          switch (vmEntity)
          {
            case null:
            case null:
              continue;
            default:
              ExpressionUtility.ProcessAction((IAbstractAction) action, methodExecInitiator, vmEntity.GetFSM());
              continue;
          }
        }
      }
    }

    public override void ProcessCustomVarGroupObjectAction(
      string storagesRootInfo,
      string funcComponentName,
      string tagsInfoStr,
      string customTagsInfoStr,
      string actionInfoStr,
      string operationVolumeStr)
    {
      if (storagesRootInfo == "0")
      {
        Logger.AddWarning(string.Format("Process custom group action statuses: empty operation root info at {0}", (object) DynamicFSM.CurrentStateInfo));
      }
      else
      {
        OperationMultiTagsInfo tagsInfo1 = new OperationMultiTagsInfo();
        tagsInfo1.Read(tagsInfoStr);
        OperationMultiTagsInfo tagsInfo2 = new OperationMultiTagsInfo();
        tagsInfo2.Read(customTagsInfoStr);
        List<VMEntity> entityListByRootInfo = GameComponent.GetEntityListByRootInfo(storagesRootInfo, funcComponentName);
        List<VMEntity> vmEntityList1 = new List<VMEntity>();
        for (int index = 0; index < entityListByRootInfo.Count; ++index)
        {
          VMEntity entity = entityListByRootInfo[index];
          if (entity != null && this.CheckObjectEngineComponentTag(entity, tagsInfo1) && this.CheckObjectCustomTag(entity, tagsInfo2))
            vmEntityList1.Add(entity);
        }
        IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
        IState currentState = methodExecInitiator.CurrentState;
        ActionDataStruct action = new ActionDataStruct(actionInfoStr, (ILocalContext) currentState);
        float num1 = (float) GameComponent.ReadContextIntParamValue(methodExecInitiator, operationVolumeStr) / 100f;
        int num2 = (int) Math.Round((double) num1 * (double) vmEntityList1.Count);
        int num3 = 0;
        List<VMEntity> vmEntityList2 = new List<VMEntity>();
        for (int index = 0; index < vmEntityList1.Count; ++index)
        {
          if (num2 < vmEntityList1.Count && VMMath.GetRandomDouble() > (double) num1)
          {
            vmEntityList2.Add(vmEntityList1[index]);
          }
          else
          {
            ExpressionUtility.ProcessAction((IAbstractAction) action, methodExecInitiator, vmEntityList1[index].GetFSM());
            ++num3;
            if (num3 >= num2)
              break;
          }
        }
        if (num3 >= num2)
          return;
        for (int index = 0; index < num2 - num3; ++index)
        {
          if (index < vmEntityList2.Count)
          {
            VMEntity entity = vmEntityList2[index];
            if (entity != null && this.CheckObjectEngineComponentTag(entity, tagsInfo1))
              ExpressionUtility.ProcessAction((IAbstractAction) action, methodExecInitiator, entity.GetFSM());
          }
        }
      }
    }

    public static List<VMEntity> GetEntityListByRootInfo(
      string opRootInfo,
      string filterFunctionalName = "")
    {
      IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
      IState currentState = methodExecInitiator.CurrentState;
      OperationPathInfo operationPathInfo = new OperationPathInfo(opRootInfo);
      VMType paramType = new VMType(typeof (IObjRef));
      GameComponent.GroupOperationInnerEntityesByRootList.Clear();
      for (int index1 = 0; index1 < operationPathInfo.RootInfoList.Count; ++index1)
      {
        IObjRef contextParamValue = (IObjRef) ExpressionUtility.GetContextParamValue(methodExecInitiator, operationPathInfo.RootInfoList[index1], paramType);
        if (contextParamValue != null && contextParamValue.EngineGuid != Guid.Empty)
        {
          VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(contextParamValue.EngineGuid);
          if (entityByEngineGuid != null && typeof (IWorldObject).IsAssignableFrom(entityByEngineGuid.EditorTemplate.GetType()))
          {
            List<VMBaseEntity> allChildEntities = entityByEngineGuid.GetAllChildEntities();
            if (allChildEntities != null)
            {
              for (int index2 = 0; index2 < allChildEntities.Count; ++index2)
              {
                VMEntity vmEntity = (VMEntity) allChildEntities[index2];
                if (vmEntity != null && (!(filterFunctionalName != string.Empty) || vmEntity.GetComponentByName(filterFunctionalName) != null))
                  GameComponent.GroupOperationInnerEntityesByRootList.Add(vmEntity);
              }
            }
            bool flag = true;
            if ("" != filterFunctionalName && entityByEngineGuid.GetComponentByName(filterFunctionalName) == null)
              flag = false;
            if (flag)
              GameComponent.GroupOperationInnerEntityesByRootList.Add(entityByEngineGuid);
          }
        }
      }
      return GameComponent.GroupOperationInnerEntityesByRootList;
    }

    public static List<VMEntity> GetStorageEntityListByRootInfo(string opRootInfo)
    {
      IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
      IState currentState = methodExecInitiator.CurrentState;
      OperationPathInfo operationPathInfo = new OperationPathInfo(opRootInfo);
      VMType paramType = new VMType(typeof (IObjRef));
      GameComponent.GroupOperationInnerEntityesByRootList.Clear();
      for (int index1 = 0; index1 < operationPathInfo.RootInfoList.Count; ++index1)
      {
        IObjRef contextParamValue = (IObjRef) ExpressionUtility.GetContextParamValue(methodExecInitiator, operationPathInfo.RootInfoList[index1], paramType);
        if (contextParamValue != null && contextParamValue.EngineGuid != Guid.Empty)
        {
          VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(contextParamValue.EngineGuid);
          if (entityByEngineGuid != null && typeof (IWorldObject).IsAssignableFrom(entityByEngineGuid.EditorTemplate.GetType()))
          {
            List<VMBaseEntity> allChildEntities = entityByEngineGuid.GetAllChildEntities();
            if (allChildEntities != null)
            {
              for (int index2 = 0; index2 < allChildEntities.Count; ++index2)
              {
                VMEntity vmEntity = (VMEntity) allChildEntities[index2];
                if (vmEntity != null && vmEntity.EntityStorageComponent != null)
                  GameComponent.GroupOperationInnerEntityesByRootList.Add(vmEntity);
              }
            }
            if (entityByEngineGuid.EntityStorageComponent != null)
              GameComponent.GroupOperationInnerEntityesByRootList.Add(entityByEngineGuid);
          }
        }
      }
      return GameComponent.GroupOperationInnerEntityesByRootList;
    }

    public override void SetCustomGroupObjectStatuses(
      string storagesRootInfo,
      string funcComponentName,
      string objectStatusesData)
    {
      if (storagesRootInfo == "0")
      {
        Logger.AddWarning(string.Format("Set custom group object statuses: empty operation root info at {0}", (object) DynamicFSM.CurrentStateInfo));
      }
      else
      {
        IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
        Dictionary<string, bool> statusesParamInfo = GameComponent.GetBoolStatusesParamInfo(objectStatusesData);
        List<VMEntity> entityListByRootInfo = GameComponent.GetEntityListByRootInfo(storagesRootInfo, funcComponentName);
        for (int index = 0; index < entityListByRootInfo.Count; ++index)
        {
          VMEntity vmEntity = entityListByRootInfo[index];
          if (vmEntity != null)
          {
            if (vmEntity.Instance == null)
            {
              Logger.AddError(string.Format("Group operation processing: entity {0} not inited in engine at {1}", (object) vmEntity.Name, (object) DynamicFSM.CurrentStateInfo));
              break;
            }
            VMComponent componentByName = vmEntity.GetComponentByName(funcComponentName);
            if (componentByName != null)
              GameComponent.SetComponentStatuses(componentByName, statusesParamInfo);
            else
              Logger.AddError(string.Format("Object {0} hasn't component {1} for set statuses! at {1}", (object) vmEntity.Name, (object) DynamicFSM.CurrentStateInfo));
          }
        }
      }
    }

    public override void SetCustomVarGroupObjectStatuses(
      string storagesRootInfo,
      string funcComponentName,
      string tagsInfoStr,
      string customTagsInfoStr,
      string objectStatusesData,
      string operationVolumeStr)
    {
      if (storagesRootInfo == "0")
      {
        Logger.AddWarning(string.Format("Set custom group object statuses: empty storages guids list at {0}", (object) DynamicFSM.CurrentStateInfo));
      }
      else
      {
        OperationMultiTagsInfo tagsInfo1 = new OperationMultiTagsInfo();
        tagsInfo1.Read(tagsInfoStr);
        OperationMultiTagsInfo tagsInfo2 = new OperationMultiTagsInfo();
        tagsInfo2.Read(customTagsInfoStr);
        Dictionary<string, bool> statusesParamInfo = GameComponent.GetBoolStatusesParamInfo(objectStatusesData);
        List<VMEntity> entityListByRootInfo = GameComponent.GetEntityListByRootInfo(storagesRootInfo, funcComponentName);
        List<VMEntity> vmEntityList1 = new List<VMEntity>();
        for (int index = 0; index < entityListByRootInfo.Count; ++index)
        {
          VMEntity entity = entityListByRootInfo[index];
          if (entity != null && entity.GetComponentByName(funcComponentName) != null && this.CheckObjectEngineComponentTag(entity, tagsInfo1) && this.CheckObjectCustomTag(entity, tagsInfo2))
            vmEntityList1.Add(entity);
        }
        float num1 = (float) GameComponent.ReadContextIntParamValue(VMEngineAPIManager.LastMethodExecInitiator, operationVolumeStr) / 100f;
        int num2 = (int) Math.Round((double) num1 * (double) vmEntityList1.Count);
        int num3 = 0;
        List<VMEntity> vmEntityList2 = new List<VMEntity>();
        for (int index = 0; index < vmEntityList1.Count; ++index)
        {
          VMEntity vmEntity = vmEntityList1[index];
          if (num2 < vmEntityList1.Count && VMMath.GetRandomDouble() > (double) num1)
          {
            vmEntityList2.Add(vmEntity);
          }
          else
          {
            GameComponent.SetComponentStatuses(vmEntity.GetComponentByName(funcComponentName), statusesParamInfo);
            ++num3;
            if (num3 >= num2)
              break;
          }
        }
        if (num3 >= num2)
          return;
        for (int index = 0; index < num2 - num3; ++index)
        {
          if (index < vmEntityList2.Count)
          {
            VMEntity entity = vmEntityList2[index];
            if (entity != null)
            {
              VMComponent componentByName = entity.GetComponentByName(funcComponentName);
              if (componentByName != null && this.CheckObjectEngineComponentTag(entity, tagsInfo1))
                GameComponent.SetComponentStatuses(componentByName, statusesParamInfo);
            }
          }
        }
      }
    }

    public static object ExtractParamFuncParam(string paramFuncInfo, Type paramType)
    {
      paramFuncInfo = paramFuncInfo.ToUpper();
      string[] separator = new string[1]{ "PARAM&VAL" };
      string[] strArray = paramFuncInfo.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      if (!(paramType == typeof (int) | paramType == typeof (bool)))
        return (object) null;
      int int32 = StringUtility.ToInt32(strArray[1]);
      return paramType == typeof (bool) ? (object) (int32 > 0) : (object) int32;
    }

    public static string ExtractParamFuncName(string paramFuncInfo)
    {
      string str = paramFuncInfo;
      if (paramFuncInfo.Contains("PARAM&VAL"))
      {
        string[] separator = new string[1]{ "PARAM&VAL" };
        str = paramFuncInfo.Split(separator, StringSplitOptions.RemoveEmptyEntries)[0];
      }
      string[] strArray = str.Split('.');
      return strArray.Length > 1 ? strArray[1] : str;
    }

    public override IBlueprintRef GetObjectClass(IObjRef objRef)
    {
      if (objRef == null)
      {
        Logger.AddError(string.Format("Cannot get object class: target object is null at {0}", (object) DynamicFSM.CurrentStateInfo));
        return (IBlueprintRef) null;
      }
      VMObjRef vmObjRef = (VMObjRef) objRef;
      if (vmObjRef.Object == null)
      {
        Logger.AddWarning(string.Format("Object template for {0} not defined at {1}", (object) objRef.Name, (object) DynamicFSM.CurrentStateInfo));
        VMBlueprintRef objectClass = new VMBlueprintRef();
        objectClass.Initialize((IBlueprint) IStaticDataContainer.StaticDataContainer.GameRoot);
        return (IBlueprintRef) objectClass;
      }
      IBlueprint blueprint = vmObjRef.Object;
      VMBlueprintRef objectClass1 = new VMBlueprintRef();
      objectClass1.Initialize(vmObjRef.Object);
      return (IBlueprintRef) objectClass1;
    }

    public override bool IsObjectDerivedFromTemplate(IObjRef objRef, IBlueprintRef classRef)
    {
      if (objRef == null)
      {
        Logger.AddWarning(string.Format("Object reference for object derived from template checking not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      if (classRef == null)
      {
        Logger.AddWarning(string.Format("Template reference for object derived from template checking not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      VMObjRef vmObjRef = (VMObjRef) objRef;
      if (vmObjRef.Object == null)
      {
        Logger.AddWarning(string.Format("Object template for {0} not defined at {1}", (object) objRef.Name, (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      IBlueprint blueprint = classRef.Blueprint;
      if (blueprint == null)
      {
        Logger.AddWarning(string.Format("Template for object derived from template checking not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      try
      {
        IBlueprint other = ((VMObjRef) objRef).Object;
        if (other != null)
        {
          if (blueprint.IsEqual((PLVirtualMachine.Common.IObject) other))
            return true;
          if (typeof (VMWorldObject).IsAssignableFrom(other.GetType()))
          {
            if (typeof (VMWorldObject).IsAssignableFrom(blueprint.GetType()))
            {
              if (((VMWorldObject) other).EngineBaseTemplateGuid == ((VMWorldObject) blueprint).EngineTemplateGuid)
                return true;
            }
          }
        }
        else
          Logger.AddError(string.Format("Object {0} static template not found at {1}", (object) objRef.Name, (object) DynamicFSM.CurrentStateInfo));
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Cannot check object {0} from template {1} derivation: error {2} at {3}", (object) vmObjRef.Object.Name, (object) blueprint.Name, (object) ex.ToString(), (object) DynamicFSM.CurrentStateInfo));
      }
      return false;
    }

    public override bool IsObjectDerivedFromClass(IObjRef objRef, IBlueprintRef classRef)
    {
      if (classRef == null)
      {
        Logger.AddError(string.Format("Checking inheritance base class not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      if (classRef.Blueprint == null)
      {
        Logger.AddError(string.Format("Checking inheritance base class not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      VMObjRef vmObjRef = (VMObjRef) objRef;
      if (vmObjRef.Object == null)
      {
        Logger.AddError(string.Format("Cannot define object {0} base class hierarchy: object template not defined at {1}", (object) objRef.Name, (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      return typeof (IBlueprint).IsAssignableFrom(vmObjRef.Object.GetType()) && ((VMLogicObject) vmObjRef.Object).IsDerivedFrom(classRef.Blueprint.BaseGuid, true);
    }

    public override bool IsObjectCompatible(IObjRef objRef, VMType type)
    {
      if (type == null)
      {
        Logger.AddError(string.Format("Cannot check object type compatibility: type not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      if (objRef == null)
      {
        Logger.AddError(string.Format("Cannot check object type compatibility: object not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      if (typeof (IObjRef).IsAssignableFrom(type.BaseType))
      {
        if (type.SpecialType == null || "" == type.SpecialType)
          return true;
        bool flag = false;
        VMObjRef vmObjRef = (VMObjRef) objRef;
        if (vmObjRef.Object != null)
        {
          flag = vmObjRef.Object.IsFunctionalSupport(type.GetFunctionalParts());
        }
        else
        {
          IBlueprint templateByEngineGuid = WorldEntityUtility.GetEditorTemplateByEngineGuid(vmObjRef.EngineGuid);
          if (templateByEngineGuid != null)
            flag = templateByEngineGuid.IsFunctionalSupport(type.GetFunctionalParts());
        }
        if (flag)
          return true;
        VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(vmObjRef.EngineGuid);
        if (entityByEngineGuid != null)
          return entityByEngineGuid.IsFunctionalSupport(type.GetFunctionalParts());
      }
      return false;
    }

    public override IObjRef GetNearestOwnerByFunctional(IObjRef objRef, VMType type)
    {
      if (objRef == null)
      {
        Logger.AddError(string.Format("Cannot get nearest owner by functional: target object not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return (IObjRef) new VMObjRef();
      }
      if (objRef.Empty)
      {
        Logger.AddError(string.Format("Cannot get nearest owner by functional: target object not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return (IObjRef) new VMObjRef();
      }
      if (type == null)
      {
        Logger.AddError(string.Format("Cannot get nearest owner by functional for {0}: functional type not defined at {1}", (object) objRef.EngineGuid, (object) DynamicFSM.CurrentStateInfo));
        return (IObjRef) new VMObjRef();
      }
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(((VMObjRef) objRef).EngineGuid);
      if (entityByEngineGuid == null)
        return (IObjRef) new VMObjRef();
      VMBaseEntity ownerByFunctional1 = entityByEngineGuid.GetNearestOwnerByFunctional(type);
      VMObjRef ownerByFunctional2 = new VMObjRef();
      ownerByFunctional2.InitializeInstance((IEngineRTInstance) ownerByFunctional1);
      return (IObjRef) ownerByFunctional2;
    }

    public override IObjRef GetObjectWithTag(IObjRef objRef, string tag)
    {
      VMObjRef vmObjRef = (VMObjRef) objRef;
      if (vmObjRef.Object == null)
      {
        Logger.AddError(string.Format("Root object for tag object finding not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return (IObjRef) null;
      }
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(vmObjRef.EngineGuid);
      if (entityByEngineGuid == null)
      {
        Logger.AddError(string.Format("Root object {0} FSM for tag object finding not found at {1}", (object) vmObjRef.EngineGuid, (object) DynamicFSM.CurrentStateInfo));
        return (IObjRef) null;
      }
      List<VMBaseEntity> allChildEntities = entityByEngineGuid.GetAllChildEntities();
      if (allChildEntities != null)
      {
        for (int index = 0; index < allChildEntities.Count; ++index)
        {
          if (allChildEntities[index].GetCustomTag() == tag)
          {
            VMObjRef objectWithTag = new VMObjRef();
            objectWithTag.InitializeInstance((IEngineRTInstance) allChildEntities[index]);
            return (IObjRef) objectWithTag;
          }
          VMTags componentByName = (VMTags) allChildEntities[index].GetComponentByName("TagsComponent");
          if (componentByName != null && componentByName.TagsList.Contains(tag))
          {
            VMObjRef objectWithTag = new VMObjRef();
            objectWithTag.InitializeInstance((IEngineRTInstance) allChildEntities[index]);
            return (IObjRef) objectWithTag;
          }
        }
      }
      return (IObjRef) new VMObjRef();
    }

    public override void AddGlobalLogicMapPage(ILogicMapRef newMMPage)
    {
      DynamicMindMap.AddGlobalMindMap(newMMPage.LogicMap);
    }

    public override void AddLogicMapPage(ILogicMapRef newMMPage)
    {
      DynamicMindMap.AddMindMap(newMMPage.LogicMap);
    }

    public override void RemoveLogicMapPage(ILogicMapRef remMMPage)
    {
      DynamicMindMap.RemoveMindMap(remMMPage.LogicMap);
    }

    public void StartGameTime()
    {
    }

    public static void SetComponentStatuses(
      VMComponent component,
      Dictionary<string, bool> statusParamsInfo)
    {
      foreach (KeyValuePair<string, bool> keyValuePair in statusParamsInfo)
      {
        PropertyInfo componentPropertyInfo = InfoAttribute.GetComponentPropertyInfo(component.Name, keyValuePair.Key);
        if ((PropertyInfo) null != componentPropertyInfo)
          componentPropertyInfo.SetValue((object) component, (object) keyValuePair.Value, (object[]) null);
      }
    }

    public override int ReadIntParamValue(string valueStr)
    {
      return GameComponent.ReadContextIntParamValue(VMEngineAPIManager.LastMethodExecInitiator ?? (IDynamicGameObjectContext) VirtualMachine.Instance.GameRootFsm, valueStr);
    }

    public override float ReadFloatParamValue(string valueStr)
    {
      return GameComponent.ReadContextFloatParamValue(VMEngineAPIManager.LastMethodExecInitiator ?? (IDynamicGameObjectContext) VirtualMachine.Instance.GameRootFsm, valueStr);
    }

    public static int ReadContextIntParamValue(
      IDynamicGameObjectContext dynContext,
      string valueStr)
    {
      if (dynContext == null)
        dynContext = (IDynamicGameObjectContext) VirtualMachine.Instance.GameRootFsm;
      if (valueStr.Length >= 15)
      {
        string paramName = valueStr;
        IParam contextParam = dynContext.GetContextParam(paramName);
        return contextParam != null ? (int) contextParam.Value : 0;
      }
      int num = 0;
      if (valueStr != "")
      {
        try
        {
          num = StringUtility.ToInt32(valueStr);
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Read int param value error: cannot convert {0} to int value at {1}", (object) valueStr, (object) DynamicFSM.CurrentStateInfo));
        }
      }
      return num;
    }

    public static float ReadContextFloatParamValue(
      IDynamicGameObjectContext dynContext,
      string valueStr)
    {
      if (dynContext == null)
        dynContext = (IDynamicGameObjectContext) VirtualMachine.Instance.GameRootFsm;
      if (valueStr.Length >= 15)
      {
        string paramName = valueStr;
        IParam contextParam = dynContext.GetContextParam(paramName);
        return contextParam != null ? (float) contextParam.Value : 0.0f;
      }
      float num = 0.0f;
      if (valueStr != "")
      {
        try
        {
          num = StringUtility.ToSingle(valueStr);
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Read float param value error: cannot convert {0} to float value at {1}", (object) valueStr, (object) DynamicFSM.CurrentStateInfo));
        }
      }
      return num;
    }

    public static Dictionary<string, bool> GetBoolStatusesParamInfo(string objectStatusesData)
    {
      Dictionary<string, bool> statusesParamInfo = new Dictionary<string, bool>();
      if (objectStatusesData != "" && objectStatusesData != "0")
      {
        string str = objectStatusesData;
        char[] chArray = new char[1]{ ',' };
        foreach (string paramFuncInfo in str.Split(chArray))
        {
          string paramFuncName = GameComponent.ExtractParamFuncName(paramFuncInfo);
          object paramFuncParam = GameComponent.ExtractParamFuncParam(paramFuncInfo, typeof (bool));
          statusesParamInfo.Add(paramFuncName, (bool) paramFuncParam);
        }
      }
      return statusesParamInfo;
    }

    private bool CheckObjectEngineComponentTag(VMEntity entity, OperationMultiTagsInfo tagsInfo)
    {
      if (tagsInfo.TagsList.Count <= 0)
        return true;
      VMTags componentByName = (VMTags) entity.GetComponentByName("TagsComponent");
      if (componentByName == null)
        return false;
      List<string> tagsList = componentByName.TagsList;
      return tagsInfo.CheckTags(tagsList);
    }

    private bool CheckObjectCustomTag(VMEntity entity, OperationMultiTagsInfo tagsInfo)
    {
      if (tagsInfo.TagsList.Count <= 0)
        return true;
      if (entity.GetComponentByName("Common") == null)
        return false;
      string customTag = entity.GetCustomTag();
      return tagsInfo.CheckTag(customTag);
    }
  }
}
