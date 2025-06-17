using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using PLVirtualMachine.GameLogic;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;
using VirtualMachine.Data.Customs;

namespace PLVirtualMachine.FSM
{
  [TypeData(EDataType.TBranch)]
  [DataFactory("Branch")]
  public class VMBranch(ulong guid) :
    VMState(guid),
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
    ILocalContext {
    [FieldData("BranchConditions", DataFieldType.Reference)]
    private List<ICondition> branchesConditions = [];
    [FieldData("BranchType")]
    private EStateType branchType;
    [FieldData("BranchVariantInfo")]
    private List<NameTypeData> branchVariantInfos;
    private List<VMMessageCastInfo> messageCastInfo;

    public override void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "BranchConditions":
              branchesConditions = EditorDataReadUtility.ReadReferenceList(xml, creator, branchesConditions);
              continue;
            case "BranchType":
              branchType = EditorDataReadUtility.ReadEnum<EStateType>(xml);
              continue;
            case "BranchVariantInfo":
              branchVariantInfos = EditorDataReadUtility.ReadEditorDataSerializableList(xml, creator, branchVariantInfos);
              continue;
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

    public override EStateType StateType => branchType;

    public ICondition GetBranchCondition(int exitPntIndex)
    {
      return exitPntIndex >= 0 && exitPntIndex < branchesConditions.Count ? branchesConditions[exitPntIndex] : null;
    }

    public override int GetExitPointsCount() => branchesConditions.Count + 1;

    public VMMessageCastInfo GetBranchVariantCastInfo(int varIndex)
    {
      if (branchType != EStateType.STATE_TYPE_MESSAGE_CAST_BRANCH)
      {
        Logger.AddError("Invalid branch type for cast info get");
        return null;
      }
      if (messageCastInfo == null)
      {
        Logger.AddError(string.Format("Message cast info in branch {0} not loaded", BaseGuid));
        LoadMessageCastInfo();
      }
      if (varIndex >= 0 && varIndex < messageCastInfo.Count())
        return messageCastInfo[varIndex];
      Logger.AddError("Invalid branch variant index");
      return null;
    }

    public IVariable MakeCastedVariable(IVariable prevVariable, int varIndex)
    {
      if (messageCastInfo == null)
      {
        Logger.AddError(string.Format("Message cast info in branch {0} not loaded", BaseGuid));
        LoadMessageCastInfo();
      }
      IVariable variable = prevVariable;
      if (branchType == EStateType.STATE_TYPE_MESSAGE_CAST_BRANCH && varIndex < messageCastInfo.Count())
      {
        VMMessageCastInfo branchVariantCastInfo = GetBranchVariantCastInfo(varIndex);
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
      if (!VMBaseObjectUtility.CheckOrders(branchesConditions))
        Logger.AddError(string.Format("Branch line id={0} has invalid conditions ordering", BaseGuid));
      UpdateLinks();
    }

    public override void OnPostLoad()
    {
      if (messageCastInfo != null)
        return;
      LoadMessageCastInfo();
    }

    public override void Clear()
    {
      base.Clear();
      if (branchesConditions != null)
      {
        foreach (VMPartCondition branchesCondition in branchesConditions)
          branchesCondition.Clear();
        branchesConditions.Clear();
        branchesConditions = null;
      }
      if (branchVariantInfos != null)
      {
        branchVariantInfos.Clear();
        branchVariantInfos = null;
      }
      if (messageCastInfo == null)
        return;
      messageCastInfo.Clear();
      messageCastInfo = null;
    }

    private void LoadMessageCastInfo()
    {
      messageCastInfo = [];
      if (branchVariantInfos == null)
        return;
      foreach (NameTypeData branchVariantInfo in branchVariantInfos)
      {
        List<IVariable> contextVariables = GetLocalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE, null);
        ContextVariable message = null;
        for (int index = 0; index < contextVariables.Count; ++index)
        {
          if (contextVariables[index] is ContextVariable contextVariable && contextVariable.Name == branchVariantInfo.Name)
          {
            message = contextVariable;
            break;
          }
        }
        if (message == null)
          Logger.AddError(string.Format("Branch variant info {0} not loaded", branchVariantInfo.Name));
        else
          messageCastInfo.Add(new VMMessageCastInfo(message, branchVariantInfo.Type));
      }
      branchVariantInfos = null;
    }
  }
}
