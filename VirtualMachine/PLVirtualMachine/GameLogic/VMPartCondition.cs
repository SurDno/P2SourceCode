// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.GameLogic.VMPartCondition
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Data;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

#nullable disable
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
    [FieldData("Name", DataFieldType.None)]
    protected string name = "";
    [FieldData("ConditionType", DataFieldType.None)]
    private EConditionType conditionType = EConditionType.CONDITION_TYPE_CONST_FALSE;
    [FieldData("FirstExpression", DataFieldType.Reference)]
    private IExpression firstExpression;
    [FieldData("SecondExpression", DataFieldType.Reference)]
    private IExpression secondExpression;
    [FieldData("OrderIndex", DataFieldType.None)]
    protected int orderIndex;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "ConditionType":
              this.conditionType = EditorDataReadUtility.ReadEnum<EConditionType>(xml);
              continue;
            case "FirstExpression":
              this.firstExpression = EditorDataReadUtility.ReadReference<IExpression>(xml, creator);
              continue;
            case "SecondExpression":
              this.secondExpression = EditorDataReadUtility.ReadReference<IExpression>(xml, creator);
              continue;
            case "OrderIndex":
              this.orderIndex = EditorDataReadUtility.ReadValue(xml, this.orderIndex);
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

    public VMPartCondition(ulong guid) => this.guid = guid;

    public ulong BaseGuid => this.guid;

    public string Name => this.name;

    public int Order => this.orderIndex;

    public virtual bool IsPart() => true;

    public EConditionType ConditionType => this.conditionType;

    public IExpression FirstExpression => this.firstExpression;

    public IExpression SecondExpression => this.secondExpression;

    public virtual void Update()
    {
      if (this.firstExpression != null)
        this.firstExpression.Update();
      if (this.secondExpression == null)
        return;
      this.secondExpression.Update();
    }

    public virtual bool IsUpdated
    {
      get
      {
        return (this.firstExpression == null || this.firstExpression.IsUpdated) && (this.secondExpression == null || this.secondExpression.IsUpdated);
      }
    }

    public virtual List<GameTime> GetCheckRaisingTimePoints()
    {
      List<GameTime> raisingTimePoints = new List<GameTime>();
      if ((this.ConditionType == EConditionType.CONDITION_TYPE_VALUE_EQUAL || this.ConditionType == EConditionType.CONDITION_TYPE_VALUE_LARGER_EQUAL || this.ConditionType == EConditionType.CONDITION_TYPE_VALUE_LARGER) && this.firstExpression != null && this.firstExpression.Type == ExpressionType.EXPRESSION_SRC_FUNCTION && this.firstExpression.TargetFunction != null && this.firstExpression.TargetFunction.EndsWith(EngineAPIManager.GetSpecialFunctionName(ESpecialFunctionName.SFN_GET_GAME_TIME, typeof (VMGameComponent))) && this.secondExpression != null && this.secondExpression.Type == ExpressionType.EXPRESSION_SRC_CONST && this.secondExpression.ResultType.BaseType == typeof (GameTime) && this.secondExpression.TargetConstant != null && this.secondExpression.TargetConstant.Value != null && this.secondExpression.TargetConstant.Value.GetType() == typeof (GameTime))
        raisingTimePoints.Add((GameTime) this.secondExpression.TargetConstant.Value);
      return raisingTimePoints;
    }

    public virtual List<VMParameter> GetCheckRaisingParams()
    {
      List<VMParameter> checkRaisingParams = new List<VMParameter>();
      if (this.conditionType != EConditionType.CONDITION_TYPE_CONST_FALSE && this.conditionType != EConditionType.CONDITION_TYPE_CONST_TRUE)
      {
        if (this.firstExpression != null && this.firstExpression.Type == ExpressionType.EXPRESSION_SRC_PARAM && ((VMExpression) this.firstExpression).TargetParam != null)
        {
          if (!((VMExpression) this.firstExpression).TargetParam.IsBinded)
            this.firstExpression.Update();
          if (((VMExpression) this.firstExpression).TargetParam.IsBinded && typeof (VMParameter) == ((VMExpression) this.firstExpression).TargetParam.Variable.GetType())
          {
            VMParameter variable = (VMParameter) ((VMExpression) this.firstExpression).TargetParam.Variable;
            checkRaisingParams.Add(variable);
          }
        }
        if (this.secondExpression != null && this.secondExpression.Type == ExpressionType.EXPRESSION_SRC_PARAM && ((VMExpression) this.secondExpression).TargetParam != null)
        {
          if (!((VMExpression) this.secondExpression).TargetParam.IsBinded)
            this.secondExpression.Update();
          if (((VMExpression) this.secondExpression).TargetParam.IsBinded && typeof (VMParameter) == ((VMExpression) this.secondExpression).TargetParam.Variable.GetType())
          {
            VMParameter variable = (VMParameter) ((VMExpression) this.secondExpression).TargetParam.Variable;
            checkRaisingParams.Add(variable);
          }
        }
      }
      return checkRaisingParams;
    }

    public virtual List<BaseFunction> GetCheckRaisingFunctions()
    {
      List<BaseFunction> raisingFunctions = new List<BaseFunction>();
      if (this.conditionType != EConditionType.CONDITION_TYPE_CONST_FALSE && this.conditionType != EConditionType.CONDITION_TYPE_CONST_TRUE)
      {
        if (this.firstExpression != null && this.firstExpression.Type == ExpressionType.EXPRESSION_SRC_FUNCTION)
        {
          if (((VMExpression) this.firstExpression).TargetFunctionInstance == null)
            this.firstExpression.Update();
          if (((VMExpression) this.firstExpression).TargetFunctionInstance != null)
            raisingFunctions.Add(((VMExpression) this.firstExpression).TargetFunctionInstance);
        }
        if (this.secondExpression != null && this.secondExpression.Type == ExpressionType.EXPRESSION_SRC_FUNCTION)
        {
          if (((VMExpression) this.secondExpression).TargetFunctionInstance == null)
            this.secondExpression.Update();
          if (((VMExpression) this.secondExpression).TargetFunctionInstance != null)
            raisingFunctions.Add(((VMExpression) this.secondExpression).TargetFunctionInstance);
        }
      }
      return raisingFunctions;
    }

    public virtual bool IsConstant()
    {
      return this.conditionType == EConditionType.CONDITION_TYPE_CONST_FALSE || this.conditionType == EConditionType.CONDITION_TYPE_CONST_TRUE;
    }

    public bool IsEqual(IObject other)
    {
      return other != null && typeof (VMPartCondition) == other.GetType() && (long) this.BaseGuid == (long) ((VMPartCondition) other).BaseGuid;
    }

    public string GuidStr => this.guid.ToString();

    public virtual void Clear()
    {
      if (this.firstExpression != null)
        ((VMExpression) this.firstExpression).Clear();
      if (this.secondExpression == null)
        return;
      ((VMExpression) this.secondExpression).Clear();
    }
  }
}
