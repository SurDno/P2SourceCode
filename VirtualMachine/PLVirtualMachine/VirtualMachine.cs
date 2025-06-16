using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
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

namespace PLVirtualMachine;

public class VirtualMachine : IVirtualMachine {
	private Queue<DynamicFSM> dynamicFSMObjectsQueue = new();
	private CSDequeue<RaisedEventInfo> processingEventsFSMDeQueue = new();
	private CSDequeue<RaisedEventInfo> processingEventsHotFSMDeQueue = new();
	private bool isStaticHierarchyBuilded;
	private bool isWorldLoaded;
	private bool isStartGame;
	private float MainEventQueueTimeMaxPerFrame;
	private const int iterationProcessObjectsCount = 100;
	private static string fatalErrorText = "";
	public static float LOADING_UPD_INTERVAL = 0.2f;

	public VirtualMachine() {
		Instance = this;
		IVariableService.Initialize(new VMVariableService());
	}

	public static VirtualMachine Instance { get; private set; }

	public bool IsInitialized { get; private set; }

	public bool IsLoaded { get; private set; }

	public bool OnSaveLoaded { get; private set; }

	public bool IsDataLoaded { get; private set; }

	public DynamicFSM GameRootFsm { get; private set; }

	public VMEntity GameRootEntity { get; private set; }

	public VMEntity WorldHierarchyRootEntity { get; private set; }

	public IEnumerable<VMEntity> WorldHierarchyEntitiesForSave {
		get {
			yield return WorldHierarchyRootEntity;
			var allChildEntities = WorldHierarchyRootEntity.GetAllChildEntities();
			if (allChildEntities != null)
				foreach (VMEntity vmEntity in allChildEntities)
					yield return vmEntity;
		}
	}

	public IEnumerable<VMEntity> GameEntitiesForSave {
		get {
			yield return GameRootEntity;
			if (GameRootEntity.GetAllChildEntities() != null)
				foreach (VMEntity allChildEntity in GameRootEntity.GetAllChildEntities())
					yield return allChildEntity;
		}
	}

	public int DynamicFSMObjectsCount => dynamicFSMObjectsQueue.Count;

	public IEnumerator Initialize(bool debug, float maxEventQueueTimePerFrame = 0.0f) {
		DebugUtility.IsDebug = debug;
		DebugUtility.Clear();
		VMEngineAPIManager.Start();
		DelayTimer.Begin(TimeSpan.FromSeconds(LOADING_UPD_INTERVAL));
		IsInitialized = true;
		MainEventQueueTimeMaxPerFrame = maxEventQueueTimePerFrame;
		yield break;
	}

	public void Update(TimeSpan delta) {
		try {
			if (fatalErrorText.Length > 1) {
				Logger.AddError(fatalErrorText);
				throw new Exception(fatalErrorText);
			}

			if (!IsLoaded)
				return;
			UpdateInternal(delta);
		} catch (Exception ex) {
			Logger.AddError(ex + " at " + DynamicFSM.CurrentStateInfo);
		}
	}

	public void Clear() {
		fatalErrorText = "";
		ClearHierarhy();
		GlobalVariableUtility.Clear();
		DebugUtility.Clear();
		IsLoaded = false;
		isWorldLoaded = false;
		ExpressionUtility.Clear();
		GameTimeManager.ClearContexts();
		DynamicMindMap.Clear();
		AssyncProcessManager.Clear();
		VMEngineAPIManager.OnRestart();
		WorldEntityUtility.Clear();
		dynamicFSMObjectsQueue.Clear();
		processingEventsFSMDeQueue.Clear();
		ResetRoot();
		GameRootFsm = null;
		GameRootEntity = null;
		WorldHierarchyRootEntity = null;
		DynamicFSM.ClearAll();
		VMSpeaking.ClearAll();
	}

	public void UpdateInternal(TimeSpan delta) {
		DebugUtility.Update();
		if (OnSaveLoaded) {
			WorldHierarchyRootEntity.TestHierarchyConsistency();
			GameRootFsm.RaiseEventByName(
				EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_LOAD_GAME, typeof(VMGameComponent), true),
				EEventRaisingMode.ERM_ATONCE);
			OnSaveLoaded = false;
		}

		UpdateFSM();
		UpdateFSMEventsRT();
		AssyncProcessManager.Update(delta);
		GameTimeManager.Update(delta);
		DynamicMindMap.Update();
	}

	public void AddFSMToProcessingEvents(RaisedEventInfo eventInfo) {
		if (eventInfo.Instance == null)
			return;
		if (ServiceCache.OptimizationService.IsUnity)
			eventInfo.MakeHashHistory(CurrentEventInfo);
		if (MainEventsQueueProcessingMode)
			processingEventsHotFSMDeQueue.PushBack(eventInfo);
		else
			processingEventsFSMDeQueue.PushBack(eventInfo);
	}

	private IEnumerator UpdateFSMEvents() {
		if (!DynamicTalkingFSM.IsTalking && !processingEventsFSMDeQueue.Empty()) {
			MainEventsQueueProcessingMode = true;
			var startMainEventProcTicks = Stopwatch.GetTimestamp();
			do {
				var eventInfo = processingEventsFSMDeQueue.PopFront();
				if (eventInfo != null && eventInfo.Instance != null) {
					CurrentEventInfo = eventInfo;
					eventInfo.OwnerFSM.ProcessEvent(eventInfo);
					if (!processingEventsHotFSMDeQueue.Empty()) {
						processingEventsFSMDeQueue.MergeFront(processingEventsHotFSMDeQueue);
						processingEventsHotFSMDeQueue.Clear();
					}

					CurrentEventInfo = null;
					if (DelayTimer.Check)
						yield return null;
				} else
					break;
			} while ((MainEventQueueTimeMaxPerFrame <= 9.9999999747524271E-07 ||
			          (Stopwatch.GetTimestamp() - startMainEventProcTicks) / (double)Stopwatch.Frequency <
			          MainEventQueueTimeMaxPerFrame) && !processingEventsFSMDeQueue.Empty());

			MainEventsQueueProcessingMode = false;
		}
	}

	private void UpdateFSMEventsRT() {
		if (DynamicTalkingFSM.IsTalking || processingEventsFSMDeQueue.Empty())
			return;
		MainEventsQueueProcessingMode = true;
		do {
			var eventInfo = processingEventsFSMDeQueue.PopFront();
			if (eventInfo != null && eventInfo.Instance != null) {
				CurrentEventInfo = eventInfo;
				eventInfo.OwnerFSM.ProcessEvent(eventInfo);
				if (!processingEventsHotFSMDeQueue.Empty()) {
					processingEventsFSMDeQueue.MergeFront(processingEventsHotFSMDeQueue);
					processingEventsHotFSMDeQueue.Clear();
				}

				CurrentEventInfo = null;
			} else
				break;
		} while (!processingEventsFSMDeQueue.Empty());

		MainEventsQueueProcessingMode = false;
	}

	private void UpdateFSM() {
		if (dynamicFSMObjectsQueue.Count <= 0)
			return;
		var num = 0;
		var count = dynamicFSMObjectsQueue.Count;
		for (var index = 0; index < count; ++index) {
			var dynamicFsm = dynamicFSMObjectsQueue.Dequeue();
			dynamicFsm.Think();
			dynamicFSMObjectsQueue.Enqueue(dynamicFsm);
			if (dynamicFsm.Active)
				++num;
			if (num >= 100)
				break;
		}
	}

	private IEnumerator DoStartGame() {
		if (GameRootFsm == null)
			Logger.AddError("Game root fsm not inited!!!");
		else {
			isStartGame = false;
			GameRootFsm.Think();
			yield return null;
			yield return UpdateFSMEvents();
			yield return null;
			var iTerationsCount = dynamicFSMObjectsQueue.Count;
			for (var i = 0; i < iTerationsCount; ++i) {
				var dynamicFsm = dynamicFSMObjectsQueue.Dequeue();
				if (dynamicFsm.FSMStaticObject.Static &&
				    dynamicFsm.FSMStaticObject.GetCategory() != EObjectCategory.OBJECT_CATEGORY_GAME)
					dynamicFsm.Think();
				dynamicFSMObjectsQueue.Enqueue(dynamicFsm);
				if (DelayTimer.Check)
					yield return null;
			}

			yield return UpdateFSMEvents();
			yield return null;
			GameRootFsm.RaiseEventByName(((VMGameRoot)IStaticDataContainer.StaticDataContainer.GameRoot)
				.GetStartGameEventFuncName());
			yield return UpdateFSMEvents();
		}
	}

	public void SpawnObjectToWorld(VMEntity spawnEntity, IEntity spawnMilestone, AreaEnum areaType = AreaEnum.Unknown) {
		if (!spawnEntity.Instantiated)
			return;
		if (spawnMilestone != null)
			spawnEntity.SetPosition(spawnMilestone, areaType);
		if (spawnEntity.GetFSM() == null)
			Logger.AddError(string.Format(
				"Cannot spawn object '{0}' to world, spawning object dynamic fsm with guid={1} not found in vm!!!",
				spawnEntity.Name, spawnEntity.EngineGuid));
		else
			spawnEntity.GetFSM().Active = true;
	}

	public void SpawnObject(
		VMEntity instObjEntity,
		IEntity spawnMilestoneRealEntity,
		AreaEnum areaType = AreaEnum.Unknown) {
		if (spawnMilestoneRealEntity != null) {
			if (instObjEntity.IsPlayerControllable(true)) {
				GameTimeManager.MakePlayCharacterEntity(instObjEntity);
				instObjEntity.SetPosition(spawnMilestoneRealEntity);
			}

			SpawnObjectToWorld(instObjEntity, spawnMilestoneRealEntity, areaType);
		} else
			SpawnObjectToWorld(instObjEntity, spawnMilestoneRealEntity, areaType);
	}

	public VMEntity CreateRoot(VMLogicObject logicObj, VMBaseEntity parentEntity) {
		if (logicObj.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GAME) {
			if (GameRootFsm == null) {
				var entity = new VMEntity();
				var engineGuid = ((VMGameRoot)logicObj).EngineGuid;
				entity.Initialize(logicObj, engineGuid);
				entity.OnCreate();
				GameRootFsm = new DynamicFSM(entity, logicObj);
				return entity;
			}

			Logger.AddError("Root object already created !");
			return null;
		}

		Logger.AddError("Is not : " + EObjectCategory.OBJECT_CATEGORY_GAME);
		return null;
	}

	public void RemoveDynamicObject(Guid remObjId) {
		var entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(remObjId);
		if (entityByEngineGuid == null) {
			if (EngineAPIManager.ObjectCreationExtraDebugInfoMode)
				Logger.AddError(string.Format("Cannot remove object: dynamic object with guid = {0} not exist at {1}",
					remObjId, DynamicFSM.CurrentStateInfo));
			else
				Logger.AddWarning(string.Format("Cannot remove object: dynamic object with guid = {0} not exist at {1}",
					remObjId, DynamicFSM.CurrentStateInfo));
		} else {
			if (entityByEngineGuid.FSMExist)
				entityByEngineGuid.GetFSM().Active = false;
			entityByEngineGuid.Remove();
			if (entityByEngineGuid.EditorTemplate.Static)
				return;
			GlobalVariableUtility.RemoveInstanceFromGlobalTemplatedLists(entityByEngineGuid.EditorTemplate, remObjId);
		}
	}

	public IBlueprint GetVirtualObjectBaseTemplate(VMEntity virtualObjEntity) {
		var baseTemplateGuid = GetVirtualObjectBaseTemplateGuid(virtualObjEntity);
		return baseTemplateGuid == Guid.Empty
			? null
			: (IBlueprint)((VMGameRoot)IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(
				baseTemplateGuid);
	}

	public Guid GetVirtualObjectBaseTemplateGuid(VMEntity virtualObjEntity) {
		if (!virtualObjEntity.IsWorldEntity)
			return Guid.Empty;
		var instance = virtualObjEntity.Instance;
		if (instance == null)
			return Guid.Empty;
		var baseTemplate = HierarchyManager.GetBaseTemplate(instance);
		return baseTemplate == null ? Guid.Empty : baseTemplate.Id;
	}

	public void RegisterDynamicObject(VMEntity entity, ILogicObject templateObj) {
		WorldEntityUtility.AddDynamicObjectEntityByEngineGuid(entity.EngineGuid, entity);
		WorldEntityUtility.AddDynamicGuidsToStaticWorldTemplate(entity.EngineGuid, templateObj);
		if (templateObj.Static) {
			if (typeof(IWorldHierarchyObject).IsAssignableFrom(templateObj.GetType())) {
				var worldHierarchyObject = (IWorldHierarchyObject)templateObj;
				if (!worldHierarchyObject.HierarchyGuid.IsEmpty)
					WorldEntityUtility.AddDynamicObjectEntityByHierarchyGuid(worldHierarchyObject.HierarchyGuid,
						entity);
			} else
				WorldEntityUtility.AddDynamicObjectEntityByStaticGuid(templateObj.Blueprint.BaseGuid, entity);
		}

		GlobalVariableUtility.RegistrInGlobalVariables(entity);
	}

	public void RegisterActiveFSM(DynamicFSM fsm) {
		dynamicFSMObjectsQueue.Enqueue(fsm);
	}

	public void Terminate() { }

	private bool CheckWorldLoaded() {
		if (isWorldLoaded)
			return true;
		isWorldLoaded = true;
		((GameComponent)VMGameComponent.Instance).StartGameTime();
		return isWorldLoaded;
	}

	public IEnumerator Load() {
		ForceCreateFSM = !ServiceCache.OptimizationService.LazyFsm;
		var progress = ServiceLocator.GetService<ILoadProgress>();
		DebugUtility.Init();
		if (IsLoaded)
			Clear();
		ResetRoot();
		GameRootEntity = CreateRoot((VMLogicObject)IStaticDataContainer.StaticDataContainer.GameRoot, null);
		progress?.OnBeforeCreateHierarchy();
		yield return null;
		WorldHierarchyRootEntity = null;
		yield return LoadVMWorldHierarchy();
		progress?.OnAfterCreateHierarchy();
		yield return null;
		foreach (var hierarchyStaticObject in ((VMGameRoot)IStaticDataContainer.StaticDataContainer.GameRoot)
		         .GetNotHierarchyStaticObjects()) {
			var templateObject = hierarchyStaticObject.Object;
			if (templateObject == null)
				Logger.AddError("Static object is null, id : " + hierarchyStaticObject.BaseGuid);
			else if (!(templateObject is IGameRoot))
				yield return CreateEntityUtility.CreateObject(templateObject, null);
		}

		GameRootEntity.AfterCreate();
		yield return null;
		GameTimeManager.Start();
		IsLoaded = true;
		isStartGame = true;
		CheckWorldLoaded();
		yield return DoStartGame();
		progress?.OnLoadComplete();
	}

	public void Save(IDataWriter writer) {
		if (processingEventsFSMDeQueue.Count() > 0) {
			var toList = processingEventsFSMDeQueue.ToList;
			var dictionary = new Dictionary<string, int>();
			for (var index = 0; index < toList.Count; ++index) {
				var name = toList[index].Instance.Name;
				if (dictionary.ContainsKey(name))
					++dictionary[name];
				else
					dictionary.Add(name, 1);
			}

			var str1 = "";
			foreach (var keyValuePair in dictionary) {
				if (str1.Length > 0)
					str1 += ", ";
				var str2 = keyValuePair.Key + " (" + keyValuePair.Value + ")";
				str1 += str2;
			}

			Logger.AddError(
				string.Format("SaveLoad error: main vm events queue isn't empty at start save ! Events: {0}", str1));
		}

		SaveManagerUtility.SaveDynamicSerializableList(writer, "HierarchyObjects", WorldHierarchyEntitiesForSave);
		SaveManagerUtility.SaveDynamicSerializableList(writer, "GameObjects", GameEntitiesForSave);
		SaveManagerUtility.SaveDynamicSerializableList(writer, "FreeEntityList", VMEntity.GetFreeEntities());
		GameTimeManager.StateSave(writer);
		DynamicMindMap.SaveMindMapsToXML(writer);
	}

	public IEnumerator Load(XmlElement element) {
		ForceCreateFSM = !ServiceCache.OptimizationService.LazyFsm;
		var progress = ServiceLocator.GetService<ILoadProgress>();
		DelayTimer.Begin(TimeSpan.FromSeconds(3.0));
		DebugUtility.Init();
		if (IsLoaded)
			Clear();
		progress?.OnBeforeCreateHierarchy();
		yield return null;
		yield return LoadVMWorldHierarchy();
		progress?.OnAfterCreateHierarchy();
		yield return null;
		var hierarchyObjectsNode = (XmlElement)element.FirstChild;
		VMSaveLoadManagerUtility.LoadEntities(hierarchyObjectsNode);
		yield return null;
		GameRootEntity = CreateRoot((VMLogicObject)IStaticDataContainer.StaticDataContainer.GameRoot, null);
		var notHierarchyObjectsNode = (XmlElement)hierarchyObjectsNode.NextSibling;
		VMSaveLoadManagerUtility.LoadEntities(notHierarchyObjectsNode);
		yield return null;
		var freeEntityesNode = (XmlElement)notHierarchyObjectsNode.NextSibling;
		var freeEntityInfoList = new List<FreeEntityInfo>();
		VMSaveLoadManager.LoadDynamiSerializableList(freeEntityesNode, freeEntityInfoList);
		VMEntity.LoadFreeEntities(freeEntityInfoList);
		yield return null;
		var vmTimeManagerNode = (XmlElement)freeEntityesNode.NextSibling;
		GameTimeManager.LoadFromXML(vmTimeManagerNode);
		WorldHierarchyRootEntity.AfterSaveLoading();
		yield return null;
		GameRootEntity.AfterSaveLoading();
		yield return null;
		DynamicMindMap.LoadMindMapsFromXML((XmlElement)vmTimeManagerNode.NextSibling);
		yield return null;
		processingEventsFSMDeQueue.Clear();
		VMSaveLoadManagerUtility.Clear();
		IsLoaded = true;
		OnSaveLoaded = true;
		progress?.OnLoadComplete();
	}

	public void Unload() {
		Clear();
	}

	private IEnumerator LoadVMWorldHierarchy() {
		var infos = new Queue<HierachyInfo>();
		WorldHierarchyRootEntity = (VMEntity)CreateVMHierarchy(HierarchyManager.GameHierarchyRoot, null, infos);
		while (infos.Count != 0) {
			var hierachyInfo = infos.Dequeue();
			CreateVMHierarchy((IWorldHierarchyObject)hierachyInfo.HierarchyTemplateObject, hierachyInfo.ParentVMEntity,
				infos);
			if (DelayTimer.Check)
				yield return null;
		}

		WorldHierarchyRootEntity.AfterCreate();
	}

	public static VMBaseEntity CreateVMHierarchy(
		IWorldHierarchyObject hierarchyTemplateObject,
		VMBaseEntity parentVMEntity,
		Queue<HierachyInfo> infos) {
		var vmHierarchy = CreateHierarchyEntityUtility.CreateObject(hierarchyTemplateObject, parentVMEntity);
		foreach (var hierarchyChild in hierarchyTemplateObject.HierarchyChilds)
			infos.Enqueue(new HierachyInfo {
				HierarchyTemplateObject = hierarchyChild,
				ParentVMEntity = vmHierarchy
			});
		return vmHierarchy;
	}

	public static void SetFatalError(string fatalErrorText) {
		Logger.AddError(fatalErrorText);
		VirtualMachine.fatalErrorText = fatalErrorText;
	}

	private void ClearHierarhy() {
		WorldHierarchyRootEntity.DisposeInstance(true);
		GameRootEntity.DisposeInstance(true);
	}

	private void BuildStaticHierarchy() {
		if (isStaticHierarchyBuilded)
			return;
		HierarchyManager.BuildStaticHierarchy();
		isStaticHierarchyBuilded = true;
	}

	private void ResetRoot() {
		GameRootFsm = null;
		VMEntityUtility.ResetRoot();
	}

	private bool MainEventsQueueProcessingMode { get; set; }

	private RaisedEventInfo CurrentEventInfo { get; set; }

	public IEnumerator LoadData(string dataFolderName, int threadCount, int dataCapacity) {
		if (!Directory.Exists(dataFolderName))
			Logger.AddError("Data folder not found : " + dataFolderName);
		else {
			var progress = ServiceLocator.GetService<ILoadProgress>();
			DelayTimer.Begin(TimeSpan.FromSeconds(1.0));
			progress?.OnBeforeLoadData();
			yield return XMLDataLoader.Instance.LoadDataFromXML(dataFolderName, threadCount, dataCapacity);
			progress?.OnAfterLoadData();
			yield return null;
			GameTimeManager.Init();
			foreach (var gameMode in ((VMGameRoot)IStaticDataContainer.StaticDataContainer.GameRoot).GameModes)
				GameTimeManager.CreateGameTimeContext(gameMode.Value);
			yield return null;
			BuildStaticHierarchy();
			progress?.OnBuildHierarchy();
			IsDataLoaded = true;
			progress?.OnLoadDataComplete();
			yield return null;
		}
	}

	public void UnloadData() {
		WorldEntityUtility.ClearStatic();
		isStaticHierarchyBuilded = false;
		XMLDataLoader.Instance.Clear();
		VMTypePool.Clear();
		IsDataLoaded = false;
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