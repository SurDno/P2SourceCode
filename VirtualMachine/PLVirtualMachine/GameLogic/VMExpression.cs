using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Data;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.GameLogic
{
  [TypeData(EDataType.TExpression)]
  [DataFactory("Expression")]
  public class VMExpression : 
    IStub,
    IEditorDataReader,
    IExpression,
    IObject,
    IEditorBaseTemplate,
    IFunctionalPoint,
    IStaticUpdateable
  {
    private ulong guid;
    [FieldData("ExpressionType", DataFieldType.None)]
    private ExpressionType expressionType;
    [FieldData("TargetFunctionName", DataFieldType.None)]
    private string targetFunctionName;
    [FieldData("Const", DataFieldType.Reference)]
    private VMParameter constInstance;
    [FieldData("LocalContext", DataFieldType.Reference)]
    private ILocalContext localContext;
    [FieldData("FormulaChilds", DataFieldType.Reference)]
    private List<IExpression> formulaChilds;
    [FieldData("FormulaOperations", DataFieldType.None)]
    private List<FormulaOperation> formulaOperations;
    [FieldData("Inversion", DataFieldType.None)]
    private bool inversion;
    [FieldData("TargetObject", DataFieldType.None)]
    private CommonVariable targetObject;
    [FieldData("TargetParam", DataFieldType.None)]
    private CommonVariable targetParam;
    [FieldData("SourceParams", DataFieldType.None)]
    private List<CommonVariable> sourceParams = new List<CommonVariable>();
    private BaseFunction functionInstance;
    private bool isValid;
    private bool isUpdated;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Const":
              this.constInstance = EditorDataReadUtility.ReadReference<VMParameter>(xml, creator);
              continue;
            case "ExpressionType":
              this.expressionType = EditorDataReadUtility.ReadEnum<ExpressionType>(xml);
              continue;
            case "FormulaChilds":
              this.formulaChilds = EditorDataReadUtility.ReadReferenceList<IExpression>(xml, creator, this.formulaChilds);
              continue;
            case "FormulaOperations":
              this.formulaOperations = EditorDataReadUtility.ReadEnumList<FormulaOperation>(xml, this.formulaOperations);
              continue;
            case "Inversion":
              this.inversion = EditorDataReadUtility.ReadValue(xml, this.inversion);
              continue;
            case "LocalContext":
              this.localContext = EditorDataReadUtility.ReadReference<ILocalContext>(xml, creator);
              continue;
            case "SourceParams":
              this.sourceParams = EditorDataReadUtility.ReadSerializableList<CommonVariable>(xml, this.sourceParams);
              continue;
            case "TargetFunctionName":
              this.targetFunctionName = EditorDataReadUtility.ReadValue(xml, this.targetFunctionName);
              continue;
            case "TargetObject":
              this.targetObject = EditorDataReadUtility.ReadSerializable<CommonVariable>(xml);
              continue;
            case "TargetParam":
              this.targetParam = EditorDataReadUtility.ReadSerializable<CommonVariable>(xml);
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

    public VMExpression(ulong guid) => this.guid = guid;

    public ulong BaseGuid => this.guid;

    public ILocalContext LocalContext => this.localContext;

    public bool IsValid => this.isValid;

    public ExpressionType Type => this.expressionType;

    public bool Inversion => this.inversion;

    public VMType ResultType
    {
      get
      {
        if (!this.IsUpdated)
          this.Update();
        if (this.expressionType == ExpressionType.EXPRESSION_SRC_PARAM)
          return this.targetParam == null ? VMType.Empty : this.targetParam.Type;
        if (this.expressionType == ExpressionType.EXPRESSION_SRC_FUNCTION)
          return this.functionInstance == null || this.functionInstance.OutputParam == null ? VMType.Empty : this.functionInstance.OutputParam.Type;
        if (this.expressionType == ExpressionType.EXPRESSION_SRC_COMPLEX)
        {
          if (this.formulaChilds.Count >= 1)
          {
            VMType resultType = this.formulaChilds[0].ResultType;
            for (int index = 1; index < this.formulaChilds.Count; ++index)
            {
              if (this.formulaChilds[index].ResultType.BaseType == typeof (float) && resultType.IsIntegerNumber)
                resultType = this.formulaChilds[index].ResultType;
              else if (this.formulaChilds[index].ResultType.BaseType == typeof (double) && resultType.BaseType != typeof (double))
                resultType = this.formulaChilds[index].ResultType;
            }
            return resultType;
          }
          Logger.AddError(string.Format("Invalid complex expression {0}: no child expressions found", (object) this.BaseGuid));
          return VMType.Empty;
        }
        return this.expressionType == ExpressionType.EXPRESSION_SRC_CONST && this.constInstance != null ? this.constInstance.Type : VMType.Empty;
      }
    }

    public string TargetFunction
    {
      get => this.targetFunctionName == null ? string.Empty : this.targetFunctionName;
    }

    public BaseFunction TargetFunctionInstance => this.functionInstance;

    public IParam TargetConstant => (IParam) this.constInstance;

    public CommonVariable TargetObject => this.targetObject;

    public CommonVariable TargetParam => this.targetParam;

    public List<CommonVariable> SourceParams => this.sourceParams;

    public int ChildExpressionsCount
    {
      get => this.Type != ExpressionType.EXPRESSION_SRC_COMPLEX ? 0 : this.formulaChilds.Count;
    }

    public IExpression GetChildExpression(int childIndex)
    {
      if (this.Type != ExpressionType.EXPRESSION_SRC_COMPLEX)
      {
        Logger.AddError(string.Format("Cannot get child from non-complex expression guid={0}", (object) this.BaseGuid));
        return (IExpression) null;
      }
      if (childIndex >= 0 && childIndex < this.formulaOperations.Count)
        return this.formulaChilds[childIndex];
      Logger.AddError(string.Format("Invalid formula child index", (object) this.BaseGuid));
      return (IExpression) null;
    }

    public FormulaOperation GetChildOperations(int childIndex)
    {
      if (this.Type != ExpressionType.EXPRESSION_SRC_COMPLEX)
      {
        Logger.AddError(string.Format("Cannot get child from non-complex expression guid={0}", (object) this.BaseGuid));
        return FormulaOperation.FORMULA_OP_NONE;
      }
      if (childIndex >= 0 && childIndex < this.formulaOperations.Count)
        return this.formulaOperations[childIndex];
      Logger.AddError(string.Format("Invalid formula child index", (object) this.BaseGuid));
      return FormulaOperation.FORMULA_OP_NONE;
    }

    public void Update()
    {
      long baseGuid = (long) this.BaseGuid;
      if (this.isUpdated && this.isValid)
        return;
      if (this.expressionType == ExpressionType.EXPRESSION_SRC_COMPLEX)
      {
        int expressionsCount = this.ChildExpressionsCount;
        this.isValid = true;
        for (int childIndex = 0; childIndex < expressionsCount; ++childIndex)
        {
          IExpression childExpression = this.GetChildExpression(childIndex);
          childExpression.Update();
          if (childExpression == null)
          {
            Logger.AddError(string.Format("Child expression with index {0} not defined!", (object) childIndex));
            this.isValid = false;
            break;
          }
        }
      }
      else if (this.expressionType == ExpressionType.EXPRESSION_SRC_CONST)
      {
        this.isValid = this.constInstance != null;
      }
      else
      {
        IContext ownerContext1 = (IContext) null;
        if (this.LocalContext.Owner != null && typeof (IContext).IsAssignableFrom(this.LocalContext.Owner.GetType()))
          ownerContext1 = (IContext) this.LocalContext.Owner;
        this.targetObject.Bind(ownerContext1, new VMType(typeof (IObjRef)), this.LocalContext);
        if (!this.targetObject.IsBinded)
        {
          Logger.AddError(string.Format("Expression {0} target object variable {1} not binded", (object) this.BaseGuid, (object) this.targetObject.ToString()));
          this.isValid = false;
          return;
        }
        this.functionInstance = (BaseFunction) null;
        List<VMType> vmTypeList = new List<VMType>();
        if (this.expressionType == ExpressionType.EXPRESSION_SRC_FUNCTION)
        {
          IVariable contextVariable = this.targetObject.GetContextVariable(this.TargetFunction);
          if (contextVariable != null)
            this.functionInstance = (BaseFunction) contextVariable;
          if (this.functionInstance == null)
          {
            this.isValid = false;
            return;
          }
          for (int index = 0; index < this.functionInstance.InputParams.Count; ++index)
            vmTypeList.Add(this.functionInstance.InputParams[index].Type);
        }
        else if (this.expressionType == ExpressionType.EXPRESSION_SRC_PARAM)
        {
          IContext ownerContext2 = (IContext) null;
          if (this.LocalContext.Owner != null && typeof (IContext).IsAssignableFrom(this.LocalContext.Owner.GetType()))
            ownerContext2 = (IContext) this.LocalContext.Owner;
          if (this.targetObject != null && this.targetObject.VariableContext != null)
            ownerContext2 = this.targetObject.VariableContext;
          this.targetParam.Bind(ownerContext2, localContext: this.LocalContext);
          if (!this.targetParam.IsBinded)
          {
            this.targetParam.Bind(ownerContext2, localContext: this.LocalContext);
            this.isValid = false;
            return;
          }
        }
        if (vmTypeList.Count > 0)
        {
          for (int index = 0; index < vmTypeList.Count; ++index)
          {
            if (index >= this.SourceParams.Count)
            {
              this.isValid = false;
              return;
            }
            CommonVariable sourceParam = this.sourceParams[index];
            sourceParam.Bind(ownerContext1, vmTypeList[index], this.LocalContext);
            if (!sourceParam.IsBinded)
            {
              this.isValid = false;
              return;
            }
          }
        }
        this.isValid = true;
      }
      this.isUpdated = this.isValid;
    }

    public bool IsUpdated => this.isUpdated;

    public bool IsEqual(IObject other)
    {
      return other != null && typeof (VMExpression) == other.GetType() && (long) this.BaseGuid == (long) ((VMExpression) other).BaseGuid;
    }

    public string GuidStr => this.guid.ToString();

    public void Clear()
    {
      if (this.constInstance != null)
      {
        this.constInstance.Clear();
        this.constInstance = (VMParameter) null;
      }
      this.localContext = (ILocalContext) null;
      if (this.formulaChilds != null)
      {
        foreach (VMExpression formulaChild in this.formulaChilds)
          formulaChild.Clear();
        this.formulaChilds.Clear();
        this.formulaChilds = (List<IExpression>) null;
      }
      if (this.formulaOperations != null)
      {
        this.formulaOperations.Clear();
        this.formulaOperations = (List<FormulaOperation>) null;
      }
      if (this.targetObject != null)
      {
        this.targetObject.Clear();
        this.targetObject = (CommonVariable) null;
      }
      if (this.targetParam != null)
      {
        this.targetParam.Clear();
        this.targetParam = (CommonVariable) null;
      }
      if (this.sourceParams != null)
      {
        foreach (ContextVariable sourceParam in this.sourceParams)
          sourceParam.Clear();
        this.sourceParams.Clear();
        this.sourceParams = (List<CommonVariable>) null;
      }
      this.functionInstance = (BaseFunction) null;
    }
  }
}
