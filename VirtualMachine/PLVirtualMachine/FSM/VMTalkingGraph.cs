using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Objects;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;
using VirtualMachine.Data.Customs;

namespace PLVirtualMachine.FSM
{
  [TypeData(EDataType.TTalking)]
  [DataFactory("Talking")]
  public class VMTalkingGraph : 
    FiniteStateMachine,
    IStub,
    IEditorDataReader,
    ITalkingGraph,
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
    private IGameObjectContext talkingContext;

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

    public VMTalkingGraph(ulong guid)
      : base(guid)
    {
    }

    public override bool IgnoreBlock => true;

    public bool OnlyOnce => this.ignoreBlock;

    public IVariable GetSpeechAuthorInfo(ulong id)
    {
      if (this.talkingContext == null)
      {
        Logger.AddError("Cannot load authors list: talking context not defined");
        return (IVariable) null;
      }
      if (((VMLogicObject) this.talkingContext).IsFunctionalSupport("Speaking"))
        return (long) this.talkingContext.BaseGuid == (long) id ? ((VMLogicObject) this.talkingContext).GetSelf() : (IVariable) null;
      if (this.talkingContext.GetCategory() == EObjectCategory.OBJECT_CATEGORY_CLASS && ((VMLogicObject) this.talkingContext).IsFunctionalSupport("Speaking"))
        return (long) this.talkingContext.BaseGuid == (long) id ? ((VMLogicObject) this.talkingContext).GetSelf() : (IVariable) null;
      List<IObjRef> staticObjects1 = ((VMLogicObject) this.talkingContext).GetStaticObjects();
      if (staticObjects1 != null)
      {
        for (int index = 0; index < staticObjects1.Count; ++index)
        {
          if (((VMLogicObject) staticObjects1[index]).IsFunctionalSupport("Speaking") && (long) staticObjects1[index].Object.BaseGuid == (long) id)
            return ((VMLogicObject) staticObjects1[index].Object).GetSelf();
        }
      }
      foreach (IVariable contextVariable in ((VMLogicObject) this.talkingContext).GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM))
      {
        if (typeof (IObjRef).IsAssignableFrom(contextVariable.Type.BaseType))
        {
          VMParameter speechAuthorInfo = (VMParameter) contextVariable;
          if (speechAuthorInfo.Value != null)
          {
            IObjRef objRef = (IObjRef) ((VMParameter) contextVariable).Value;
            if (objRef.Object != null && !((VMLogicObject) objRef.Object).Static && (long) speechAuthorInfo.BaseGuid == (long) id)
              return (IVariable) speechAuthorInfo;
          }
        }
      }
      List<IObjRef> staticObjects2 = ((VMLogicObject) IStaticDataContainer.StaticDataContainer.GameRoot).GetStaticObjects();
      if (staticObjects2 != null)
      {
        for (int index = 0; index < staticObjects2.Count; ++index)
        {
          if (((VMLogicObject) staticObjects2[index].Object).IsFunctionalSupport("Speaking") && (long) staticObjects2[index].Object.BaseGuid == (long) id)
            return ((VMLogicObject) staticObjects2[index].Object).GetSelf();
        }
      }
      Logger.AddError("В контексте диалога нет ни одного персонажа. Диалог не имеет авторов!");
      return (IVariable) null;
    }

    public override void OnAfterLoad()
    {
      if (this.IsAfterLoaded)
        return;
      this.talkingContext = (IGameObjectContext) ((VMBaseObject) this.Parent).Owner;
      base.OnAfterLoad();
    }

    public override void Clear()
    {
      base.Clear();
      this.talkingContext = (IGameObjectContext) null;
    }
  }
}
