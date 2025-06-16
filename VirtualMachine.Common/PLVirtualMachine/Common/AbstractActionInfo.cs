using System.Collections.Generic;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common
{
  public class AbstractActionInfo
  {
    private IAbstractAction instance;
    private BaseFunction targetFunction;
    private IEventRef targetEvent;
    private bool isValid;

    public AbstractActionInfo(IAbstractAction actionInstance) => instance = actionInstance;

    public BaseFunction TargetFunctionInstance => targetFunction;

    public bool IsValid => isValid;

    public void Update()
    {
      isValid = true;
      IGameRoot gameRoot = IStaticDataContainer.StaticDataContainer.GameRoot;
      if (instance.ActionType == EActionType.ACTION_TYPE_NONE)
        return;
      CommonVariable targetObject = instance.TargetObject;
      if (targetObject == null)
      {
        isValid = false;
      }
      else
      {
        IContextElement contextElement = null;
        if (typeof (IContextElement).IsAssignableFrom(instance.GetType()))
          contextElement = (IContextElement) instance;
        IGameObjectContext ownerContext1 = (IGameObjectContext) gameRoot;
        if (instance.LocalContext != null && instance.LocalContext.Owner != null)
          ownerContext1 = (IGameObjectContext) instance.LocalContext.Owner;
        targetObject.Bind(ownerContext1, new VMType(typeof (IObjRef)), instance.LocalContext, contextElement);
        if (targetObject.IsBinded)
        {
          if (targetObject.VariableContext != null)
          {
            IContext variableContext = targetObject.VariableContext;
            if (typeof (IBlueprint).IsAssignableFrom(variableContext.GetType()) && !((ILogicObject) variableContext).Static && ownerContext1 != null && typeof (IBlueprint).IsAssignableFrom(ownerContext1.GetType()) && !((IBlueprint) ownerContext1).IsDerivedFrom(((IEditorBaseTemplate) variableContext).BaseGuid, true))
            {
              isValid = false;
              return;
            }
          }
          List<VMType> vmTypeList = new List<VMType>();
          if (instance.ActionType == EActionType.ACTION_TYPE_DO_FUNCTION)
          {
            targetFunction = null;
            IVariable contextVariable = targetObject.GetContextVariable(instance.TargetFunction);
            if (contextVariable == null)
            {
              if (typeof (IAbstractEditableAction).IsAssignableFrom(instance.GetType()))
                ((IAbstractEditableAction) instance).CheckFunctionUpdate();
              contextVariable = targetObject.GetContextVariable(instance.TargetFunction);
            }
            if (contextVariable != null && typeof (BaseFunction).IsAssignableFrom(contextVariable.GetType()))
              targetFunction = (BaseFunction) contextVariable;
            if (targetFunction != null)
            {
              for (int index = 0; index < targetFunction.InputParams.Count; ++index)
                vmTypeList.Add(targetFunction.InputParams[index].Type);
              if (instance.TargetParam != null && !instance.TargetParam.IsNull && targetFunction.HasOutput)
              {
                ILocalContext localContext = instance.LocalContext;
                if (localContext != null && localContext.Owner != null)
                {
                  instance.TargetParam.Bind((IContext) localContext.Owner, targetFunction.OutputParam.Type, localContext);
                  if (!instance.TargetParam.IsBinded)
                  {
                    isValid = false;
                    return;
                  }
                }
              }
            }
            else
            {
              isValid = false;
              return;
            }
          }
          else if (instance.ActionType == EActionType.ACTION_TYPE_SET_PARAM || instance.ActionType == EActionType.ACTION_TYPE_MATH || instance.ActionType == EActionType.ACTION_TYPE_SET_EXPRESSION)
          {
            targetFunction = null;
            ILocalContext localContext = instance.LocalContext;
            IContext ownerContext2 = (IContext) localContext.Owner;
            if (targetObject != null && targetObject.VariableContext != null)
              ownerContext2 = targetObject.VariableContext;
            instance.TargetParam.Bind(ownerContext2, localContext: localContext, contextElement: contextElement);
            if (instance.TargetParam.IsBinded)
            {
              VMType type = instance.TargetParam.Type;
              if (instance.ActionType == EActionType.ACTION_TYPE_MATH && !type.IsNumber)
              {
                isValid = false;
                return;
              }
              if (instance.ActionType == EActionType.ACTION_TYPE_MATH && instance.MathOperationType == EMathOperationType.ACTION_OPERATION_TYPE_NONE)
              {
                isValid = false;
                return;
              }
              vmTypeList.Add(instance.TargetParam.Type);
            }
            else
            {
              isValid = false;
              return;
            }
          }
          else if (instance.ActionType == EActionType.ACTION_TYPE_RAISE_EVENT)
          {
            targetFunction = null;
            targetEvent = null;
            IVariable contextVariable = targetObject.GetContextVariable(instance.TargetFunction);
            if (contextVariable != null && typeof (IEventRef).IsAssignableFrom(contextVariable.GetType()))
              targetEvent = (IEventRef) contextVariable;
            if (targetEvent == null)
            {
              isValid = false;
              return;
            }
            for (int index = 0; index < targetEvent.Event.ReturnMessages.Count; ++index)
              vmTypeList.Add(targetEvent.Event.ReturnMessages[index].Type);
          }
          if (vmTypeList.Count <= 0)
            return;
          if (instance.ActionType == EActionType.ACTION_TYPE_SET_EXPRESSION && typeof (ISingleAction).IsAssignableFrom(instance.GetType()))
          {
            if (((ISingleAction) instance).SourceExpression == null)
            {
              isValid = false;
            }
            else
            {
              if (!((ISingleAction) instance).SourceExpression.IsValid)
                ((ISingleAction) instance).SourceExpression.Update();
              if (VMTypeUtility.IsTypesCompatible(instance.TargetParam.Type, ((ISingleAction) instance).SourceExpression.ResultType))
                return;
              isValid = false;
            }
          }
          else if (instance.SourceConstant != null && vmTypeList.Count == 1)
          {
            if (VMTypeUtility.IsTypesCompatible(vmTypeList[0], instance.SourceConstant.Type))
              return;
            isValid = false;
          }
          else
          {
            for (int index = 0; index < vmTypeList.Count; ++index)
            {
              if (index >= instance.SourceParams.Count)
              {
                isValid = false;
              }
              else
              {
                ILocalContext localContext = instance.LocalContext;
                CommonVariable sourceParam = instance.SourceParams[index];
                sourceParam.Bind((IContext) localContext.Owner, vmTypeList[index], localContext, contextElement);
                if (!sourceParam.IsBinded)
                  isValid = false;
              }
            }
          }
        }
        else
          isValid = false;
      }
    }

    public void Clear()
    {
      instance = null;
      targetFunction = null;
      targetEvent = null;
    }
  }
}
