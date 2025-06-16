using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Common.Types;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("GameComponent", null)]
  public class VMGameComponent : VMComponent
  {
    public const string ComponentName = "GameComponent";
    protected static VMGameComponent instance;

    public override void Initialize(VMBaseEntity parent) => base.Initialize(parent);

    [Event("Timer tick event", "timerId")]
    [SpecialEvent(ESpecialEventName.SEN_ON_GLOBAL_TIMER)]
    public event Action<ulong> OnTimer;

    [Event("Sleep finish event", "stateId")]
    [SpecialEvent(ESpecialEventName.SEN_ON_LOCAL_TIMER)]
    public event Action<ulong> OnEndStateSleep;

    [Event("Cutscene finish event", "")]
    public event Action OnEndCutsceneEvent;

    [Event("OnStart", "")]
    [SpecialEvent(ESpecialEventName.SEN_START_GAME)]
    public event Action OnStartGame;

    [Event("OnLoad", "")]
    [SpecialEvent(ESpecialEventName.SEN_LOAD_GAME)]
    public event Action OnLoadGame;

    [Event("Game mode change event", "")]
    public event Action OnGameModeChanged;

    [Event("Region disease level changed", "Region:Region, level of disease")]
    public event Action<IEntity, int> OnRegionDiseaseLevelChangedEvent;

    [Event("Region reputation changed", "Region:Region, reputation")]
    public event Action<IEntity, float> OnRegionReputationChangedEvent;

    [Event("Common logic event", "value")]
    public event Action<string> OnCommonConsoleEvent;

    [Event("Logic event with data", "name,value")]
    public event Action<string, string> OnValueLogicEvent;

    [Event("Logic event with template entity", "name,template entity")]
    public event VMGameComponent.OnEntityEventHandler OnTemplateEntityLogicEvent;

    [Event("Logic event with entity", "name,entity")]
    public event Action<string, IEntity> OnEntityLogicEvent;

    [Event("Furniture loaded once", "Entity, Region:Region, BuildingEnum, DiseasedStateEnum")]
    public event Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> OnFurnitureLoadedOnce;

    [Event("Furniture loaded", "Entity, Region:Region, BuildingEnum, DiseasedStateEnum")]
    public event Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> OnFurnitureLoaded;

    [Event("Region loaded once", "Region:Region")]
    public event Action<IEntity> OnRegionLoadedOnce;

    [Event("Region loaded", "Region:Region")]
    public event Action<IEntity> OnRegionLoaded;

    [Event("Need create drop bag event", "template object", false)]
    public event VMGameComponent.NeedCreateDropBagEventType NeedCreateDropBagEvent;

    [Event("Need delete drop bag event", "object", false)]
    public event Action<IEntity> NeedDeleteDropBagEvent;

    [Method("Add drop bag entity", "Target", "")]
    public void AddDropBagEntity(IEntity entity)
    {
      if (entity == null)
      {
        Logger.AddError(string.Format("Cannot add drop bag entity, because entity is null at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
      else
      {
        Logger.AddWarning(string.Format("Process create drop bag entity {0} at {1}", (object) entity.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        try
        {
          ServiceLocator.GetService<IDropBagService>().AddEntity(entity);
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Cannot add drop bag entity, error: {0} at {1}", (object) ex.ToString(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        }
      }
    }

    [Method("Reset drop bag", "", "")]
    public void ResetDropBags() => ServiceLocator.GetService<IDropBagService>().Reset();

    [Method("Block Map Interface", "Block", "")]
    public void BlockMapInterface(bool block)
    {
      ServiceLocator.GetService<IInterfaceBlockingService>().BlockMapInterface = block;
    }

    [Method("Block MindMap Interface", "Block", "")]
    public void BlockMindMapInterface(bool block)
    {
      ServiceLocator.GetService<IInterfaceBlockingService>().BlockMindMapInterface = block;
    }

    [Method("Block Inventory Interface", "Block", "")]
    public void BlockInventoryInterface(bool block)
    {
      ServiceLocator.GetService<IInterfaceBlockingService>().BlockInventoryInterface = block;
    }

    [Method("Block Stats Interface", "Block", "")]
    public void BlockStatsInterface(bool block)
    {
      ServiceLocator.GetService<IInterfaceBlockingService>().BlockStatsInterface = block;
    }

    [Method("Block Bounds Interface", "Block", "")]
    public void BlockBoundsInterface(bool block)
    {
      ServiceLocator.GetService<IInterfaceBlockingService>().BlockBoundsInterface = block;
    }

    [Method("Set text context int", "Tag, Value", "")]
    public void SetTextContextInt(string tag, int value)
    {
      ServiceLocator.GetService<ITextContextService>().SetInt(tag, value);
    }

    [Method("Unlock Achievement", "Id", "")]
    public void UnlockAchievement(string id)
    {
      ServiceLocator.GetService<IAchievementService>().Unlock(id);
    }

    [Method("Reset Achievement", "Id", "")]
    public void ResetAchievement(string id)
    {
      ServiceLocator.GetService<IAchievementService>().Reset(id);
    }

    [Method("Set current game mode", "game mode", "")]
    public virtual void SetCurrentGameTimeContext(IGameModeRef gameMode)
    {
    }

    [Method("Get current game mode", "", "")]
    public virtual IGameModeRef GetCurrentGameTimeContext() => (IGameModeRef) null;

    [Method("Get current game time", "", "")]
    [SpecialFunction(ESpecialFunctionName.SFN_GET_GAME_TIME)]
    public virtual GameTime GetCurrGameTime() => new GameTime();

    [Method("Get current day time", "", "")]
    [SpecialFunction(ESpecialFunctionName.SFN_GET_DAY_TIME)]
    public virtual GameTime GetCurrDayTime() => new GameTime();

    [Method("Set current game time", "New current time,Send all events", "")]
    public virtual void SetCurrGameTime(GameTime currTime, bool sendTimerEvents)
    {
    }

    [Method("Set current solar time", "New current time", "")]
    public virtual void SetCurrSolarTime(GameTime currTime)
    {
    }

    [Method("Add time", "Adding time", "")]
    public virtual void AddTime(GameTime addingTime)
    {
    }

    [Method("Set current game and solar time", "New current time,Send all events", "")]
    public virtual void SetCurrGeneralTime(GameTime currTime, bool sendTimerEvents)
    {
    }

    [Method("Set game time speed", "Game time speed factor", "")]
    public virtual void SetGameTimeSpeedFactor(float timeSpeed)
    {
    }

    [Method("Set solar time speed", "Solar time speed factor", "")]
    public virtual void SetSolarTimeSpeedFactor(float timeSpeed)
    {
    }

    [Method("Set general time speed", "Time speed factor", "")]
    public virtual void SetGeneralTimeSpeedFactor(float timeSpeed)
    {
    }

    [Method("Get game localization", "", "")]
    public virtual EVMGameLocalizationName GetGameLocalization() => EVMGameLocalizationName.english;

    [Method("Create object", "Template", "")]
    public virtual IObjRef CreateObject(IBlueprintRef staticObj) => (IObjRef) null;

    [Method("Remove object", "Object", "")]
    public virtual void RemoveObject(IObjRef remObj)
    {
    }

    [Method("Is object exist", "Object", "")]
    public virtual bool IsObjectExist(IObjRef remObj) => false;

    [Method("Remove objects group", "Removing objects", "")]
    public virtual void RemoveObjectsGroup(VMCommonList objList)
    {
    }

    [Method("Enable object", "Object,Enable", "")]
    public virtual void EnableObject(IObjRef obj, bool enable)
    {
    }

    [Method("Enable objects group", "Enabling objects,Enable", "")]
    public virtual void EnableObjectsGroup(VMCommonList objList, bool enable)
    {
    }

    [Method("Create object in point", "Template,Point", "")]
    public virtual IObjRef CreateObjectTo(IBlueprintRef staticObj, IObjRef milestone)
    {
      return (IObjRef) null;
    }

    [Method("Start timer", "Interval", "")]
    public virtual ulong StartTimer(float fInterval) => 0;

    [Method("SleepState", "Game time length", "")]
    public virtual ulong SleepState(GameTime interval) => 0;

    [Method("SleepState", "Seconds", "")]
    public virtual ulong SleepStateRT(float interval) => 0;

    [Method("Start game timer", "Game time interval", "")]
    public virtual ulong StartGameTimer(GameTime interval) => 0;

    [Method("Start loop timer", "Interval in seconds", "")]
    public virtual ulong StartLoopTimer(float interval) => 0;

    [Method("Start loop game timer", "Game time interval", "")]
    public virtual ulong StartLoopGameTimer(GameTime interval) => 0;

    [Method("Start game timer at context", "Game time interval,context", "")]
    public virtual ulong StartGameTimerAtContext(GameTime interval, IGameModeRef gameMode) => 0;

    [Method("Start loop game timer at context", "Game time interval,context", "")]
    public virtual ulong StartLoopGameTimerAtContext(GameTime interval, IGameModeRef gameMode) => 0;

    [Method("Stop timer", "Timer index", "")]
    public virtual void StopTimer(ulong timerIndex)
    {
    }

    [Method("Stop timer", "Timer index,context", "")]
    public virtual void StopTimerAtContext(ulong timerIndex, IGameModeRef gameMode)
    {
    }

    [Method("", "guids", "")]
    public virtual void ResetCustomTags(string sRootInfo)
    {
    }

    [Method("", ",,", "")]
    public virtual void SetCustomTagsDistribution(
      string storagesRootInfo,
      string storagTagDistributionInfo,
      string funcComponentName)
    {
    }

    [Method("", ",", "")]
    public virtual void ProcessCustomGroupObjectAction(string storagesRootInfo, string actionInfo)
    {
    }

    [Method("", ",,,,,", "")]
    public virtual void ProcessCustomVarGroupObjectAction(
      string storagesRootInfo,
      string funcComponentName,
      string tags,
      string customTags,
      string actionInfo,
      string operationVolume)
    {
    }

    [Method("", ",,", "")]
    public virtual void SetCustomGroupObjectStatuses(
      string storagesRootInfo,
      string funcComponentName,
      string objectStatusesData)
    {
    }

    [Method("", ",,,,,", "")]
    public virtual void SetCustomVarGroupObjectStatuses(
      string storagesRootInfo,
      string funcComponentName,
      string tags,
      string customTags,
      string objectStatusesData,
      string operationVolume)
    {
    }

    [Method("Get object template", "Object", "")]
    [SpecialFunction(ESpecialFunctionName.SFN_GET_OBJECT_CLASS)]
    public virtual IBlueprintRef GetObjectClass(IObjRef objRef) => (IBlueprintRef) null;

    [Method("Is object derived from class", "Object,class", "")]
    public virtual bool IsObjectDerivedFromClass(IObjRef objRef, IBlueprintRef classRef) => false;

    [Method("Is object derived from template", "Object,class", "")]
    public virtual bool IsObjectDerivedFromTemplate(IObjRef objRef, IBlueprintRef classRef)
    {
      return false;
    }

    [Method("Is object compatible", "Object,Type", "")]
    [SpecialFunction(ESpecialFunctionName.SFN_CHECK_OBJECT_COMPATIBILITY)]
    public virtual bool IsObjectCompatible(IObjRef objRef, VMType type) => false;

    [Method("Get nearest owner by functional", "Object,Type", "")]
    public virtual IObjRef GetNearestOwnerByFunctional(IObjRef objRef, VMType type)
    {
      return (IObjRef) null;
    }

    [Method("Get object with tag", "Object, tag", "")]
    public virtual IObjRef GetObjectWithTag(IObjRef objRef, string tag) => (IObjRef) null;

    [Method("Add Notification", "Notification, List of texts", "")]
    public void AddNotification(NotificationEnum notification, VMCommonList textList)
    {
      try
      {
        INotificationService service = ServiceLocator.GetService<INotificationService>();
        if (service == null)
          Logger.AddError(string.Format("Add notification function call error: notification service not defined"));
        else if (textList != null)
        {
          List<LocalizedText> localizedTextList = new List<LocalizedText>();
          for (int objIndex = 0; objIndex < textList.ObjectsCount; ++objIndex)
          {
            object obj = textList.GetObject(objIndex);
            if (obj != null && typeof (ITextRef).IsAssignableFrom(obj.GetType()))
            {
              LocalizedText engineTextInstance = EngineAPIManager.CreateEngineTextInstance((ITextRef) obj);
              localizedTextList.Add(engineTextInstance);
            }
          }
          service.AddNotify(notification, (object) localizedTextList);
        }
        else
          service.AddNotify(notification);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Add notification function call error: {0}", (object) ex.ToString()));
      }
    }

    [Method("Add Notification", "Notification, Object", "")]
    public void AddNotification_v1(NotificationEnum notification, IEntity target)
    {
      try
      {
        INotificationService service = ServiceLocator.GetService<INotificationService>();
        if (service == null)
          Logger.AddError(string.Format("Add notification function call error: notification service not defined"));
        else if (target != null)
          service.AddNotify(notification, (object) target);
        else
          service.AddNotify(notification);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Add notification function call error: {0}", (object) ex.ToString()));
      }
    }

    [Method("Add Notification Template", "Notification, Template", "")]
    public void AddNotificationTemplate(NotificationEnum notification, [Template] IEntity target)
    {
      try
      {
        INotificationService service = ServiceLocator.GetService<INotificationService>();
        if (service == null)
          Logger.AddError(string.Format("Add notification template function call error: notification service not defined"));
        else if (target != null)
          service.AddNotify(notification, (object) target);
        else
          service.AddNotify(notification);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Add notification template function call error: {0}", (object) ex.ToString()));
      }
    }

    [Method("Add Notification index", "Notification, index", "")]
    public void AddNotificationIndex(NotificationEnum notification, int index)
    {
      try
      {
        INotificationService service = ServiceLocator.GetService<INotificationService>();
        if (service == null)
          Logger.AddError(string.Format("Add notification index function call error: notification service not defined"));
        else
          service.AddNotify(notification, (object) index);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Add notification index function call error: {0}", (object) ex.ToString()));
      }
    }

    [Method("Block Notification Type", "Type", "")]
    public void BlockNotificationType(NotificationEnum type)
    {
      try
      {
        INotificationService service = ServiceLocator.GetService<INotificationService>();
        if (service == null)
          Logger.AddError(string.Format("Block notification type function call error: notification service not defined"));
        else
          service.BlockType(type);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Block notification type function call error: {0}", (object) ex.ToString()));
      }
    }

    [Method("Unblock Notification Type", "Type", "")]
    public void UnblockNotificationType(NotificationEnum type)
    {
      try
      {
        INotificationService service = ServiceLocator.GetService<INotificationService>();
        if (service == null)
          Logger.AddError(string.Format("Unblock notification type function call error: notification service not defined"));
        else
          service.UnblockType(type);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Unblock notification type function call error: {0}", (object) ex.ToString()));
      }
    }

    [Method("Get plague level", "Object", "")]
    public float GetPlagueLevel(IEntity obj)
    {
      if (obj == null)
      {
        Logger.AddError(string.Format("Get plague level object not defined"));
        return 0.0f;
      }
      try
      {
        return ServiceLocator.GetService<IPlagueController>().GetLevel(obj);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Get plague level error at object {0}: {1}", (object) obj.Name, (object) ex));
        return 0.0f;
      }
    }

    [Method("Add global mind map page", "", "")]
    public virtual void AddGlobalLogicMapPage(ILogicMapRef newMMPage)
    {
    }

    [Method("Add mind map page", "", "")]
    public virtual void AddLogicMapPage(ILogicMapRef newMMPage)
    {
    }

    [Method("Remove mind map page", "", "")]
    public virtual void RemoveLogicMapPage(ILogicMapRef remMMPage)
    {
    }

    [Method("Reset Spreading", "", "")]
    public void ResetSpreading() => ServiceLocator.GetService<ISpreadingService>().Reset();

    [Property("Deaths", "", false, 0, false)]
    public int Deaths
    {
      get => ServiceLocator.GetService<IProfilesService>().GetIntValue(nameof (Deaths));
      set => ServiceLocator.GetService<IProfilesService>().SetIntValue(nameof (Deaths), value);
    }

    [Property("SpokeMarkUM", "", false, false, false)]
    public bool SpokeMarkUM
    {
      get => ServiceLocator.GetService<IProfilesService>().GetBoolValue(nameof (SpokeMarkUM));
      set
      {
        ServiceLocator.GetService<IProfilesService>().SetBoolValue(nameof (SpokeMarkUM), value);
      }
    }

    [Property("MadeTheDeal", "", false, false, false)]
    public bool MadeTheDeal
    {
      get => ServiceLocator.GetService<IProfilesService>().GetBoolValue(nameof (MadeTheDeal));
      set
      {
        ServiceLocator.GetService<IProfilesService>().SetBoolValue(nameof (MadeTheDeal), value);
      }
    }

    [Property("RefusedDeal", "", false, false, false)]
    public bool RefusedDeal
    {
      get => ServiceLocator.GetService<IProfilesService>().GetBoolValue(nameof (RefusedDeal));
      set
      {
        ServiceLocator.GetService<IProfilesService>().SetBoolValue(nameof (RefusedDeal), value);
      }
    }

    [Method("Get Profile Value", "Name", "")]
    public string GetProfileValue(string name)
    {
      return ServiceLocator.GetService<IProfilesService>().GetValue(name);
    }

    [Method("Set Profile Value", "Name, Value", "")]
    public void SetProfileValue(string name, string value)
    {
      ServiceLocator.GetService<IProfilesService>().SetValue(name, value);
    }

    [Method("Get Profile Int Value", "Name", "")]
    public int GetProfileIntValue(string name)
    {
      return ServiceLocator.GetService<IProfilesService>().GetIntValue(name);
    }

    [Method("Set Profile Int Value", "Name, Value", "")]
    public void SetProfileIntValue(string name, int value)
    {
      ServiceLocator.GetService<IProfilesService>().SetIntValue(name, value);
    }

    [Method("Get Profile Bool Value", "Name", "")]
    public bool GetProfileBoolValue(string name)
    {
      return ServiceLocator.GetService<IProfilesService>().GetBoolValue(name);
    }

    [Method("Set Profile Bool Value", "Name, Value", "")]
    public void SetProfileBoolValue(string name, bool value)
    {
      ServiceLocator.GetService<IProfilesService>().SetBoolValue(name, value);
    }

    [Method("Get Profile Float Value", "Name", "")]
    public float GetProfileFloatValue(string name)
    {
      return ServiceLocator.GetService<IProfilesService>().GetFloatValue(name);
    }

    [Method("Set Profile Float Value", "Name, Value", "")]
    public void SetProfileFloatValue(string name, float value)
    {
      ServiceLocator.GetService<IProfilesService>().SetFloatValue(name, value);
    }

    [Property("Backer Item Unlocked", "", false, false, false)]
    public bool BackerItemUnlocked
    {
      get => ServiceLocator.GetService<IBackerUnlocksService>().ItemUnlocked;
      set => throw new NotImplementedException();
    }

    [Property("Backer Quest Unlocked", "", false, false, false)]
    public bool BackerQuestUnlocked
    {
      get => ServiceLocator.GetService<IBackerUnlocksService>().QuestUnlocked;
      set => throw new NotImplementedException();
    }

    [Property("Backer Polyhedral Room Unlocked", "", false, false, false)]
    public bool BackerPolyhedralRoomUnlocked
    {
      get => ServiceLocator.GetService<IBackerUnlocksService>().PolyhedralRoomUnlocked;
      set => throw new NotImplementedException();
    }

    [Property("Jerboa Amount", "", false, 0.0f, false)]
    public float JerboaAmount
    {
      get => ServiceLocator.GetService<IJerboaService>().Amount;
      set => ServiceLocator.GetService<IJerboaService>().Amount = value;
    }

    [Property("Jerboa Color", "", false, JerboaColorEnum.Default, false)]
    public JerboaColorEnum JerboaColor
    {
      get => ServiceLocator.GetService<IJerboaService>().Color;
      set => ServiceLocator.GetService<IJerboaService>().Color = value;
    }

    [Property("Herb Brown Twyre Amount", "", false, 0, false)]
    public int BrownTwyreAmount
    {
      get => ServiceLocator.GetService<ISteppeHerbService>().BrownTwyreAmount;
      set => ServiceLocator.GetService<ISteppeHerbService>().BrownTwyreAmount = value;
    }

    [Property("Herb Blood Twyre Amount", "", false, 0, false)]
    public int BloodTwyreAmount
    {
      get => ServiceLocator.GetService<ISteppeHerbService>().BloodTwyreAmount;
      set => ServiceLocator.GetService<ISteppeHerbService>().BloodTwyreAmount = value;
    }

    [Property("Herb Black Twyre Amount", "", false, 0, false)]
    public int BlackTwyreAmount
    {
      get => ServiceLocator.GetService<ISteppeHerbService>().BlackTwyreAmount;
      set => ServiceLocator.GetService<ISteppeHerbService>().BlackTwyreAmount = value;
    }

    [Property("Herb Swevery Amount", "", false, 0, false)]
    public int SweveryAmount
    {
      get => ServiceLocator.GetService<ISteppeHerbService>().SweveryAmount;
      set => ServiceLocator.GetService<ISteppeHerbService>().SweveryAmount = value;
    }

    [Method("Start blueprint", "blueprint,target", "")]
    public void StartBlueprint(IBlueprintObject blueprint, IEntity target)
    {
      ServiceLocator.GetService<IBlueprintService>().Start(blueprint, target, (Action) null);
    }

    [Method("Jerboa syncronize", "", "")]
    public void JerboaSyncronize() => ServiceLocator.GetService<IJerboaService>().Syncronize();

    [Method("Herb Reset", "", "")]
    public void HerbReset() => ServiceLocator.GetService<ISteppeHerbService>().Reset();

    public virtual void OnRegionDiseaseLevelChanged(IRegionComponent region, int level)
    {
      this.OnRegionDiseaseLevelChangedEvent(region.Owner, level);
    }

    public virtual void OnRegionReputationChanged(IRegionComponent region, float value)
    {
      this.OnRegionReputationChangedEvent(region.Owner, value);
    }

    public virtual int ReadIntParamValue(string valueStr) => 0;

    public virtual float ReadFloatParamValue(string valueStr) => 0.0f;

    public void OnGameModeChange()
    {
      Action onGameModeChanged = this.OnGameModeChanged;
      if (onGameModeChanged == null)
        return;
      onGameModeChanged();
    }

    public static VMGameComponent Instance => VMGameComponent.instance;

    private void FireEntityEvent(string name, IEntity entity)
    {
      if (entity != null)
      {
        if (entity.IsTemplate)
        {
          VMGameComponent.OnEntityEventHandler entityLogicEvent = this.OnTemplateEntityLogicEvent;
          if (entityLogicEvent == null)
            return;
          entityLogicEvent(name, entity);
        }
        else
        {
          Action<string, IEntity> entityLogicEvent = this.OnEntityLogicEvent;
          if (entityLogicEvent == null)
            return;
          entityLogicEvent(name, entity);
        }
      }
      else
        Logger.AddError(string.Format("Fire entity not defined for FireEntityEvent calling at {0}!", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
    }

    public override void OnCreate()
    {
      ILogicEventService service1 = ServiceLocator.GetService<ILogicEventService>();
      service1.OnCommonEvent += new Action<string>(this.FireCommonEvent);
      service1.OnValueEvent += new Action<string, string>(this.FireValueEvent);
      service1.OnEntityEvent += new Action<string, IEntity>(this.FireEntityEvent);
      ISpreadingService service2 = ServiceLocator.GetService<ISpreadingService>();
      service2.OnFurnitureLoadedOnce += new Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum>(this.FireFurnitureLoadedOnce);
      service2.OnFurnitureLoaded += new Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum>(this.FireFurnitureLoaded);
      service2.OnRegionLoadedOnce += new Action<IEntity>(this.FireRegionLoadedOnce);
      service2.OnRegionLoaded += new Action<IEntity>(this.FireRegionLoaded);
      IDropBagService service3 = ServiceLocator.GetService<IDropBagService>();
      service3.OnCreateEntity += new Action<IEntity>(this.DropBagService_OnCreateEntity);
      service3.OnDeleteEntity += new Action<IEntity>(this.DropBagService_OnDeleteEntity);
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      ILogicEventService service1 = ServiceLocator.GetService<ILogicEventService>();
      service1.OnCommonEvent -= new Action<string>(this.FireCommonEvent);
      service1.OnValueEvent -= new Action<string, string>(this.FireValueEvent);
      service1.OnEntityEvent -= new Action<string, IEntity>(this.FireEntityEvent);
      ISpreadingService service2 = ServiceLocator.GetService<ISpreadingService>();
      service2.OnFurnitureLoadedOnce -= new Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum>(this.FireFurnitureLoadedOnce);
      service2.OnFurnitureLoaded -= new Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum>(this.FireFurnitureLoaded);
      service2.OnRegionLoadedOnce -= new Action<IEntity>(this.FireRegionLoadedOnce);
      service2.OnRegionLoaded -= new Action<IEntity>(this.FireRegionLoaded);
      IDropBagService service3 = ServiceLocator.GetService<IDropBagService>();
      service3.OnCreateEntity -= new Action<IEntity>(this.DropBagService_OnCreateEntity);
      service3.OnDeleteEntity -= new Action<IEntity>(this.DropBagService_OnDeleteEntity);
      base.Clear();
    }

    private void FireRegionLoadedOnce(IEntity region)
    {
      Action<IEntity> regionLoadedOnce = this.OnRegionLoadedOnce;
      if (regionLoadedOnce == null)
        return;
      regionLoadedOnce(region);
    }

    private void FireRegionLoaded(IEntity region)
    {
      Action<IEntity> onRegionLoaded = this.OnRegionLoaded;
      if (onRegionLoaded == null)
        return;
      onRegionLoaded(region);
    }

    private void FireFurnitureLoadedOnce(
      IEntity entity,
      IEntity region,
      BuildingEnum building,
      DiseasedStateEnum diseased)
    {
      Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> furnitureLoadedOnce = this.OnFurnitureLoadedOnce;
      if (furnitureLoadedOnce == null)
        return;
      furnitureLoadedOnce(entity, region, building, diseased);
    }

    private void FireFurnitureLoaded(
      IEntity entity,
      IEntity region,
      BuildingEnum building,
      DiseasedStateEnum diseased)
    {
      Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> onFurnitureLoaded = this.OnFurnitureLoaded;
      if (onFurnitureLoaded == null)
        return;
      onFurnitureLoaded(entity, region, building, diseased);
    }

    private void FireValueEvent(string name, string value)
    {
      Action<string, string> onValueLogicEvent = this.OnValueLogicEvent;
      if (onValueLogicEvent == null)
        return;
      onValueLogicEvent(name, value);
    }

    private void FireCommonEvent(string name)
    {
      Action<string> commonConsoleEvent = this.OnCommonConsoleEvent;
      if (commonConsoleEvent == null)
        return;
      commonConsoleEvent(name);
    }

    private void DropBagService_OnDeleteEntity(IEntity entity)
    {
      Action<IEntity> deleteDropBagEvent = this.NeedDeleteDropBagEvent;
      if (deleteDropBagEvent == null)
        return;
      deleteDropBagEvent(entity);
    }

    private void DropBagService_OnCreateEntity(IEntity entity)
    {
      VMGameComponent.NeedCreateDropBagEventType createDropBagEvent = this.NeedCreateDropBagEvent;
      if (createDropBagEvent == null)
        return;
      createDropBagEvent(entity);
    }

    public delegate void OnEntityEventHandler(string name, [Template] IEntity entity);

    public delegate void NeedCreateDropBagEventType([Template] IEntity entity);
  }
}
