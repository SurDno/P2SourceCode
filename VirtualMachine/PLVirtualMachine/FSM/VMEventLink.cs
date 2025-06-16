using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.FSM
{
  [TypeData(EDataType.TGraphLink)]
  [DataFactory("GraphLink")]
  public class VMEventLink : 
    VMBaseObject,
    IStub,
    IEditorDataReader,
    IEventLink,
    ILink,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    ILocalContext,
    IParamListSource
  {
    [FieldData("Event", DataFieldType.Reference)]
    private IEvent linkEvent;
    [FieldData("EventObject", DataFieldType.None)]
    private CommonVariable linkEventObject;
    [FieldData("Source", DataFieldType.Reference)]
    private IState sourceState;
    [FieldData("SourceExitPointIndex", DataFieldType.None)]
    private int sourceExitPointIndex = -1;
    [FieldData("Destination", DataFieldType.Reference)]
    private IState destState;
    [FieldData("DestEntryPointIndex", DataFieldType.None)]
    private int destEntryPointIndex = -1;
    [FieldData("ParentGraphAssocLink", DataFieldType.Reference)]
    private VMEventLink parentGraphAssociatedLink;
    [FieldData("Enabled", DataFieldType.None)]
    private bool enabled = true;
    [FieldData("SourceParams", DataFieldType.None)]
    private List<CommonVariable> sourceParams = new List<CommonVariable>();

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "DestEntryPointIndex":
              this.destEntryPointIndex = EditorDataReadUtility.ReadValue(xml, this.destEntryPointIndex);
              continue;
            case "Destination":
              this.destState = EditorDataReadUtility.ReadReference<IState>(xml, creator);
              continue;
            case "Enabled":
              this.enabled = EditorDataReadUtility.ReadValue(xml, this.enabled);
              continue;
            case "Event":
              this.linkEvent = EditorDataReadUtility.ReadReference<IEvent>(xml, creator);
              continue;
            case "EventObject":
              this.linkEventObject = EditorDataReadUtility.ReadSerializable<CommonVariable>(xml);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "Parent":
              this.parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "ParentGraphAssocLink":
              this.parentGraphAssociatedLink = EditorDataReadUtility.ReadReference<VMEventLink>(xml, creator);
              continue;
            case "Source":
              this.sourceState = EditorDataReadUtility.ReadReference<IState>(xml, creator);
              continue;
            case "SourceExitPointIndex":
              this.sourceExitPointIndex = EditorDataReadUtility.ReadValue(xml, this.sourceExitPointIndex);
              continue;
            case "SourceParams":
              this.sourceParams = EditorDataReadUtility.ReadSerializableList<CommonVariable>(xml, this.sourceParams);
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

    public VMEventLink(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;

    public IGraphObject Source => (IGraphObject) this.sourceState;

    public IState SourceState => this.sourceState;

    public int SourceExitPoint => this.sourceExitPointIndex;

    public IGraphObject Destination => (IGraphObject) this.destState;

    public IState DestState => this.destState;

    public int DestEntryPoint => this.destEntryPointIndex;

    public EventInfo Event
    {
      get
      {
        return this.parentGraphAssociatedLink != null ? this.parentGraphAssociatedLink.Event : new EventInfo(this.linkEvent, this.linkEventObject);
      }
    }

    public override IContainer Owner
    {
      get
      {
        if (this.sourceState != null)
          return this.sourceState.Owner;
        if (this.destState != null)
          return this.destState.Owner;
        Logger.AddError(string.Format("Invalid link with id = {0}", (object) this.BaseGuid));
        return (IContainer) null;
      }
    }

    public bool IsInitial()
    {
      return this.IsValid && (this.Event.EventInstance != null && this.linkEvent != null && this.linkEvent.IsInitial((IObject) this.Owner) || this.parentGraphAssociatedLink != null);
    }

    public bool Enabled => this.enabled;

    public bool IsValid => this.destState != null || this.sourceState != null;

    public IEventLink GetParentGraphAssociatedLink() => (IEventLink) this.parentGraphAssociatedLink;

    public bool ExitFromSubGraph
    {
      get
      {
        if (this.destState != null || !((FiniteStateMachine) this.Parent).IsSubGraph)
          return false;
        return this.LinkExitType == ELinkExitType.LINK_EXIT_TYPE_OUTER_GRAPH || this.LinkExitType == ELinkExitType.LINK_EXIT_TYPE_OUTER_EVENT_EXECUTION;
      }
    }

    public ELinkExitType LinkExitType
    {
      get
      {
        return this.destState == null ? (ELinkExitType) this.destEntryPointIndex : ELinkExitType.LINK_EXIT_TYPE_NONE;
      }
    }

    public List<IVariable> GetLocalContextVariables(
      EContextVariableCategory eContextVarCategory,
      IContextElement currentElement,
      int iCounter = 0)
    {
      List<IVariable> contextVariables = new List<IVariable>();
      if (this.linkEvent != null)
      {
        List<BaseMessage> returnMessages = this.linkEvent.ReturnMessages;
        for (int index = 0; index < returnMessages.Count; ++index)
          contextVariables.Add((IVariable) returnMessages[index]);
      }
      if ((eContextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR || eContextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_ALL) && ((IFiniteStateMachine) this.Parent).InputParams.Count > 0)
      {
        for (int index = 0; index < ((IFiniteStateMachine) this.Parent).InputParams.Count; ++index)
          contextVariables.Add((IVariable) ((IFiniteStateMachine) this.Parent).InputParams[index]);
      }
      return contextVariables;
    }

    public IVariable GetLocalContextVariable(string variableUniName, IContextElement currentElement = null)
    {
      if (this.linkEvent != null)
      {
        List<BaseMessage> returnMessages = this.linkEvent.ReturnMessages;
        for (int index = 0; index < returnMessages.Count; ++index)
        {
          if (returnMessages[index].Name == variableUniName)
            return (IVariable) returnMessages[index];
        }
        if (((IFiniteStateMachine) this.Parent).InputParams.Count > 0)
        {
          for (int index = 0; index < ((IFiniteStateMachine) this.Parent).InputParams.Count; ++index)
          {
            if (((IFiniteStateMachine) this.Parent).InputParams[index].Name == variableUniName)
              return (IVariable) ((IFiniteStateMachine) this.Parent).InputParams[index];
          }
        }
      }
      return (IVariable) null;
    }

    public List<CommonVariable> SourceParams => this.sourceParams;

    public override void Clear()
    {
      base.Clear();
      if (this.linkEvent != null)
      {
        this.linkEvent.Clear();
        this.linkEvent = (IEvent) null;
      }
      if (this.linkEventObject != null)
      {
        this.linkEventObject.Clear();
        this.linkEventObject = (CommonVariable) null;
      }
      if (this.sourceState != null)
        this.sourceState = (IState) null;
      if (this.destState != null)
        this.destState = (IState) null;
      this.parentGraphAssociatedLink = (VMEventLink) null;
      if (this.sourceParams == null)
        return;
      foreach (ContextVariable sourceParam in this.sourceParams)
        sourceParam.Clear();
      this.sourceParams.Clear();
      this.sourceParams = (List<CommonVariable>) null;
    }
  }
}
