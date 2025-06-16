using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Components.Movable;
using Engine.Common.Services;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Data;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.Dynamic.Components;
using PLVirtualMachine.Objects;
using PLVirtualMachine.Time;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace PLVirtualMachine
{
  public class VirtualMachine : IVirtualMachine
  {
    private Queue<DynamicFSM> dynamicFSMObjectsQueue = new Queue<DynamicFSM>();
    private CSDequeue<RaisedEventInfo> processingEventsFSMDeQueue = new CSDequeue<RaisedEventInfo>();
    private CSDequeue<RaisedEventInfo> processingEventsHotFSMDeQueue = new CSDequeue<RaisedEventInfo>();
    private bool isStaticHierarchyBuilded;
    private bool isWorldLoaded;
    private bool isStartGame;
    private float MainEventQueueTimeMaxPerFrame;
    private const int iterationProcessObjectsCount = 100;
    private static string fatalErrorText = "";
    public static float LOADING_UPD_INTERVAL = 0.2f;

    public VirtualMachine()
    {
      VirtualMachine.Instance = this;
      IVariableService.Initialize((IVariableService) new VMVariableService());
    }

    public static VirtualMachine Instance { get; private set; }

    public bool IsInitialized { get; private set; }

    public bool IsLoaded { get; private set; }

    public bool OnSaveLoaded { get; private set; }

    public bool IsDataLoaded { get; private set; }

    public DynamicFSM GameRootFsm { get; private set; }

    public VMEntity GameRootEntity { get; private set; }

    public VMEntity WorldHierarchyRootEntity { get; private set; }

    public IEnumerable<VMEntity> WorldHierarchyEntitiesForSave
    {
      get
      {
        yield return this.WorldHierarchyRootEntity;
        List<VMBaseEntity> allChildEntities = this.WorldHierarchyRootEntity.GetAllChildEntities();
        if (allChildEntities != null)
        {
          foreach (VMEntity vmEntity in allChildEntities)
            yield return vmEntity;
        }
      }
    }

    public IEnumerable<VMEntity> GameEntitiesForSave
    {
      get
      {
        yield return this.GameRootEntity;
        if (this.GameRootEntity.GetAllChildEntities() != null)
        {
          foreach (VMEntity allChildEntity in this.GameRootEntity.GetAllChildEntities())
            yield return allChildEntity;
        }
      }
    }

    public int DynamicFSMObjectsCount => this.dynamicFSMObjectsQueue.Count;

    public IEnumerator Initialize(bool debug, float maxEventQueueTimePerFrame = 0.0f)
    {
      DebugUtility.IsDebug = debug;
      DebugUtility.Clear();
      VMEngineAPIManager.Start();
      DelayTimer.Begin(TimeSpan.FromSeconds((double) VirtualMachine.LOADING_UPD_INTERVAL));
      this.IsInitialized = true;
      this.MainEventQueueTimeMaxPerFrame = maxEventQueueTimePerFrame;
      yield break;
    }

    public void Update(TimeSpan delta)
    {
      try
      {
        if (VirtualMachine.fatalErrorText.Length > 1)
        {
          Logger.AddError(VirtualMachine.fatalErrorText);
          throw new Exception(VirtualMachine.fatalErrorText);
        }
        if (!this.IsLoaded)
          return;
        this.UpdateInternal(delta);
      }
      catch (Exception ex)
      {
        Logger.AddError(ex.ToString() + " at " + DynamicFSM.CurrentStateInfo);
      }
    }

    public void Clear()
    {
      VirtualMachine.fatalErrorText = "";
      this.ClearHierarhy();
      GlobalVariableUtility.Clear();
      DebugUtility.Clear();
      this.IsLoaded = false;
      this.isWorldLoaded = false;
      ExpressionUtility.Clear();
      GameTimeManager.ClearContexts();
      DynamicMindMap.Clear();
      AssyncProcessManager.Clear();
      VMEngineAPIManager.OnRestart();
      WorldEntityUtility.Clear();
      this.dynamicFSMObjectsQueue.Clear();
      this.processingEventsFSMDeQueue.Clear();
      this.ResetRoot();
      this.GameRootFsm = (DynamicFSM) null;
      this.GameRootEntity = (VMEntity) null;
      this.WorldHierarchyRootEntity = (VMEntity) null;
      DynamicFSM.ClearAll();
      VMSpeaking.ClearAll();
    }

    public void UpdateInternal(TimeSpan delta)
    {
      DebugUtility.Update();
      if (this.OnSaveLoaded)
      {
        this.WorldHierarchyRootEntity.TestHierarchyConsistency();
        this.GameRootFsm.RaiseEventByName(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_LOAD_GAME, typeof (VMGameComponent), true), EEventRaisingMode.ERM_ATONCE);
        this.OnSaveLoaded = false;
      }
      this.UpdateFSM();
      this.UpdateFSMEventsRT();
      AssyncProcessManager.Update(delta);
      GameTimeManager.Update(delta);
      DynamicMindMap.Update();
    }

    public void AddFSMToProcessingEvents(RaisedEventInfo eventInfo)
    {
      if (eventInfo.Instance == null)
        return;
      if (ServiceCache.OptimizationService.IsUnity)
        eventInfo.MakeHashHistory(this.CurrentEventInfo);
      if (this.MainEventsQueueProcessingMode)
        this.processingEventsHotFSMDeQueue.PushBack(eventInfo);
      else
        this.processingEventsFSMDeQueue.PushBack(eventInfo);
    }

    private IEnumerator UpdateFSMEvents()
    {
      if (!DynamicTalkingFSM.IsTalking && !this.processingEventsFSMDeQueue.Empty())
      {
        this.MainEventsQueueProcessingMode = true;
        long startMainEventProcTicks = Stopwatch.GetTimestamp();
        do
        {
          RaisedEventInfo eventInfo = this.processingEventsFSMDeQueue.PopFront();
          if (eventInfo != null && eventInfo.Instance != null)
          {
            this.CurrentEventInfo = eventInfo;
            eventInfo.OwnerFSM.ProcessEvent(eventInfo);
            if (!this.processingEventsHotFSMDeQueue.Empty())
            {
              this.processingEventsFSMDeQueue.MergeFront(this.processingEventsHotFSMDeQueue);
              this.processingEventsHotFSMDeQueue.Clear();
            }
            this.CurrentEventInfo = (RaisedEventInfo) null;
            if (DelayTimer.Check)
              yield return (object) null;
          }
          else
            break;
        }
        while (((double) this.MainEventQueueTimeMaxPerFrame <= 9.9999999747524271E-07 || (double) (Stopwatch.GetTimestamp() - startMainEventProcTicks) / (double) Stopwatch.Frequency < (double) this.MainEventQueueTimeMaxPerFrame) && !this.processingEventsFSMDeQueue.Empty());
        this.MainEventsQueueProcessingMode = false;
      }
    }

    private void UpdateFSMEventsRT()
    {
      if (DynamicTalkingFSM.IsTalking || this.processingEventsFSMDeQueue.Empty())
        return;
      this.MainEventsQueueProcessingMode = true;
      do
      {
        RaisedEventInfo eventInfo = this.processingEventsFSMDeQueue.PopFront();
        if (eventInfo != null && eventInfo.Instance != null)
        {
          this.CurrentEventInfo = eventInfo;
          eventInfo.OwnerFSM.ProcessEvent(eventInfo);
          if (!this.processingEventsHotFSMDeQueue.Empty())
          {
            this.processingEventsFSMDeQueue.MergeFront(this.processingEventsHotFSMDeQueue);
            this.processingEventsHotFSMDeQueue.Clear();
          }
          this.CurrentEventInfo = (RaisedEventInfo) null;
        }
        else
          break;
      }
      while (!this.processingEventsFSMDeQueue.Empty());
      this.MainEventsQueueProcessingMode = false;
    }

    private void UpdateFSM()
    {
      if (this.dynamicFSMObjectsQueue.Count <= 0)
        return;
      int num = 0;
      int count = this.dynamicFSMObjectsQueue.Count;
      for (int index = 0; index < count; ++index)
      {
        DynamicFSM dynamicFsm = this.dynamicFSMObjectsQueue.Dequeue();
        dynamicFsm.Think();
        this.dynamicFSMObjectsQueue.Enqueue(dynamicFsm);
        if (dynamicFsm.Active)
          ++num;
        if (num >= 100)
          break;
      }
    }

    private IEnumerator DoStartGame()
    {
      if (this.GameRootFsm == null)
      {
        Logger.AddError(string.Format("Game root fsm not inited!!!"));
      }
      else
      {
        this.isStartGame = false;
        this.GameRootFsm.Think();
        yield return (object) null;
        yield return (object) this.UpdateFSMEvents();
        yield return (object) null;
        int iTerationsCount = this.dynamicFSMObjectsQueue.Count;
        for (int i = 0; i < iTerationsCount; ++i)
        {
          DynamicFSM dynamicFsm = this.dynamicFSMObjectsQueue.Dequeue();
          if (dynamicFsm.FSMStaticObject.Static && dynamicFsm.FSMStaticObject.GetCategory() != EObjectCategory.OBJECT_CATEGORY_GAME)
            dynamicFsm.Think();
          this.dynamicFSMObjectsQueue.Enqueue(dynamicFsm);
          if (DelayTimer.Check)
            yield return (object) null;
        }
        yield return (object) this.UpdateFSMEvents();
        yield return (object) null;
        this.GameRootFsm.RaiseEventByName(((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetStartGameEventFuncName());
        yield return (object) this.UpdateFSMEvents();
      }
    }

    public void SpawnObjectToWorld(VMEntity spawnEntity, IEntity spawnMilestone, AreaEnum areaType = AreaEnum.Unknown)
    {
      if (!spawnEntity.Instantiated)
        return;
      if (spawnMilestone != null)
        spawnEntity.SetPosition(spawnMilestone, areaType);
      if (spawnEntity.GetFSM() == null)
        Logger.AddError(string.Format("Cannot spawn object '{0}' to world, spawning object dynamic fsm with guid={1} not found in vm!!!", (object) spawnEntity.Name, (object) spawnEntity.EngineGuid));
      else
        spawnEntity.GetFSM().Active = true;
    }

    public void SpawnObject(
      VMEntity instObjEntity,
      IEntity spawnMilestoneRealEntity,
      AreaEnum areaType = AreaEnum.Unknown)
    {
      if (spawnMilestoneRealEntity != null)
      {
        if (instObjEntity.IsPlayerControllable(true))
        {
          GameTimeManager.MakePlayCharacterEntity(instObjEntity);
          instObjEntity.SetPosition(spawnMilestoneRealEntity, AreaEnum.Unknown);
        }
        this.SpawnObjectToWorld(instObjEntity, spawnMilestoneRealEntity, areaType);
      }
      else
        this.SpawnObjectToWorld(instObjEntity, spawnMilestoneRealEntity, areaType);
    }

    public VMEntity CreateRoot(VMLogicObject logicObj, VMBaseEntity parentEntity)
    {
      if (logicObj.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GAME)
      {
        if (this.GameRootFsm == null)
        {
          VMEntity entity = new VMEntity();
          Guid engineGuid = ((VMGameRoot) logicObj).EngineGuid;
          entity.Initialize((ILogicObject) logicObj, engineGuid);
          entity.OnCreate(false);
          this.GameRootFsm = new DynamicFSM(entity, logicObj);
          return entity;
        }
        Logger.AddError("Root object already created !");
        return (VMEntity) null;
      }
      Logger.AddError("Is not : " + (object) EObjectCategory.OBJECT_CATEGORY_GAME);
      return (VMEntity) null;
    }

    public void RemoveDynamicObject(Guid remObjId)
    {
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(remObjId);
      if (entityByEngineGuid == null)
      {
        if (EngineAPIManager.ObjectCreationExtraDebugInfoMode)
          Logger.AddError(string.Format("Cannot remove object: dynamic object with guid = {0} not exist at {1}", (object) remObjId, (object) DynamicFSM.CurrentStateInfo));
        else
          Logger.AddWarning(string.Format("Cannot remove object: dynamic object with guid = {0} not exist at {1}", (object) remObjId, (object) DynamicFSM.CurrentStateInfo));
      }
      else
      {
        if (entityByEngineGuid.FSMExist)
          entityByEngineGuid.GetFSM().Active = false;
        entityByEngineGuid.Remove();
        if (entityByEngineGuid.EditorTemplate.Static)
          return;
        GlobalVariableUtility.RemoveInstanceFromGlobalTemplatedLists(entityByEngineGuid.EditorTemplate, remObjId);
      }
    }

    public IBlueprint GetVirtualObjectBaseTemplate(VMEntity virtualObjEntity)
    {
      Guid baseTemplateGuid = this.GetVirtualObjectBaseTemplateGuid(virtualObjEntity);
      return baseTemplateGuid == Guid.Empty ? (IBlueprint) null : (IBlueprint) ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(baseTemplateGuid);
    }

    public Guid GetVirtualObjectBaseTemplateGuid(VMEntity virtualObjEntity)
    {
      if (!virtualObjEntity.IsWorldEntity)
        return Guid.Empty;
      IEntity instance = virtualObjEntity.Instance;
      if (instance == null)
        return Guid.Empty;
      IEntity baseTemplate = HierarchyManager.GetBaseTemplate(instance);
      return baseTemplate == null ? Guid.Empty : baseTemplate.Id;
    }

    public void RegisterDynamicObject(VMEntity entity, ILogicObject templateObj)
    {
      WorldEntityUtility.AddDynamicObjectEntityByEngineGuid(entity.EngineGuid, entity);
      WorldEntityUtility.AddDynamicGuidsToStaticWorldTemplate(entity.EngineGuid, templateObj);
      if (templateObj.Static)
      {
        if (typeof (IWorldHierarchyObject).IsAssignableFrom(templateObj.GetType()))
        {
          IWorldHierarchyObject worldHierarchyObject = (IWorldHierarchyObject) templateObj;
          if (!worldHierarchyObject.HierarchyGuid.IsEmpty)
            WorldEntityUtility.AddDynamicObjectEntityByHierarchyGuid(worldHierarchyObject.HierarchyGuid, entity);
        }
        else
          WorldEntityUtility.AddDynamicObjectEntityByStaticGuid(templateObj.Blueprint.BaseGuid, entity);
      }
      GlobalVariableUtility.RegistrInGlobalVariables(entity);
    }

    public void RegisterActiveFSM(DynamicFSM fsm) => this.dynamicFSMObjectsQueue.Enqueue(fsm);

    public void Terminate()
    {
    }

    private bool CheckWorldLoaded()
    {
      if (this.isWorldLoaded)
        return true;
      this.isWorldLoaded = true;
      ((GameComponent) VMGameComponent.Instance).StartGameTime();
      return this.isWorldLoaded;
    }

    public IEnumerator Load()
    {
      this.ForceCreateFSM = !ServiceCache.OptimizationService.LazyFsm;
      ILoadProgress progress = ServiceLocator.GetService<ILoadProgress>();
      DebugUtility.Init();
      if (this.IsLoaded)
        this.Clear();
      this.ResetRoot();
      this.GameRootEntity = this.CreateRoot((VMLogicObject) IStaticDataContainer.StaticDataContainer.GameRoot, (VMBaseEntity) null);
      progress?.OnBeforeCreateHierarchy();
      yield return (object) null;
      this.WorldHierarchyRootEntity = (VMEntity) null;
      yield return (object) this.LoadVMWorldHierarchy();
      progress?.OnAfterCreateHierarchy();
      yield return (object) null;
      foreach (IObjRef hierarchyStaticObject in ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetNotHierarchyStaticObjects())
      {
        IBlueprint templateObject = hierarchyStaticObject.Object;
        if (templateObject == null)
          Logger.AddError("Static object is null, id : " + (object) hierarchyStaticObject.BaseGuid);
        else if (!(templateObject is IGameRoot))
          yield return (object) CreateEntityUtility.CreateObject(templateObject, (VMBaseEntity) null);
      }
      this.GameRootEntity.AfterCreate();
      yield return (object) null;
      GameTimeManager.Start();
      this.IsLoaded = true;
      this.isStartGame = true;
      this.CheckWorldLoaded();
      yield return (object) this.DoStartGame();
      progress?.OnLoadComplete();
    }

    public void Save(IDataWriter writer)
    {
      if (this.processingEventsFSMDeQueue.Count() > 0)
      {
        List<RaisedEventInfo> toList = this.processingEventsFSMDeQueue.ToList;
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        for (int index = 0; index < toList.Count; ++index)
        {
          string name = toList[index].Instance.Name;
          if (dictionary.ContainsKey(name))
            ++dictionary[name];
          else
            dictionary.Add(name, 1);
        }
        string str1 = "";
        foreach (KeyValuePair<string, int> keyValuePair in dictionary)
        {
          if (str1.Length > 0)
            str1 += ", ";
          string str2 = keyValuePair.Key + " (" + keyValuePair.Value.ToString() + ")";
          str1 += str2;
        }
        Logger.AddError(string.Format("SaveLoad error: main vm events queue isn't empty at start save ! Events: {0}", (object) str1));
      }
      SaveManagerUtility.SaveDynamicSerializableList<VMEntity>(writer, "HierarchyObjects", this.WorldHierarchyEntitiesForSave);
      SaveManagerUtility.SaveDynamicSerializableList<VMEntity>(writer, "GameObjects", this.GameEntitiesForSave);
      SaveManagerUtility.SaveDynamicSerializableList<FreeEntityInfo>(writer, "FreeEntityList", VMEntity.GetFreeEntities());
      GameTimeManager.StateSave(writer);
      DynamicMindMap.SaveMindMapsToXML(writer);
    }

    public IEnumerator Load(XmlElement element)
    {
      this.ForceCreateFSM = !ServiceCache.OptimizationService.LazyFsm;
      ILoadProgress progress = ServiceLocator.GetService<ILoadProgress>();
      DelayTimer.Begin(TimeSpan.FromSeconds(3.0));
      DebugUtility.Init();
      if (this.IsLoaded)
        this.Clear();
      progress?.OnBeforeCreateHierarchy();
      yield return (object) null;
      yield return (object) this.LoadVMWorldHierarchy();
      progress?.OnAfterCreateHierarchy();
      yield return (object) null;
      XmlElement hierarchyObjectsNode = (XmlElement) element.FirstChild;
      VMSaveLoadManagerUtility.LoadEntities(hierarchyObjectsNode);
      yield return (object) null;
      this.GameRootEntity = this.CreateRoot((VMLogicObject) IStaticDataContainer.StaticDataContainer.GameRoot, (VMBaseEntity) null);
      XmlElement notHierarchyObjectsNode = (XmlElement) hierarchyObjectsNode.NextSibling;
      VMSaveLoadManagerUtility.LoadEntities(notHierarchyObjectsNode);
      yield return (object) null;
      XmlElement freeEntityesNode = (XmlElement) notHierarchyObjectsNode.NextSibling;
      List<FreeEntityInfo> freeEntityInfoList = new List<FreeEntityInfo>();
      VMSaveLoadManager.LoadDynamiSerializableList<FreeEntityInfo>(freeEntityesNode, freeEntityInfoList);
      VMEntity.LoadFreeEntities(freeEntityInfoList);
      yield return (object) null;
      XmlElement vmTimeManagerNode = (XmlElement) freeEntityesNode.NextSibling;
      GameTimeManager.LoadFromXML(vmTimeManagerNode);
      this.WorldHierarchyRootEntity.AfterSaveLoading();
      yield return (object) null;
      this.GameRootEntity.AfterSaveLoading();
      yield return (object) null;
      DynamicMindMap.LoadMindMapsFromXML((XmlElement) vmTimeManagerNode.NextSibling);
      yield return (object) null;
      this.processingEventsFSMDeQueue.Clear();
      VMSaveLoadManagerUtility.Clear();
      this.IsLoaded = true;
      this.OnSaveLoaded = true;
      progress?.OnLoadComplete();
    }

    public void Unload() => this.Clear();

    private IEnumerator LoadVMWorldHierarchy()
    {
      Queue<HierachyInfo> infos = new Queue<HierachyInfo>();
      this.WorldHierarchyRootEntity = (VMEntity) VirtualMachine.CreateVMHierarchy(HierarchyManager.GameHierarchyRoot, (VMBaseEntity) null, infos);
      while (infos.Count != 0)
      {
        HierachyInfo hierachyInfo = infos.Dequeue();
        VirtualMachine.CreateVMHierarchy((IWorldHierarchyObject) hierachyInfo.HierarchyTemplateObject, hierachyInfo.ParentVMEntity, infos);
        if (DelayTimer.Check)
          yield return (object) null;
      }
      this.WorldHierarchyRootEntity.AfterCreate();
    }

    public static VMBaseEntity CreateVMHierarchy(
      IWorldHierarchyObject hierarchyTemplateObject,
      VMBaseEntity parentVMEntity,
      Queue<HierachyInfo> infos)
    {
      VMBaseEntity vmHierarchy = CreateHierarchyEntityUtility.CreateObject(hierarchyTemplateObject, parentVMEntity);
      foreach (IHierarchyObject hierarchyChild in hierarchyTemplateObject.HierarchyChilds)
        infos.Enqueue(new HierachyInfo()
        {
          HierarchyTemplateObject = hierarchyChild,
          ParentVMEntity = vmHierarchy
        });
      return vmHierarchy;
    }

    public static void SetFatalError(string fatalErrorText)
    {
      Logger.AddError(fatalErrorText);
      VirtualMachine.fatalErrorText = fatalErrorText;
    }

    private void ClearHierarhy()
    {
      this.WorldHierarchyRootEntity.DisposeInstance(true);
      this.GameRootEntity.DisposeInstance(true);
    }

    private void BuildStaticHierarchy()
    {
      if (this.isStaticHierarchyBuilded)
        return;
      HierarchyManager.BuildStaticHierarchy();
      this.isStaticHierarchyBuilded = true;
    }

    private void ResetRoot()
    {
      this.GameRootFsm = (DynamicFSM) null;
      VMEntityUtility.ResetRoot();
    }

    private bool MainEventsQueueProcessingMode { get; set; }

    private RaisedEventInfo CurrentEventInfo { get; set; }

    public IEnumerator LoadData(string dataFolderName, int threadCount, int dataCapacity)
    {
      if (!Directory.Exists(dataFolderName))
      {
        Logger.AddError("Data folder not found : " + dataFolderName);
      }
      else
      {
        ILoadProgress progress = ServiceLocator.GetService<ILoadProgress>();
        DelayTimer.Begin(TimeSpan.FromSeconds(1.0));
        progress?.OnBeforeLoadData();
        yield return (object) XMLDataLoader.Instance.LoadDataFromXML(dataFolderName, threadCount, dataCapacity);
        progress?.OnAfterLoadData();
        yield return (object) null;
        GameTimeManager.Init();
        foreach (KeyValuePair<string, IGameMode> gameMode in ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GameModes)
          GameTimeManager.CreateGameTimeContext(gameMode.Value);
        yield return (object) null;
        this.BuildStaticHierarchy();
        progress?.OnBuildHierarchy();
        this.IsDataLoaded = true;
        progress?.OnLoadDataComplete();
        yield return (object) null;
      }
    }

    public void UnloadData()
    {
      WorldEntityUtility.ClearStatic();
      this.isStaticHierarchyBuilded = false;
      XMLDataLoader.Instance.Clear();
      VMTypePool.Clear();
      this.IsDataLoaded = false;
      HierarchyManager.Clear();
      GameTimeManager.Clear();
      GlobalVariableUtility.ClearAll();
      ExpressionUtility.Clear();
      DynamicParameterUtility.Clear();
      DynamicEvent.StaticClear();
      VMEngineAPIManager.Clear();
      WorldEntityUtility.Clear();
      VMStorable.ClearAll();
    }

    public bool ForceCreateFSM { get; private set; }
  }
}
