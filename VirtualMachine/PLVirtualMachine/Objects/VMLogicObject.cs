using Cofe.Loggers;
using Engine.Common.Types;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.FSM;
using PLVirtualMachine.GameLogic;
using System.Collections.Generic;

namespace PLVirtualMachine.Objects
{
  public abstract class VMLogicObject : VMBaseObject, IOnAfterLoaded, ILogicObject, IContext, INamed
  {
    [FieldData("ChildObjects", DataFieldType.Reference)]
    protected List<IContainer> gameObjects;
    [FieldData("Events", DataFieldType.Reference)]
    protected List<IEvent> customEventsList;
    [FieldData("CustomParams", DataFieldType.None)]
    protected Dictionary<string, IParam> customParamsDict;
    [FieldData("StandartParams", DataFieldType.None)]
    protected Dictionary<string, IParam> standartParamsDict;
    [FieldData("EventGraph", DataFieldType.Reference)]
    protected IFiniteStateMachine stateGraph;
    [FieldData("FunctionalComponents", DataFieldType.Reference)]
    protected List<IFunctionalComponent> functionalComponents = new List<IFunctionalComponent>();
    [FieldData("GameTimeContext", DataFieldType.Reference)]
    protected IGameMode gameTimeContext;
    protected List<IBlueprintRef> blueprints;
    protected List<IObjRef> staticObjects;
    protected List<IEvent> events;
    protected List<BaseFunction> functions;
    protected Dictionary<string, IVariable> contextVariablesTotalCache;
    protected Dictionary<string, IVariable> statesVariablesTotalCache;
    protected bool variablesTotalCacheLoaded;
    protected VMParameter stateParam;
    protected IEvent startEvent;
    protected VMObjRef selfRef;
    private bool functionalsLoaded;
    private bool customEventsLoaded;
    private bool afterLoaded;
    private static HashSet<string> funcHashSet = new HashSet<string>();

    public VMLogicObject(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory()
    {
      switch ((EDataType) GuidUtility.GetTypeId(this.BaseGuid))
      {
        case EDataType.TGame:
          return EObjectCategory.OBJECT_CATEGORY_GAME;
        case EDataType.TQuest:
          return EObjectCategory.OBJECT_CATEGORY_QUEST;
        case EDataType.TCharacter:
          return EObjectCategory.OBJECT_CATEGORY_CHARACTER;
        case EDataType.TItem:
          return EObjectCategory.OBJECT_CATEGORY_ITEM;
        case EDataType.TGeom:
          return EObjectCategory.OBJECT_CATEGORY_GEOM;
        case EDataType.TWorldGroup:
          return EObjectCategory.OBJECT_CATEGORY_WORLD_GROUP;
        case EDataType.TOther:
          return EObjectCategory.OBJECT_CATEGORY_OTHERS;
        default:
          return EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;
      }
    }

    public virtual bool IsDerivedFrom(ulong iBlueprintGuid, bool bWithSelf = false)
    {
      return bWithSelf && iBlueprintGuid != 0UL && (long) this.BaseGuid == (long) iBlueprintGuid;
    }

    public virtual IFiniteStateMachine StateGraph => this.stateGraph;

    public virtual bool Static => false;

    public IGameMode GameTimeContext
    {
      get
      {
        return this.gameTimeContext == null ? ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).MainGameMode : this.gameTimeContext;
      }
    }

    public List<IObjRef> GetStaticObjects() => this.staticObjects;

    public List<IBlueprintRef> GetBlueprints()
    {
      if (this.blueprints == null || this.blueprints.Count == 0)
        this.PreloadBlueprints();
      return this.blueprints;
    }

    public List<BaseFunction> Functions => this.functions;

    public List<IEvent> Events => this.events;

    public virtual List<IEvent> CustomEvents => this.customEventsList;

    public bool IsFunctionalSupport(string sEngComponentName)
    {
      return this.FunctionalComponents.ContainsKey(sEngComponentName);
    }

    public bool IsFunctionalSupport(IEnumerable<string> functionalsList)
    {
      Dictionary<string, IFunctionalComponent> functionalComponents = this.FunctionalComponents;
      foreach (string functionals in functionalsList)
      {
        if (!functionalComponents.ContainsKey(functionals))
          return false;
      }
      return true;
    }

    public abstract IBlueprint Blueprint { get; }

    public abstract Dictionary<string, IFunctionalComponent> FunctionalComponents { get; }

    public int StandartParamsCount
    {
      get => this.standartParamsDict != null ? this.standartParamsDict.Count : 0;
    }

    public int CustomParamsCount => this.customParamsDict != null ? this.customParamsDict.Count : 0;

    public IEnumerable<string> GetComponentNames()
    {
      foreach (KeyValuePair<string, IFunctionalComponent> functionalComponent in this.FunctionalComponents)
        yield return functionalComponent.Key;
    }

    public virtual IEnumerable<IVariable> GetContextVariables(
      EContextVariableCategory contextVarCategory)
    {
      if (!this.IsUpdated)
        this.Update();
      switch (contextVarCategory)
      {
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GAME:
          VMObjRef contextVariable1 = new VMObjRef();
          contextVariable1.Initialize((IBlueprint) IStaticDataContainer.StaticDataContainer.GameRoot);
          yield return (IVariable) contextVariable1;
          break;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_OBJECT:
          foreach (IVariable staticObject in ((VMLogicObject) IStaticDataContainer.StaticDataContainer.GameRoot).GetStaticObjects())
            yield return staticObject;
          yield return this.GetSelf();
          break;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_EVENT:
          if (this.events == null)
            break;
          for (int i = 0; i < this.events.Count; ++i)
          {
            VMEventRef contextVariable2 = new VMEventRef();
            contextVariable2.Initialize(this.events[i]);
            yield return (IVariable) contextVariable2;
          }
          break;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION:
          if (this.functions == null)
            break;
          for (int i = 0; i < this.functions.Count; ++i)
            yield return (IVariable) this.functions[i];
          break;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM:
          if (this.customParamsDict != null)
          {
            foreach (KeyValuePair<string, IParam> keyValuePair in this.customParamsDict)
              yield return (IVariable) keyValuePair.Value;
          }
          if (this.standartParamsDict == null)
            break;
          foreach (KeyValuePair<string, IParam> keyValuePair in this.standartParamsDict)
            yield return (IVariable) keyValuePair.Value;
          break;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_BLUEPRINT:
          foreach (IVariable blueprint in ((VMLogicObject) IStaticDataContainer.StaticDataContainer.GameRoot).GetBlueprints())
            yield return blueprint;
          break;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_SAMPLE:
          List<ISample> samples = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).Samples;
          for (int i = 0; i < samples.Count; ++i)
          {
            VMSampleRef contextVariable3 = new VMSampleRef();
            contextVariable3.Initialize(samples[i]);
            yield return (IVariable) contextVariable3;
          }
          samples = (List<ISample>) null;
          break;
      }
    }

    public IVariable GetContextVariable(string variableName)
    {
      IVariable contextVariable = (IVariable) null;
      if (this.contextVariablesTotalCache != null)
        this.contextVariablesTotalCache.TryGetValue(variableName, out contextVariable);
      if (contextVariable == null && this.CanReloadVariablesTotalCache())
      {
        this.UpdateContextVariablesTotalCache();
        if (this.contextVariablesTotalCache != null)
          this.contextVariablesTotalCache.TryGetValue(variableName, out contextVariable);
      }
      if (contextVariable == null && this.IsVariableState(variableName))
        contextVariable = this.GetStateVariable(variableName);
      return contextVariable;
    }

    private IVariable GetStateVariable(string variableName)
    {
      if (this.statesVariablesTotalCache == null)
        this.UpdateStatesVaribalesTotalCache();
      if (this.statesVariablesTotalCache != null)
      {
        IVariable stateVariable = (IVariable) null;
        if (this.statesVariablesTotalCache.TryGetValue(variableName, out stateVariable))
          return stateVariable;
      }
      return (IVariable) null;
    }

    private bool IsVariableState(string variableName)
    {
      ulong result;
      if (GuidUtility.GetGuidFormat(variableName) != EGuidFormat.GT_BASE || !ulong.TryParse(variableName, out result))
        return false;
      EDataType typeId = (EDataType) GuidUtility.GetTypeId(result);
      switch (typeId)
      {
        case EDataType.TState:
        case EDataType.TBranch:
        case EDataType.TGraph:
        case EDataType.TTalking:
          return true;
        default:
          return typeId == EDataType.TSpeech;
      }
    }

    public virtual IEnumerable<IStateRef> GetObjectStates()
    {
      if (this.stateGraph != null)
      {
        foreach (IStateRef allState in ((FiniteStateMachine) this.stateGraph).AllStates)
          yield return allState;
      }
    }

    public IEnumerable<VMLogicObject> GetAllLogicObjects()
    {
      yield return this;
      if (this.gameObjects != null)
      {
        for (int index = 0; index < this.gameObjects.Count; ++index)
        {
          if (this.gameObjects[index] is VMLogicObject gameObject)
          {
            foreach (VMLogicObject allLogicObject in gameObject.GetAllLogicObjects())
              yield return allLogicObject;
          }
        }
      }
    }

    public IEvent GetStartEvent() => this.startEvent;

    public string GetStartEventFuncName()
    {
      return this.startEvent != null ? this.startEvent.FunctionalName : string.Empty;
    }

    public virtual VMParameter GetTemplateStateParam() => this.GetStateParam();

    public IVariable GetSelf()
    {
      if (this.selfRef == null)
      {
        this.selfRef = new VMObjRef();
        this.selfRef.Initialize((IBlueprint) this);
      }
      return (IVariable) this.selfRef;
    }

    public VMParameter GetStateParam()
    {
      if (!this.IsUpdated)
        this.Update();
      if (this.stateParam == null)
        this.UpdateStateParam();
      return this.stateParam;
    }

    public virtual void OnAfterLoad()
    {
      this.LoadFunctionals();
      this.PreLoadChildObjects();
      if (this.customParamsDict != null)
      {
        foreach (KeyValuePair<string, IParam> keyValuePair in this.customParamsDict)
        {
          if (keyValuePair.Value is VMParameter vmParameter)
            vmParameter.OnAfterLoad();
        }
      }
      if (this.standartParamsDict != null)
      {
        foreach (KeyValuePair<string, IParam> keyValuePair in this.standartParamsDict)
        {
          if (keyValuePair.Value is VMParameter vmParameter)
            vmParameter.OnAfterLoad();
        }
      }
      if (this.customEventsList != null)
      {
        for (int index = 0; index < this.customEventsList.Count; ++index)
          ((VMEvent) this.customEventsList[index]).OnAfterLoad();
      }
      if (this.StateGraph == null)
        return;
      this.startEvent = (IEvent) null;
      ulong num = 0;
      if (this.events != null)
      {
        for (int index = 0; index < this.events.Count; ++index)
        {
          if (!this.events[index].IsManual)
          {
            VMFunctionalComponent parent = (VMFunctionalComponent) this.events[index].Parent;
            if (parent.Main && (this.startEvent == null || parent.BaseGuid <= num) && this.events[index].Name == EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_START_OBJECT_FSM, typeof (VMCommon)) && this.startEvent == null)
            {
              this.startEvent = this.events[index];
              num = parent.BaseGuid;
            }
          }
        }
      }
      if (this.startEvent == null)
        Logger.AddError(string.Format("OnStart event not found in object {0}", (object) this.Name));
      if (this.stateGraph != null)
        ((VMState) this.stateGraph).OnAfterLoad();
      this.afterLoaded = true;
    }

    public override void OnPostLoad()
    {
      if (this.stateGraph == null)
        return;
      ((VMBaseObject) this.stateGraph).OnPostLoad();
    }

    public bool TryGetProperty(string name, out IParam param)
    {
      if (this.standartParamsDict != null)
        return this.standartParamsDict.TryGetValue(name, out param);
      param = (IParam) null;
      return false;
    }

    public IParam GetProperty(string componentName, string propertyName)
    {
      string key = componentName + "." + propertyName;
      if (this.standartParamsDict != null && this.standartParamsDict.ContainsKey(key))
        return this.standartParamsDict[key];
      Logger.AddError(string.Format("Property with name {0} for component {1} not found", (object) propertyName, (object) componentName));
      return (IParam) null;
    }

    public bool IsAfterLoaded => this.afterLoaded;

    public override void Clear()
    {
      base.Clear();
      if (this.gameObjects != null)
      {
        foreach (IContainer gameObject in this.gameObjects)
          gameObject.Clear();
        this.gameObjects.Clear();
        this.gameObjects = (List<IContainer>) null;
      }
      if (this.customEventsList != null)
      {
        foreach (IContainer customEvents in this.customEventsList)
          customEvents.Clear();
        this.customEventsList.Clear();
        this.customEventsList = (List<IEvent>) null;
      }
      if (this.customParamsDict != null)
      {
        foreach (KeyValuePair<string, IParam> keyValuePair in this.customParamsDict)
        {
          if (typeof (VMParameter) == keyValuePair.Value.GetType())
            ((VMParameter) keyValuePair.Value).Clear();
        }
        this.customParamsDict.Clear();
        this.customParamsDict = (Dictionary<string, IParam>) null;
      }
      if (this.standartParamsDict != null)
      {
        foreach (KeyValuePair<string, IParam> keyValuePair in this.standartParamsDict)
        {
          if (typeof (VMParameter) == keyValuePair.Value.GetType())
            ((VMParameter) keyValuePair.Value).Clear();
        }
        this.standartParamsDict.Clear();
        this.standartParamsDict = (Dictionary<string, IParam>) null;
      }
      if (this.stateGraph != null)
      {
        this.stateGraph.Clear();
        this.stateGraph = (IFiniteStateMachine) null;
      }
      if (this.functionalComponents != null)
      {
        foreach (IContainer functionalComponent in this.functionalComponents)
          functionalComponent.Clear();
        this.functionalComponents.Clear();
        this.functionalComponents = (List<IFunctionalComponent>) null;
      }
      this.gameTimeContext = (IGameMode) null;
      if (this.blueprints != null)
      {
        this.blueprints.Clear();
        this.blueprints = (List<IBlueprintRef>) null;
      }
      if (this.staticObjects != null)
      {
        this.staticObjects.Clear();
        this.staticObjects = (List<IObjRef>) null;
      }
      if (this.events != null)
      {
        foreach (IContainer container in this.events)
          container.Clear();
        this.events.Clear();
        this.events = (List<IEvent>) null;
      }
      if (this.functions != null)
      {
        foreach (BaseFunction function in this.functions)
          function.Clear();
        this.functions.Clear();
        this.functions = (List<BaseFunction>) null;
      }
      if (this.contextVariablesTotalCache != null)
      {
        this.contextVariablesTotalCache.Clear();
        this.contextVariablesTotalCache = (Dictionary<string, IVariable>) null;
      }
      if (this.statesVariablesTotalCache != null)
      {
        this.statesVariablesTotalCache.Clear();
        this.statesVariablesTotalCache = (Dictionary<string, IVariable>) null;
      }
      if (this.stateParam != null)
      {
        this.stateParam.Clear();
        this.stateParam = (VMParameter) null;
      }
      this.startEvent = (IEvent) null;
      this.selfRef = (VMObjRef) null;
    }

    private void UpdateStateParam()
    {
      if (this.customParamsDict == null)
        return;
      foreach (KeyValuePair<string, IParam> keyValuePair in this.customParamsDict)
      {
        if (keyValuePair.Key.EndsWith("_state"))
        {
          VMParameter vmParameter = (VMParameter) keyValuePair.Value;
          if (vmParameter.Implicit)
          {
            this.stateParam = vmParameter;
            break;
          }
        }
      }
    }

    public virtual bool DirectEngineCreated => false;

    protected void PreLoadChildObjects()
    {
      if (!this.customEventsLoaded)
        this.PreloadCustomEvents();
      this.PreloadChildWorldObjects();
    }

    protected void PreloadCustomEvents()
    {
      List<IEvent> customEvents = this.CustomEvents;
      if (customEvents != null)
      {
        for (int index = 0; index < customEvents.Count; ++index)
        {
          if (this.events == null)
            this.events = new List<IEvent>();
          this.events.Add(customEvents[index]);
        }
      }
      this.customEventsLoaded = true;
    }

    protected virtual void PreloadChildWorldObjects() => this.PreloadStaticObjects();

    protected virtual void UpdateContextVariablesTotalCache()
    {
      if (this.contextVariablesTotalCache != null)
        this.contextVariablesTotalCache.Clear();
      if (this.events != null)
      {
        for (int index = 0; index < this.events.Count; ++index)
        {
          VMEventRef vmEventRef = new VMEventRef();
          vmEventRef.Initialize(this.events[index]);
          this.LoadContextVaraiblesTotalCache((IVariable) vmEventRef);
        }
      }
      if (this.functions != null)
      {
        for (int index = 0; index < this.functions.Count; ++index)
          this.LoadContextVaraiblesTotalCache((IVariable) this.functions[index]);
      }
      if (this.customParamsDict != null)
      {
        foreach (KeyValuePair<string, IParam> keyValuePair in this.customParamsDict)
          this.LoadContextVaraiblesTotalCache((IVariable) keyValuePair.Value);
      }
      if (this.standartParamsDict != null)
      {
        foreach (KeyValuePair<string, IParam> keyValuePair in this.standartParamsDict)
          this.LoadContextVaraiblesTotalCache((IVariable) keyValuePair.Value);
      }
      if (this.contextVariablesTotalCache == null || !this.afterLoaded || this.contextVariablesTotalCache.Count <= 0)
        return;
      this.variablesTotalCacheLoaded = true;
    }

    protected void UpdateStatesVaribalesTotalCache()
    {
      if (this.IsVirtual || this.StateGraph == null)
        return;
      if (this.statesVariablesTotalCache == null)
        this.statesVariablesTotalCache = new Dictionary<string, IVariable>();
      else
        this.statesVariablesTotalCache.Clear();
      foreach (IStateRef objectState in this.GetObjectStates())
        this.statesVariablesTotalCache[objectState.Name] = (IVariable) objectState;
    }

    protected void LoadContextVaraiblesTotalCache(IVariable variable)
    {
      if (this.contextVariablesTotalCache == null)
        this.contextVariablesTotalCache = new Dictionary<string, IVariable>();
      this.contextVariablesTotalCache[variable.Name] = variable;
      switch (variable)
      {
        case IObject @object:
          this.contextVariablesTotalCache[@object.GuidStr] = variable;
          break;
        case IRef @ref:
          if (variable.Category != EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_SAMPLE)
            break;
          this.contextVariablesTotalCache[@ref.StaticInstance.GuidStr] = variable;
          break;
      }
    }

    protected virtual bool CanReloadVariablesTotalCache() => !this.variablesTotalCacheLoaded;

    protected virtual void PreloadStaticObjects()
    {
      if (this.staticObjects != null)
        this.staticObjects.Clear();
      this.LoadStaticObjects(EObjectCategory.OBJECT_CATEGORY_WORLD_GROUP);
      this.LoadStaticObjects(EObjectCategory.OBJECT_CATEGORY_GEOM);
      this.LoadStaticObjects(EObjectCategory.OBJECT_CATEGORY_OTHERS);
      this.LoadStaticObjects(EObjectCategory.OBJECT_CATEGORY_ITEM);
      this.LoadStaticObjects(EObjectCategory.OBJECT_CATEGORY_CHARACTER);
      this.LoadStaticObjects(EObjectCategory.OBJECT_CATEGORY_QUEST);
    }

    private void LoadStaticObjects(EObjectCategory eLoadObjCategory)
    {
      if (this.gameObjects == null)
        return;
      for (int index = 0; index < this.gameObjects.Count; ++index)
      {
        IContainer gameObject = this.gameObjects[index];
        if (gameObject.GetCategory() == eLoadObjCategory)
        {
          VMBlueprint vmBlueprint = (VMBlueprint) gameObject;
          if (vmBlueprint.Static)
          {
            VMObjRef vmObjRef = new VMObjRef();
            vmObjRef.Initialize((IBlueprint) vmBlueprint);
            if (this.staticObjects == null)
              this.staticObjects = new List<IObjRef>();
            this.staticObjects.Add((IObjRef) vmObjRef);
          }
          List<IObjRef> staticObjects = vmBlueprint.GetStaticObjects();
          if (staticObjects != null)
          {
            if (this.staticObjects == null)
              this.staticObjects = new List<IObjRef>();
            this.staticObjects.AddRange((IEnumerable<IObjRef>) staticObjects);
          }
        }
      }
    }

    protected void PreloadBlueprints()
    {
      if (this.gameObjects == null || this.gameObjects.Count == 0)
        return;
      if (this.blueprints == null)
        this.blueprints = new List<IBlueprintRef>();
      this.blueprints.Clear();
      this.blueprints.Capacity = this.gameObjects.Count * 2;
      for (int index1 = 0; index1 < this.gameObjects.Count; ++index1)
      {
        IContainer gameObject = this.gameObjects[index1];
        if (gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_CLASS || gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_QUEST || gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_CHARACTER || gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_ITEM || gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GEOM || gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_WORLD_GROUP || gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_OTHERS)
        {
          VMBlueprint vmBlueprint = (VMBlueprint) gameObject;
          if (!vmBlueprint.Static || gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_CLASS)
          {
            VMBlueprintRef vmBlueprintRef = new VMBlueprintRef();
            vmBlueprintRef.Initialize((IBlueprint) vmBlueprint);
            this.blueprints.Add((IBlueprintRef) vmBlueprintRef);
          }
          List<IBlueprintRef> blueprints = vmBlueprint.GetBlueprints();
          if (blueprints != null)
          {
            for (int index2 = 0; index2 < blueprints.Count; ++index2)
              this.blueprints.Add(blueprints[index2]);
          }
        }
      }
    }

    private void LoadFunctionals()
    {
      if (this.functionalsLoaded)
        return;
      for (int index = 0; index < this.functionalComponents.Count; ++index)
        ((VMFunctionalComponent) this.functionalComponents[index]).OnAfterLoad();
      if (this.events != null)
        this.events.Clear();
      if (this.functions != null)
        this.functions.Clear();
      Dictionary<string, IFunctionalComponent> functionalComponents = this.FunctionalComponents;
      VMLogicObject.funcHashSet.Clear();
      foreach (KeyValuePair<string, IFunctionalComponent> keyValuePair in functionalComponents)
      {
        VMFunctionalComponent functionalComponent = (VMFunctionalComponent) keyValuePair.Value;
        if (!functionalComponent.IsAfterLoaded)
          functionalComponent.OnAfterLoad();
        List<IEvent> engineEvents = functionalComponent.EngineEvents;
        for (int index = 0; index < engineEvents.Count; ++index)
        {
          IEvent @event = engineEvents[index];
          if (this.events == null)
            this.events = new List<IEvent>();
          this.events.Add(@event);
        }
        List<BaseFunction> engineFunctions = functionalComponent.EngineFunctions;
        for (int index = 0; index < engineFunctions.Count; ++index)
        {
          BaseFunction baseFunction = engineFunctions[index];
          if (this.functions == null)
            this.functions = new List<BaseFunction>();
          if (VMLogicObject.funcHashSet.Contains(baseFunction.Name))
            Logger.AddError(string.Format("Function name {0} dublicated at {1}", (object) baseFunction.Name, (object) this.Name));
          else
            VMLogicObject.funcHashSet.Add(baseFunction.Name);
          this.functions.Add(baseFunction);
        }
      }
      if (functionalComponents.Count <= 0)
        return;
      this.functionalsLoaded = true;
    }
  }
}
