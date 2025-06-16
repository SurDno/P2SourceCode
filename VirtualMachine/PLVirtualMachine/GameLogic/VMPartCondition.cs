using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.GameLogic
{
  [TypeData(EDataType.TPartCondition)]
  [DataFactory("PartCondition")]
  public class VMPartCondition : 
    IStub,
    IEditorDataReader,
    ICondition,
    IObject,
    IEditorBaseTemplate,
    IOrderedChild,
    IStaticUpdateable
  {
    protected ulong guid;
    [FieldData("Name")]
    protected string name = "";
    [FieldData("ConditionType")]
    private EConditionType conditionType = EConditionType.CONDITION_TYPE_CONST_FALSE;
    [FieldData("FirstExpression", DataFieldType.Reference)]
    private IExpression firstExpression;
    [FieldData("SecondExpression", DataFieldType.Reference)]
    private IExpression secondExpression;
    [FieldData("OrderIndex")]
    protected int orderIndex;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
              continue;
            case "ConditionType":
              conditionType = EditorDataReadUtility.ReadEnum<EConditionType>(xml);
              continue;
            case "FirstExpression":
              firstExpression = EditorDataReadUtility.ReadReference<IExpression>(xml, creator);
              continue;
            case "SecondExpression":
              secondExpression = EditorDataReadUtility.ReadReference<IExpression>(xml, creator);
              continue;
            case "OrderIndex":
              orderIndex = EditorDataReadUtility.ReadValue(xml, orderIndex);
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

    public VMPartCondition(ulong guid) => this.guid = guid;

    public ulong BaseGuid => guid;

    public string Name => name;

    public int Order => orderIndex;

    public virtual bool IsPart() => true;

    public EConditionType ConditionType => conditionType;

    public IExpression FirstExpression => firstExpression;

    public IExpression SecondExpression => secondExpression;

    public virtual void Update()
    {
      if (firstExpression != null)
        firstExpression.Update();
      if (secondExpression == null)
        return;
      secondExpression.Update();
    }

    public virtual bool IsUpdated
    {
      get
      {
        return (firstExpression == null || firstExpression.IsUpdated) && (secondExpression == null || secondExpression.IsUpdated);
      }
    }

    public virtual List<GameTime> GetCheckRaisingTimePoints()
    {
      List<GameTime> raisingTimePoints = new List<GameTime>();
      if ((ConditionType == EConditionType.CONDITION_TYPE_VALUE_EQUAL || ConditionType == EConditionType.CONDITION_TYPE_VALUE_LARGER_EQUAL || ConditionType == EConditionType.CONDITION_TYPE_VALUE_LARGER) && firstExpression != null && firstExpression.Type == ExpressionType.EXPRESSION_SRC_FUNCTION && firstExpression.TargetFunction != null && firstExpression.TargetFunction.EndsWith(EngineAPIManager.GetSpecialFunctionName(ESpecialFunctionName.SFN_GET_GAME_TIME, typeof (VMGameComponent))) && secondExpression != null && secondExpression.Type == ExpressionType.EXPRESSION_SRC_CONST && secondExpression.ResultType.BaseType == typeof (GameTime) && secondExpression.TargetConstant != null && secondExpression.TargetConstant.Value != null && secondExpression.TargetConstant.Value.GetType() == typeof (GameTime))
        raisingTimePoints.Add((GameTime) secondExpression.TargetConstant.Value);
      return raisingTimePoints;
    }

    public virtual List<VMParameter> GetCheckRaisingParams()
    {
      List<VMParameter> checkRaisingParams = new List<VMParameter>();
      if (conditionType != EConditionType.CONDITION_TYPE_CONST_FALSE && conditionType != EConditionType.CONDITION_TYPE_CONST_TRUE)
      {
        if (firstExpression != null && firstExpression.Type == ExpressionType.EXPRESSION_SRC_PARAM && ((VMExpression) firstExpression).TargetParam != null)
        {
          if (!((VMExpression) firstExpression).TargetParam.IsBinded)
            firstExpression.Update();
          if (((VMExpression) firstExpression).TargetParam.IsBinded && typeof (VMParameter) == ((VMExpression) firstExpression).TargetParam.Variable.GetType())
          {
            VMParameter variable = (VMParameter) ((VMExpression) firstExpression).TargetParam.Variable;
            checkRaisingParams.Add(variable);
          }
        }
        if (secondExpression != null && secondExpression.Type == ExpressionType.EXPRESSION_SRC_PARAM && ((VMExpression) secondExpression).TargetParam != null)
        {
          if (!((VMExpression) secondExpression).TargetParam.IsBinded)
            secondExpression.Update();
          if (((VMExpression) secondExpression).TargetParam.IsBinded && typeof (VMParameter) == ((VMExpression) secondExpression).TargetParam.Variable.GetType())
          {
            VMParameter variable = (VMParameter) ((VMExpression) secondExpression).TargetParam.Variable;
            checkRaisingParams.Add(variable);
          }
        }
      }
      return checkRaisingParams;
    }

    public virtual List<BaseFunction> GetCheckRaisingFunctions()
    {
      List<BaseFunction> raisingFunctions = new List<BaseFunction>();
      if (conditionType != EConditionType.CONDITION_TYPE_CONST_FALSE && conditionType != EConditionType.CONDITION_TYPE_CONST_TRUE)
      {
        if (firstExpression != null && firstExpression.Type == ExpressionType.EXPRESSION_SRC_FUNCTION)
        {
          if (((VMExpression) firstExpression).TargetFunctionInstance == null)
            firstExpression.Update();
          if (((VMExpression) firstExpression).TargetFunctionInstance != null)
            raisingFunctions.Add(((VMExpression) firstExpression).TargetFunctionInstance);
        }
        if (secondExpression != null && secondExpression.Type == ExpressionType.EXPRESSION_SRC_FUNCTION)
        {
          if (((VMExpression) secondExpression).TargetFunctionInstance == null)
            secondExpression.Update();
          if (((VMExpression) secondExpression).TargetFunctionInstance != null)
            raisingFunctions.Add(((VMExpression) secondExpression).TargetFunctionInstance);
        }
      }
      return raisingFunctions;
    }

    public virtual bool IsConstant()
    {
      return conditionType == EConditionType.CONDITION_TYPE_CONST_FALSE || conditionType == EConditionType.CONDITION_TYPE_CONST_TRUE;
    }

    public bool IsEqual(IObject other)
    {
      return other != null && typeof (VMPartCondition) == other.GetType() && (long) BaseGuid == (long) ((VMPartCondition) other).BaseGuid;
    }

    public string GuidStr => guid.ToString();

    public virtual void Clear()
    {
      if (firstExpression != null)
        ((VMExpression) firstExpression).Clear();
      if (secondExpression == null)
        return;
      ((VMExpression) secondExpression).Clear();
    }
  }
}
