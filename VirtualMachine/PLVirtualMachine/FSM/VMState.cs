using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Comparers;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using System;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Common.Utility;
using VirtualMachine.Data;

namespace PLVirtualMachine.FSM
{
  [TypeData(EDataType.TState)]
  [DataFactory("State")]
  public class VMState : 
    VMBaseObject,
    IStub,
    IEditorDataReader,
    IState,
    IGraphObject,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    ILocalContext
  {
    [FieldData("EntryPoints", DataFieldType.Reference)]
    protected List<IEntryPoint> entryPoints = new List<IEntryPoint>();
    [FieldData("Owner", DataFieldType.Reference)]
    protected IContainer owner;
    [FieldData("InputLinks", DataFieldType.Reference)]
    protected List<VMEventLink> inputLinks = new List<VMEventLink>();
    [FieldData("OutputLinks", DataFieldType.Reference)]
    protected List<ILink> outputLinks = new List<ILink>();
    [FieldData("Initial", DataFieldType.None)]
    protected bool initial;
    [FieldData("IgnoreBlock", DataFieldType.None)]
    protected bool ignoreBlock;
    private Dictionary<ulong, List<IContextElement>> localContextDependencyDict;
    private bool localVarsCacheLoaded;
    private NamedCache<IVariable> localVarsCache;
    private NamedCache<IVariable> localMessagesCache;
    private Dictionary<ulong, List<IEventLink>> outputLinksByEventGuid;
    private Dictionary<string, List<IEventLink>> outputLinksByEventName;
    private bool afterLoaded;
    private int MAX_CIRCLE_STATE_ITERATIONS_COUNT = 50;
    private bool isStable = true;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
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
            case "IgnoreBlock":
              this.ignoreBlock = EditorDataReadUtility.ReadValue(xml, this.ignoreBlock);
              continue;
            case "Initial":
              this.initial = EditorDataReadUtility.ReadValue(xml, this.initial);
              continue;
            case "InputLinks":
              this.inputLinks = EditorDataReadUtility.ReadReferenceList<VMEventLink>(xml, creator, this.inputLinks);
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

    public VMState(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;

    public virtual EStateType StateType => EStateType.STATE_TYPE_STATE;

    public virtual List<IEntryPoint> EntryPoints => this.entryPoints;

    public virtual int GetExitPointsCount() => 0;

    public List<IVariable> GetLocalContextVariables(
      EContextVariableCategory contextVarCategory,
      IContextElement currentElement,
      int counter = 0)
    {
      List<IVariable> contextVariables1 = new List<IVariable>();
      ++counter;
      int num = this.MAX_CIRCLE_STATE_ITERATIONS_COUNT;
      if (this.Parent != null && typeof (IFiniteStateMachine).IsAssignableFrom(this.Parent.GetType()))
        num = ((IFiniteStateMachine) this.Parent).States.Count;
      if (counter > num)
        return contextVariables1;
      if (contextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE || contextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_ALL)
      {
        bool flag1 = this.Initial;
        if (this.inputLinks.Count == 1 && this.inputLinks[0].SourceState == null)
          flag1 = true;
        if (flag1 && ((IFiniteStateMachine) this.Parent).IsSubGraph)
        {
          List<IVariable> contextVariables2 = ((VMState) this.Parent).GetLocalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE, (IContextElement) null, 0);
          contextVariables1.AddRange((IEnumerable<IVariable>) contextVariables2);
          if (((IFiniteStateMachine) this.Parent).InputParams.Count > 0)
          {
            for (int index = 0; index < ((IFiniteStateMachine) this.Parent).InputParams.Count; ++index)
              contextVariables1.Add((IVariable) ((IFiniteStateMachine) this.Parent).InputParams[index]);
          }
        }
        for (int index1 = 0; index1 < this.inputLinks.Count; ++index1)
        {
          if (this.inputLinks[index1].SourceState != null)
          {
            bool flag2 = true;
            if (this.inputLinks[index1].Event != null && this.inputLinks[index1].Event.EventInstance != null)
              flag2 = false;
            if (typeof (VMTalkingGraph) == this.Parent.GetType())
              flag2 = false;
            bool flag3 = typeof (IBranch).IsAssignableFrom(this.inputLinks[index1].SourceState.GetType());
            if (flag3 | flag2)
            {
              List<IVariable> contextVariables3 = ((VMState) this.inputLinks[index1].SourceState).GetLocalContextVariables(contextVarCategory, currentElement, counter);
              if (flag3)
              {
                VMBranch sourceState = (VMBranch) this.inputLinks[index1].SourceState;
                if (sourceState.StateType == EStateType.STATE_TYPE_MESSAGE_CAST_BRANCH)
                {
                  for (int index2 = 0; index2 < contextVariables3.Count; ++index2)
                    contextVariables3[index2] = sourceState.MakeCastedVariable(contextVariables3[index2], this.inputLinks[index1].SourceExitPoint);
                }
              }
              contextVariables1.AddRange((IEnumerable<IVariable>) contextVariables3);
            }
          }
          int destEntryPoint = this.inputLinks[index1].DestEntryPoint;
          if (destEntryPoint < 0 || destEntryPoint >= this.entryPoints.Count)
          {
            Logger.AddError("Wrong entry point index");
          }
          else
          {
            if (currentElement != null && this.localContextDependencyDict != null && this.localContextDependencyDict.ContainsKey(currentElement.BaseGuid))
            {
              IActionLine actionLine = (IActionLine) this.localContextDependencyDict[currentElement.BaseGuid][0];
              if (actionLine != null && (this.entryPoints[destEntryPoint] == null || this.entryPoints[destEntryPoint].ActionLine == null || (long) this.entryPoints[destEntryPoint].ActionLine.BaseGuid != (long) actionLine.BaseGuid))
                continue;
            }
            if (this.inputLinks[index1].Event.EventInstance != null)
            {
              List<BaseMessage> returnMessages = this.inputLinks[index1].Event.EventInstance.ReturnMessages;
              for (int index3 = 0; index3 < returnMessages.Count; ++index3)
                contextVariables1.Add((IVariable) returnMessages[index3]);
            }
          }
        }
        if (this.Parent != null && this.Parent.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH && ((VMState) this.Parent).IsProcedure)
        {
          List<IVariable> contextVariables4 = ((VMState) this.Parent).GetLocalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE, (IContextElement) null, counter);
          contextVariables1.AddRange((IEnumerable<IVariable>) contextVariables4);
        }
      }
      if ((contextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR || contextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_ALL) && currentElement != null && !typeof (IBranch).IsAssignableFrom(this.GetType()) && this.localContextDependencyDict != null && this.localContextDependencyDict.ContainsKey(currentElement.BaseGuid))
      {
        List<IContextElement> contextElementList = this.localContextDependencyDict[currentElement.BaseGuid];
        for (int index = 0; index < contextElementList.Count; ++index)
          contextVariables1.AddRange((IEnumerable<IVariable>) contextElementList[index].LocalContextVariables);
      }
      return contextVariables1;
    }

    public IVariable GetLocalContextVariable(string variableUniName, IContextElement currentElement = null)
    {
      if (!this.localVarsCacheLoaded)
      {
        List<IVariable> contextVariables1 = this.GetLocalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR, (IContextElement) null, 0);
        this.localVarsCache.Clear();
        for (int index = 0; index < contextVariables1.Count; ++index)
          this.localVarsCache.Add(contextVariables1[index]);
        List<IVariable> contextVariables2 = this.GetLocalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE, (IContextElement) null, 0);
        for (int index = 0; index < contextVariables2.Count; ++index)
          this.localMessagesCache.Add(contextVariables2[index]);
        this.localVarsCacheLoaded = true;
      }
      IVariable result;
      if (this.localVarsCache.TryGetValue(variableUniName, out result) || this.localMessagesCache.TryGetValue(variableUniName, out result))
        return result;
      if (currentElement != null && !typeof (IBranch).IsAssignableFrom(this.GetType()) && this.localContextDependencyDict != null && this.localContextDependencyDict.ContainsKey(currentElement.BaseGuid))
      {
        List<IContextElement> contextElementList = this.localContextDependencyDict[currentElement.BaseGuid];
        for (int index = 0; index < contextElementList.Count; ++index)
        {
          IVariable localContextVariable = contextElementList[index].GetLocalContextVariable(variableUniName);
          if (localContextVariable != null)
            return localContextVariable;
        }
      }
      return (IVariable) null;
    }

    public bool Initial => this.initial;

    public virtual bool IgnoreBlock => this.ignoreBlock;

    public override IContainer Owner => this.owner;

    public List<ILink> OutputLinks => this.outputLinks;

    public VMEventLink GetOutputLinkBySourceExitPointIndex(int iSrcPntIndex)
    {
      for (int index = 0; index < this.outputLinks.Count; ++index)
      {
        VMEventLink outputLink = (VMEventLink) this.outputLinks[index];
        if (outputLink.Enabled && outputLink.SourceExitPoint == iSrcPntIndex)
          return outputLink;
      }
      return (VMEventLink) null;
    }

    public List<IEventLink> GetOutputLinksByEventGuid(ulong eventId)
    {
      if (this.outputLinksByEventGuid == null)
        return (List<IEventLink>) null;
      List<IEventLink> linksByEventGuid;
      this.outputLinksByEventGuid.TryGetValue(eventId, out linksByEventGuid);
      return linksByEventGuid;
    }

    public List<IEventLink> GetOutputLinksByEvent(IEvent evnt)
    {
      if (this.outputLinksByEventName == null)
        return (List<IEventLink>) null;
      List<IEventLink> outputLinksByEvent;
      this.outputLinksByEventName.TryGetValue(evnt.FunctionalName, out outputLinksByEvent);
      return outputLinksByEvent;
    }

    public VMEventLink GetAfterExitLink()
    {
      for (int index = 0; index < this.outputLinks.Count; ++index)
      {
        VMEventLink outputLink = (VMEventLink) this.outputLinks[index];
        if (outputLink.Enabled && outputLink.Event.EventInstance == null)
          return outputLink;
      }
      return (VMEventLink) null;
    }

    public override void Clear()
    {
      base.Clear();
      if (this.entryPoints != null)
      {
        foreach (IEntryPoint entryPoint in this.entryPoints)
        {
          if (typeof (VMEntryPoint) == entryPoint.GetType())
            ((VMBaseObject) entryPoint).Clear();
        }
        this.entryPoints.Clear();
        this.entryPoints = (List<IEntryPoint>) null;
      }
      this.owner = (IContainer) null;
      if (this.inputLinks != null)
      {
        foreach (VMBaseObject inputLink in this.inputLinks)
          inputLink.Clear();
        this.inputLinks.Clear();
        this.inputLinks = (List<VMEventLink>) null;
      }
      if (this.outputLinks != null)
      {
        foreach (ILink outputLink in this.outputLinks)
        {
          if (typeof (VMEventLink) == outputLink.GetType())
            ((VMBaseObject) outputLink).Clear();
        }
        this.outputLinks.Clear();
        this.outputLinks = (List<ILink>) null;
      }
      if (this.localContextDependencyDict != null)
      {
        foreach (KeyValuePair<ulong, List<IContextElement>> keyValuePair in this.localContextDependencyDict)
          keyValuePair.Value.Clear();
        this.localContextDependencyDict.Clear();
        this.localContextDependencyDict = (Dictionary<ulong, List<IContextElement>>) null;
      }
      this.localVarsCache.Clear();
      this.localMessagesCache.Clear();
      if (this.outputLinksByEventGuid != null)
      {
        foreach (KeyValuePair<ulong, List<IEventLink>> keyValuePair in this.outputLinksByEventGuid)
          keyValuePair.Value.Clear();
        this.outputLinksByEventGuid.Clear();
        this.outputLinksByEventGuid = (Dictionary<ulong, List<IEventLink>>) null;
      }
      if (this.outputLinksByEventName == null)
        return;
      foreach (KeyValuePair<string, List<IEventLink>> keyValuePair in this.outputLinksByEventName)
        keyValuePair.Value.Clear();
      this.outputLinksByEventName.Clear();
      this.outputLinksByEventName = (Dictionary<string, List<IEventLink>>) null;
    }

    protected void MakeLocalContextElementsDependencys(
      IContextElement contextElement,
      List<IContextElement> prevParents = null)
    {
      if (contextElement == null)
        return;
      if (((IActionLine) contextElement).Actions.Count > 0 && this.localContextDependencyDict == null)
        this.localContextDependencyDict = new Dictionary<ulong, List<IContextElement>>((IEqualityComparer<ulong>) UlongComparer.Instance);
      foreach (IGameAction action in ((IActionLine) contextElement).Actions)
      {
        List<IContextElement> contextElementList;
        if (!this.localContextDependencyDict.TryGetValue(action.BaseGuid, out contextElementList))
        {
          contextElementList = new List<IContextElement>();
          this.localContextDependencyDict.Add(action.BaseGuid, contextElementList);
        }
        if (prevParents != null)
          contextElementList.AddRange((IEnumerable<IContextElement>) prevParents);
        contextElementList.Add(contextElement);
      }
      foreach (IGameAction action in ((IActionLine) contextElement).Actions)
      {
        if (typeof (IActionLine).IsAssignableFrom(action.GetType()))
          this.MakeLocalContextElementsDependencys((IContextElement) action, this.localContextDependencyDict[action.BaseGuid]);
      }
    }

    protected void UpdateLinks()
    {
      for (int index = 0; index < this.inputLinks.Count; ++index)
        this.inputLinks[index].Update();
      for (int index = 0; index < this.outputLinks.Count; ++index)
        this.outputLinks[index].Update();
    }

    protected IContainer CalculateOwner()
    {
      if (this.Parent == null)
        return (IContainer) null;
      return this.Parent.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH || this.Parent.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT ? ((VMState) this.Parent).CalculateOwner() : this.Parent;
    }

    public virtual void OnAfterLoad()
    {
      this.owner = this.CalculateOwner();
      if (this.owner == null)
      {
        Logger.AddError(string.Format("Parent graph not defined at state {0} !", (object) this.Name));
      }
      else
      {
        this.UpdateLinks();
        for (int index = 0; index < this.outputLinks.Count; ++index)
        {
          VMEventLink outputLink = (VMEventLink) this.outputLinks[index];
          if (outputLink.Event == null || outputLink.Event.EventInstance == null)
            break;
        }
        this.LoadOutputEventLinks();
        try
        {
          if (this.localContextDependencyDict != null)
            this.localContextDependencyDict.Clear();
          for (int index = 0; index < this.entryPoints.Count; ++index)
          {
            IActionLine actionLine = this.entryPoints[index].ActionLine;
            if (actionLine != null)
              this.MakeLocalContextElementsDependencys((IContextElement) actionLine);
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(ex.ToString());
        }
        this.afterLoaded = true;
      }
    }

    public bool IsAfterLoaded => this.afterLoaded;

    public virtual bool IsProcedure => this.Parent != null && ((IState) this.Parent).IsProcedure;

    public bool IsStable => this.isStable;

    protected void LoadOutputEventLinks()
    {
      this.isStable = true;
      for (int index = 0; index < this.outputLinks.Count; ++index)
      {
        VMEventLink outputLink = (VMEventLink) this.outputLinks[index];
        if (outputLink.Event == null)
          this.isStable = false;
        else if (outputLink.Event.EventInstance == null)
          this.isStable = false;
        else if (((VMEventLink) this.outputLinks[index]).Enabled)
        {
          if (this.outputLinksByEventGuid == null)
            this.outputLinksByEventGuid = new Dictionary<ulong, List<IEventLink>>((IEqualityComparer<ulong>) UlongComparer.Instance);
          if (!this.outputLinksByEventGuid.ContainsKey(outputLink.Event.EventInstance.BaseGuid))
            this.outputLinksByEventGuid.Add(outputLink.Event.EventInstance.BaseGuid, new List<IEventLink>());
          this.outputLinksByEventGuid[outputLink.Event.EventInstance.BaseGuid].Add((IEventLink) outputLink);
          if (this.outputLinksByEventName == null)
            this.outputLinksByEventName = new Dictionary<string, List<IEventLink>>();
          if (!this.outputLinksByEventName.ContainsKey(outputLink.Event.EventInstance.FunctionalName))
            this.outputLinksByEventName.Add(outputLink.Event.EventInstance.FunctionalName, new List<IEventLink>());
          this.outputLinksByEventName[outputLink.Event.EventInstance.FunctionalName].Add((IEventLink) outputLink);
        }
      }
    }
  }
}
