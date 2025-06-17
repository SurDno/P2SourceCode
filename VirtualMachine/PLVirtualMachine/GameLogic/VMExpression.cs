using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.GameLogic
{
  [TypeData(EDataType.TExpression)]
  [DataFactory("Expression")]
  public class VMExpression(ulong guid) :
    IStub,
    IEditorDataReader,
    IExpression,
    IObject,
    IEditorBaseTemplate,
    IFunctionalPoint,
    IStaticUpdateable {
    [FieldData("ExpressionType")]
    private ExpressionType expressionType;
    [FieldData("TargetFunctionName")]
    private string targetFunctionName;
    [FieldData("Const", DataFieldType.Reference)]
    private VMParameter constInstance;
    [FieldData("LocalContext", DataFieldType.Reference)]
    private ILocalContext localContext;
    [FieldData("FormulaChilds", DataFieldType.Reference)]
    private List<IExpression> formulaChilds;
    [FieldData("FormulaOperations")]
    private List<FormulaOperation> formulaOperations;
    [FieldData("Inversion")]
    private bool inversion;
    [FieldData("TargetObject")]
    private CommonVariable targetObject;
    [FieldData("TargetParam")]
    private CommonVariable targetParam;
    [FieldData("SourceParams")]
    private List<CommonVariable> sourceParams = [];
    private BaseFunction functionInstance;
    private bool isValid;
    private bool isUpdated;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Const":
              constInstance = EditorDataReadUtility.ReadReference<VMParameter>(xml, creator);
              continue;
            case "ExpressionType":
              expressionType = EditorDataReadUtility.ReadEnum<ExpressionType>(xml);
              continue;
            case "FormulaChilds":
              formulaChilds = EditorDataReadUtility.ReadReferenceList(xml, creator, formulaChilds);
              continue;
            case "FormulaOperations":
              formulaOperations = EditorDataReadUtility.ReadEnumList(xml, formulaOperations);
              continue;
            case "Inversion":
              inversion = EditorDataReadUtility.ReadValue(xml, inversion);
              continue;
            case "LocalContext":
              localContext = EditorDataReadUtility.ReadReference<ILocalContext>(xml, creator);
              continue;
            case "SourceParams":
              sourceParams = EditorDataReadUtility.ReadSerializableList(xml, sourceParams);
              continue;
            case "TargetFunctionName":
              targetFunctionName = EditorDataReadUtility.ReadValue(xml, targetFunctionName);
              continue;
            case "TargetObject":
              targetObject = EditorDataReadUtility.ReadSerializable<CommonVariable>(xml);
              continue;
            case "TargetParam":
              targetParam = EditorDataReadUtility.ReadSerializable<CommonVariable>(xml);
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

    public ulong BaseGuid => guid;

    public ILocalContext LocalContext => localContext;

    public bool IsValid => isValid;

    public ExpressionType Type => expressionType;

    public bool Inversion => inversion;

    public VMType ResultType
    {
      get
      {
        if (!IsUpdated)
          Update();
        if (expressionType == ExpressionType.EXPRESSION_SRC_PARAM)
          return targetParam == null ? VMType.Empty : targetParam.Type;
        if (expressionType == ExpressionType.EXPRESSION_SRC_FUNCTION)
          return functionInstance == null || functionInstance.OutputParam == null ? VMType.Empty : functionInstance.OutputParam.Type;
        if (expressionType == ExpressionType.EXPRESSION_SRC_COMPLEX)
        {
          if (formulaChilds.Count >= 1)
          {
            VMType resultType = formulaChilds[0].ResultType;
            for (int index = 1; index < formulaChilds.Count; ++index)
            {
              if (formulaChilds[index].ResultType.BaseType == typeof (float) && resultType.IsIntegerNumber)
                resultType = formulaChilds[index].ResultType;
              else if (formulaChilds[index].ResultType.BaseType == typeof (double) && resultType.BaseType != typeof (double))
                resultType = formulaChilds[index].ResultType;
            }
            return resultType;
          }
          Logger.AddError(string.Format("Invalid complex expression {0}: no child expressions found", BaseGuid));
          return VMType.Empty;
        }
        return expressionType == ExpressionType.EXPRESSION_SRC_CONST && constInstance != null ? constInstance.Type : VMType.Empty;
      }
    }

    public string TargetFunction => targetFunctionName == null ? string.Empty : targetFunctionName;

    public BaseFunction TargetFunctionInstance => functionInstance;

    public IParam TargetConstant => constInstance;

    public CommonVariable TargetObject => targetObject;

    public CommonVariable TargetParam => targetParam;

    public List<CommonVariable> SourceParams => sourceParams;

    public int ChildExpressionsCount => Type != ExpressionType.EXPRESSION_SRC_COMPLEX ? 0 : formulaChilds.Count;

    public IExpression GetChildExpression(int childIndex)
    {
      if (Type != ExpressionType.EXPRESSION_SRC_COMPLEX)
      {
        Logger.AddError(string.Format("Cannot get child from non-complex expression guid={0}", BaseGuid));
        return null;
      }
      if (childIndex >= 0 && childIndex < formulaOperations.Count)
        return formulaChilds[childIndex];
      Logger.AddError(string.Format("Invalid formula child index", BaseGuid));
      return null;
    }

    public FormulaOperation GetChildOperations(int childIndex)
    {
      if (Type != ExpressionType.EXPRESSION_SRC_COMPLEX)
      {
        Logger.AddError(string.Format("Cannot get child from non-complex expression guid={0}", BaseGuid));
        return FormulaOperation.FORMULA_OP_NONE;
      }
      if (childIndex >= 0 && childIndex < formulaOperations.Count)
        return formulaOperations[childIndex];
      Logger.AddError(string.Format("Invalid formula child index", BaseGuid));
      return FormulaOperation.FORMULA_OP_NONE;
    }

    public void Update()
    {
      long baseGuid = (long) BaseGuid;
      if (isUpdated && isValid)
        return;
      if (expressionType == ExpressionType.EXPRESSION_SRC_COMPLEX)
      {
        int expressionsCount = ChildExpressionsCount;
        isValid = true;
        for (int childIndex = 0; childIndex < expressionsCount; ++childIndex)
        {
          IExpression childExpression = GetChildExpression(childIndex);
          childExpression.Update();
          if (childExpression == null)
          {
            Logger.AddError(string.Format("Child expression with index {0} not defined!", childIndex));
            isValid = false;
            break;
          }
        }
      }
      else if (expressionType == ExpressionType.EXPRESSION_SRC_CONST)
      {
        isValid = constInstance != null;
      }
      else
      {
        IContext ownerContext1 = null;
        if (LocalContext.Owner != null && typeof (IContext).IsAssignableFrom(LocalContext.Owner.GetType()))
          ownerContext1 = (IContext) LocalContext.Owner;
        targetObject.Bind(ownerContext1, new VMType(typeof (IObjRef)), LocalContext);
        if (!targetObject.IsBinded)
        {
          Logger.AddError(string.Format("Expression {0} target object variable {1} not binded", BaseGuid, targetObject));
          isValid = false;
          return;
        }
        functionInstance = null;
        List<VMType> vmTypeList = [];
        if (expressionType == ExpressionType.EXPRESSION_SRC_FUNCTION)
        {
          IVariable contextVariable = targetObject.GetContextVariable(TargetFunction);
          if (contextVariable != null)
            functionInstance = (BaseFunction) contextVariable;
          if (functionInstance == null)
          {
            isValid = false;
            return;
          }
          for (int index = 0; index < functionInstance.InputParams.Count; ++index)
            vmTypeList.Add(functionInstance.InputParams[index].Type);
        }
        else if (expressionType == ExpressionType.EXPRESSION_SRC_PARAM)
        {
          IContext ownerContext2 = null;
          if (LocalContext.Owner != null && typeof (IContext).IsAssignableFrom(LocalContext.Owner.GetType()))
            ownerContext2 = (IContext) LocalContext.Owner;
          if (targetObject != null && targetObject.VariableContext != null)
            ownerContext2 = targetObject.VariableContext;
          targetParam.Bind(ownerContext2, localContext: LocalContext);
          if (!targetParam.IsBinded)
          {
            targetParam.Bind(ownerContext2, localContext: LocalContext);
            isValid = false;
            return;
          }
        }
        if (vmTypeList.Count > 0)
        {
          for (int index = 0; index < vmTypeList.Count; ++index)
          {
            if (index >= SourceParams.Count)
            {
              isValid = false;
              return;
            }
            CommonVariable sourceParam = sourceParams[index];
            sourceParam.Bind(ownerContext1, vmTypeList[index], LocalContext);
            if (!sourceParam.IsBinded)
            {
              isValid = false;
              return;
            }
          }
        }
        isValid = true;
      }
      isUpdated = isValid;
    }

    public bool IsUpdated => isUpdated;

    public bool IsEqual(IObject other)
    {
      return other != null && typeof (VMExpression) == other.GetType() && (long) BaseGuid == (long) ((VMExpression) other).BaseGuid;
    }

    public string GuidStr => guid.ToString();

    public void Clear()
    {
      if (constInstance != null)
      {
        constInstance.Clear();
        constInstance = null;
      }
      localContext = null;
      if (formulaChilds != null)
      {
        foreach (VMExpression formulaChild in formulaChilds)
          formulaChild.Clear();
        formulaChilds.Clear();
        formulaChilds = null;
      }
      if (formulaOperations != null)
      {
        formulaOperations.Clear();
        formulaOperations = null;
      }
      if (targetObject != null)
      {
        targetObject.Clear();
        targetObject = null;
      }
      if (targetParam != null)
      {
        targetParam.Clear();
        targetParam = null;
      }
      if (sourceParams != null)
      {
        foreach (ContextVariable sourceParam in sourceParams)
          sourceParam.Clear();
        sourceParams.Clear();
        sourceParams = null;
      }
      functionInstance = null;
    }
  }
}
