using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using PLVirtualMachine.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;
using VirtualMachine.Data.Customs;

namespace PLVirtualMachine.FSM
{
  [TypeData(EDataType.TBranch)]
  [DataFactory("Branch")]
  public class VMBranch : 
    VMState,
    IStub,
    IEditorDataReader,
    IBranch,
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
    [FieldData("BranchConditions", DataFieldType.Reference)]
    private List<ICondition> branchesConditions = new List<ICondition>();
    [FieldData("BranchType", DataFieldType.None)]
    private EStateType branchType;
    [FieldData("BranchVariantInfo", DataFieldType.None)]
    private List<NameTypeData> branchVariantInfos;
    private List<VMMessageCastInfo> messageCastInfo;

    public override void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "BranchConditions":
              this.branchesConditions = EditorDataReadUtility.ReadReferenceList<ICondition>(xml, creator, this.branchesConditions);
              continue;
            case "BranchType":
              this.branchType = EditorDataReadUtility.ReadEnum<EStateType>(xml);
              continue;
            case "BranchVariantInfo":
              this.branchVariantInfos = EditorDataReadUtility.ReadEditorDataSerializableList<NameTypeData>(xml, creator, this.branchVariantInfos);
              continue;
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

    public VMBranch(ulong guid)
      : base(guid)
    {
    }

    public override EStateType StateType => this.branchType;

    public ICondition GetBranchCondition(int exitPntIndex)
    {
      return exitPntIndex >= 0 && exitPntIndex < this.branchesConditions.Count ? this.branchesConditions[exitPntIndex] : (ICondition) null;
    }

    public override int GetExitPointsCount() => this.branchesConditions.Count + 1;

    public VMMessageCastInfo GetBranchVariantCastInfo(int varIndex)
    {
      if (this.branchType != EStateType.STATE_TYPE_MESSAGE_CAST_BRANCH)
      {
        Logger.AddError("Invalid branch type for cast info get");
        return (VMMessageCastInfo) null;
      }
      if (this.messageCastInfo == null)
      {
        Logger.AddError(string.Format("Message cast info in branch {0} not loaded", (object) this.BaseGuid));
        this.LoadMessageCastInfo();
      }
      if (varIndex >= 0 && varIndex < this.messageCastInfo.Count<VMMessageCastInfo>())
        return this.messageCastInfo[varIndex];
      Logger.AddError("Invalid branch variant index");
      return (VMMessageCastInfo) null;
    }

    public IVariable MakeCastedVariable(IVariable prevVariable, int varIndex)
    {
      if (this.messageCastInfo == null)
      {
        Logger.AddError(string.Format("Message cast info in branch {0} not loaded", (object) this.BaseGuid));
        this.LoadMessageCastInfo();
      }
      IVariable variable = prevVariable;
      if (this.branchType == EStateType.STATE_TYPE_MESSAGE_CAST_BRANCH && varIndex < this.messageCastInfo.Count<VMMessageCastInfo>())
      {
        VMMessageCastInfo branchVariantCastInfo = this.GetBranchVariantCastInfo(varIndex);
        if (branchVariantCastInfo.Message != null && branchVariantCastInfo.CastType != null && branchVariantCastInfo.Message.Name == prevVariable.Name)
        {
          string name = prevVariable.Name;
          Type type = prevVariable.GetType();
          variable = (IVariable) Activator.CreateInstance(type);
          if (typeof (ContextVariable).IsAssignableFrom(type))
            ((ContextVariable) variable).Initialize(name, branchVariantCastInfo.CastType);
        }
      }
      return variable;
    }

    public override void OnAfterLoad()
    {
      if (!VMBaseObjectUtility.CheckOrders<ICondition>(this.branchesConditions))
        Logger.AddError(string.Format("Branch line id={0} has invalid conditions ordering", (object) this.BaseGuid));
      this.UpdateLinks();
    }

    public override void OnPostLoad()
    {
      if (this.messageCastInfo != null)
        return;
      this.LoadMessageCastInfo();
    }

    public override void Clear()
    {
      base.Clear();
      if (this.branchesConditions != null)
      {
        foreach (VMPartCondition branchesCondition in this.branchesConditions)
          branchesCondition.Clear();
        this.branchesConditions.Clear();
        this.branchesConditions = (List<ICondition>) null;
      }
      if (this.branchVariantInfos != null)
      {
        this.branchVariantInfos.Clear();
        this.branchVariantInfos = (List<NameTypeData>) null;
      }
      if (this.messageCastInfo == null)
        return;
      this.messageCastInfo.Clear();
      this.messageCastInfo = (List<VMMessageCastInfo>) null;
    }

    private void LoadMessageCastInfo()
    {
      this.messageCastInfo = new List<VMMessageCastInfo>();
      if (this.branchVariantInfos == null)
        return;
      foreach (NameTypeData branchVariantInfo in this.branchVariantInfos)
      {
        List<IVariable> contextVariables = this.GetLocalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE, (IContextElement) null, 0);
        ContextVariable message = (ContextVariable) null;
        for (int index = 0; index < contextVariables.Count; ++index)
        {
          if (contextVariables[index] is ContextVariable contextVariable && contextVariable.Name == branchVariantInfo.Name)
          {
            message = contextVariable;
            break;
          }
        }
        if (message == null)
          Logger.AddError(string.Format("Branch variant info {0} not loaded", (object) branchVariantInfo.Name));
        else
          this.messageCastInfo.Add(new VMMessageCastInfo(message, branchVariantInfo.Type));
      }
      this.branchVariantInfos = (List<NameTypeData>) null;
    }
  }
}
