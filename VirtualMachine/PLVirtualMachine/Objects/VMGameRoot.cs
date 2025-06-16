using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Comparers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Data;
using PLVirtualMachine.FSM;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.LogicMap;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;
using VirtualMachine.Data.Customs;

namespace PLVirtualMachine.Objects
{
  [TypeData(EDataType.TGame)]
  [DataFactory("GameRoot")]
  public class VMGameRoot : 
    VMLogicObject,
    IStub,
    IEditorDataReader,
    IBlueprint,
    IGameObjectContext,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    IContext,
    ILogicObject,
    IGameRoot
  {
    [FieldData("Samples", DataFieldType.Reference)]
    private List<ISample> samplesList = new List<ISample>();
    [FieldData("GameModes", DataFieldType.Reference)]
    private List<IGameMode> gameModesList = new List<IGameMode>();
    [FieldData("LogicMaps", DataFieldType.Reference)]
    private List<ILogicMap> logicMapsList = new List<ILogicMap>();
    [FieldData("BaseToEngineGuidsTable")]
    private BaseToEngineGuidsTableData baseToEngineGuidsTable;
    [FieldData("HierarchyScenesStructure")]
    private Dictionary<ulong, HierarchySceneInfoData> hierarchyScenesStructure;
    [FieldData("HierarchyEngineGuidsTable")]
    private List<Guid> hierarchyEngineGuidsTable;
    [FieldData("WorldObjectSaveOptimizeMode")]
    private bool worldObjectSaveOptimizedMode;
    private IGameMode mainGameMode;
    private Dictionary<string, IFunctionalComponent> allFunctionalComponents;
    private List<IVariable> allClassesRefs;
    private List<IVariable> globalVariablesRefs;
    private List<IVariable> logicMapRefs;
    private List<IVariable> gameModeRefs;
    private Dictionary<ulong, ILogicMap> logicMapsByGuidsDict;
    private Dictionary<Guid, VMWorldObject> engineTemplateObjectGuidsDict = new Dictionary<Guid, VMWorldObject>(GuidComparer.Instance);
    private Dictionary<ulong, VMWorldObject> engineTemplateObjectBaseGuidsDict = new Dictionary<ulong, VMWorldObject>(UlongComparer.Instance);
    private Dictionary<ulong, VMLogicObject> staticObjectsBaseGuidsDict = new Dictionary<ulong, VMLogicObject>(UlongComparer.Instance);
    private Dictionary<Guid, ISampleRef> samplesByEngineGuid;
    private Dictionary<ulong, ISampleRef> samplesByBaseGuid;
    private Dictionary<ulong, IBlueprint> allClasses = new Dictionary<ulong, IBlueprint>(UlongComparer.Instance);
    private Dictionary<ulong, IBlueprint> allBlueprints = new Dictionary<ulong, IBlueprint>(UlongComparer.Instance);
    private List<VMTalkingGraph> allTalkings;
    private Guid gameRootEngineGuid = Guid.Empty;
    private string startGameEventFuncName = "";

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "BaseToEngineGuidsTable":
              baseToEngineGuidsTable = EditorDataReadUtility.ReadEditorDataSerializable<BaseToEngineGuidsTableData>(xml, creator, typeContext);
              continue;
            case "ChildObjects":
              gameObjects = EditorDataReadUtility.ReadReferenceList(xml, creator, gameObjects);
              continue;
            case "CustomParams":
              customParamsDict = EditorDataReadUtility.ReadStringReferenceDictionary(xml, creator, customParamsDict);
              continue;
            case "EventGraph":
              stateGraph = EditorDataReadUtility.ReadReference<IFiniteStateMachine>(xml, creator);
              continue;
            case "Events":
              customEventsList = EditorDataReadUtility.ReadReferenceList(xml, creator, customEventsList);
              continue;
            case "FunctionalComponents":
              functionalComponents = EditorDataReadUtility.ReadReferenceList(xml, creator, functionalComponents);
              continue;
            case "GameModes":
              gameModesList = EditorDataReadUtility.ReadReferenceList(xml, creator, gameModesList);
              continue;
            case "GameTimeContext":
              gameTimeContext = EditorDataReadUtility.ReadReference<IGameMode>(xml, creator);
              continue;
            case "HierarchyEngineGuidsTable":
              hierarchyEngineGuidsTable = EditorDataReadUtility.ReadValueList(xml, hierarchyEngineGuidsTable);
              continue;
            case "HierarchyScenesStructure":
              hierarchyScenesStructure = EditorDataReadUtility.ReadUlongEditorDataSerializableDictionary(xml, creator, hierarchyScenesStructure);
              continue;
            case "LogicMaps":
              logicMapsList = EditorDataReadUtility.ReadReferenceList(xml, creator, logicMapsList);
              continue;
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
              continue;
            case "Parent":
              parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "Samples":
              samplesList = EditorDataReadUtility.ReadReferenceList(xml, creator, samplesList);
              continue;
            case "StandartParams":
              standartParamsDict = EditorDataReadUtility.ReadStringReferenceDictionary(xml, creator, standartParamsDict);
              continue;
            case "WorldObjectSaveOptimizeMode":
              worldObjectSaveOptimizedMode = EditorDataReadUtility.ReadValue(xml, worldObjectSaveOptimizedMode);
              continue;
            default:
              if (XMLDataLoader.Logs.Add(typeContext + " : " + xml.Name))
                Logger.AddError(typeContext + " : " + xml.Name);
              XmlReaderUtility.SkipNode(xml);
              continue;
          }
        }

        if (xml.NodeType == XmlNodeType.EndElement)
          break;
      }
    }

    public VMGameRoot(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GAME;

    public List<IBlueprint> BaseBlueprints => null;

    public override bool Static => true;

    public bool IsEngineRoot => false;

    public override IEnumerable<IVariable> GetContextVariables(
      EContextVariableCategory contextVarCategory)
    {
      switch (contextVarCategory)
      {
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GLOBAL_VAR:
          return globalVariablesRefs;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_CLASS:
          return allClassesRefs;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOGIC_MAP:
          return logicMapRefs;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GAMEMODE:
          return gameModeRefs;
        default:
          return base.GetContextVariables(contextVarCategory);
      }
    }

    public override Dictionary<string, IFunctionalComponent> FunctionalComponents
    {
      get => allFunctionalComponents;
    }

    public IEnumerable<IBlueprint> Blueprints
    {
      get => allBlueprints.Values;
    }

    public List<ISample> Samples => samplesList;

    public Dictionary<string, IGameMode> GameModes
    {
      get
      {
        Dictionary<string, IGameMode> gameModes = new Dictionary<string, IGameMode>();
        for (int index = 0; index < gameModesList.Count; ++index)
          gameModes.Add(gameModesList[index].Name, gameModesList[index]);
        return gameModes;
      }
    }

    public Dictionary<ulong, ILogicMap> LogicMaps => logicMapsByGuidsDict;

    public IEnumerable<IHierarchyObject> HierarchyChilds => null;

    public override IBlueprint Blueprint => this;

    public Guid EngineGuid
    {
      get
      {
        if (gameRootEngineGuid == Guid.Empty)
          gameRootEngineGuid = hierarchyEngineGuidsTable[0];
        return gameRootEngineGuid;
      }
    }

    public List<Guid> HierarchyEngineGuidsTable => hierarchyEngineGuidsTable;

    public HierarchyGuid HierarchyGuid => HierarchyGuid.Empty;

    public Guid EngineTemplateGuid => Guid.Empty;

    public IContainer VirtualObjectTemplate => null;

    public IEnumerable<Guid> MountingSceneGuids
    {
      get
      {
        yield break;
      }
    }

    public void InitInstanceGuid(Guid instanceGuid)
    {
    }

    public IEnumerable<IHierarchyObject> SimpleChilds
    {
      get
      {
        yield break;
      }
    }

    public void OnBuildHierarchy() => PreloadChildWorldObjects();

    public IGameMode MainGameMode => mainGameMode;

    public IBlueprint EditorTemplate => this;

    protected override void PreloadChildWorldObjects()
    {
      base.PreloadChildWorldObjects();
      PreloadBlueprints();
    }

    protected override void UpdateContextVariablesTotalCache()
    {
      base.UpdateContextVariablesTotalCache();
      VMObjRef vmObjRef = new VMObjRef();
      vmObjRef.Initialize((IBlueprint) IStaticDataContainer.StaticDataContainer.GameRoot);
      LoadContextVaraiblesTotalCache(vmObjRef);
      foreach (IObjRef staticObject in ((VMLogicObject) IStaticDataContainer.StaticDataContainer.GameRoot).GetStaticObjects())
      {
        if (((BaseRef) staticObject).Exist)
          LoadContextVaraiblesTotalCache(staticObject);
      }
      foreach (IVariable blueprint in ((VMLogicObject) IStaticDataContainer.StaticDataContainer.GameRoot).GetBlueprints())
        LoadContextVaraiblesTotalCache(blueprint);
      List<VMTalkingGraph> templatesTalkings = GetAllTemplatesTalkings();
      List<IVariable> variableList1 = new List<IVariable>();
      foreach (VMTalkingGraph vmTalkingGraph in templatesTalkings)
      {
        VMStateRef vmStateRef = new VMStateRef();
        vmStateRef.Initialize(vmTalkingGraph);
        variableList1.Add(vmStateRef);
      }
      IEnumerable<VMLogicMapNode> allLogicMapNodes = GetAllLogicMapNodes();
      List<IVariable> variableList2 = new List<IVariable>();
      foreach (VMLogicMapNode lmNode in allLogicMapNodes)
      {
        VMLogicMapNodeRef vmLogicMapNodeRef = new VMLogicMapNodeRef();
        vmLogicMapNodeRef.Initialize(lmNode);
        variableList2.Add(vmLogicMapNodeRef);
      }
      foreach (IVariable contextVariable in GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_CLASS))
        LoadContextVaraiblesTotalCache(contextVariable);
      foreach (IVariable contextVariable in GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GLOBAL_VAR))
        LoadContextVaraiblesTotalCache(contextVariable);
      foreach (IVariable contextVariable in GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOGIC_MAP))
        LoadContextVaraiblesTotalCache(contextVariable);
      IEnumerable<IVariable> contextVariables = GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GAMEMODE);
      foreach (IVariable variable in variableList1)
        LoadContextVaraiblesTotalCache(variable);
      foreach (IVariable variable in variableList2)
        LoadContextVaraiblesTotalCache(variable);
      foreach (IVariable variable in contextVariables)
        LoadContextVaraiblesTotalCache(variable);
      foreach (ISample sample in ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).Samples)
      {
        VMSampleRef vmSampleRef = new VMSampleRef();
        vmSampleRef.Initialize(sample);
        LoadContextVaraiblesTotalCache(vmSampleRef);
      }
    }

    public ulong GetBaseGuidByEngineTemplateGuid(Guid guid)
    {
      return baseToEngineGuidsTable.GetBaseGuidByEngineTemplateGuid(guid);
    }

    public Guid GetEngineTemplateGuidByBaseGuid(ulong id)
    {
      return baseToEngineGuidsTable.GetEngineTemplateGuidByBaseGuid(id);
    }

    public IWorldBlueprint GetEngineTemplateByGuid(Guid engineTemplateGuid)
    {
      VMWorldObject engineTemplateByGuid;
      engineTemplateObjectGuidsDict.TryGetValue(engineTemplateGuid, out engineTemplateByGuid);
      return engineTemplateByGuid;
    }

    public IWorldBlueprint GetEngineTemplateByGuid(ulong editorTemplateGuid)
    {
      VMWorldObject engineTemplateByGuid;
      engineTemplateObjectBaseGuidsDict.TryGetValue(editorTemplateGuid, out engineTemplateByGuid);
      return engineTemplateByGuid;
    }

    public IWorldHierarchyObject GetWorldHierarhyObjectByGuid(HierarchyGuid hGuid)
    {
      return HierarchyManager.GetWorldHierarhyObjectByGuid(hGuid);
    }

    public IBlueprint GetBlueprintByGuid(ulong bpGuid)
    {
      IBlueprint blueprint;
      return allBlueprints.TryGetValue(bpGuid, out blueprint) || allClasses.TryGetValue(bpGuid, out blueprint) ? blueprint : null;
    }

    public HierarchyGuid GetHierarchyGuidByHierarchyPath(string sPath) => HierarchyGuid.Empty;

    public List<IObjRef> GetNotHierarchyStaticObjects()
    {
      List<IObjRef> hierarchyStaticObjects = new List<IObjRef>(staticObjectsBaseGuidsDict.Count);
      foreach (KeyValuePair<ulong, VMLogicObject> keyValuePair in staticObjectsBaseGuidsDict)
      {
        VMLogicObject vmLogicObject = keyValuePair.Value;
        if (!(vmLogicObject is IWorldObject worldObject) || !worldObject.IsEngineRoot)
        {
          VMObjRef vmObjRef = new VMObjRef();
          vmObjRef.Initialize((IBlueprint) vmLogicObject);
          hierarchyStaticObjects.Add(vmObjRef);
        }
      }
      return hierarchyStaticObjects;
    }

    public string GetStartGameEventFuncName() => startGameEventFuncName;

    public List<VMTalkingGraph> GetAllTemplatesTalkings()
    {
      if (allTalkings == null)
        PreloadTalkings();
      return allTalkings;
    }

    public IEnumerable<VMLogicMapNode> GetAllLogicMapNodes()
    {
      for (int i = 0; i < logicMapsList.Count; ++i)
      {
        for (int j = 0; j < logicMapsList[i].Nodes.Count; ++j)
          yield return (VMLogicMapNode) logicMapsList[i].Nodes[j];
      }
    }

    public Dictionary<ulong, HierarchySceneInfoData> HierarchyScenesStructure
    {
      get => hierarchyScenesStructure;
    }

    public override void OnAfterLoad()
    {
      ComputeFunctionalComponents();
      IEnumerable<VMLogicObject> allLogicObjects = GetAllLogicObjects();
      ComputeBlueprints(allLogicObjects);
      ComputeTemplates(allLogicObjects);
      ComputeSamples();
      base.OnAfterLoad();
      ComputeVariables();
      ComputeMaps();
      ComputeModes();
      ComputeStartEvents();
    }

    public bool WorldObjectSaveOptimizedMode => worldObjectSaveOptimizedMode;

    public override void Clear()
    {
      base.Clear();
      samplesList.Clear();
      samplesList = null;
      gameModesList.Clear();
      gameModesList = null;
      if (logicMapsList != null)
      {
        foreach (IContainer logicMaps in logicMapsList)
          logicMaps.Clear();
        logicMapsList.Clear();
      }
      logicMapsList = null;
      if (hierarchyScenesStructure != null)
      {
        foreach (KeyValuePair<ulong, HierarchySceneInfoData> keyValuePair in hierarchyScenesStructure)
          keyValuePair.Value.Clear();
        hierarchyScenesStructure.Clear();
        hierarchyScenesStructure = null;
      }
      if (hierarchyEngineGuidsTable != null)
      {
        hierarchyEngineGuidsTable.Clear();
        hierarchyEngineGuidsTable = null;
      }
      mainGameMode = null;
      if (allFunctionalComponents != null)
      {
        foreach (KeyValuePair<string, IFunctionalComponent> functionalComponent in allFunctionalComponents)
          functionalComponent.Value.Clear();
        allFunctionalComponents.Clear();
        allFunctionalComponents = null;
      }
      if (allClassesRefs != null)
        allClassesRefs.Clear();
      if (globalVariablesRefs != null)
        globalVariablesRefs.Clear();
      if (logicMapRefs != null)
        logicMapRefs.Clear();
      if (gameModeRefs != null)
        gameModeRefs.Clear();
      if (logicMapsByGuidsDict != null)
      {
        logicMapsByGuidsDict.Clear();
        logicMapsByGuidsDict = null;
      }
      engineTemplateObjectGuidsDict.Clear();
      engineTemplateObjectGuidsDict = null;
      engineTemplateObjectBaseGuidsDict.Clear();
      engineTemplateObjectBaseGuidsDict = null;
      staticObjectsBaseGuidsDict.Clear();
      staticObjectsBaseGuidsDict = null;
      samplesByEngineGuid.Clear();
      samplesByEngineGuid = null;
      samplesByBaseGuid.Clear();
      samplesByBaseGuid = null;
      allClasses.Clear();
      allClasses = null;
      allBlueprints.Clear();
      allBlueprints = null;
      allTalkings.Clear();
      allTalkings = null;
    }

    private void ComputeFunctionalComponents()
    {
      allFunctionalComponents = new Dictionary<string, IFunctionalComponent>(functionalComponents.Count);
      foreach (IFunctionalComponent functionalComponent in functionalComponents)
        allFunctionalComponents.Add(functionalComponent.Name, functionalComponent);
    }

    private void ComputeStartEvents()
    {
      startGameEventFuncName = "";
      ulong num = 0;
      foreach (IEvent @event in events)
      {
        if (!@event.IsManual)
        {
          VMFunctionalComponent parent = (VMFunctionalComponent) @event.Parent;
          if (parent.Main && (!(startGameEventFuncName != "") || parent.BaseGuid <= num) && @event.Name == EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_START_GAME, typeof (VMGameComponent)) && startGameEventFuncName == "")
          {
            startGameEventFuncName = @event.FunctionalName;
            num = parent.BaseGuid;
          }
        }
      }
      if (!(startGameEventFuncName == ""))
        return;
      Logger.AddError("Start game event not found in game root !");
    }

    private void ComputeVariables()
    {
      globalVariablesRefs = new List<IVariable>();
      foreach (KeyValuePair<ulong, IBlueprint> allBlueprint in allBlueprints)
      {
        IBlueprint blueprint = allBlueprint.Value;
        GlobalVariable instancesListVariable = GlobalVariableUtility.CreateGlobalTemplateInstancesListVariable(blueprint);
        GlobalVariableUtility.RegistrGlobalTemplateInstancesList(blueprint);
        globalVariablesRefs.Add(instancesListVariable);
      }
    }

    private void ComputeMaps()
    {
      logicMapsByGuidsDict = new Dictionary<ulong, ILogicMap>(logicMapsList.Count, UlongComparer.Instance);
      logicMapRefs = new List<IVariable>(logicMapsList.Count);
      foreach (ILogicMap logicMaps in logicMapsList)
      {
        VMLogicMapRef vmLogicMapRef = new VMLogicMapRef();
        vmLogicMapRef.Initialize(logicMaps);
        logicMapRefs.Add(vmLogicMapRef);
        logicMapsByGuidsDict.Add(logicMaps.BaseGuid, logicMaps);
      }
    }

    private void ComputeModes()
    {
      gameModeRefs = new List<IVariable>(gameModesList.Count);
      foreach (IGameMode gameModes in gameModesList)
      {
        if (gameModes.IsMain)
          mainGameMode = gameModes;
        VMGameModeRef vmGameModeRef = new VMGameModeRef();
        vmGameModeRef.Initialize(gameModes);
        gameModeRefs.Add(vmGameModeRef);
      }
      if (mainGameMode != null)
        return;
      Logger.AddError("Main game mode not defined!");
    }

    private void ComputeBlueprints(IEnumerable<VMLogicObject> logicObjects)
    {
      allBlueprints.Clear();
      allClasses.Clear();
      foreach (VMLogicObject logicObject in logicObjects)
      {
        if (!logicObject.Static)
        {
          if (!allBlueprints.ContainsKey(logicObject.BaseGuid))
          {
            allBlueprints.Add(logicObject.BaseGuid, (IBlueprint) logicObject);
            if (logicObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_CLASS)
              allClasses.Add(logicObject.BaseGuid, (IBlueprint) logicObject);
          }
          else
            Logger.AddError(string.Format("Logic object blueprint {0} dublicated in game", logicObject.Name));
        }
        else if (!logicObject.IsVirtual)
        {
          if (!staticObjectsBaseGuidsDict.ContainsKey(logicObject.BaseGuid))
            staticObjectsBaseGuidsDict.Add(logicObject.BaseGuid, logicObject);
          else
            Logger.AddError(string.Format("Logic static object {0} dublicated in game", logicObject.Name));
        }
      }
      allClassesRefs = new List<IVariable>(allClasses.Count);
      foreach (KeyValuePair<ulong, IBlueprint> allClass in allClasses)
      {
        VMBlueprintRef vmBlueprintRef = new VMBlueprintRef();
        vmBlueprintRef.Initialize(allClass.Value);
        allClassesRefs.Add(vmBlueprintRef);
      }
    }

    private void ComputeSamples()
    {
      samplesByBaseGuid = new Dictionary<ulong, ISampleRef>(samplesList.Count, UlongComparer.Instance);
      samplesByEngineGuid = new Dictionary<Guid, ISampleRef>(samplesList.Count, GuidComparer.Instance);
      foreach (ISample samples in samplesList)
      {
        VMSampleRef vmSampleRef = new VMSampleRef();
        vmSampleRef.Initialize(samples);
        if (!samplesByBaseGuid.ContainsKey(samples.BaseGuid))
          samplesByBaseGuid.Add(samples.BaseGuid, vmSampleRef);
        else
          Logger.AddError(string.Format("Sample base guid={0} is dublicated", samples.BaseGuid));
        if (!samplesByEngineGuid.ContainsKey(((VMSample) samples).EngineTemplateGuid))
          samplesByEngineGuid.Add(((VMSample) samples).EngineTemplateGuid, vmSampleRef);
        else
          Logger.AddError(string.Format("Sample guid={0} is dublicated", ((VMSample) samples).EngineTemplateGuid));
      }
    }

    private void ComputeTemplates(IEnumerable<VMLogicObject> logicObjects)
    {
      engineTemplateObjectBaseGuidsDict.Clear();
      engineTemplateObjectGuidsDict.Clear();
      foreach (VMLogicObject logicObject in logicObjects)
      {
        if ((logicObject.GetCategory() & EObjectCategory.OBJECT_CATEGORY_TEMPLATES) > EObjectCategory.OBJECT_CATEGORY_NONE)
        {
          VMWorldObject vmWorldObject = (VMWorldObject) logicObject;
          if (!vmWorldObject.IsVirtual)
          {
            if (!engineTemplateObjectBaseGuidsDict.ContainsKey(vmWorldObject.BaseGuid))
              engineTemplateObjectBaseGuidsDict.Add(vmWorldObject.BaseGuid, vmWorldObject);
            else
              Logger.AddError(string.Format("Template base guid={0} is dublicated", vmWorldObject.BaseGuid));
            if (vmWorldObject.EngineTemplateGuid != Guid.Empty)
            {
              if (!engineTemplateObjectGuidsDict.ContainsKey(vmWorldObject.EngineTemplateGuid))
                engineTemplateObjectGuidsDict.Add(vmWorldObject.EngineTemplateGuid, vmWorldObject);
              else
                Logger.AddError(string.Format("Engine template guid={0} is dublicated", vmWorldObject.EngineTemplateGuid));
            }
          }
        }
      }
    }

    private void PreloadTalkings()
    {
      allTalkings = new List<VMTalkingGraph>();
      for (int index = 0; index < blueprints.Count; ++index)
      {
        if (blueprints[index].Blueprint is VMBlueprint blueprint && blueprint.IsFunctionalSupport("Speaking") && blueprint.StateGraph != null)
        {
          foreach (IFiniteStateMachine finiteStateMachine in blueprint.StateGraph.GetSubGraphStructure(EGraphType.GRAPH_TYPE_TALKING, false))
          {
            if (finiteStateMachine is VMTalkingGraph vmTalkingGraph)
              allTalkings.Add(vmTalkingGraph);
          }
        }
      }
    }
  }
}
