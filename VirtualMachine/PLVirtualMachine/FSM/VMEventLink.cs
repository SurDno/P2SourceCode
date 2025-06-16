using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
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
    [FieldData("EventObject")]
    private CommonVariable linkEventObject;
    [FieldData("Source", DataFieldType.Reference)]
    private IState sourceState;
    [FieldData("SourceExitPointIndex")]
    private int sourceExitPointIndex = -1;
    [FieldData("Destination", DataFieldType.Reference)]
    private IState destState;
    [FieldData("DestEntryPointIndex")]
    private int destEntryPointIndex = -1;
    [FieldData("ParentGraphAssocLink", DataFieldType.Reference)]
    private VMEventLink parentGraphAssociatedLink;
    [FieldData("Enabled")]
    private bool enabled = true;
    [FieldData("SourceParams")]
    private List<CommonVariable> sourceParams = new List<CommonVariable>();

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "DestEntryPointIndex":
              destEntryPointIndex = EditorDataReadUtility.ReadValue(xml, destEntryPointIndex);
              continue;
            case "Destination":
              destState = EditorDataReadUtility.ReadReference<IState>(xml, creator);
              continue;
            case "Enabled":
              enabled = EditorDataReadUtility.ReadValue(xml, enabled);
              continue;
            case "Event":
              linkEvent = EditorDataReadUtility.ReadReference<IEvent>(xml, creator);
              continue;
            case "EventObject":
              linkEventObject = EditorDataReadUtility.ReadSerializable<CommonVariable>(xml);
              continue;
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
              continue;
            case "Parent":
              parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "ParentGraphAssocLink":
              parentGraphAssociatedLink = EditorDataReadUtility.ReadReference<VMEventLink>(xml, creator);
              continue;
            case "Source":
              sourceState = EditorDataReadUtility.ReadReference<IState>(xml, creator);
              continue;
            case "SourceExitPointIndex":
              sourceExitPointIndex = EditorDataReadUtility.ReadValue(xml, sourceExitPointIndex);
              continue;
            case "SourceParams":
              sourceParams = EditorDataReadUtility.ReadSerializableList(xml, sourceParams);
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

    public VMEventLink(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;

    public IGraphObject Source => sourceState;

    public IState SourceState => sourceState;

    public int SourceExitPoint => sourceExitPointIndex;

    public IGraphObject Destination => destState;

    public IState DestState => destState;

    public int DestEntryPoint => destEntryPointIndex;

    public EventInfo Event
    {
      get
      {
        return parentGraphAssociatedLink != null ? parentGraphAssociatedLink.Event : new EventInfo(linkEvent, linkEventObject);
      }
    }

    public override IContainer Owner
    {
      get
      {
        if (sourceState != null)
          return sourceState.Owner;
        if (destState != null)
          return destState.Owner;
        Logger.AddError(string.Format("Invalid link with id = {0}", BaseGuid));
        return null;
      }
    }

    public bool IsInitial()
    {
      return IsValid && (Event.EventInstance != null && linkEvent != null && linkEvent.IsInitial(Owner) || parentGraphAssociatedLink != null);
    }

    public bool Enabled => enabled;

    public bool IsValid => destState != null || sourceState != null;

    public IEventLink GetParentGraphAssociatedLink() => parentGraphAssociatedLink;

    public bool ExitFromSubGraph
    {
      get
      {
        if (destState != null || !((FiniteStateMachine) Parent).IsSubGraph)
          return false;
        return LinkExitType == ELinkExitType.LINK_EXIT_TYPE_OUTER_GRAPH || LinkExitType == ELinkExitType.LINK_EXIT_TYPE_OUTER_EVENT_EXECUTION;
      }
    }

    public ELinkExitType LinkExitType
    {
      get
      {
        return destState == null ? (ELinkExitType) destEntryPointIndex : ELinkExitType.LINK_EXIT_TYPE_NONE;
      }
    }

    public List<IVariable> GetLocalContextVariables(
      EContextVariableCategory eContextVarCategory,
      IContextElement currentElement,
      int iCounter = 0)
    {
      List<IVariable> contextVariables = new List<IVariable>();
      if (linkEvent != null)
      {
        List<BaseMessage> returnMessages = linkEvent.ReturnMessages;
        for (int index = 0; index < returnMessages.Count; ++index)
          contextVariables.Add(returnMessages[index]);
      }
      if ((eContextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR || eContextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_ALL) && ((IFiniteStateMachine) Parent).InputParams.Count > 0)
      {
        for (int index = 0; index < ((IFiniteStateMachine) Parent).InputParams.Count; ++index)
          contextVariables.Add(((IFiniteStateMachine) Parent).InputParams[index]);
      }
      return contextVariables;
    }

    public IVariable GetLocalContextVariable(string variableUniName, IContextElement currentElement = null)
    {
      if (linkEvent != null)
      {
        List<BaseMessage> returnMessages = linkEvent.ReturnMessages;
        for (int index = 0; index < returnMessages.Count; ++index)
        {
          if (returnMessages[index].Name == variableUniName)
            return returnMessages[index];
        }
        if (((IFiniteStateMachine) Parent).InputParams.Count > 0)
        {
          for (int index = 0; index < ((IFiniteStateMachine) Parent).InputParams.Count; ++index)
          {
            if (((IFiniteStateMachine) Parent).InputParams[index].Name == variableUniName)
              return ((IFiniteStateMachine) Parent).InputParams[index];
          }
        }
      }
      return null;
    }

    public List<CommonVariable> SourceParams => sourceParams;

    public override void Clear()
    {
      base.Clear();
      if (linkEvent != null)
      {
        linkEvent.Clear();
        linkEvent = null;
      }
      if (linkEventObject != null)
      {
        linkEventObject.Clear();
        linkEventObject = null;
      }
      if (sourceState != null)
        sourceState = null;
      if (destState != null)
        destState = null;
      parentGraphAssociatedLink = null;
      if (sourceParams == null)
        return;
      foreach (ContextVariable sourceParam in sourceParams)
        sourceParam.Clear();
      sourceParams.Clear();
      sourceParams = null;
    }
  }
}
