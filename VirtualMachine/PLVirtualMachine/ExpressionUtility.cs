using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Services;
using Engine.Common.Types;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.GameLogic;
using IObject = Engine.Common.IObject;

namespace PLVirtualMachine
{
  public static class ExpressionUtility
  {
    private static string GetDayTimeFunctionName;
    private static string GetItemsCountByTemplateFunctionName;
    private static string GetObjectClassFunctionName;
    private static string SetMultiStorablesFixedPricesFunctionName;
    private static EntityMethodExecuteData lastActionMethodExecuteData;
    private static List<object> inputParamsList = new List<object>();
    private static List<VMType> inputTypesList = new List<VMType>();
    private static Dictionary<string, double> execFunctionsTimingDict = new Dictionary<string, double>();
    private static Dictionary<string, int> execFunctionsCountDict = new Dictionary<string, int>();
    private static Dictionary<ulong, double> execConditionTimingDict = new Dictionary<ulong, double>();
    private static Dictionary<ulong, int> execConditionCountDict = new Dictionary<ulong, int>();
    public static double CalculateConditionResultTimeMaxRT = 0.0;
    public static double ConditionUpdateingMaxRT = 0.0;

    public static void Clear()
    {
      inputParamsList.Clear();
      inputTypesList.Clear();
      lastActionMethodExecuteData = null;
    }

    public static bool CalculateConditionResult(
      ICondition condition,
      IDynamicGameObjectContext dynamicObjContext,
      INamed actor = null)
    {
      return DoCalculateConditionResult(condition, dynamicObjContext, actor);
    }

    public static bool DoCalculateConditionResult(
      ICondition condition,
      IDynamicGameObjectContext dynamicObjContext,
      INamed actor = null)
    {
      if (condition == null)
      {
        Logger.AddError(string.Format("Condition not defined at {0}!", DynamicFSM.CurrentStateInfo));
        return false;
      }
      if (!condition.IsUpdated)
        condition.Update();
      try
      {
        if (!condition.IsPart())
        {
          VMCondition vmCondition = (VMCondition) condition;
          if (vmCondition.Operation == EConditionOperation.COP_XOR)
          {
            bool flag1 = true;
            bool flag2 = false;
            for (int index = 0; index < vmCondition.Predicates.Count; ++index)
            {
              if (DoCalculateConditionResult(vmCondition.Predicates[index], dynamicObjContext, actor))
                flag2 = true;
              else
                flag1 = false;
            }
            return flag2 && !flag1;
          }
          if (vmCondition.Operation == EConditionOperation.COP_NONE)
            return false;
          bool conditionResult = false;
          if (vmCondition.Operation == EConditionOperation.COP_AND)
            conditionResult = true;
          int num = vmCondition.Predicates.Count;
          if (vmCondition.Operation == EConditionOperation.COP_ROOT)
            num = 1;
          for (int index = 0; index < num; ++index)
          {
            if (DoCalculateConditionResult(vmCondition.Predicates[index], dynamicObjContext, actor))
            {
              if (vmCondition.Operation == EConditionOperation.COP_OR || vmCondition.Operation == EConditionOperation.COP_ROOT)
              {
                conditionResult = true;
                break;
              }
            }
            else if (vmCondition.Operation == EConditionOperation.COP_AND)
            {
              conditionResult = false;
              break;
            }
          }
          return conditionResult;
        }
        VMPartCondition vmPartCondition = (VMPartCondition) condition;
        if (vmPartCondition.ConditionType == EConditionType.CONDITION_TYPE_CONST_TRUE)
          return true;
        if (vmPartCondition.ConditionType == EConditionType.CONDITION_TYPE_CONST_FALSE)
          return false;
        if (vmPartCondition.ConditionType == EConditionType.CONDITION_TYPE_VALUE_EXPRESSION)
        {
          if (vmPartCondition.FirstExpression == null)
          {
            Logger.AddError(string.Format("Expression not definied at {0}!", DynamicFSM.CurrentStateInfo));
            return false;
          }
          if (vmPartCondition.FirstExpression.ResultType.BaseType != typeof (bool))
          {
            Logger.AddError(string.Format("This expression type must be boolean at {0} !", DynamicFSM.CurrentStateInfo));
            return false;
          }
          object expressionResult = CalculateExpressionResult(vmPartCondition.FirstExpression, dynamicObjContext);
          if (expressionResult.GetType() == typeof (bool))
            return (bool) expressionResult;
          Logger.AddError(string.Format("This expression return value must be boolean at {0}!", DynamicFSM.CurrentStateInfo));
          return false;
        }
        if (vmPartCondition.FirstExpression == null)
        {
          Logger.AddError(string.Format("Condition {0} first expression not defined at {1}!", vmPartCondition.BaseGuid, DynamicFSM.CurrentStateInfo));
          return false;
        }
        if (vmPartCondition.SecondExpression == null)
        {
          Logger.AddError(string.Format("Condition {0} second expression not defined at {1}!", vmPartCondition.BaseGuid, DynamicFSM.CurrentStateInfo));
          return false;
        }
        VMType resultType1 = vmPartCondition.FirstExpression.ResultType;
        if (resultType1 == null)
        {
          Logger.AddError(string.Format("Condition {0} first expression {1} is invalid at {2}!", vmPartCondition.BaseGuid, "unknown", DynamicFSM.CurrentStateInfo));
          return false;
        }
        VMType resultType2 = vmPartCondition.SecondExpression.ResultType;
        if (!VMTypeUtility.IsTypesCompatible(resultType1, resultType2))
        {
          if (actor != null)
            Logger.AddError(string.Format("Condition expressions types are not compatible at {0}!", actor.Name));
          else
            Logger.AddError(string.Format("Condition expressions types are not compatible at {0}!", DynamicFSM.CurrentStateInfo));
          return false;
        }
        object expressionResult1 = CalculateExpressionResult(vmPartCondition.FirstExpression, dynamicObjContext);
        object expressionResult2 = CalculateExpressionResult(vmPartCondition.SecondExpression, dynamicObjContext);
        EConditionType econditionType = vmPartCondition.ConditionType;
        if (resultType1.BaseType == typeof (GameTime) && vmPartCondition.FirstExpression.Type == ExpressionType.EXPRESSION_SRC_FUNCTION && vmPartCondition.FirstExpression.TargetFunction.EndsWith(EngineAPIManager.GetSpecialFunctionName(ESpecialFunctionName.SFN_GET_GAME_TIME, typeof (VMGameComponent))) && (econditionType == EConditionType.CONDITION_TYPE_VALUE_EQUAL || econditionType == EConditionType.CONDITION_TYPE_VALUE_LARGER))
          econditionType = EConditionType.CONDITION_TYPE_VALUE_LARGER_EQUAL;
        bool conditionResult1 = false;
        switch (econditionType)
        {
          case EConditionType.CONDITION_TYPE_VALUE_LESS:
            conditionResult1 = VMTypeMathUtility.IsValueLess(expressionResult1, expressionResult2);
            break;
          case EConditionType.CONDITION_TYPE_VALUE_LESS_EQUAL:
            conditionResult1 = VMTypeMathUtility.IsValueLess(expressionResult1, expressionResult2, true);
            break;
          case EConditionType.CONDITION_TYPE_VALUE_LARGER:
            conditionResult1 = VMTypeMathUtility.IsValueLarger(expressionResult1, expressionResult2);
            break;
          case EConditionType.CONDITION_TYPE_VALUE_LARGER_EQUAL:
            conditionResult1 = VMTypeMathUtility.IsValueLarger(expressionResult1, expressionResult2, true);
            break;
          case EConditionType.CONDITION_TYPE_VALUE_EQUAL:
            conditionResult1 = VMTypeMathUtility.IsValueEqual(expressionResult1, expressionResult2);
            break;
          case EConditionType.CONDITION_TYPE_VALUE_NOT_EQUAL:
            conditionResult1 = !VMTypeMathUtility.IsValueEqual(expressionResult1, expressionResult2);
            break;
        }
        return conditionResult1;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Calculating condition result error: {0} at {1}", ex, DynamicFSM.CurrentStateInfo));
      }
      return false;
    }

    public static EntityMethodExecuteData GetLastActionMethodExecuteData()
    {
      return lastActionMethodExecuteData;
    }

    public static object CalculateExpressionResult(
      IExpression expression,
      IDynamicGameObjectContext dynamicObjContext)
    {
      if (!StaticInited)
        InitStatic();
      if (!expression.IsValid)
        expression.Update();
      VMVariableService instance = (VMVariableService) IVariableService.Instance;
      if (expression.Type == ExpressionType.EXPRESSION_SRC_CONST)
        return expression.TargetConstant.Value;
      if (expression.Type == ExpressionType.EXPRESSION_SRC_PARAM)
      {
        CommonVariable targetParam = expression.TargetParam;
        if (targetParam == null)
        {
          Logger.AddError(string.Format("Expression {0}: target param is null at {1} !!!", expression.BaseGuid, DynamicFSM.CurrentStateInfo));
          return null;
        }
        if (!targetParam.IsBinded)
        {
          Logger.AddError(string.Format("Expression {0}: target param {1} not defined at {2}", expression.BaseGuid, targetParam, DynamicFSM.CurrentStateInfo));
          return null;
        }
        object expressionResult1 = instance.GetDynamicVariableValue(targetParam, expression.ResultType, dynamicObjContext);
        if (expression.Inversion && expressionResult1 != null)
          expressionResult1 = VMTypeMathUtility.MakeInversion(expressionResult1);
        if (expressionResult1 == null || !typeof (IObjRef).IsAssignableFrom(expression.ResultType.BaseType) || !typeof (IEntity).IsAssignableFrom(expressionResult1.GetType()))
          return expressionResult1;
        VMObjRef expressionResult2 = new VMObjRef();
        expressionResult2.Initialize(((IObject) expressionResult1).Id);
        return expressionResult2;
      }
      if (expression.Type == ExpressionType.EXPRESSION_SRC_FUNCTION)
      {
        IDynamicGameObjectContext dynamicContext = instance.GetDynamicContext(expression.TargetObject.VariableContext, dynamicObjContext);
        if (dynamicContext == null)
        {
          instance.GetDynamicContext(expression.TargetObject.VariableContext, dynamicObjContext);
          Logger.AddError(string.Format("No target entity found for method {0} at {1}", expression.TargetFunction, DynamicFSM.CurrentStateInfo));
        }
        else
        {
          VMFunction contextFunction = (VMFunction) dynamicContext.GetContextFunction(expression.TargetFunction);
          if (contextFunction == null)
          {
            Logger.AddError(string.Format("Expression function {0} not found at {1}", expression.TargetFunction, DynamicFSM.CurrentStateInfo));
            return null;
          }
          if (contextFunction.Name == GetDayTimeFunctionName || contextFunction.Name == GetObjectClassFunctionName || contextFunction.Name == GetItemsCountByTemplateFunctionName || contextFunction.Name == SetMultiStorablesFixedPricesFunctionName)
            return DoFastFunctionCall(contextFunction, expression.SourceParams, dynamicObjContext);
          inputParamsList.Clear();
          inputTypesList.Clear();
          for (int index = 0; index < contextFunction.InputParams.Count; ++index)
          {
            VMType type = contextFunction.InputParams[index].Type;
            CommonVariable sourceParam = expression.SourceParams[index];
            object dynamicVariableValue = instance.GetDynamicVariableValue(sourceParam, type, dynamicObjContext);
            inputTypesList.Add(type);
            inputParamsList.Add(dynamicVariableValue);
          }
          Type componentApiType = contextFunction.ParentComponentAPIType;
          object expressionResult = null;
          if (contextFunction.Entity != null)
          {
            expressionResult = VMEngineAPIManager.ExecMethod(dynamicObjContext, contextFunction.Entity.EngineGuid, componentApiType, contextFunction.ParentComponentAPIName, contextFunction.APIName, inputParamsList, inputTypesList, contextFunction.Type);
            if (expression.Inversion)
              expressionResult = VMTypeMathUtility.MakeInversion(expressionResult);
          }
          else
            Logger.AddError(string.Format("No target entity found for method {0} at {1}", contextFunction.Name, DynamicFSM.CurrentStateInfo));
          return expressionResult;
        }
      }
      else if (expression.Type == ExpressionType.EXPRESSION_SRC_COMPLEX)
      {
        if (!expression.ResultType.IsNumber)
        {
          Logger.AddError(string.Format("Complex expression {0} result type must be number at {1}", expression.BaseGuid, DynamicFSM.CurrentStateInfo));
          return 0;
        }
        int expressionsCount = expression.ChildExpressionsCount;
        double num = 0.0;
        double prevValue = 0.0;
        for (int childIndex = 0; childIndex < expressionsCount; ++childIndex)
        {
          VMExpression childExpression = (VMExpression) expression.GetChildExpression(childIndex);
          List<FormulaOperation> dOperations = new List<FormulaOperation>();
          dOperations.Add(expression.GetChildOperations(childIndex));
          if (dOperations[0] == FormulaOperation.FORMULA_OP_NONE)
          {
            if (childIndex == 0)
            {
              num += prevValue;
              prevValue = CalculateExpressionValue(0.0, childExpression, dOperations, dynamicObjContext);
            }
            else
              Logger.AddError(string.Format("Operation on child expression index = {0} not defined in complex expression guid={1} at {2}", childIndex, expression.BaseGuid, DynamicFSM.CurrentStateInfo));
          }
          else if (dOperations[0] == FormulaOperation.FORMULA_OP_PLUS || dOperations[0] == FormulaOperation.FORMULA_OP_MINUS)
          {
            num += prevValue;
            prevValue = CalculateExpressionValue(0.0, childExpression, dOperations, dynamicObjContext);
          }
          else if (dOperations[0] == FormulaOperation.FORMULA_OP_MULTIPLY || dOperations[0] == FormulaOperation.FORMULA_OP_DIVIDE || dOperations[0] == FormulaOperation.FORMULA_OP_RDIVIDE || dOperations[0] == FormulaOperation.FORMULA_OP_POWER)
            prevValue = CalculateExpressionValue(prevValue, childExpression, dOperations, dynamicObjContext);
          else if (childIndex == 0 && (dOperations[0] == FormulaOperation.FORMULA_OP_COS || dOperations[0] == FormulaOperation.FORMULA_OP_SIN || dOperations[0] == FormulaOperation.FORMULA_OP_EXP || dOperations[0] == FormulaOperation.FORMULA_OP_LOG || dOperations[0] == FormulaOperation.FORMULA_OP_LOG10))
          {
            dOperations.Insert(0, FormulaOperation.FORMULA_OP_NONE);
            num += prevValue;
            prevValue = CalculateExpressionValue(0.0, childExpression, dOperations, dynamicObjContext);
          }
          else
            Logger.AddError(string.Format("Base operation on child expression index = {0} not defined in complex expression guid={1} at {2}", childIndex, expression.BaseGuid, DynamicFSM.CurrentStateInfo));
        }
        object expressionResult = Convert.ChangeType(num + prevValue, expression.ResultType.BaseType);
        if (expression.Inversion)
          expressionResult = VMTypeMathUtility.MakeInversion(expressionResult);
        return expressionResult;
      }
      return null;
    }

    public static double CalculateExpressionValue(
      double prevValue,
      VMExpression expression,
      List<FormulaOperation> dOperations,
      IDynamicGameObjectContext dynamicObjContext)
    {
      object expressionResult = CalculateExpressionResult(expression, dynamicObjContext);
      if (!VMTypeUtility.IsTypeNumber(expressionResult.GetType()) && !VMTypeUtility.IsTypeNumber(expression.ResultType.BaseType))
      {
        Logger.AddError(string.Format("Child expression {0} result type in complex expression must be number at {1}", expression.BaseGuid, DynamicFSM.CurrentStateInfo));
        return 0.0;
      }
      double expressionValue = (double) Convert.ChangeType(expressionResult, typeof (double));
      if (dOperations.Count > 1)
      {
        FormulaOperation dOperation = dOperations[1];
        switch (dOperation)
        {
          case FormulaOperation.FORMULA_OP_LOG:
            expressionValue = Math.Log(expressionValue);
            break;
          case FormulaOperation.FORMULA_OP_LOG10:
            expressionValue = Math.Log10(expressionValue);
            break;
          case FormulaOperation.FORMULA_OP_EXP:
            expressionValue = Math.Exp(expressionValue);
            break;
          case FormulaOperation.FORMULA_OP_SIN:
            expressionValue = Math.Sin(expressionValue);
            break;
          case FormulaOperation.FORMULA_OP_COS:
            expressionValue = Math.Cos(expressionValue);
            break;
          default:
            Logger.AddError(string.Format("Invalid child expression {0} secondary operation: {1} at {2}", expression.BaseGuid, dOperation.ToString(), DynamicFSM.CurrentStateInfo));
            break;
        }
      }
      FormulaOperation dOperation1 = dOperations[0];
      switch (dOperation1)
      {
        case FormulaOperation.FORMULA_OP_NONE:
          return expressionValue;
        case FormulaOperation.FORMULA_OP_PLUS:
          expressionValue = prevValue + expressionValue;
          goto case FormulaOperation.FORMULA_OP_NONE;
        case FormulaOperation.FORMULA_OP_MINUS:
          expressionValue = prevValue - expressionValue;
          goto case FormulaOperation.FORMULA_OP_NONE;
        case FormulaOperation.FORMULA_OP_MULTIPLY:
          expressionValue = prevValue * expressionValue;
          goto case FormulaOperation.FORMULA_OP_NONE;
        case FormulaOperation.FORMULA_OP_DIVIDE:
          expressionValue = prevValue / expressionValue;
          goto case FormulaOperation.FORMULA_OP_NONE;
        case FormulaOperation.FORMULA_OP_RDIVIDE:
          double num1 = Math.Floor(prevValue);
          double num2 = prevValue - num1;
          expressionValue = (int) num1 % (int) Math.Floor(expressionValue) + num2;
          goto case FormulaOperation.FORMULA_OP_NONE;
        case FormulaOperation.FORMULA_OP_POWER:
          expressionValue = Math.Pow(prevValue, expressionValue);
          goto case FormulaOperation.FORMULA_OP_NONE;
        default:
          Logger.AddError(string.Format("Invalid child expression {0} base operation: {1} at {2}", expression.BaseGuid, dOperation1.ToString(), DynamicFSM.CurrentStateInfo));
          goto case FormulaOperation.FORMULA_OP_NONE;
      }
    }

    public static object GetContextParamValue(
      IDynamicGameObjectContext dynamicObjContext,
      CommonVariable variable,
      VMType paramType,
      ILocalContext localContext = null)
    {
      if (paramType == null)
        return null;
      if (variable == null)
        return BaseSerializer.GetDefaultValue(paramType.BaseType);
      if (localContext == null)
        localContext = dynamicObjContext.CurrentState;
      variable.Bind(dynamicObjContext.FSMStaticObject, paramType, localContext);
      return ((VMVariableService) IVariableService.Instance).GetDynamicVariableValue(variable, paramType, dynamicObjContext) ?? ((VMVariableService) IVariableService.Instance).GetDynamicVariableValue(variable, paramType, dynamicObjContext);
    }

    public static void ProcessAction(
      IAbstractAction action,
      IDynamicGameObjectContext dynamicObjContext,
      DynamicFSM contextObjFSM = null)
    {
      if (!StaticInited)
        InitStatic();
      try
      {
        bool flag = typeof (ISingleAction).IsAssignableFrom(action.GetType());
        string str = "Abstract";
        if (flag)
        {
          str = ((INamed) action).Name;
          if (str == string.Empty)
            str = ((IEditorBaseTemplate) action).BaseGuid.ToString();
        }
        VMVariableService instance = (VMVariableService) IVariableService.Instance;
        if (!action.IsValid)
        {
          action.Update();
          if (!action.IsValid)
          {
            Logger.AddError(string.Format("Action {0} has errors {1} at {2}:", str, "unknown", DynamicFSM.CurrentStateInfo));
            action.Update();
          }
        }
        if (action.ActionType == EActionType.ACTION_TYPE_SET_PARAM || action.ActionType == EActionType.ACTION_TYPE_MATH)
        {
          lastActionMethodExecuteData = null;
          try
          {
            CommonVariable targetObject = action.TargetObject;
            CommonVariable targetParam = action.TargetParam;
            if (targetParam == null)
              Logger.AddError(string.Format("Action {0}: target param is null at {1} !!!", str, DynamicFSM.CurrentStateInfo));
            else if (!targetParam.IsBinded)
              Logger.AddError(string.Format("Action {0}: target param {1} not defined at {2}", str, targetParam, DynamicFSM.CurrentStateInfo));
            else if (action.SourceParams.Count < 1)
            {
              Logger.AddError(string.Format("Action {0}: source param not defined at {1}", str, DynamicFSM.CurrentStateInfo));
            }
            else
            {
              CommonVariable sourceParam = action.SourceParams[0];
              if (sourceParam == null)
                Logger.AddError(string.Format("Action {0}: source param is null at {1} !!!", str, DynamicFSM.CurrentStateInfo));
              else if (!sourceParam.IsBinded)
                Logger.AddError(string.Format("Action {0}: source param {1} not defined at {2}", str, sourceParam, DynamicFSM.CurrentStateInfo));
              else if (targetParam.CommonVariableType != ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE)
              {
                Logger.AddError(string.Format("Action {0}: target param {1} must be parameter at {2}", str, targetParam, DynamicFSM.CurrentStateInfo));
              }
              else
              {
                IParam obj = instance.IsContextBinded(targetParam, dynamicObjContext) || contextObjFSM != null ? instance.GetDynamicParam(targetParam, dynamicObjContext, contextObjFSM) : instance.GetDynamicParam(targetObject, targetParam, dynamicObjContext);
                if (obj != null)
                {
                  object dynamicVariableValue = instance.GetDynamicVariableValue(sourceParam, obj.Type, dynamicObjContext, contextObjFSM);
                  if (action.ActionType == EActionType.ACTION_TYPE_SET_PARAM)
                  {
                    obj.Value = dynamicVariableValue;
                  }
                  else
                  {
                    if (action.ActionType != EActionType.ACTION_TYPE_MATH)
                      return;
                    if (dynamicVariableValue != null)
                    {
                      if (action.MathOperationType == EMathOperationType.ACTION_OPERATION_TYPE_ADDICTION)
                        obj.Value = VMTypeMathUtility.DoMathAdd(obj.Value, dynamicVariableValue);
                      else if (action.MathOperationType == EMathOperationType.ACTION_OPERATION_TYPE_SUBTRACTION)
                        obj.Value = VMTypeMathUtility.DoMathSubstarct(obj.Value, dynamicVariableValue);
                      else if (action.MathOperationType == EMathOperationType.ACTION_OPERATION_TYPE_MULTIPLY)
                        obj.Value = VMTypeMathUtility.DoMathMultiply(obj.Value, dynamicVariableValue);
                      else if (action.MathOperationType == EMathOperationType.ACTION_OPERATION_TYPE_DIVISION)
                        obj.Value = VMTypeMathUtility.DoMathDivide(obj.Value, dynamicVariableValue);
                      else
                        obj.Value = dynamicVariableValue;
                    }
                    else
                      Logger.AddError(string.Format("Action {0}: source math operation param value not defined at {1}", str, DynamicFSM.CurrentStateInfo));
                  }
                }
                else
                  Logger.AddError(string.Format("Action {0}: target param {1} not found at {2}", str, targetParam, DynamicFSM.CurrentStateInfo));
              }
            }
          }
          catch (Exception ex)
          {
            Logger.AddError(string.Format("Action {0}: set param error: {1} at {2}", str, ex, DynamicFSM.CurrentStateInfo));
          }
        }
        else if (action.ActionType == EActionType.ACTION_TYPE_DO_FUNCTION)
        {
          IDynamicGameObjectContext ownerContext = dynamicObjContext;
          IDynamicGameObjectContext gameObjectContext = (IDynamicGameObjectContext) contextObjFSM ?? instance.GetDynamicContext(action.TargetObject.VariableContext, ownerContext);
          if (gameObjectContext == null)
          {
            Logger.AddError(string.Format("No target entity found for method {0} at {1}", action.TargetFunction, DynamicFSM.CurrentStateInfo));
          }
          else
          {
            VMFunction contextFunction = (VMFunction) gameObjectContext.GetContextFunction(action.TargetFunction);
            if (contextFunction == null)
            {
              Logger.AddError(string.Format("Action function not found at {0}", DynamicFSM.CurrentStateInfo));
            }
            else
            {
              if (contextFunction.Name == GetDayTimeFunctionName || contextFunction.Name == GetObjectClassFunctionName || contextFunction.Name == GetItemsCountByTemplateFunctionName || contextFunction.Name == SetMultiStorablesFixedPricesFunctionName)
              {
                DoFastFunctionCall(contextFunction, action.SourceParams, dynamicObjContext);
                if (action.TargetParam != null && contextFunction.HasOutput)
                {
                  if (action.TargetParam.IsBinded)
                  {
                    IParam dynamicParam = instance.GetDynamicParam(action.TargetParam, dynamicObjContext, contextObjFSM);
                    if (dynamicParam != null)
                      dynamicParam.Value = DoFastFunctionCall(contextFunction, action.SourceParams, dynamicObjContext);
                  }
                }
                else
                  DoFastFunctionCall(contextFunction, action.SourceParams, dynamicObjContext);
              }
              else
              {
                inputParamsList.Clear();
                inputTypesList.Clear();
                int count = contextFunction.InputParams.Count;
                for (int index = 0; index < count; ++index)
                {
                  VMType type = contextFunction.InputParams[index].Type;
                  CommonVariable sourceParam = action.SourceParams[index];
                  object dynamicVariableValue = instance.GetDynamicVariableValue(sourceParam, type, dynamicObjContext, contextObjFSM);
                  inputTypesList.Add(type);
                  inputParamsList.Add(dynamicVariableValue);
                }
                Type componentApiType = contextFunction.ParentComponentAPIType;
                object obj;
                if (contextFunction.Entity != null)
                {
                  obj = VMEngineAPIManager.ExecMethod(dynamicObjContext, contextFunction.Entity.EngineGuid, componentApiType, contextFunction.ParentComponentAPIName, contextFunction.APIName, inputParamsList, inputTypesList, contextFunction.Type);
                }
                else
                {
                  Logger.AddError(string.Format("No target entity found for method {0} at {1}", contextFunction.Name, DynamicFSM.CurrentStateInfo));
                  obj = null;
                }
                if (action.TargetParam != null && contextFunction.HasOutput && action.TargetParam.IsBinded)
                {
                  IParam dynamicParam = instance.GetDynamicParam(action.TargetParam, dynamicObjContext, contextObjFSM);
                  if (dynamicParam != null && obj != null)
                    dynamicParam.Value = obj;
                }
              }
              lastActionMethodExecuteData = VMEngineAPIManager.EntityMethodExecuteData;
            }
          }
        }
        else if (action.ActionType == EActionType.ACTION_TYPE_SET_EXPRESSION)
        {
          if (!flag)
            return;
          if (((ISingleAction) action).SourceExpression == null)
          {
            Logger.AddError(string.Format("Source expression not defined in action with guid = {0} at {1}", "unknown", DynamicFSM.CurrentStateInfo));
          }
          else
          {
            object expressionResult = CalculateExpressionResult(((ISingleAction) action).SourceExpression, dynamicObjContext);
            if (action.TargetParam == null || action.TargetParam.IsNull)
              return;
            CommonVariable targetParam = action.TargetParam;
            if (!targetParam.IsBinded)
            {
              Logger.AddError(string.Format("Action {0}: target param {1} not defined at {2}", str, targetParam, DynamicFSM.CurrentStateInfo));
            }
            else
            {
              IParam dynamicParam = instance.GetDynamicParam(targetParam, dynamicObjContext, contextObjFSM);
              if (dynamicParam != null)
                dynamicParam.Value = expressionResult;
              else
                Logger.AddError(string.Format("Action {0}: target param {1} not found at {2}", str, targetParam, DynamicFSM.CurrentStateInfo));
            }
          }
        }
        else
        {
          if (action.ActionType != EActionType.ACTION_TYPE_RAISE_EVENT)
            return;
          IDynamicGameObjectContext ownerContext = dynamicObjContext;
          if (contextObjFSM != null)
            ownerContext = contextObjFSM;
          IDynamicGameObjectContext dynamicContext = instance.GetDynamicContext(action.TargetObject.VariableContext, ownerContext);
          if (dynamicContext == null)
          {
            Logger.AddError(string.Format("Action event target object not found at {0}", DynamicFSM.CurrentStateInfo));
          }
          else
          {
            string targetEvent = action.TargetEvent;
            DynamicEvent contextEvent;
            if (GuidUtility.GetGuidFormat(targetEvent) == EGuidFormat.GT_BASE)
            {
              ulong uint64 = StringUtility.ToUInt64(targetEvent);
              contextEvent = dynamicContext.GetContextEvent(uint64);
            }
            else
              contextEvent = dynamicContext.GetContextEvent(targetEvent);
            if (contextEvent == null)
            {
              Logger.AddError(string.Format("Action raising event not found at {0}", DynamicFSM.CurrentStateInfo));
            }
            else
            {
              int count = contextEvent.StaticEvent.ReturnMessages.Count;
              if (action.SourceParams.Count != contextEvent.StaticEvent.ReturnMessages.Count)
              {
                Logger.AddError(string.Format("Event raising error: action {0} source params count not equal to event {1} messages count at {2}", str, contextEvent.StaticEvent.Name, DynamicFSM.CurrentStateInfo));
                if (action.SourceParams.Count < contextEvent.StaticEvent.ReturnMessages.Count)
                  count = action.SourceParams.Count;
              }
              List<EventMessage> raisingEventMessageList = new List<EventMessage>();
              for (int index = 0; index < count; ++index)
              {
                CommonVariable sourceParam = action.SourceParams[index];
                VMType type = contextEvent.StaticEvent.ReturnMessages[index].Type;
                object contextParamValue = GetContextParamValue(dynamicObjContext, sourceParam, type);
                string name = contextEvent.StaticEvent.ReturnMessages[index].Name;
                EventMessage eventMessage = new EventMessage();
                eventMessage.Initialize(name, type, contextParamValue);
                raisingEventMessageList.Add(eventMessage);
              }
              contextEvent.Raise(raisingEventMessageList, dynamicObjContext.FsmEventProcessingMode, Guid.Empty);
              lastActionMethodExecuteData = null;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(ex + " at " + DynamicFSM.CurrentStateInfo);
        throw;
      }
    }

    public static IRef GetRefByEngineInstance(IObject engInstance, Type refType)
    {
      IRef byEngineInstance = null;
      if (typeof (IObjRef).IsAssignableFrom(refType))
      {
        byEngineInstance = new VMObjRef();
        ((VMObjRef) byEngineInstance).Initialize(engInstance.Id);
      }
      else if (typeof (IBlueprintRef).IsAssignableFrom(refType))
      {
        byEngineInstance = new VMBlueprintRef();
        ((VMBlueprintRef) byEngineInstance).Initialize(engInstance.Id);
      }
      else if (typeof (ISampleRef).IsAssignableFrom(refType))
      {
        byEngineInstance = new VMSampleRef();
        ((VMSampleRef) byEngineInstance).Initialize(engInstance.Id);
      }
      return byEngineInstance;
    }

    private static void MakeExecFuncDetailTimingInfo(
      string componentName,
      string funcName,
      double elapsedTime)
    {
      string key = componentName + funcName;
      if (!execFunctionsTimingDict.ContainsKey(key))
      {
        execFunctionsTimingDict.Add(key, 0.0);
        execFunctionsCountDict.Add(key, 0);
      }
      execFunctionsTimingDict[key] += elapsedTime;
      ++execFunctionsCountDict[key];
    }

    private static object DoFastFunctionCall(
      VMFunction dynamicObjFunction,
      List<CommonVariable> sourceParams,
      IDynamicGameObjectContext dynamicObjContext)
    {
      VMVariableService instance = (VMVariableService) IVariableService.Instance;
      if (dynamicObjFunction.Name == GetDayTimeFunctionName)
        return ((VMGameComponent) dynamicObjFunction.Entity.GetComponentByName("GameComponent")).GetCurrDayTime();
      if (dynamicObjFunction.Name == GetObjectClassFunctionName)
        return ((VMGameComponent) dynamicObjFunction.Entity.GetComponentByName("GameComponent")).GetObjectClass((IObjRef) instance.GetDynamicVariableValue(sourceParams[0], dynamicObjFunction.InputParams[0].Type, dynamicObjContext));
      if (dynamicObjFunction.Name == GetItemsCountByTemplateFunctionName)
      {
        VMStorage componentByName = (VMStorage) dynamicObjFunction.Entity.GetComponentByName("Storage");
        VMBlueprintRef dynamicVariableValue = (VMBlueprintRef) instance.GetDynamicVariableValue(sourceParams[0], dynamicObjFunction.InputParams[0].Type, dynamicObjContext);
        Guid engineTemplateGuid = dynamicVariableValue.EngineTemplateGuid;
        if (Guid.Empty == engineTemplateGuid && dynamicVariableValue.Blueprint != null && typeof (IWorldBlueprint).IsAssignableFrom(dynamicVariableValue.Blueprint.GetType()))
          engineTemplateGuid = ((IEngineTemplated) dynamicVariableValue.Blueprint).EngineTemplateGuid;
        if (Guid.Empty != engineTemplateGuid)
        {
          IEntity template = ServiceCache.TemplateService.GetTemplate<IEntity>(engineTemplateGuid);
          if (template != null)
            return componentByName.GetItemsCountByTemplate(template);
        }
        else
          Logger.AddError(string.Format("Template with Guid={0} not found in engine at {1}", engineTemplateGuid, DynamicFSM.CurrentStateInfo));
      }
      else if (dynamicObjFunction.Name == SetMultiStorablesFixedPricesFunctionName)
      {
        string dynamicVariableValue1 = (string) instance.GetDynamicVariableValue(sourceParams[0], dynamicObjFunction.InputParams[0].Type, dynamicObjContext);
        string dynamicVariableValue2 = (string) instance.GetDynamicVariableValue(sourceParams[1], dynamicObjFunction.InputParams[0].Type, dynamicObjContext);
        string dynamicVariableValue3 = (string) instance.GetDynamicVariableValue(sourceParams[2], dynamicObjFunction.InputParams[0].Type, dynamicObjContext);
        ((VMMarket) dynamicObjFunction.Entity.GetComponentByName("Market")).SetMultiStorablesFixedPrices(dynamicVariableValue1, dynamicVariableValue2, dynamicVariableValue3);
      }
      return null;
    }

    private static bool StaticInited { get; set; }

    private static void InitStatic()
    {
      GetDayTimeFunctionName = EngineAPIManager.GetSpecialFunctionName(ESpecialFunctionName.SFN_GET_DAY_TIME, typeof (VMGameComponent), true);
      GetObjectClassFunctionName = EngineAPIManager.GetSpecialFunctionName(ESpecialFunctionName.SFN_GET_OBJECT_CLASS, typeof (VMGameComponent), true);
      GetItemsCountByTemplateFunctionName = EngineAPIManager.GetSpecialFunctionName(ESpecialFunctionName.SFN_GET_ITEMS_COUNT_BY_TEMPLATE, typeof (VMStorage), true);
      SetMultiStorablesFixedPricesFunctionName = EngineAPIManager.GetSpecialFunctionName(ESpecialFunctionName.SFN_SET_MULTI_STORABLES_FIXED_PRICES, typeof (VMMarket), true);
      StaticInited = true;
    }
  }
}
