using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public class AbstractActionInfo
  {
    private IAbstractAction instance;
    private BaseFunction targetFunction;
    private IEventRef targetEvent;
    private bool isValid;

    public AbstractActionInfo(IAbstractAction actionInstance) => this.instance = actionInstance;

    public BaseFunction TargetFunctionInstance => this.targetFunction;

    public bool IsValid => this.isValid;

    public void Update()
    {
      this.isValid = true;
      IGameRoot gameRoot = IStaticDataContainer.StaticDataContainer.GameRoot;
      if (this.instance.ActionType == EActionType.ACTION_TYPE_NONE)
        return;
      CommonVariable targetObject = this.instance.TargetObject;
      if (targetObject == null)
      {
        this.isValid = false;
      }
      else
      {
        IContextElement contextElement = (IContextElement) null;
        if (typeof (IContextElement).IsAssignableFrom(this.instance.GetType()))
          contextElement = (IContextElement) this.instance;
        IGameObjectContext ownerContext1 = (IGameObjectContext) gameRoot;
        if (this.instance.LocalContext != null && this.instance.LocalContext.Owner != null)
          ownerContext1 = (IGameObjectContext) this.instance.LocalContext.Owner;
        targetObject.Bind((IContext) ownerContext1, new VMType(typeof (IObjRef)), this.instance.LocalContext, contextElement);
        if (targetObject.IsBinded)
        {
          if (targetObject.VariableContext != null)
          {
            IContext variableContext = targetObject.VariableContext;
            if (typeof (IBlueprint).IsAssignableFrom(variableContext.GetType()) && !((ILogicObject) variableContext).Static && ownerContext1 != null && typeof (IBlueprint).IsAssignableFrom(ownerContext1.GetType()) && !((IBlueprint) ownerContext1).IsDerivedFrom(((IEditorBaseTemplate) variableContext).BaseGuid, true))
            {
              this.isValid = false;
              return;
            }
          }
          List<VMType> vmTypeList = new List<VMType>();
          if (this.instance.ActionType == EActionType.ACTION_TYPE_DO_FUNCTION)
          {
            this.targetFunction = (BaseFunction) null;
            IVariable contextVariable = targetObject.GetContextVariable(this.instance.TargetFunction);
            if (contextVariable == null)
            {
              if (typeof (IAbstractEditableAction).IsAssignableFrom(this.instance.GetType()))
                ((IAbstractEditableAction) this.instance).CheckFunctionUpdate();
              contextVariable = targetObject.GetContextVariable(this.instance.TargetFunction);
            }
            if (contextVariable != null && typeof (BaseFunction).IsAssignableFrom(contextVariable.GetType()))
              this.targetFunction = (BaseFunction) contextVariable;
            if (this.targetFunction != null)
            {
              for (int index = 0; index < this.targetFunction.InputParams.Count; ++index)
                vmTypeList.Add(this.targetFunction.InputParams[index].Type);
              if (this.instance.TargetParam != null && !this.instance.TargetParam.IsNull && this.targetFunction.HasOutput)
              {
                ILocalContext localContext = this.instance.LocalContext;
                if (localContext != null && localContext.Owner != null)
                {
                  this.instance.TargetParam.Bind((IContext) localContext.Owner, this.targetFunction.OutputParam.Type, localContext);
                  if (!this.instance.TargetParam.IsBinded)
                  {
                    this.isValid = false;
                    return;
                  }
                }
              }
            }
            else
            {
              this.isValid = false;
              return;
            }
          }
          else if (this.instance.ActionType == EActionType.ACTION_TYPE_SET_PARAM || this.instance.ActionType == EActionType.ACTION_TYPE_MATH || this.instance.ActionType == EActionType.ACTION_TYPE_SET_EXPRESSION)
          {
            this.targetFunction = (BaseFunction) null;
            ILocalContext localContext = this.instance.LocalContext;
            IContext ownerContext2 = (IContext) localContext.Owner;
            if (targetObject != null && targetObject.VariableContext != null)
              ownerContext2 = targetObject.VariableContext;
            this.instance.TargetParam.Bind(ownerContext2, localContext: localContext, contextElement: contextElement);
            if (this.instance.TargetParam.IsBinded)
            {
              VMType type = this.instance.TargetParam.Type;
              if (this.instance.ActionType == EActionType.ACTION_TYPE_MATH && !type.IsNumber)
              {
                this.isValid = false;
                return;
              }
              if (this.instance.ActionType == EActionType.ACTION_TYPE_MATH && this.instance.MathOperationType == EMathOperationType.ACTION_OPERATION_TYPE_NONE)
              {
                this.isValid = false;
                return;
              }
              vmTypeList.Add(this.instance.TargetParam.Type);
            }
            else
            {
              this.isValid = false;
              return;
            }
          }
          else if (this.instance.ActionType == EActionType.ACTION_TYPE_RAISE_EVENT)
          {
            this.targetFunction = (BaseFunction) null;
            this.targetEvent = (IEventRef) null;
            IVariable contextVariable = targetObject.GetContextVariable(this.instance.TargetFunction);
            if (contextVariable != null && typeof (IEventRef).IsAssignableFrom(contextVariable.GetType()))
              this.targetEvent = (IEventRef) contextVariable;
            if (this.targetEvent == null)
            {
              this.isValid = false;
              return;
            }
            for (int index = 0; index < this.targetEvent.Event.ReturnMessages.Count; ++index)
              vmTypeList.Add(this.targetEvent.Event.ReturnMessages[index].Type);
          }
          if (vmTypeList.Count <= 0)
            return;
          if (this.instance.ActionType == EActionType.ACTION_TYPE_SET_EXPRESSION && typeof (ISingleAction).IsAssignableFrom(this.instance.GetType()))
          {
            if (((ISingleAction) this.instance).SourceExpression == null)
            {
              this.isValid = false;
            }
            else
            {
              if (!((ISingleAction) this.instance).SourceExpression.IsValid)
                ((ISingleAction) this.instance).SourceExpression.Update();
              if (VMTypeUtility.IsTypesCompatible(this.instance.TargetParam.Type, ((ISingleAction) this.instance).SourceExpression.ResultType))
                return;
              this.isValid = false;
            }
          }
          else if (this.instance.SourceConstant != null && vmTypeList.Count == 1)
          {
            if (VMTypeUtility.IsTypesCompatible(vmTypeList[0], this.instance.SourceConstant.Type))
              return;
            this.isValid = false;
          }
          else
          {
            for (int index = 0; index < vmTypeList.Count; ++index)
            {
              if (index >= this.instance.SourceParams.Count)
              {
                this.isValid = false;
              }
              else
              {
                ILocalContext localContext = this.instance.LocalContext;
                CommonVariable sourceParam = this.instance.SourceParams[index];
                sourceParam.Bind((IContext) localContext.Owner, vmTypeList[index], localContext, contextElement);
                if (!sourceParam.IsBinded)
                  this.isValid = false;
              }
            }
          }
        }
        else
          this.isValid = false;
      }
    }

    public void Clear()
    {
      this.instance = (IAbstractAction) null;
      this.targetFunction = (BaseFunction) null;
      this.targetEvent = (IEventRef) null;
    }
  }
}
