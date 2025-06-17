using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Comparers;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Common.Utility;
using VirtualMachine.Data;

namespace PLVirtualMachine.FSM
{
  [TypeData(EDataType.TState)]
  [DataFactory("State")]
  public class VMState(ulong guid) :
    VMBaseObject(guid),
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
    ILocalContext {
    [FieldData("EntryPoints", DataFieldType.Reference)]
    protected List<IEntryPoint> entryPoints = [];
    [FieldData("Owner", DataFieldType.Reference)]
    protected IContainer owner;
    [FieldData("InputLinks", DataFieldType.Reference)]
    protected List<VMEventLink> inputLinks = [];
    [FieldData("OutputLinks", DataFieldType.Reference)]
    protected List<ILink> outputLinks = [];
    [FieldData("Initial")]
    protected bool initial;
    [FieldData("IgnoreBlock")]
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
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "EntryPoints":
              entryPoints = EditorDataReadUtility.ReadReferenceList(xml, creator, entryPoints);
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

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;

    public virtual EStateType StateType => EStateType.STATE_TYPE_STATE;

    public virtual List<IEntryPoint> EntryPoints => entryPoints;

    public virtual int GetExitPointsCount() => 0;

    public List<IVariable> GetLocalContextVariables(
      EContextVariableCategory contextVarCategory,
      IContextElement currentElement,
      int counter = 0)
    {
      List<IVariable> contextVariables1 = [];
      ++counter;
      int num = MAX_CIRCLE_STATE_ITERATIONS_COUNT;
      if (Parent != null && typeof (IFiniteStateMachine).IsAssignableFrom(Parent.GetType()))
        num = ((IFiniteStateMachine) Parent).States.Count;
      if (counter > num)
        return contextVariables1;
      if (contextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE || contextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_ALL)
      {
        bool flag1 = Initial;
        if (inputLinks.Count == 1 && inputLinks[0].SourceState == null)
          flag1 = true;
        if (flag1 && ((IFiniteStateMachine) Parent).IsSubGraph)
        {
          List<IVariable> contextVariables2 = ((VMState) Parent).GetLocalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE, null);
          contextVariables1.AddRange(contextVariables2);
          if (((IFiniteStateMachine) Parent).InputParams.Count > 0)
          {
            for (int index = 0; index < ((IFiniteStateMachine) Parent).InputParams.Count; ++index)
              contextVariables1.Add(((IFiniteStateMachine) Parent).InputParams[index]);
          }
        }
        for (int index1 = 0; index1 < inputLinks.Count; ++index1)
        {
          if (inputLinks[index1].SourceState != null)
          {
            bool flag2 = true;
            if (inputLinks[index1].Event != null && inputLinks[index1].Event.EventInstance != null)
              flag2 = false;
            if (typeof (VMTalkingGraph) == Parent.GetType())
              flag2 = false;
            bool flag3 = typeof (IBranch).IsAssignableFrom(inputLinks[index1].SourceState.GetType());
            if (flag3 | flag2)
            {
              List<IVariable> contextVariables3 = ((VMState) inputLinks[index1].SourceState).GetLocalContextVariables(contextVarCategory, currentElement, counter);
              if (flag3)
              {
                VMBranch sourceState = (VMBranch) inputLinks[index1].SourceState;
                if (sourceState.StateType == EStateType.STATE_TYPE_MESSAGE_CAST_BRANCH)
                {
                  for (int index2 = 0; index2 < contextVariables3.Count; ++index2)
                    contextVariables3[index2] = sourceState.MakeCastedVariable(contextVariables3[index2], inputLinks[index1].SourceExitPoint);
                }
              }
              contextVariables1.AddRange(contextVariables3);
            }
          }
          int destEntryPoint = inputLinks[index1].DestEntryPoint;
          if (destEntryPoint < 0 || destEntryPoint >= entryPoints.Count)
          {
            Logger.AddError("Wrong entry point index");
          }
          else
          {
            if (currentElement != null && localContextDependencyDict != null && localContextDependencyDict.ContainsKey(currentElement.BaseGuid))
            {
              IActionLine actionLine = (IActionLine) localContextDependencyDict[currentElement.BaseGuid][0];
              if (actionLine != null && (entryPoints[destEntryPoint] == null || entryPoints[destEntryPoint].ActionLine == null || (long) entryPoints[destEntryPoint].ActionLine.BaseGuid != (long) actionLine.BaseGuid))
                continue;
            }
            if (inputLinks[index1].Event.EventInstance != null)
            {
              List<BaseMessage> returnMessages = inputLinks[index1].Event.EventInstance.ReturnMessages;
              for (int index3 = 0; index3 < returnMessages.Count; ++index3)
                contextVariables1.Add(returnMessages[index3]);
            }
          }
        }
        if (Parent != null && Parent.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH && ((VMState) Parent).IsProcedure)
        {
          List<IVariable> contextVariables4 = ((VMState) Parent).GetLocalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE, null, counter);
          contextVariables1.AddRange(contextVariables4);
        }
      }
      if ((contextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR || contextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_ALL) && currentElement != null && !typeof (IBranch).IsAssignableFrom(GetType()) && localContextDependencyDict != null && localContextDependencyDict.ContainsKey(currentElement.BaseGuid))
      {
        List<IContextElement> contextElementList = localContextDependencyDict[currentElement.BaseGuid];
        for (int index = 0; index < contextElementList.Count; ++index)
          contextVariables1.AddRange(contextElementList[index].LocalContextVariables);
      }
      return contextVariables1;
    }

    public IVariable GetLocalContextVariable(string variableUniName, IContextElement currentElement = null)
    {
      if (!localVarsCacheLoaded)
      {
        List<IVariable> contextVariables1 = GetLocalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR, null);
        localVarsCache.Clear();
        for (int index = 0; index < contextVariables1.Count; ++index)
          localVarsCache.Add(contextVariables1[index]);
        List<IVariable> contextVariables2 = GetLocalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE, null);
        for (int index = 0; index < contextVariables2.Count; ++index)
          localMessagesCache.Add(contextVariables2[index]);
        localVarsCacheLoaded = true;
      }

      if (localVarsCache.TryGetValue(variableUniName, out IVariable result) || localMessagesCache.TryGetValue(variableUniName, out result))
        return result;
      if (currentElement != null && !typeof (IBranch).IsAssignableFrom(GetType()) && localContextDependencyDict != null && localContextDependencyDict.ContainsKey(currentElement.BaseGuid))
      {
        List<IContextElement> contextElementList = localContextDependencyDict[currentElement.BaseGuid];
        for (int index = 0; index < contextElementList.Count; ++index)
        {
          IVariable localContextVariable = contextElementList[index].GetLocalContextVariable(variableUniName);
          if (localContextVariable != null)
            return localContextVariable;
        }
      }
      return null;
    }

    public bool Initial => initial;

    public virtual bool IgnoreBlock => ignoreBlock;

    public override IContainer Owner => owner;

    public List<ILink> OutputLinks => outputLinks;

    public VMEventLink GetOutputLinkBySourceExitPointIndex(int iSrcPntIndex)
    {
      for (int index = 0; index < outputLinks.Count; ++index)
      {
        VMEventLink outputLink = (VMEventLink) outputLinks[index];
        if (outputLink.Enabled && outputLink.SourceExitPoint == iSrcPntIndex)
          return outputLink;
      }
      return null;
    }

    public List<IEventLink> GetOutputLinksByEventGuid(ulong eventId)
    {
      if (outputLinksByEventGuid == null)
        return null;
      outputLinksByEventGuid.TryGetValue(eventId, out List<IEventLink> linksByEventGuid);
      return linksByEventGuid;
    }

    public List<IEventLink> GetOutputLinksByEvent(IEvent evnt)
    {
      if (outputLinksByEventName == null)
        return null;
      outputLinksByEventName.TryGetValue(evnt.FunctionalName, out List<IEventLink> outputLinksByEvent);
      return outputLinksByEvent;
    }

    public VMEventLink GetAfterExitLink()
    {
      for (int index = 0; index < outputLinks.Count; ++index)
      {
        VMEventLink outputLink = (VMEventLink) outputLinks[index];
        if (outputLink.Enabled && outputLink.Event.EventInstance == null)
          return outputLink;
      }
      return null;
    }

    public override void Clear()
    {
      base.Clear();
      if (entryPoints != null)
      {
        foreach (IEntryPoint entryPoint in entryPoints)
        {
          if (typeof (VMEntryPoint) == entryPoint.GetType())
            ((VMBaseObject) entryPoint).Clear();
        }
        entryPoints.Clear();
        entryPoints = null;
      }
      owner = null;
      if (inputLinks != null)
      {
        foreach (VMBaseObject inputLink in inputLinks)
          inputLink.Clear();
        inputLinks.Clear();
        inputLinks = null;
      }
      if (outputLinks != null)
      {
        foreach (ILink outputLink in outputLinks)
        {
          if (typeof (VMEventLink) == outputLink.GetType())
            ((VMBaseObject) outputLink).Clear();
        }
        outputLinks.Clear();
        outputLinks = null;
      }
      if (localContextDependencyDict != null)
      {
        foreach (KeyValuePair<ulong, List<IContextElement>> keyValuePair in localContextDependencyDict)
          keyValuePair.Value.Clear();
        localContextDependencyDict.Clear();
        localContextDependencyDict = null;
      }
      localVarsCache.Clear();
      localMessagesCache.Clear();
      if (outputLinksByEventGuid != null)
      {
        foreach (KeyValuePair<ulong, List<IEventLink>> keyValuePair in outputLinksByEventGuid)
          keyValuePair.Value.Clear();
        outputLinksByEventGuid.Clear();
        outputLinksByEventGuid = null;
      }
      if (outputLinksByEventName == null)
        return;
      foreach (KeyValuePair<string, List<IEventLink>> keyValuePair in outputLinksByEventName)
        keyValuePair.Value.Clear();
      outputLinksByEventName.Clear();
      outputLinksByEventName = null;
    }

    protected void MakeLocalContextElementsDependencys(
      IContextElement contextElement,
      List<IContextElement> prevParents = null)
    {
      if (contextElement == null)
        return;
      if (((IActionLine) contextElement).Actions.Count > 0 && localContextDependencyDict == null)
        localContextDependencyDict = new Dictionary<ulong, List<IContextElement>>(UlongComparer.Instance);
      foreach (IGameAction action in ((IActionLine) contextElement).Actions)
      {
        if (!localContextDependencyDict.TryGetValue(action.BaseGuid, out List<IContextElement> contextElementList))
        {
          contextElementList = [];
          localContextDependencyDict.Add(action.BaseGuid, contextElementList);
        }
        if (prevParents != null)
          contextElementList.AddRange(prevParents);
        contextElementList.Add(contextElement);
      }
      foreach (IGameAction action in ((IActionLine) contextElement).Actions)
      {
        if (typeof (IActionLine).IsAssignableFrom(action.GetType()))
          MakeLocalContextElementsDependencys(action, localContextDependencyDict[action.BaseGuid]);
      }
    }

    protected void UpdateLinks()
    {
      for (int index = 0; index < inputLinks.Count; ++index)
        inputLinks[index].Update();
      for (int index = 0; index < outputLinks.Count; ++index)
        outputLinks[index].Update();
    }

    protected IContainer CalculateOwner()
    {
      if (Parent == null)
        return null;
      return Parent.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH || Parent.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT ? ((VMState) Parent).CalculateOwner() : Parent;
    }

    public virtual void OnAfterLoad()
    {
      owner = CalculateOwner();
      if (owner == null)
      {
        Logger.AddError(string.Format("Parent graph not defined at state {0} !", Name));
      }
      else
      {
        UpdateLinks();
        for (int index = 0; index < outputLinks.Count; ++index)
        {
          VMEventLink outputLink = (VMEventLink) outputLinks[index];
          if (outputLink.Event == null || outputLink.Event.EventInstance == null)
            break;
        }
        LoadOutputEventLinks();
        try
        {
          if (localContextDependencyDict != null)
            localContextDependencyDict.Clear();
          for (int index = 0; index < entryPoints.Count; ++index)
          {
            IActionLine actionLine = entryPoints[index].ActionLine;
            if (actionLine != null)
              MakeLocalContextElementsDependencys(actionLine);
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(ex.ToString());
        }
        afterLoaded = true;
      }
    }

    public bool IsAfterLoaded => afterLoaded;

    public virtual bool IsProcedure => Parent != null && ((IState) Parent).IsProcedure;

    public bool IsStable => isStable;

    protected void LoadOutputEventLinks()
    {
      isStable = true;
      for (int index = 0; index < outputLinks.Count; ++index)
      {
        VMEventLink outputLink = (VMEventLink) outputLinks[index];
        if (outputLink.Event == null)
          isStable = false;
        else if (outputLink.Event.EventInstance == null)
          isStable = false;
        else if (((VMEventLink) outputLinks[index]).Enabled)
        {
          if (outputLinksByEventGuid == null)
            outputLinksByEventGuid = new Dictionary<ulong, List<IEventLink>>(UlongComparer.Instance);
          if (!outputLinksByEventGuid.ContainsKey(outputLink.Event.EventInstance.BaseGuid))
            outputLinksByEventGuid.Add(outputLink.Event.EventInstance.BaseGuid, []);
          outputLinksByEventGuid[outputLink.Event.EventInstance.BaseGuid].Add(outputLink);
          if (outputLinksByEventName == null)
            outputLinksByEventName = new Dictionary<string, List<IEventLink>>();
          if (!outputLinksByEventName.ContainsKey(outputLink.Event.EventInstance.FunctionalName))
            outputLinksByEventName.Add(outputLink.Event.EventInstance.FunctionalName, []);
          outputLinksByEventName[outputLink.Event.EventInstance.FunctionalName].Add(outputLink);
        }
      }
    }
  }
}
