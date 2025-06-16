using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Comparers;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.GameLogic;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;
using VirtualMachine.Data.Customs;

namespace PLVirtualMachine.FSM;

[TypeData(EDataType.TGraph)]
[DataFactory("Graph")]
public class FiniteStateMachine :
	VMState,
	IStub,
	IEditorDataReader,
	IFiniteStateMachine,
	IState,
	IGraphObject,
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable,
	ILocalContext,
	IGraph {
	[FieldData("States", DataFieldType.Reference)]
	protected List<IState> states = new();

	[FieldData("EventLinks", DataFieldType.Reference)]
	protected List<ILink> eventLinks = new();

	[FieldData("SubstituteGraph", DataFieldType.Reference)]
	protected IFiniteStateMachine substituteGraph;

	[FieldData("GraphType")] protected EGraphType graphType;
	[FieldData("InputParamsInfo")] protected List<NameTypeData> inputParamsInfo;
	private EObjectCategory graphOwnerCategory;
	private List<IEventLink> enterLinks = new();
	private Dictionary<ulong, List<IEventLink>> enterLinksByEventGuid = new(UlongComparer.Instance);
	private Dictionary<string, List<IEventLink>> enterLinksByEventName = new();
	protected List<InputParam> inputParams = new();
	private bool inputParamsLoaded;
	private List<IFiniteStateMachine> subGraphes = new();
	private Dictionary<ulong, IState> statesDict = new(UlongComparer.Instance);
	private List<IStateRef> allStatesList = new();

	public override void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext) {
		while (xml.Read()) {
			if (xml.NodeType == XmlNodeType.Element)
				switch (xml.Name) {
					case "EntryPoints":
						entryPoints = EditorDataReadUtility.ReadReferenceList(xml, creator, entryPoints);
						continue;
					case "EventLinks":
						eventLinks = EditorDataReadUtility.ReadReferenceList(xml, creator, eventLinks);
						continue;
					case "GraphType":
						graphType = EditorDataReadUtility.ReadEnum<EGraphType>(xml);
						continue;
					case "IgnoreBlock":
						ignoreBlock = EditorDataReadUtility.ReadValue(xml, ignoreBlock);
						continue;
					case "Initial":
						initial = EditorDataReadUtility.ReadValue(xml, initial);
						continue;
					case "InputLinks":
						inputLinks = EditorDataReadUtility.ReadReferenceList(xml, creator, inputLinks);
						continue;
					case "InputParamsInfo":
						inputParamsInfo =
							EditorDataReadUtility.ReadEditorDataSerializableList(xml, creator, inputParamsInfo);
						continue;
					case "Name":
						name = EditorDataReadUtility.ReadValue(xml, name);
						continue;
					case "OutputLinks":
						outputLinks = EditorDataReadUtility.ReadReferenceList(xml, creator, outputLinks);
						continue;
					case "Owner":
						owner = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
						continue;
					case "Parent":
						parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
						continue;
					case "States":
						states = EditorDataReadUtility.ReadReferenceList(xml, creator, states);
						continue;
					case "SubstituteGraph":
						substituteGraph = EditorDataReadUtility.ReadReference<IFiniteStateMachine>(xml, creator);
						continue;
					default:
						if (XMLDataLoader.Logs.Add(typeContext + " : " + xml.Name))
							Logger.AddError(typeContext + " : " + xml.Name);
						XmlReaderUtility.SkipNode(xml);
						continue;
				}

			if (xml.NodeType == XmlNodeType.EndElement)
				break;
		}
	}

	public FiniteStateMachine(ulong guid)
		: base(guid) { }

	public List<IEventLink> GetEnterLinksByEvent(DynamicEvent dynEvent) {
		List<IEventLink> eventLinkList;
		return enterLinksByEventGuid.TryGetValue(dynEvent.BaseGuid, out eventLinkList) ||
		       enterLinksByEventName.TryGetValue(dynEvent.StaticEvent.FunctionalName, out eventLinkList)
			? eventLinkList
			: null;
	}

	public override EStateType StateType => EStateType.STATE_TYPE_GRAPH;

	public override bool IgnoreBlock => InitState.IgnoreBlock;

	public List<IEventLink> EnterLinks =>
		substituteGraph != null ? ((FiniteStateMachine)substituteGraph).EnterLinks : enterLinks;

	public bool Abstract => substituteGraph == null && states.Count == 0;

	public IState InitState {
		get {
			if (substituteGraph != null)
				return substituteGraph.InitState;
			for (var index = 0; index < states.Count; ++index)
				if (states[index].Initial)
					return states[index];
			Logger.AddError(string.Format("Init state not found in {0}", Name));
			return null;
		}
	}

	public override EObjectCategory GetCategory() {
		return substituteGraph != null ? substituteGraph.GetCategory() : EObjectCategory.OBJECT_CATEGORY_GRAPH;
	}

	public virtual EGraphType GraphType => substituteGraph != null ? substituteGraph.GraphType : graphType;

	public List<InputParam> InputParams {
		get {
			if (substituteGraph != null)
				return substituteGraph.InputParams;
			if (!inputParamsLoaded)
				LoadInputParams();
			return inputParams;
		}
	}

	public override bool IsEqual(IObject other) {
		if (other == null) {
			Logger.AddError("null state cannot be compared");
			return false;
		}

		if (!typeof(IFiniteStateMachine).IsAssignableFrom(other.GetType()))
			return false;
		return (substituteGraph != null && substituteGraph.IsEqual(other)) || base.IsEqual(other);
	}

	public List<IState> States => substituteGraph != null ? substituteGraph.States : states;

	public List<ILink> Links => substituteGraph != null ? substituteGraph.Links : eventLinks;

	public List<ILink> GetLinksByDestState(IGraphObject state) {
		var linksByDestState = new List<ILink>();
		for (var index = 0; index < eventLinks.Count; ++index) {
			var eventLink = (IEventLink)eventLinks[index];
			if (eventLink.DestState != null && (long)eventLink.DestState.BaseGuid == (long)state.BaseGuid)
				linksByDestState.Add(eventLink);
		}

		return linksByDestState;
	}

	public List<ILink> GetLinksBySourceState(IGraphObject state) {
		var linksBySourceState = new List<ILink>();
		for (var index = 0; index < eventLinks.Count; ++index) {
			var eventLink = (IEventLink)eventLinks[index];
			if (eventLink.SourceState != null && (long)eventLink.SourceState.BaseGuid == (long)state.BaseGuid)
				linksBySourceState.Add(eventLink);
		}

		return linksBySourceState;
	}

	public EObjectCategory GraphOwnerCategory => substituteGraph != null
		? ((FiniteStateMachine)substituteGraph).GraphOwnerCategory
		: graphOwnerCategory;

	public override List<IEntryPoint> EntryPoints =>
		substituteGraph != null ? substituteGraph.EntryPoints : base.EntryPoints;

	public bool IsSubGraph => Parent.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH;

	public override bool IsProcedure => GraphType == EGraphType.GRAPH_TYPE_PROCEDURE;

	public IState GetSubgraphExitState() {
		if (substituteGraph != null)
			return substituteGraph.GetSubgraphExitState();
		for (var index = 0; index < OutputLinks.Count; ++index) {
			var outputLink = (VMEventLink)OutputLinks[index];
			if (outputLink != null && outputLink.Event == null)
				return outputLink.DestState != null
					? outputLink.DestState
					: ((IFiniteStateMachine)outputLink.Parent).GetSubgraphExitState();
		}

		return null;
	}

	public IFiniteStateMachine SubstituteGraph => substituteGraph;

	public List<IFiniteStateMachine> GetSubGraphStructure(
		EGraphType graphType = EGraphType.GRAPH_TYPE_ALL,
		bool bWithBaseClasses = true) {
		var subGraphStructure1 = new List<IFiniteStateMachine>();
		if ((GraphType == graphType || graphType == EGraphType.GRAPH_TYPE_ALL) && SubstituteGraph == null)
			subGraphStructure1.Add(this);
		for (var index = 0; index < states.Count; ++index)
			if (states[index].GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH) {
				var subGraphStructure2 = ((FiniteStateMachine)states[index]).GetSubGraphStructure(graphType, false);
				subGraphStructure1.AddRange(subGraphStructure2);
			}

		if (bWithBaseClasses && typeof(IBlueprint).IsAssignableFrom(Owner.GetType())) {
			var baseBlueprints = ((IBlueprint)Owner).BaseBlueprints;
			if (baseBlueprints != null)
				for (var index = 0; index < baseBlueprints.Count; ++index) {
					var subGraphStructure3 = baseBlueprints[index].StateGraph.GetSubGraphStructure(graphType);
					subGraphStructure1.AddRange(subGraphStructure3);
				}
		}

		return subGraphStructure1;
	}

	public IState GetStateByGuid(ulong stateId, bool withBaseClasses = true) {
		if (statesDict.ContainsKey(stateId))
			return statesDict[stateId];
		for (var index = 0; index < subGraphes.Count; ++index) {
			var stateByGuid = subGraphes[index].GetStateByGuid(stateId, false);
			if (stateByGuid != null)
				return stateByGuid;
		}

		if (withBaseClasses && typeof(IBlueprint).IsAssignableFrom(Owner.GetType())) {
			var baseBlueprints = ((IBlueprint)Owner).BaseBlueprints;
			if (baseBlueprints != null)
				for (var index = 0; index < baseBlueprints.Count; ++index)
					if (baseBlueprints[index].StateGraph != null) {
						var stateByGuid = baseBlueprints[index].StateGraph.GetStateByGuid(stateId, withBaseClasses);
						if (stateByGuid != null)
							return stateByGuid;
					}
		}

		return null;
	}

	public override void OnAfterLoad() {
		Update();
		if (owner == null)
			Logger.AddError(string.Format("Parent graph not defined at graph {0} !", Name));
		else {
			UpdateGraph();
			graphOwnerCategory = Owner.GetCategory();
			LoadEnterEventLinks();
			LoadOutputEventLinks();
			var count = states.Count;
			for (var index = 0; index < states.Count; ++index) {
				((VMState)states[index]).OnAfterLoad();
				if (typeof(FiniteStateMachine).IsAssignableFrom(states[index].GetType()))
					count += ((FiniteStateMachine)states[index]).AllStates.Count;
			}

			allStatesList.Capacity = count;
			foreach (var state in states) {
				var vmStateRef = new VMStateRef();
				vmStateRef.Initialize(state);
				allStatesList.Add(vmStateRef);
				if (typeof(FiniteStateMachine).IsAssignableFrom(state.GetType()))
					allStatesList.AddRange(((FiniteStateMachine)state).AllStates);
			}
		}
	}

	public override void OnPostLoad() {
		for (var index = 0; index < states.Count; ++index)
			((VMBaseObject)states[index]).OnPostLoad();
	}

	public List<IStateRef> AllStates => allStatesList;

	public void UpdateGraph() {
		subGraphes.Clear();
		statesDict.Clear();
		for (var index = 0; index < states.Count; ++index) {
			states[index].Update();
			if (typeof(IFiniteStateMachine).IsAssignableFrom(states[index].GetType()))
				subGraphes.Add((IFiniteStateMachine)states[index]);
			statesDict.Add(states[index].BaseGuid, states[index]);
		}

		for (var index = 0; index < eventLinks.Count; ++index)
			eventLinks[index].Update();
	}

	public override void Clear() {
		base.Clear();
		if (states != null) {
			foreach (IContainer state in states)
				state.Clear();
			states.Clear();
			states = null;
		}

		if (eventLinks != null) {
			foreach (IContainer eventLink in eventLinks)
				eventLink.Clear();
			eventLinks.Clear();
			eventLinks = null;
		}

		if (substituteGraph != null) {
			substituteGraph.Clear();
			substituteGraph = null;
		}

		if (inputParamsInfo != null) {
			inputParamsInfo.Clear();
			inputParamsInfo = null;
		}

		if (enterLinks != null) {
			foreach (IContainer enterLink in enterLinks)
				enterLink.Clear();
			enterLinks.Clear();
			enterLinks = null;
		}

		if (enterLinksByEventGuid != null) {
			foreach (var keyValuePair in enterLinksByEventGuid)
				keyValuePair.Value.Clear();
			enterLinksByEventGuid.Clear();
			enterLinksByEventGuid = null;
		}

		if (enterLinksByEventName != null) {
			foreach (var keyValuePair in enterLinksByEventName)
				keyValuePair.Value.Clear();
			enterLinksByEventName.Clear();
			enterLinksByEventName = null;
		}

		if (inputParams != null) {
			foreach (ContextVariable inputParam in inputParams)
				inputParam.Clear();
			inputParams.Clear();
			inputParams = null;
		}

		if (subGraphes != null) {
			foreach (IContainer subGraphe in subGraphes)
				subGraphe.Clear();
			subGraphes.Clear();
			subGraphes = null;
		}

		if (statesDict != null) {
			statesDict.Clear();
			statesDict = null;
		}

		if (allStatesList == null)
			return;
		allStatesList.Clear();
		allStatesList = null;
	}

	protected void LoadEnterEventLinks() {
		enterLinks.Clear();
		enterLinksByEventGuid.Clear();
		enterLinksByEventName.Clear();
		for (var index = 0; index < eventLinks.Count; ++index) {
			var eventLink = (IEventLink)eventLinks[index];
			if (eventLink.SourceState == null && ((VMEventLink)eventLink).GetParentGraphAssociatedLink() == null) {
				enterLinks.Add(eventLink);
				if (eventLink.Event != null && eventLink.Event.EventInstance != null && eventLink.Enabled) {
					var baseGuid = eventLink.Event.EventInstance.BaseGuid;
					List<IEventLink> eventLinkList;
					if (!enterLinksByEventGuid.TryGetValue(baseGuid, out eventLinkList)) {
						eventLinkList = new List<IEventLink>();
						enterLinksByEventGuid.Add(baseGuid, eventLinkList);
					}

					eventLinkList.Add(eventLink);
					var functionalName = eventLink.Event.EventInstance.FunctionalName;
					if (!enterLinksByEventName.TryGetValue(functionalName, out eventLinkList)) {
						eventLinkList = new List<IEventLink>();
						enterLinksByEventName.Add(functionalName, eventLinkList);
					}

					eventLinkList.Add(eventLink);
				}
			}
		}
	}

	private void LoadInputParams() {
		var count = inputParams.Count;
		inputParams.Clear();
		if (inputParamsInfo != null) {
			foreach (var nameTypeData in inputParamsInfo) {
				var inputParam = new InputParam();
				inputParam.Initialize(nameTypeData.Name, nameTypeData.Type);
				inputParams.Add(inputParam);
			}

			inputParamsInfo = null;
		}

		inputParamsLoaded = true;
	}
}