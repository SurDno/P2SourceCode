using System.Collections.Generic;
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

namespace PLVirtualMachine.Objects;

public abstract class VMLogicObject : VMBaseObject, IOnAfterLoaded, ILogicObject, IContext, INamed {
	[FieldData("ChildObjects", DataFieldType.Reference)]
	protected List<IContainer> gameObjects;

	[FieldData("Events", DataFieldType.Reference)]
	protected List<IEvent> customEventsList;

	[FieldData("CustomParams")] protected Dictionary<string, IParam> customParamsDict;
	[FieldData("StandartParams")] protected Dictionary<string, IParam> standartParamsDict;

	[FieldData("EventGraph", DataFieldType.Reference)]
	protected IFiniteStateMachine stateGraph;

	[FieldData("FunctionalComponents", DataFieldType.Reference)]
	protected List<IFunctionalComponent> functionalComponents = new();

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
	private static HashSet<string> funcHashSet = new();

	public VMLogicObject(ulong guid)
		: base(guid) { }

	public override EObjectCategory GetCategory() {
		switch ((EDataType)GuidUtility.GetTypeId(BaseGuid)) {
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

	public virtual bool IsDerivedFrom(ulong iBlueprintGuid, bool bWithSelf = false) {
		return bWithSelf && iBlueprintGuid != 0UL && (long)BaseGuid == (long)iBlueprintGuid;
	}

	public virtual IFiniteStateMachine StateGraph => stateGraph;

	public virtual bool Static => false;

	public IGameMode GameTimeContext => gameTimeContext == null
		? ((VMGameRoot)IStaticDataContainer.StaticDataContainer.GameRoot).MainGameMode
		: gameTimeContext;

	public List<IObjRef> GetStaticObjects() {
		return staticObjects;
	}

	public List<IBlueprintRef> GetBlueprints() {
		if (blueprints == null || blueprints.Count == 0)
			PreloadBlueprints();
		return blueprints;
	}

	public List<BaseFunction> Functions => functions;

	public List<IEvent> Events => events;

	public virtual List<IEvent> CustomEvents => customEventsList;

	public bool IsFunctionalSupport(string sEngComponentName) {
		return FunctionalComponents.ContainsKey(sEngComponentName);
	}

	public bool IsFunctionalSupport(IEnumerable<string> functionalsList) {
		var functionalComponents = FunctionalComponents;
		foreach (var functionals in functionalsList)
			if (!functionalComponents.ContainsKey(functionals))
				return false;
		return true;
	}

	public abstract IBlueprint Blueprint { get; }

	public abstract Dictionary<string, IFunctionalComponent> FunctionalComponents { get; }

	public int StandartParamsCount => standartParamsDict != null ? standartParamsDict.Count : 0;

	public int CustomParamsCount => customParamsDict != null ? customParamsDict.Count : 0;

	public IEnumerable<string> GetComponentNames() {
		foreach (var functionalComponent in FunctionalComponents)
			yield return functionalComponent.Key;
	}

	public virtual IEnumerable<IVariable> GetContextVariables(
		EContextVariableCategory contextVarCategory) {
		if (!IsUpdated)
			Update();
		switch (contextVarCategory) {
			case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GAME:
				var contextVariable1 = new VMObjRef();
				contextVariable1.Initialize((IBlueprint)IStaticDataContainer.StaticDataContainer.GameRoot);
				yield return contextVariable1;
				break;
			case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_OBJECT:
				foreach (IVariable staticObject in ((VMLogicObject)IStaticDataContainer.StaticDataContainer.GameRoot)
				         .GetStaticObjects())
					yield return staticObject;
				yield return GetSelf();
				break;
			case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_EVENT:
				if (events == null)
					break;
				for (var i = 0; i < events.Count; ++i) {
					var contextVariable2 = new VMEventRef();
					contextVariable2.Initialize(events[i]);
					yield return contextVariable2;
				}

				break;
			case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION:
				if (functions == null)
					break;
				for (var i = 0; i < functions.Count; ++i)
					yield return functions[i];
				break;
			case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM:
				if (customParamsDict != null)
					foreach (var keyValuePair in customParamsDict)
						yield return keyValuePair.Value;
				if (standartParamsDict == null)
					break;
				foreach (var keyValuePair in standartParamsDict)
					yield return keyValuePair.Value;
				break;
			case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_BLUEPRINT:
				foreach (IVariable blueprint in ((VMLogicObject)IStaticDataContainer.StaticDataContainer.GameRoot)
				         .GetBlueprints())
					yield return blueprint;
				break;
			case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_SAMPLE:
				var samples = ((VMGameRoot)IStaticDataContainer.StaticDataContainer.GameRoot).Samples;
				for (var i = 0; i < samples.Count; ++i) {
					var contextVariable3 = new VMSampleRef();
					contextVariable3.Initialize(samples[i]);
					yield return contextVariable3;
				}

				samples = null;
				break;
		}
	}

	public IVariable GetContextVariable(string variableName) {
		IVariable contextVariable = null;
		if (contextVariablesTotalCache != null)
			contextVariablesTotalCache.TryGetValue(variableName, out contextVariable);
		if (contextVariable == null && CanReloadVariablesTotalCache()) {
			UpdateContextVariablesTotalCache();
			if (contextVariablesTotalCache != null)
				contextVariablesTotalCache.TryGetValue(variableName, out contextVariable);
		}

		if (contextVariable == null && IsVariableState(variableName))
			contextVariable = GetStateVariable(variableName);
		return contextVariable;
	}

	private IVariable GetStateVariable(string variableName) {
		if (statesVariablesTotalCache == null)
			UpdateStatesVaribalesTotalCache();
		if (statesVariablesTotalCache != null) {
			IVariable stateVariable = null;
			if (statesVariablesTotalCache.TryGetValue(variableName, out stateVariable))
				return stateVariable;
		}

		return null;
	}

	private bool IsVariableState(string variableName) {
		ulong result;
		if (GuidUtility.GetGuidFormat(variableName) != EGuidFormat.GT_BASE || !ulong.TryParse(variableName, out result))
			return false;
		var typeId = (EDataType)GuidUtility.GetTypeId(result);
		switch (typeId) {
			case EDataType.TState:
			case EDataType.TBranch:
			case EDataType.TGraph:
			case EDataType.TTalking:
				return true;
			default:
				return typeId == EDataType.TSpeech;
		}
	}

	public virtual IEnumerable<IStateRef> GetObjectStates() {
		if (stateGraph != null)
			foreach (var allState in ((FiniteStateMachine)stateGraph).AllStates)
				yield return allState;
	}

	public IEnumerable<VMLogicObject> GetAllLogicObjects() {
		yield return this;
		if (gameObjects != null)
			for (var index = 0; index < gameObjects.Count; ++index)
				if (gameObjects[index] is VMLogicObject gameObject)
					foreach (var allLogicObject in gameObject.GetAllLogicObjects())
						yield return allLogicObject;
	}

	public IEvent GetStartEvent() {
		return startEvent;
	}

	public string GetStartEventFuncName() {
		return startEvent != null ? startEvent.FunctionalName : string.Empty;
	}

	public virtual VMParameter GetTemplateStateParam() {
		return GetStateParam();
	}

	public IVariable GetSelf() {
		if (selfRef == null) {
			selfRef = new VMObjRef();
			selfRef.Initialize((IBlueprint)this);
		}

		return selfRef;
	}

	public VMParameter GetStateParam() {
		if (!IsUpdated)
			Update();
		if (stateParam == null)
			UpdateStateParam();
		return stateParam;
	}

	public virtual void OnAfterLoad() {
		LoadFunctionals();
		PreLoadChildObjects();
		if (customParamsDict != null)
			foreach (var keyValuePair in customParamsDict)
				if (keyValuePair.Value is VMParameter vmParameter)
					vmParameter.OnAfterLoad();
		if (standartParamsDict != null)
			foreach (var keyValuePair in standartParamsDict)
				if (keyValuePair.Value is VMParameter vmParameter)
					vmParameter.OnAfterLoad();
		if (customEventsList != null)
			for (var index = 0; index < customEventsList.Count; ++index)
				((VMEvent)customEventsList[index]).OnAfterLoad();
		if (StateGraph == null)
			return;
		startEvent = null;
		ulong num = 0;
		if (events != null)
			for (var index = 0; index < events.Count; ++index)
				if (!events[index].IsManual) {
					var parent = (VMFunctionalComponent)events[index].Parent;
					if (parent.Main && (startEvent == null || parent.BaseGuid <= num) &&
					    events[index].Name ==
					    EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_START_OBJECT_FSM,
						    typeof(VMCommon)) && startEvent == null) {
						startEvent = events[index];
						num = parent.BaseGuid;
					}
				}

		if (startEvent == null)
			Logger.AddError(string.Format("OnStart event not found in object {0}", Name));
		if (stateGraph != null)
			((VMState)stateGraph).OnAfterLoad();
		afterLoaded = true;
	}

	public override void OnPostLoad() {
		if (stateGraph == null)
			return;
		((VMBaseObject)stateGraph).OnPostLoad();
	}

	public bool TryGetProperty(string name, out IParam param) {
		if (standartParamsDict != null)
			return standartParamsDict.TryGetValue(name, out param);
		param = null;
		return false;
	}

	public IParam GetProperty(string componentName, string propertyName) {
		var key = componentName + "." + propertyName;
		if (standartParamsDict != null && standartParamsDict.ContainsKey(key))
			return standartParamsDict[key];
		Logger.AddError(
			string.Format("Property with name {0} for component {1} not found", propertyName, componentName));
		return null;
	}

	public bool IsAfterLoaded => afterLoaded;

	public override void Clear() {
		base.Clear();
		if (gameObjects != null) {
			foreach (var gameObject in gameObjects)
				gameObject.Clear();
			gameObjects.Clear();
			gameObjects = null;
		}

		if (customEventsList != null) {
			foreach (IContainer customEvents in customEventsList)
				customEvents.Clear();
			customEventsList.Clear();
			customEventsList = null;
		}

		if (customParamsDict != null) {
			foreach (var keyValuePair in customParamsDict)
				if (typeof(VMParameter) == keyValuePair.Value.GetType())
					((VMParameter)keyValuePair.Value).Clear();
			customParamsDict.Clear();
			customParamsDict = null;
		}

		if (standartParamsDict != null) {
			foreach (var keyValuePair in standartParamsDict)
				if (typeof(VMParameter) == keyValuePair.Value.GetType())
					((VMParameter)keyValuePair.Value).Clear();
			standartParamsDict.Clear();
			standartParamsDict = null;
		}

		if (stateGraph != null) {
			stateGraph.Clear();
			stateGraph = null;
		}

		if (functionalComponents != null) {
			foreach (IContainer functionalComponent in functionalComponents)
				functionalComponent.Clear();
			functionalComponents.Clear();
			functionalComponents = null;
		}

		gameTimeContext = null;
		if (blueprints != null) {
			blueprints.Clear();
			blueprints = null;
		}

		if (staticObjects != null) {
			staticObjects.Clear();
			staticObjects = null;
		}

		if (events != null) {
			foreach (IContainer container in events)
				container.Clear();
			events.Clear();
			events = null;
		}

		if (functions != null) {
			foreach (var function in functions)
				function.Clear();
			functions.Clear();
			functions = null;
		}

		if (contextVariablesTotalCache != null) {
			contextVariablesTotalCache.Clear();
			contextVariablesTotalCache = null;
		}

		if (statesVariablesTotalCache != null) {
			statesVariablesTotalCache.Clear();
			statesVariablesTotalCache = null;
		}

		if (stateParam != null) {
			stateParam.Clear();
			stateParam = null;
		}

		startEvent = null;
		selfRef = null;
	}

	private void UpdateStateParam() {
		if (customParamsDict == null)
			return;
		foreach (var keyValuePair in customParamsDict)
			if (keyValuePair.Key.EndsWith("_state")) {
				var vmParameter = (VMParameter)keyValuePair.Value;
				if (vmParameter.Implicit) {
					stateParam = vmParameter;
					break;
				}
			}
	}

	public virtual bool DirectEngineCreated => false;

	protected void PreLoadChildObjects() {
		if (!customEventsLoaded)
			PreloadCustomEvents();
		PreloadChildWorldObjects();
	}

	protected void PreloadCustomEvents() {
		var customEvents = CustomEvents;
		if (customEvents != null)
			for (var index = 0; index < customEvents.Count; ++index) {
				if (events == null)
					events = new List<IEvent>();
				events.Add(customEvents[index]);
			}

		customEventsLoaded = true;
	}

	protected virtual void PreloadChildWorldObjects() {
		PreloadStaticObjects();
	}

	protected virtual void UpdateContextVariablesTotalCache() {
		if (contextVariablesTotalCache != null)
			contextVariablesTotalCache.Clear();
		if (events != null)
			for (var index = 0; index < events.Count; ++index) {
				var vmEventRef = new VMEventRef();
				vmEventRef.Initialize(events[index]);
				LoadContextVaraiblesTotalCache(vmEventRef);
			}

		if (functions != null)
			for (var index = 0; index < functions.Count; ++index)
				LoadContextVaraiblesTotalCache(functions[index]);
		if (customParamsDict != null)
			foreach (var keyValuePair in customParamsDict)
				LoadContextVaraiblesTotalCache(keyValuePair.Value);
		if (standartParamsDict != null)
			foreach (var keyValuePair in standartParamsDict)
				LoadContextVaraiblesTotalCache(keyValuePair.Value);
		if (contextVariablesTotalCache == null || !afterLoaded || contextVariablesTotalCache.Count <= 0)
			return;
		variablesTotalCacheLoaded = true;
	}

	protected void UpdateStatesVaribalesTotalCache() {
		if (IsVirtual || StateGraph == null)
			return;
		if (statesVariablesTotalCache == null)
			statesVariablesTotalCache = new Dictionary<string, IVariable>();
		else
			statesVariablesTotalCache.Clear();
		foreach (var objectState in GetObjectStates())
			statesVariablesTotalCache[objectState.Name] = objectState;
	}

	protected void LoadContextVaraiblesTotalCache(IVariable variable) {
		if (contextVariablesTotalCache == null)
			contextVariablesTotalCache = new Dictionary<string, IVariable>();
		contextVariablesTotalCache[variable.Name] = variable;
		switch (variable) {
			case IObject @object:
				contextVariablesTotalCache[@object.GuidStr] = variable;
				break;
			case IRef @ref:
				if (variable.Category != EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_SAMPLE)
					break;
				contextVariablesTotalCache[@ref.StaticInstance.GuidStr] = variable;
				break;
		}
	}

	protected virtual bool CanReloadVariablesTotalCache() {
		return !variablesTotalCacheLoaded;
	}

	protected virtual void PreloadStaticObjects() {
		if (staticObjects != null)
			staticObjects.Clear();
		LoadStaticObjects(EObjectCategory.OBJECT_CATEGORY_WORLD_GROUP);
		LoadStaticObjects(EObjectCategory.OBJECT_CATEGORY_GEOM);
		LoadStaticObjects(EObjectCategory.OBJECT_CATEGORY_OTHERS);
		LoadStaticObjects(EObjectCategory.OBJECT_CATEGORY_ITEM);
		LoadStaticObjects(EObjectCategory.OBJECT_CATEGORY_CHARACTER);
		LoadStaticObjects(EObjectCategory.OBJECT_CATEGORY_QUEST);
	}

	private void LoadStaticObjects(EObjectCategory eLoadObjCategory) {
		if (gameObjects == null)
			return;
		for (var index = 0; index < gameObjects.Count; ++index) {
			var gameObject = gameObjects[index];
			if (gameObject.GetCategory() == eLoadObjCategory) {
				var vmBlueprint = (VMBlueprint)gameObject;
				if (vmBlueprint.Static) {
					var vmObjRef = new VMObjRef();
					vmObjRef.Initialize(vmBlueprint);
					if (this.staticObjects == null)
						this.staticObjects = new List<IObjRef>();
					this.staticObjects.Add(vmObjRef);
				}

				var staticObjects = vmBlueprint.GetStaticObjects();
				if (staticObjects != null) {
					if (this.staticObjects == null)
						this.staticObjects = new List<IObjRef>();
					this.staticObjects.AddRange(staticObjects);
				}
			}
		}
	}

	protected void PreloadBlueprints() {
		if (gameObjects == null || gameObjects.Count == 0)
			return;
		if (this.blueprints == null)
			this.blueprints = new List<IBlueprintRef>();
		this.blueprints.Clear();
		this.blueprints.Capacity = gameObjects.Count * 2;
		for (var index1 = 0; index1 < gameObjects.Count; ++index1) {
			var gameObject = gameObjects[index1];
			if (gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_CLASS ||
			    gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_QUEST ||
			    gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_CHARACTER ||
			    gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_ITEM ||
			    gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GEOM ||
			    gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_WORLD_GROUP ||
			    gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_OTHERS) {
				var vmBlueprint = (VMBlueprint)gameObject;
				if (!vmBlueprint.Static || gameObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_CLASS) {
					var vmBlueprintRef = new VMBlueprintRef();
					vmBlueprintRef.Initialize(vmBlueprint);
					this.blueprints.Add(vmBlueprintRef);
				}

				var blueprints = vmBlueprint.GetBlueprints();
				if (blueprints != null)
					for (var index2 = 0; index2 < blueprints.Count; ++index2)
						this.blueprints.Add(blueprints[index2]);
			}
		}
	}

	private void LoadFunctionals() {
		if (functionalsLoaded)
			return;
		for (var index = 0; index < this.functionalComponents.Count; ++index)
			((VMFunctionalComponent)this.functionalComponents[index]).OnAfterLoad();
		if (events != null)
			events.Clear();
		if (functions != null)
			functions.Clear();
		var functionalComponents = FunctionalComponents;
		funcHashSet.Clear();
		foreach (var keyValuePair in functionalComponents) {
			var functionalComponent = (VMFunctionalComponent)keyValuePair.Value;
			if (!functionalComponent.IsAfterLoaded)
				functionalComponent.OnAfterLoad();
			var engineEvents = functionalComponent.EngineEvents;
			for (var index = 0; index < engineEvents.Count; ++index) {
				var @event = engineEvents[index];
				if (events == null)
					events = new List<IEvent>();
				events.Add(@event);
			}

			var engineFunctions = functionalComponent.EngineFunctions;
			for (var index = 0; index < engineFunctions.Count; ++index) {
				var baseFunction = engineFunctions[index];
				if (functions == null)
					functions = new List<BaseFunction>();
				if (funcHashSet.Contains(baseFunction.Name))
					Logger.AddError(string.Format("Function name {0} dublicated at {1}", baseFunction.Name, Name));
				else
					funcHashSet.Add(baseFunction.Name);
				functions.Add(baseFunction);
			}
		}

		if (functionalComponents.Count <= 0)
			return;
		functionalsLoaded = true;
	}
}