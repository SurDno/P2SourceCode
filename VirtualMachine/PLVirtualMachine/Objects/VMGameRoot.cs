// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Objects.VMGameRoot
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

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
using System;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;
using VirtualMachine.Data.Customs;

#nullable disable
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
    [FieldData("BaseToEngineGuidsTable", DataFieldType.None)]
    private BaseToEngineGuidsTableData baseToEngineGuidsTable;
    [FieldData("HierarchyScenesStructure", DataFieldType.None)]
    private Dictionary<ulong, HierarchySceneInfoData> hierarchyScenesStructure;
    [FieldData("HierarchyEngineGuidsTable", DataFieldType.None)]
    private List<Guid> hierarchyEngineGuidsTable;
    [FieldData("WorldObjectSaveOptimizeMode", DataFieldType.None)]
    private bool worldObjectSaveOptimizedMode;
    private IGameMode mainGameMode;
    private Dictionary<string, IFunctionalComponent> allFunctionalComponents;
    private List<IVariable> allClassesRefs;
    private List<IVariable> globalVariablesRefs;
    private List<IVariable> logicMapRefs;
    private List<IVariable> gameModeRefs;
    private Dictionary<ulong, ILogicMap> logicMapsByGuidsDict;
    private Dictionary<Guid, VMWorldObject> engineTemplateObjectGuidsDict = new Dictionary<Guid, VMWorldObject>((IEqualityComparer<Guid>) GuidComparer.Instance);
    private Dictionary<ulong, VMWorldObject> engineTemplateObjectBaseGuidsDict = new Dictionary<ulong, VMWorldObject>((IEqualityComparer<ulong>) UlongComparer.Instance);
    private Dictionary<ulong, VMLogicObject> staticObjectsBaseGuidsDict = new Dictionary<ulong, VMLogicObject>((IEqualityComparer<ulong>) UlongComparer.Instance);
    private Dictionary<Guid, ISampleRef> samplesByEngineGuid;
    private Dictionary<ulong, ISampleRef> samplesByBaseGuid;
    private Dictionary<ulong, IBlueprint> allClasses = new Dictionary<ulong, IBlueprint>((IEqualityComparer<ulong>) UlongComparer.Instance);
    private Dictionary<ulong, IBlueprint> allBlueprints = new Dictionary<ulong, IBlueprint>((IEqualityComparer<ulong>) UlongComparer.Instance);
    private List<VMTalkingGraph> allTalkings;
    private Guid gameRootEngineGuid = Guid.Empty;
    private string startGameEventFuncName = "";

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "BaseToEngineGuidsTable":
              this.baseToEngineGuidsTable = EditorDataReadUtility.ReadEditorDataSerializable<BaseToEngineGuidsTableData>(xml, creator, typeContext);
              continue;
            case "ChildObjects":
              this.gameObjects = EditorDataReadUtility.ReadReferenceList<IContainer>(xml, creator, this.gameObjects);
              continue;
            case "CustomParams":
              this.customParamsDict = EditorDataReadUtility.ReadStringReferenceDictionary<IParam>(xml, creator, this.customParamsDict);
              continue;
            case "EventGraph":
              this.stateGraph = EditorDataReadUtility.ReadReference<IFiniteStateMachine>(xml, creator);
              continue;
            case "Events":
              this.customEventsList = EditorDataReadUtility.ReadReferenceList<IEvent>(xml, creator, this.customEventsList);
              continue;
            case "FunctionalComponents":
              this.functionalComponents = EditorDataReadUtility.ReadReferenceList<IFunctionalComponent>(xml, creator, this.functionalComponents);
              continue;
            case "GameModes":
              this.gameModesList = EditorDataReadUtility.ReadReferenceList<IGameMode>(xml, creator, this.gameModesList);
              continue;
            case "GameTimeContext":
              this.gameTimeContext = EditorDataReadUtility.ReadReference<IGameMode>(xml, creator);
              continue;
            case "HierarchyEngineGuidsTable":
              this.hierarchyEngineGuidsTable = EditorDataReadUtility.ReadValueList(xml, this.hierarchyEngineGuidsTable);
              continue;
            case "HierarchyScenesStructure":
              this.hierarchyScenesStructure = EditorDataReadUtility.ReadUlongEditorDataSerializableDictionary<HierarchySceneInfoData>(xml, creator, this.hierarchyScenesStructure);
              continue;
            case "LogicMaps":
              this.logicMapsList = EditorDataReadUtility.ReadReferenceList<ILogicMap>(xml, creator, this.logicMapsList);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "Parent":
              this.parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "Samples":
              this.samplesList = EditorDataReadUtility.ReadReferenceList<ISample>(xml, creator, this.samplesList);
              continue;
            case "StandartParams":
              this.standartParamsDict = EditorDataReadUtility.ReadStringReferenceDictionary<IParam>(xml, creator, this.standartParamsDict);
              continue;
            case "WorldObjectSaveOptimizeMode":
              this.worldObjectSaveOptimizedMode = EditorDataReadUtility.ReadValue(xml, this.worldObjectSaveOptimizedMode);
              continue;
            default:
              if (XMLDataLoader.Logs.Add(typeContext + " : " + xml.Name))
                Logger.AddError(typeContext + " : " + xml.Name);
              XmlReaderUtility.SkipNode(xml);
              continue;
          }
        }
        else if (xml.NodeType == XmlNodeType.EndElement)
          break;
      }
    }

    public VMGameRoot(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GAME;

    public List<IBlueprint> BaseBlueprints => (List<IBlueprint>) null;

    public override bool Static => true;

    public bool IsEngineRoot => false;

    public override IEnumerable<IVariable> GetContextVariables(
      EContextVariableCategory contextVarCategory)
    {
      switch (contextVarCategory)
      {
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GLOBAL_VAR:
          return (IEnumerable<IVariable>) this.globalVariablesRefs;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_CLASS:
          return (IEnumerable<IVariable>) this.allClassesRefs;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOGIC_MAP:
          return (IEnumerable<IVariable>) this.logicMapRefs;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GAMEMODE:
          return (IEnumerable<IVariable>) this.gameModeRefs;
        default:
          return base.GetContextVariables(contextVarCategory);
      }
    }

    public override Dictionary<string, IFunctionalComponent> FunctionalComponents
    {
      get => this.allFunctionalComponents;
    }

    public IEnumerable<IBlueprint> Blueprints
    {
      get => (IEnumerable<IBlueprint>) this.allBlueprints.Values;
    }

    public List<ISample> Samples => this.samplesList;

    public Dictionary<string, IGameMode> GameModes
    {
      get
      {
        Dictionary<string, IGameMode> gameModes = new Dictionary<string, IGameMode>();
        for (int index = 0; index < this.gameModesList.Count; ++index)
          gameModes.Add(this.gameModesList[index].Name, this.gameModesList[index]);
        return gameModes;
      }
    }

    public Dictionary<ulong, ILogicMap> LogicMaps => this.logicMapsByGuidsDict;

    public IEnumerable<IHierarchyObject> HierarchyChilds => (IEnumerable<IHierarchyObject>) null;

    public override IBlueprint Blueprint => (IBlueprint) this;

    public Guid EngineGuid
    {
      get
      {
        if (this.gameRootEngineGuid == Guid.Empty)
          this.gameRootEngineGuid = this.hierarchyEngineGuidsTable[0];
        return this.gameRootEngineGuid;
      }
    }

    public List<Guid> HierarchyEngineGuidsTable => this.hierarchyEngineGuidsTable;

    public HierarchyGuid HierarchyGuid => HierarchyGuid.Empty;

    public Guid EngineTemplateGuid => Guid.Empty;

    public IContainer VirtualObjectTemplate => (IContainer) null;

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

    public void OnBuildHierarchy() => this.PreloadChildWorldObjects();

    public IGameMode MainGameMode => this.mainGameMode;

    public IBlueprint EditorTemplate => (IBlueprint) this;

    protected override void PreloadChildWorldObjects()
    {
      base.PreloadChildWorldObjects();
      this.PreloadBlueprints();
    }

    protected override void UpdateContextVariablesTotalCache()
    {
      base.UpdateContextVariablesTotalCache();
      VMObjRef vmObjRef = new VMObjRef();
      vmObjRef.Initialize((IBlueprint) IStaticDataContainer.StaticDataContainer.GameRoot);
      this.LoadContextVaraiblesTotalCache((IVariable) vmObjRef);
      foreach (IObjRef staticObject in ((VMLogicObject) IStaticDataContainer.StaticDataContainer.GameRoot).GetStaticObjects())
      {
        if (((BaseRef) staticObject).Exist)
          this.LoadContextVaraiblesTotalCache((IVariable) staticObject);
      }
      foreach (IVariable blueprint in ((VMLogicObject) IStaticDataContainer.StaticDataContainer.GameRoot).GetBlueprints())
        this.LoadContextVaraiblesTotalCache(blueprint);
      List<VMTalkingGraph> templatesTalkings = this.GetAllTemplatesTalkings();
      List<IVariable> variableList1 = new List<IVariable>();
      foreach (VMTalkingGraph vmTalkingGraph in templatesTalkings)
      {
        VMStateRef vmStateRef = new VMStateRef();
        vmStateRef.Initialize((IState) vmTalkingGraph);
        variableList1.Add((IVariable) vmStateRef);
      }
      IEnumerable<VMLogicMapNode> allLogicMapNodes = this.GetAllLogicMapNodes();
      List<IVariable> variableList2 = new List<IVariable>();
      foreach (VMLogicMapNode lmNode in allLogicMapNodes)
      {
        VMLogicMapNodeRef vmLogicMapNodeRef = new VMLogicMapNodeRef();
        vmLogicMapNodeRef.Initialize((IGraphObject) lmNode);
        variableList2.Add((IVariable) vmLogicMapNodeRef);
      }
      foreach (IVariable contextVariable in this.GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_CLASS))
        this.LoadContextVaraiblesTotalCache(contextVariable);
      foreach (IVariable contextVariable in this.GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GLOBAL_VAR))
        this.LoadContextVaraiblesTotalCache(contextVariable);
      foreach (IVariable contextVariable in this.GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOGIC_MAP))
        this.LoadContextVaraiblesTotalCache(contextVariable);
      IEnumerable<IVariable> contextVariables = this.GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GAMEMODE);
      foreach (IVariable variable in variableList1)
        this.LoadContextVaraiblesTotalCache(variable);
      foreach (IVariable variable in variableList2)
        this.LoadContextVaraiblesTotalCache(variable);
      foreach (IVariable variable in contextVariables)
        this.LoadContextVaraiblesTotalCache(variable);
      foreach (ISample sample in ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).Samples)
      {
        VMSampleRef vmSampleRef = new VMSampleRef();
        vmSampleRef.Initialize(sample);
        this.LoadContextVaraiblesTotalCache((IVariable) vmSampleRef);
      }
    }

    public ulong GetBaseGuidByEngineTemplateGuid(Guid guid)
    {
      return this.baseToEngineGuidsTable.GetBaseGuidByEngineTemplateGuid(guid);
    }

    public Guid GetEngineTemplateGuidByBaseGuid(ulong id)
    {
      return this.baseToEngineGuidsTable.GetEngineTemplateGuidByBaseGuid(id);
    }

    public IWorldBlueprint GetEngineTemplateByGuid(Guid engineTemplateGuid)
    {
      VMWorldObject engineTemplateByGuid;
      this.engineTemplateObjectGuidsDict.TryGetValue(engineTemplateGuid, out engineTemplateByGuid);
      return (IWorldBlueprint) engineTemplateByGuid;
    }

    public IWorldBlueprint GetEngineTemplateByGuid(ulong editorTemplateGuid)
    {
      VMWorldObject engineTemplateByGuid;
      this.engineTemplateObjectBaseGuidsDict.TryGetValue(editorTemplateGuid, out engineTemplateByGuid);
      return (IWorldBlueprint) engineTemplateByGuid;
    }

    public IWorldHierarchyObject GetWorldHierarhyObjectByGuid(HierarchyGuid hGuid)
    {
      return HierarchyManager.GetWorldHierarhyObjectByGuid(hGuid);
    }

    public IBlueprint GetBlueprintByGuid(ulong bpGuid)
    {
      IBlueprint blueprint;
      return this.allBlueprints.TryGetValue(bpGuid, out blueprint) || this.allClasses.TryGetValue(bpGuid, out blueprint) ? blueprint : (IBlueprint) null;
    }

    public HierarchyGuid GetHierarchyGuidByHierarchyPath(string sPath) => HierarchyGuid.Empty;

    public List<IObjRef> GetNotHierarchyStaticObjects()
    {
      List<IObjRef> hierarchyStaticObjects = new List<IObjRef>(this.staticObjectsBaseGuidsDict.Count);
      foreach (KeyValuePair<ulong, VMLogicObject> keyValuePair in this.staticObjectsBaseGuidsDict)
      {
        VMLogicObject vmLogicObject = keyValuePair.Value;
        if (!(vmLogicObject is IWorldObject worldObject) || !worldObject.IsEngineRoot)
        {
          VMObjRef vmObjRef = new VMObjRef();
          vmObjRef.Initialize((IBlueprint) vmLogicObject);
          hierarchyStaticObjects.Add((IObjRef) vmObjRef);
        }
      }
      return hierarchyStaticObjects;
    }

    public string GetStartGameEventFuncName() => this.startGameEventFuncName;

    public List<VMTalkingGraph> GetAllTemplatesTalkings()
    {
      if (this.allTalkings == null)
        this.PreloadTalkings();
      return this.allTalkings;
    }

    public IEnumerable<VMLogicMapNode> GetAllLogicMapNodes()
    {
      for (int i = 0; i < this.logicMapsList.Count; ++i)
      {
        for (int j = 0; j < this.logicMapsList[i].Nodes.Count; ++j)
          yield return (VMLogicMapNode) this.logicMapsList[i].Nodes[j];
      }
    }

    public Dictionary<ulong, HierarchySceneInfoData> HierarchyScenesStructure
    {
      get => this.hierarchyScenesStructure;
    }

    public override void OnAfterLoad()
    {
      this.ComputeFunctionalComponents();
      IEnumerable<VMLogicObject> allLogicObjects = this.GetAllLogicObjects();
      this.ComputeBlueprints(allLogicObjects);
      this.ComputeTemplates(allLogicObjects);
      this.ComputeSamples();
      base.OnAfterLoad();
      this.ComputeVariables();
      this.ComputeMaps();
      this.ComputeModes();
      this.ComputeStartEvents();
    }

    public bool WorldObjectSaveOptimizedMode => this.worldObjectSaveOptimizedMode;

    public override void Clear()
    {
      base.Clear();
      this.samplesList.Clear();
      this.samplesList = (List<ISample>) null;
      this.gameModesList.Clear();
      this.gameModesList = (List<IGameMode>) null;
      if (this.logicMapsList != null)
      {
        foreach (IContainer logicMaps in this.logicMapsList)
          logicMaps.Clear();
        this.logicMapsList.Clear();
      }
      this.logicMapsList = (List<ILogicMap>) null;
      if (this.hierarchyScenesStructure != null)
      {
        foreach (KeyValuePair<ulong, HierarchySceneInfoData> keyValuePair in this.hierarchyScenesStructure)
          keyValuePair.Value.Clear();
        this.hierarchyScenesStructure.Clear();
        this.hierarchyScenesStructure = (Dictionary<ulong, HierarchySceneInfoData>) null;
      }
      if (this.hierarchyEngineGuidsTable != null)
      {
        this.hierarchyEngineGuidsTable.Clear();
        this.hierarchyEngineGuidsTable = (List<Guid>) null;
      }
      this.mainGameMode = (IGameMode) null;
      if (this.allFunctionalComponents != null)
      {
        foreach (KeyValuePair<string, IFunctionalComponent> functionalComponent in this.allFunctionalComponents)
          functionalComponent.Value.Clear();
        this.allFunctionalComponents.Clear();
        this.allFunctionalComponents = (Dictionary<string, IFunctionalComponent>) null;
      }
      if (this.allClassesRefs != null)
        this.allClassesRefs.Clear();
      if (this.globalVariablesRefs != null)
        this.globalVariablesRefs.Clear();
      if (this.logicMapRefs != null)
        this.logicMapRefs.Clear();
      if (this.gameModeRefs != null)
        this.gameModeRefs.Clear();
      if (this.logicMapsByGuidsDict != null)
      {
        this.logicMapsByGuidsDict.Clear();
        this.logicMapsByGuidsDict = (Dictionary<ulong, ILogicMap>) null;
      }
      this.engineTemplateObjectGuidsDict.Clear();
      this.engineTemplateObjectGuidsDict = (Dictionary<Guid, VMWorldObject>) null;
      this.engineTemplateObjectBaseGuidsDict.Clear();
      this.engineTemplateObjectBaseGuidsDict = (Dictionary<ulong, VMWorldObject>) null;
      this.staticObjectsBaseGuidsDict.Clear();
      this.staticObjectsBaseGuidsDict = (Dictionary<ulong, VMLogicObject>) null;
      this.samplesByEngineGuid.Clear();
      this.samplesByEngineGuid = (Dictionary<Guid, ISampleRef>) null;
      this.samplesByBaseGuid.Clear();
      this.samplesByBaseGuid = (Dictionary<ulong, ISampleRef>) null;
      this.allClasses.Clear();
      this.allClasses = (Dictionary<ulong, IBlueprint>) null;
      this.allBlueprints.Clear();
      this.allBlueprints = (Dictionary<ulong, IBlueprint>) null;
      this.allTalkings.Clear();
      this.allTalkings = (List<VMTalkingGraph>) null;
    }

    private void ComputeFunctionalComponents()
    {
      this.allFunctionalComponents = new Dictionary<string, IFunctionalComponent>(this.functionalComponents.Count);
      foreach (IFunctionalComponent functionalComponent in this.functionalComponents)
        this.allFunctionalComponents.Add(functionalComponent.Name, functionalComponent);
    }

    private void ComputeStartEvents()
    {
      this.startGameEventFuncName = "";
      ulong num = 0;
      foreach (IEvent @event in this.events)
      {
        if (!@event.IsManual)
        {
          VMFunctionalComponent parent = (VMFunctionalComponent) @event.Parent;
          if (parent.Main && (!(this.startGameEventFuncName != "") || parent.BaseGuid <= num) && @event.Name == EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_START_GAME, typeof (VMGameComponent)) && this.startGameEventFuncName == "")
          {
            this.startGameEventFuncName = @event.FunctionalName;
            num = parent.BaseGuid;
          }
        }
      }
      if (!(this.startGameEventFuncName == ""))
        return;
      Logger.AddError(string.Format("Start game event not found in game root !"));
    }

    private void ComputeVariables()
    {
      this.globalVariablesRefs = new List<IVariable>();
      foreach (KeyValuePair<ulong, IBlueprint> allBlueprint in this.allBlueprints)
      {
        IBlueprint blueprint = allBlueprint.Value;
        GlobalVariable instancesListVariable = GlobalVariableUtility.CreateGlobalTemplateInstancesListVariable(blueprint);
        GlobalVariableUtility.RegistrGlobalTemplateInstancesList(blueprint);
        this.globalVariablesRefs.Add((IVariable) instancesListVariable);
      }
    }

    private void ComputeMaps()
    {
      this.logicMapsByGuidsDict = new Dictionary<ulong, ILogicMap>(this.logicMapsList.Count, (IEqualityComparer<ulong>) UlongComparer.Instance);
      this.logicMapRefs = new List<IVariable>(this.logicMapsList.Count);
      foreach (ILogicMap logicMaps in this.logicMapsList)
      {
        VMLogicMapRef vmLogicMapRef = new VMLogicMapRef();
        vmLogicMapRef.Initialize(logicMaps);
        this.logicMapRefs.Add((IVariable) vmLogicMapRef);
        this.logicMapsByGuidsDict.Add(logicMaps.BaseGuid, logicMaps);
      }
    }

    private void ComputeModes()
    {
      this.gameModeRefs = new List<IVariable>(this.gameModesList.Count);
      foreach (IGameMode gameModes in this.gameModesList)
      {
        if (gameModes.IsMain)
          this.mainGameMode = gameModes;
        VMGameModeRef vmGameModeRef = new VMGameModeRef();
        vmGameModeRef.Initialize(gameModes);
        this.gameModeRefs.Add((IVariable) vmGameModeRef);
      }
      if (this.mainGameMode != null)
        return;
      Logger.AddError(string.Format("Main game mode not defined!"));
    }

    private void ComputeBlueprints(IEnumerable<VMLogicObject> logicObjects)
    {
      this.allBlueprints.Clear();
      this.allClasses.Clear();
      foreach (VMLogicObject logicObject in logicObjects)
      {
        if (!logicObject.Static)
        {
          if (!this.allBlueprints.ContainsKey(logicObject.BaseGuid))
          {
            this.allBlueprints.Add(logicObject.BaseGuid, (IBlueprint) logicObject);
            if (logicObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_CLASS)
              this.allClasses.Add(logicObject.BaseGuid, (IBlueprint) logicObject);
          }
          else
            Logger.AddError(string.Format("Logic object blueprint {0} dublicated in game", (object) logicObject.Name));
        }
        else if (!logicObject.IsVirtual)
        {
          if (!this.staticObjectsBaseGuidsDict.ContainsKey(logicObject.BaseGuid))
            this.staticObjectsBaseGuidsDict.Add(logicObject.BaseGuid, logicObject);
          else
            Logger.AddError(string.Format("Logic static object {0} dublicated in game", (object) logicObject.Name));
        }
      }
      this.allClassesRefs = new List<IVariable>(this.allClasses.Count);
      foreach (KeyValuePair<ulong, IBlueprint> allClass in this.allClasses)
      {
        VMBlueprintRef vmBlueprintRef = new VMBlueprintRef();
        vmBlueprintRef.Initialize(allClass.Value);
        this.allClassesRefs.Add((IVariable) vmBlueprintRef);
      }
    }

    private void ComputeSamples()
    {
      this.samplesByBaseGuid = new Dictionary<ulong, ISampleRef>(this.samplesList.Count, (IEqualityComparer<ulong>) UlongComparer.Instance);
      this.samplesByEngineGuid = new Dictionary<Guid, ISampleRef>(this.samplesList.Count, (IEqualityComparer<Guid>) GuidComparer.Instance);
      foreach (ISample samples in this.samplesList)
      {
        VMSampleRef vmSampleRef = new VMSampleRef();
        vmSampleRef.Initialize(samples);
        if (!this.samplesByBaseGuid.ContainsKey(samples.BaseGuid))
          this.samplesByBaseGuid.Add(samples.BaseGuid, (ISampleRef) vmSampleRef);
        else
          Logger.AddError(string.Format("Sample base guid={0} is dublicated", (object) samples.BaseGuid));
        if (!this.samplesByEngineGuid.ContainsKey(((VMSample) samples).EngineTemplateGuid))
          this.samplesByEngineGuid.Add(((VMSample) samples).EngineTemplateGuid, (ISampleRef) vmSampleRef);
        else
          Logger.AddError(string.Format("Sample guid={0} is dublicated", (object) ((VMSample) samples).EngineTemplateGuid));
      }
    }

    private void ComputeTemplates(IEnumerable<VMLogicObject> logicObjects)
    {
      this.engineTemplateObjectBaseGuidsDict.Clear();
      this.engineTemplateObjectGuidsDict.Clear();
      foreach (VMLogicObject logicObject in logicObjects)
      {
        if ((logicObject.GetCategory() & EObjectCategory.OBJECT_CATEGORY_TEMPLATES) > EObjectCategory.OBJECT_CATEGORY_NONE)
        {
          VMWorldObject vmWorldObject = (VMWorldObject) logicObject;
          if (!vmWorldObject.IsVirtual)
          {
            if (!this.engineTemplateObjectBaseGuidsDict.ContainsKey(vmWorldObject.BaseGuid))
              this.engineTemplateObjectBaseGuidsDict.Add(vmWorldObject.BaseGuid, vmWorldObject);
            else
              Logger.AddError(string.Format("Template base guid={0} is dublicated", (object) vmWorldObject.BaseGuid));
            if (vmWorldObject.EngineTemplateGuid != Guid.Empty)
            {
              if (!this.engineTemplateObjectGuidsDict.ContainsKey(vmWorldObject.EngineTemplateGuid))
                this.engineTemplateObjectGuidsDict.Add(vmWorldObject.EngineTemplateGuid, vmWorldObject);
              else
                Logger.AddError(string.Format("Engine template guid={0} is dublicated", (object) vmWorldObject.EngineTemplateGuid));
            }
          }
        }
      }
    }

    private void PreloadTalkings()
    {
      this.allTalkings = new List<VMTalkingGraph>();
      for (int index = 0; index < this.blueprints.Count; ++index)
      {
        if (this.blueprints[index].Blueprint is VMBlueprint blueprint && blueprint.IsFunctionalSupport("Speaking") && blueprint.StateGraph != null)
        {
          foreach (IFiniteStateMachine finiteStateMachine in blueprint.StateGraph.GetSubGraphStructure(EGraphType.GRAPH_TYPE_TALKING, false))
          {
            if (finiteStateMachine is VMTalkingGraph vmTalkingGraph)
              this.allTalkings.Add(vmTalkingGraph);
          }
        }
      }
    }
  }
}
