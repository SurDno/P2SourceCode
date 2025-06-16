// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.FSM.FiniteStateMachine
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Comparers;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.GameLogic;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;
using VirtualMachine.Data.Customs;

#nullable disable
namespace PLVirtualMachine.FSM
{
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
    IGraph
  {
    [FieldData("States", DataFieldType.Reference)]
    protected List<IState> states = new List<IState>();
    [FieldData("EventLinks", DataFieldType.Reference)]
    protected List<ILink> eventLinks = new List<ILink>();
    [FieldData("SubstituteGraph", DataFieldType.Reference)]
    protected IFiniteStateMachine substituteGraph;
    [FieldData("GraphType", DataFieldType.None)]
    protected EGraphType graphType;
    [FieldData("InputParamsInfo", DataFieldType.None)]
    protected List<NameTypeData> inputParamsInfo;
    private EObjectCategory graphOwnerCategory;
    private List<IEventLink> enterLinks = new List<IEventLink>();
    private Dictionary<ulong, List<IEventLink>> enterLinksByEventGuid = new Dictionary<ulong, List<IEventLink>>((IEqualityComparer<ulong>) UlongComparer.Instance);
    private Dictionary<string, List<IEventLink>> enterLinksByEventName = new Dictionary<string, List<IEventLink>>();
    protected List<InputParam> inputParams = new List<InputParam>();
    private bool inputParamsLoaded;
    private List<IFiniteStateMachine> subGraphes = new List<IFiniteStateMachine>();
    private Dictionary<ulong, IState> statesDict = new Dictionary<ulong, IState>((IEqualityComparer<ulong>) UlongComparer.Instance);
    private List<IStateRef> allStatesList = new List<IStateRef>();

    public override void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "EntryPoints":
              this.entryPoints = EditorDataReadUtility.ReadReferenceList<IEntryPoint>(xml, creator, this.entryPoints);
              continue;
            case "EventLinks":
              this.eventLinks = EditorDataReadUtility.ReadReferenceList<ILink>(xml, creator, this.eventLinks);
              continue;
            case "GraphType":
              this.graphType = EditorDataReadUtility.ReadEnum<EGraphType>(xml);
              continue;
            case "IgnoreBlock":
              this.ignoreBlock = EditorDataReadUtility.ReadValue(xml, this.ignoreBlock);
              continue;
            case "Initial":
              this.initial = EditorDataReadUtility.ReadValue(xml, this.initial);
              continue;
            case "InputLinks":
              this.inputLinks = EditorDataReadUtility.ReadReferenceList<VMEventLink>(xml, creator, this.inputLinks);
              continue;
            case "InputParamsInfo":
              this.inputParamsInfo = EditorDataReadUtility.ReadEditorDataSerializableList<NameTypeData>(xml, creator, this.inputParamsInfo);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "OutputLinks":
              this.outputLinks = EditorDataReadUtility.ReadReferenceList<ILink>(xml, creator, this.outputLinks);
              continue;
            case "Owner":
              this.owner = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "Parent":
              this.parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "States":
              this.states = EditorDataReadUtility.ReadReferenceList<IState>(xml, creator, this.states);
              continue;
            case "SubstituteGraph":
              this.substituteGraph = EditorDataReadUtility.ReadReference<IFiniteStateMachine>(xml, creator);
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

    public FiniteStateMachine(ulong guid)
      : base(guid)
    {
    }

    public List<IEventLink> GetEnterLinksByEvent(DynamicEvent dynEvent)
    {
      List<IEventLink> eventLinkList;
      return this.enterLinksByEventGuid.TryGetValue(dynEvent.BaseGuid, out eventLinkList) || this.enterLinksByEventName.TryGetValue(dynEvent.StaticEvent.FunctionalName, out eventLinkList) ? eventLinkList : (List<IEventLink>) null;
    }

    public override EStateType StateType => EStateType.STATE_TYPE_GRAPH;

    public override bool IgnoreBlock => this.InitState.IgnoreBlock;

    public List<IEventLink> EnterLinks
    {
      get
      {
        return this.substituteGraph != null ? ((FiniteStateMachine) this.substituteGraph).EnterLinks : this.enterLinks;
      }
    }

    public bool Abstract => this.substituteGraph == null && this.states.Count == 0;

    public IState InitState
    {
      get
      {
        if (this.substituteGraph != null)
          return this.substituteGraph.InitState;
        for (int index = 0; index < this.states.Count; ++index)
        {
          if (this.states[index].Initial)
            return this.states[index];
        }
        Logger.AddError(string.Format("Init state not found in {0}", (object) this.Name));
        return (IState) null;
      }
    }

    public override EObjectCategory GetCategory()
    {
      return this.substituteGraph != null ? this.substituteGraph.GetCategory() : EObjectCategory.OBJECT_CATEGORY_GRAPH;
    }

    public virtual EGraphType GraphType
    {
      get => this.substituteGraph != null ? this.substituteGraph.GraphType : this.graphType;
    }

    public List<InputParam> InputParams
    {
      get
      {
        if (this.substituteGraph != null)
          return this.substituteGraph.InputParams;
        if (!this.inputParamsLoaded)
          this.LoadInputParams();
        return this.inputParams;
      }
    }

    public override bool IsEqual(IObject other)
    {
      if (other == null)
      {
        Logger.AddError("null state cannot be compared");
        return false;
      }
      if (!typeof (IFiniteStateMachine).IsAssignableFrom(other.GetType()))
        return false;
      return this.substituteGraph != null && this.substituteGraph.IsEqual(other) || base.IsEqual(other);
    }

    public List<IState> States
    {
      get => this.substituteGraph != null ? this.substituteGraph.States : this.states;
    }

    public List<ILink> Links
    {
      get => this.substituteGraph != null ? this.substituteGraph.Links : this.eventLinks;
    }

    public List<ILink> GetLinksByDestState(IGraphObject state)
    {
      List<ILink> linksByDestState = new List<ILink>();
      for (int index = 0; index < this.eventLinks.Count; ++index)
      {
        IEventLink eventLink = (IEventLink) this.eventLinks[index];
        if (eventLink.DestState != null && (long) eventLink.DestState.BaseGuid == (long) state.BaseGuid)
          linksByDestState.Add((ILink) eventLink);
      }
      return linksByDestState;
    }

    public List<ILink> GetLinksBySourceState(IGraphObject state)
    {
      List<ILink> linksBySourceState = new List<ILink>();
      for (int index = 0; index < this.eventLinks.Count; ++index)
      {
        IEventLink eventLink = (IEventLink) this.eventLinks[index];
        if (eventLink.SourceState != null && (long) eventLink.SourceState.BaseGuid == (long) state.BaseGuid)
          linksBySourceState.Add((ILink) eventLink);
      }
      return linksBySourceState;
    }

    public EObjectCategory GraphOwnerCategory
    {
      get
      {
        return this.substituteGraph != null ? ((FiniteStateMachine) this.substituteGraph).GraphOwnerCategory : this.graphOwnerCategory;
      }
    }

    public override List<IEntryPoint> EntryPoints
    {
      get => this.substituteGraph != null ? this.substituteGraph.EntryPoints : base.EntryPoints;
    }

    public bool IsSubGraph => this.Parent.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH;

    public override bool IsProcedure => this.GraphType == EGraphType.GRAPH_TYPE_PROCEDURE;

    public IState GetSubgraphExitState()
    {
      if (this.substituteGraph != null)
        return this.substituteGraph.GetSubgraphExitState();
      for (int index = 0; index < this.OutputLinks.Count; ++index)
      {
        VMEventLink outputLink = (VMEventLink) this.OutputLinks[index];
        if (outputLink != null && outputLink.Event == null)
          return outputLink.DestState != null ? outputLink.DestState : ((IFiniteStateMachine) outputLink.Parent).GetSubgraphExitState();
      }
      return (IState) null;
    }

    public IFiniteStateMachine SubstituteGraph => this.substituteGraph;

    public List<IFiniteStateMachine> GetSubGraphStructure(
      EGraphType graphType = EGraphType.GRAPH_TYPE_ALL,
      bool bWithBaseClasses = true)
    {
      List<IFiniteStateMachine> subGraphStructure1 = new List<IFiniteStateMachine>();
      if ((this.GraphType == graphType || graphType == EGraphType.GRAPH_TYPE_ALL) && this.SubstituteGraph == null)
        subGraphStructure1.Add((IFiniteStateMachine) this);
      for (int index = 0; index < this.states.Count; ++index)
      {
        if (this.states[index].GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH)
        {
          List<IFiniteStateMachine> subGraphStructure2 = ((FiniteStateMachine) this.states[index]).GetSubGraphStructure(graphType, false);
          subGraphStructure1.AddRange((IEnumerable<IFiniteStateMachine>) subGraphStructure2);
        }
      }
      if (bWithBaseClasses && typeof (IBlueprint).IsAssignableFrom(this.Owner.GetType()))
      {
        List<IBlueprint> baseBlueprints = ((IBlueprint) this.Owner).BaseBlueprints;
        if (baseBlueprints != null)
        {
          for (int index = 0; index < baseBlueprints.Count; ++index)
          {
            List<IFiniteStateMachine> subGraphStructure3 = baseBlueprints[index].StateGraph.GetSubGraphStructure(graphType);
            subGraphStructure1.AddRange((IEnumerable<IFiniteStateMachine>) subGraphStructure3);
          }
        }
      }
      return subGraphStructure1;
    }

    public IState GetStateByGuid(ulong stateId, bool withBaseClasses = true)
    {
      if (this.statesDict.ContainsKey(stateId))
        return this.statesDict[stateId];
      for (int index = 0; index < this.subGraphes.Count; ++index)
      {
        IState stateByGuid = this.subGraphes[index].GetStateByGuid(stateId, false);
        if (stateByGuid != null)
          return stateByGuid;
      }
      if (withBaseClasses && typeof (IBlueprint).IsAssignableFrom(this.Owner.GetType()))
      {
        List<IBlueprint> baseBlueprints = ((IBlueprint) this.Owner).BaseBlueprints;
        if (baseBlueprints != null)
        {
          for (int index = 0; index < baseBlueprints.Count; ++index)
          {
            if (baseBlueprints[index].StateGraph != null)
            {
              IState stateByGuid = baseBlueprints[index].StateGraph.GetStateByGuid(stateId, withBaseClasses);
              if (stateByGuid != null)
                return stateByGuid;
            }
          }
        }
      }
      return (IState) null;
    }

    public override void OnAfterLoad()
    {
      this.Update();
      if (this.owner == null)
      {
        Logger.AddError(string.Format("Parent graph not defined at graph {0} !", (object) this.Name));
      }
      else
      {
        this.UpdateGraph();
        this.graphOwnerCategory = this.Owner.GetCategory();
        this.LoadEnterEventLinks();
        this.LoadOutputEventLinks();
        int count = this.states.Count;
        for (int index = 0; index < this.states.Count; ++index)
        {
          ((VMState) this.states[index]).OnAfterLoad();
          if (typeof (FiniteStateMachine).IsAssignableFrom(this.states[index].GetType()))
            count += ((FiniteStateMachine) this.states[index]).AllStates.Count;
        }
        this.allStatesList.Capacity = count;
        foreach (IState state in this.states)
        {
          VMStateRef vmStateRef = new VMStateRef();
          vmStateRef.Initialize(state);
          this.allStatesList.Add((IStateRef) vmStateRef);
          if (typeof (FiniteStateMachine).IsAssignableFrom(state.GetType()))
            this.allStatesList.AddRange((IEnumerable<IStateRef>) ((FiniteStateMachine) state).AllStates);
        }
      }
    }

    public override void OnPostLoad()
    {
      for (int index = 0; index < this.states.Count; ++index)
        ((VMBaseObject) this.states[index]).OnPostLoad();
    }

    public List<IStateRef> AllStates => this.allStatesList;

    public void UpdateGraph()
    {
      this.subGraphes.Clear();
      this.statesDict.Clear();
      for (int index = 0; index < this.states.Count; ++index)
      {
        this.states[index].Update();
        if (typeof (IFiniteStateMachine).IsAssignableFrom(this.states[index].GetType()))
          this.subGraphes.Add((IFiniteStateMachine) this.states[index]);
        this.statesDict.Add(this.states[index].BaseGuid, this.states[index]);
      }
      for (int index = 0; index < this.eventLinks.Count; ++index)
        this.eventLinks[index].Update();
    }

    public override void Clear()
    {
      base.Clear();
      if (this.states != null)
      {
        foreach (IContainer state in this.states)
          state.Clear();
        this.states.Clear();
        this.states = (List<IState>) null;
      }
      if (this.eventLinks != null)
      {
        foreach (IContainer eventLink in this.eventLinks)
          eventLink.Clear();
        this.eventLinks.Clear();
        this.eventLinks = (List<ILink>) null;
      }
      if (this.substituteGraph != null)
      {
        this.substituteGraph.Clear();
        this.substituteGraph = (IFiniteStateMachine) null;
      }
      if (this.inputParamsInfo != null)
      {
        this.inputParamsInfo.Clear();
        this.inputParamsInfo = (List<NameTypeData>) null;
      }
      if (this.enterLinks != null)
      {
        foreach (IContainer enterLink in this.enterLinks)
          enterLink.Clear();
        this.enterLinks.Clear();
        this.enterLinks = (List<IEventLink>) null;
      }
      if (this.enterLinksByEventGuid != null)
      {
        foreach (KeyValuePair<ulong, List<IEventLink>> keyValuePair in this.enterLinksByEventGuid)
          keyValuePair.Value.Clear();
        this.enterLinksByEventGuid.Clear();
        this.enterLinksByEventGuid = (Dictionary<ulong, List<IEventLink>>) null;
      }
      if (this.enterLinksByEventName != null)
      {
        foreach (KeyValuePair<string, List<IEventLink>> keyValuePair in this.enterLinksByEventName)
          keyValuePair.Value.Clear();
        this.enterLinksByEventName.Clear();
        this.enterLinksByEventName = (Dictionary<string, List<IEventLink>>) null;
      }
      if (this.inputParams != null)
      {
        foreach (ContextVariable inputParam in this.inputParams)
          inputParam.Clear();
        this.inputParams.Clear();
        this.inputParams = (List<InputParam>) null;
      }
      if (this.subGraphes != null)
      {
        foreach (IContainer subGraphe in this.subGraphes)
          subGraphe.Clear();
        this.subGraphes.Clear();
        this.subGraphes = (List<IFiniteStateMachine>) null;
      }
      if (this.statesDict != null)
      {
        this.statesDict.Clear();
        this.statesDict = (Dictionary<ulong, IState>) null;
      }
      if (this.allStatesList == null)
        return;
      this.allStatesList.Clear();
      this.allStatesList = (List<IStateRef>) null;
    }

    protected void LoadEnterEventLinks()
    {
      this.enterLinks.Clear();
      this.enterLinksByEventGuid.Clear();
      this.enterLinksByEventName.Clear();
      for (int index = 0; index < this.eventLinks.Count; ++index)
      {
        IEventLink eventLink = (IEventLink) this.eventLinks[index];
        if (eventLink.SourceState == null && ((VMEventLink) eventLink).GetParentGraphAssociatedLink() == null)
        {
          this.enterLinks.Add(eventLink);
          if (eventLink.Event != null && eventLink.Event.EventInstance != null && eventLink.Enabled)
          {
            ulong baseGuid = eventLink.Event.EventInstance.BaseGuid;
            List<IEventLink> eventLinkList;
            if (!this.enterLinksByEventGuid.TryGetValue(baseGuid, out eventLinkList))
            {
              eventLinkList = new List<IEventLink>();
              this.enterLinksByEventGuid.Add(baseGuid, eventLinkList);
            }
            eventLinkList.Add(eventLink);
            string functionalName = eventLink.Event.EventInstance.FunctionalName;
            if (!this.enterLinksByEventName.TryGetValue(functionalName, out eventLinkList))
            {
              eventLinkList = new List<IEventLink>();
              this.enterLinksByEventName.Add(functionalName, eventLinkList);
            }
            eventLinkList.Add(eventLink);
          }
        }
      }
    }

    private void LoadInputParams()
    {
      int count = this.inputParams.Count;
      this.inputParams.Clear();
      if (this.inputParamsInfo != null)
      {
        foreach (NameTypeData nameTypeData in this.inputParamsInfo)
        {
          InputParam inputParam = new InputParam();
          inputParam.Initialize(nameTypeData.Name, nameTypeData.Type);
          this.inputParams.Add(inputParam);
        }
        this.inputParamsInfo = (List<NameTypeData>) null;
      }
      this.inputParamsLoaded = true;
    }
  }
}
