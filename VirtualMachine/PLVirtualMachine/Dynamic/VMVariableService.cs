using System;
using Cofe.Loggers;
using Cofe.Serializations.Converters;
using Engine.Common.Types;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.GameLogic;

namespace PLVirtualMachine.Dynamic
{
  public class VMVariableService : IVariableService
  {
    public bool IsContextBinded(CommonVariable variable, IDynamicGameObjectContext activeContext)
    {
      if (variable.Context == null)
        return false;
      return !typeof (ILogicObject).IsAssignableFrom(variable.Context.GetType()) || ((ILogicObject) variable.Context).Static || activeContext.IsStaticDerived((IBlueprint) variable.Context);
    }

    public override IContext GetContextByData(
      IGameObjectContext globalContext,
      string data,
      ILocalContext localContext = null,
      IContextElement contextElement = null)
    {
      switch (GuidUtility.GetGuidFormat(data))
      {
        case EGuidFormat.GT_BASE:
          ulong id = DefaultConverter.ParseUlong(data);
          if (id > 0UL)
          {
            ulong num = 0;
            if (VirtualMachine.Instance.WorldHierarchyRootEntity != null)
              num = VirtualMachine.Instance.WorldHierarchyRootEntity.BaseGuid;
            if ((long) IStaticDataContainer.StaticDataContainer.GameRoot.BaseGuid == (long) id)
              return IStaticDataContainer.StaticDataContainer.GameRoot;
            if ((long) num == (long) id)
              return (IContext) IStaticDataContainer.StaticDataContainer.GameRoot.GetWorldHierarhyObjectByGuid(new HierarchyGuid(data)) ?? null;
            IObject objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(id);
            if (objectByGuid != null && typeof (IContext).IsAssignableFrom(objectByGuid.GetType()))
              return (IContext) objectByGuid;
          }
          return null;
        case EGuidFormat.GT_HIERARCHY:
          return (IContext) IStaticDataContainer.StaticDataContainer.GameRoot.GetWorldHierarhyObjectByGuid(new HierarchyGuid(data)) ?? null;
        case EGuidFormat.GT_ENGINE:
          if (globalContext != null && typeof (IWorldObject).IsAssignableFrom(globalContext.GetType()))
          {
            Guid guid = DefaultConverter.ParseGuid(data);
            if (((IEngineTemplated) globalContext).EngineTemplateGuid == guid)
              return globalContext;
          }
          return null;
        default:
          try
          {
            if (localContext != null)
            {
              if (localContext.Owner != null)
              {
                if (typeof (IBlueprint).IsAssignableFrom(localContext.Owner.GetType()))
                {
                  IVariable contextByData = ((IContext) localContext.Owner).GetContextVariable(data);
                  if (contextByData == null && localContext != null)
                    contextByData = localContext.GetLocalContextVariable(data, contextElement);
                  if (contextByData != null)
                  {
                    if (typeof (IRef).IsAssignableFrom(contextByData.GetType()))
                    {
                      IRef @ref = (IRef) contextByData;
                      if (@ref.StaticInstance != null)
                      {
                        if (typeof (IContext).IsAssignableFrom(@ref.StaticInstance.GetType()))
                          return (IContext) @ref.StaticInstance;
                      }
                    }
                    else if (typeof (IContext).IsAssignableFrom(contextByData.GetType()))
                      return (IContext) contextByData;
                  }
                }
              }
            }
          }
          catch (Exception ex)
          {
            Logger.AddError(string.Format("Context getting error from data {0} at {1}, error: {2} ", data, globalContext.Name, ex));
          }
          return null;
      }
    }

    public IParam GetDynamicParam(
      CommonVariable contextVariable,
      CommonVariable paramVariable,
      IDynamicGameObjectContext activeContext)
    {
      if (paramVariable.CommonVariableType == ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE)
      {
        IDynamicGameObjectContext dynamicContext = GetDynamicContext(contextVariable.VariableContext, activeContext);
        if (dynamicContext != null)
          return GetDynamicContextParamByVariable(dynamicContext, (IVariable) paramVariable.Variable);
      }
      Logger.AddError(string.Format("Dynamic param by variable {0} in context {1} not found at {2}", paramVariable, contextVariable, DynamicFSM.CurrentStateInfo));
      return null;
    }

    public IParam GetDynamicParam(
      CommonVariable variable,
      IDynamicGameObjectContext activeContext,
      DynamicFSM ownerFSM = null)
    {
      if (variable.CommonVariableType == ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE)
      {
        IDynamicGameObjectContext ownerContext = activeContext;
        if (ownerFSM != null)
          ownerContext = ownerFSM;
        IDynamicGameObjectContext dynamicContext = GetDynamicContext(variable.Context, ownerContext);
        if (dynamicContext != null)
        {
          if (variable.Variable != null && typeof (IVariable).IsAssignableFrom(variable.Variable.GetType()))
            return GetDynamicContextParamByVariable(dynamicContext, (IVariable) variable.Variable);
        }
        else
        {
          Logger.AddError(string.Format("Cannot get dynamic param by variable, variable {0} not found at {1}", variable, DynamicFSM.CurrentStateInfo));
          return null;
        }
      }
      Logger.AddError(string.Format("Cannot get dynamic param by variable, variable {0} isn't param at {1}", variable, DynamicFSM.CurrentStateInfo));
      return null;
    }

    public object GetDynamicVariableValue(
      CommonVariable variable,
      VMType variableType,
      IDynamicGameObjectContext activeContext,
      DynamicFSM ownerFSM = null)
    {
      if (variable.IsNull)
        return BaseSerializer.GetDefaultValue(variableType.BaseType);
      if (variable.CommonVariableType == ECommonVariableType.CV_TYPE_CONSTANT)
        return variable.Variable;
      if (variable.CommonVariableType == ECommonVariableType.CV_TYPE_CONTEXT_SELF)
        return GetSelfObjctRef(variable, activeContext);
      if (variable.CommonVariableType == ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE)
      {
        if (variable.Category == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM)
        {
          if (variable.IsSelf)
            return GetSelfObjctRef(variable, activeContext);
          IParam dynamicParam = GetDynamicParam(variable, activeContext, ownerFSM);
          if (dynamicParam != null)
            return dynamicParam.Value;
          string str = "none";
          if (activeContext != null)
            str = activeContext.Entity.EditorTemplate.Name;
          Logger.AddError(string.Format("Cannot get dynamic param by variable {0} in context {1} at {2}", variable, str, DynamicFSM.CurrentStateInfo));
          return null;
        }
        IDynamicGameObjectContext ownerContext = activeContext;
        if (ownerFSM != null)
          ownerContext = ownerFSM;
        if (variable.Context != null && variable.Variable != null)
        {
          IDynamicGameObjectContext dynamicContext = GetDynamicContext(variable.Context, ownerContext);
          if (typeof (IVariable).IsAssignableFrom(variable.Variable.GetType()))
          {
            IVariable variable1 = (IVariable) variable.Variable;
            if (variable.Category == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE)
            {
              EventMessage contextMessage = ownerContext.GetContextMessage(variable1.Name);
              return contextMessage != null ? contextMessage.Value : ownerContext.GetLocalVariableValue(variable1.Name);
            }
            if (variable.Category == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR)
              return ownerContext.GetLocalVariableValue(variable1.Name);
            if (variable.Category == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM)
            {
              IParam contextParamByVariable = GetDynamicContextParamByVariable(dynamicContext, variable1);
              if (contextParamByVariable != null)
                return contextParamByVariable.Value;
            }
            else
            {
              if (variable.Category == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GLOBAL_VAR)
              {
                string globalVarName = variable1.Name;
                if (globalVarName.StartsWith("global_"))
                  globalVarName = globalVarName.Substring("global_".Length);
                return GlobalVariableUtility.GetGlobalVariableValue(globalVarName);
              }
              if (variable.Category == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_OBJECT && variable.IsSelf)
              {
                VMObjRef dynamicVariableValue = new VMObjRef();
                if (dynamicContext != null)
                {
                  dynamicVariableValue.InitializeInstance(dynamicContext.Entity);
                  return dynamicVariableValue;
                }
              }
              if (dynamicContext != null)
                return GetStaticContextValueByVariable(dynamicContext.FSMStaticObject, variable1);
            }
          }
        }
        Logger.AddError(string.Format("Cannot get dynamic variable value, variable {0} isn't binded at {1}", variable, DynamicFSM.CurrentStateInfo));
        return null;
      }
      Logger.AddError(string.Format("Cannot get dynamic variable value, variable {0} isn't binded at {1}", variable, DynamicFSM.CurrentStateInfo));
      return null;
    }

    public IDynamicGameObjectContext GetDynamicContext(
      IContext staticContext,
      IDynamicGameObjectContext ownerContext)
    {
      if (staticContext == null)
      {
        Logger.AddError(string.Format("Cannot bind dynamic context: static context not defined at {0}", DynamicFSM.CurrentStateInfo));
        return null;
      }
      if (typeof (ILogicObject).IsAssignableFrom(staticContext.GetType()))
        return GetDynamicContextByBlueprintContext((ILogicObject) staticContext, ownerContext);
      if (typeof (IVariable).IsAssignableFrom(staticContext.GetType()))
        return GetDynamicContextByVariable(ownerContext, (IVariable) staticContext);
      Logger.AddError(string.Format("Cannot get dynamic context by variable {0} at {1}", staticContext.Name, DynamicFSM.CurrentStateInfo));
      return null;
    }

    private IDynamicGameObjectContext GetDynamicContextByBlueprintContext(
      ILogicObject blueprintContext,
      IDynamicGameObjectContext ownerContext)
    {
      if (blueprintContext.Static)
        return GetDynamicContextByStaticObject(blueprintContext);
      if (ownerContext != null && ownerContext.IsStaticDerived(blueprintContext.Blueprint))
        return ownerContext;
      Logger.AddError(string.Format("Cannot get dynamic context by unbinded template context {0} at {1}", blueprintContext.Name, DynamicFSM.CurrentStateInfo));
      return null;
    }

    private IDynamicGameObjectContext GetDynamicContextByStaticObject(ILogicObject staticObject)
    {
      if (typeof (IWorldHierarchyObject).IsAssignableFrom(staticObject.GetType()) && (((IHierarchyObject) staticObject).HierarchyGuid.IsHierarchy || ((IWorldObject) staticObject).IsEngineRoot))
      {
        VMEntity entityByHierarchyGuid = WorldEntityUtility.GetDynamicObjectEntityByHierarchyGuid(((IHierarchyObject) staticObject).HierarchyGuid);
        if (entityByHierarchyGuid != null)
          return entityByHierarchyGuid.GetFSM();
        Logger.AddError(string.Format("Dynamic entity by static object {0} with id={1} not found at {2}", staticObject.Name, ((IHierarchyObject) staticObject).HierarchyGuid, DynamicFSM.CurrentStateInfo));
        return null;
      }
      VMEntity entityByStaticGuid = WorldEntityUtility.GetDynamicObjectEntityByStaticGuid(staticObject.Blueprint.BaseGuid);
      if (entityByStaticGuid != null)
        return entityByStaticGuid.GetFSM();
      Logger.AddError(string.Format("Dynamic entity by static object {0} with id={1} not found at {2}", staticObject.Name, staticObject.Blueprint.BaseGuid, DynamicFSM.CurrentStateInfo));
      return null;
    }

    private IDynamicGameObjectContext GetDynamicContextByVariable(
      IDynamicGameObjectContext dynContext,
      IVariable variable)
    {
      if (typeof (IObjRef).IsAssignableFrom(variable.GetType()) || typeof (IBlueprintRef).IsAssignableFrom(variable.GetType()))
      {
        IBlueprint staticInstance = (IBlueprint) ((IRef) variable).StaticInstance;
        if (staticInstance != null)
          return GetDynamicContextByBlueprintContext(staticInstance, dynContext);
      }
      if (variable.Category == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM)
      {
        IParam contextParamByVariable = GetDynamicContextParamByVariable(dynContext, variable);
        if (contextParamByVariable != null && contextParamByVariable.Value != null && typeof (IObjRef).IsAssignableFrom(contextParamByVariable.Value.GetType()))
          return GetDynamicContextByObjRef((IObjRef) contextParamByVariable.Value);
        if (typeof (VMParameter) == variable.GetType())
        {
          IGameObjectContext ownerContext = ((VMParameter) variable).OwnerContext;
          if (ownerContext != null && typeof (IBlueprint).IsAssignableFrom(ownerContext.GetType()) && ownerContext.Static)
            return GetDynamicContextByVariable(GetDynamicContextByBlueprintContext(ownerContext, dynContext), variable);
        }
      }
      else if (variable.Category == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE)
      {
        EventMessage contextMessage = dynContext.GetContextMessage(variable.Name);
        if (contextMessage != null)
        {
          if (contextMessage.Value != null && typeof (IObjRef).IsAssignableFrom(contextMessage.Value.GetType()))
            return GetDynamicContextByObjRef((IObjRef) contextMessage.Value);
        }
        else
        {
          object localVariableValue = dynContext.GetLocalVariableValue(variable.Name);
          if (localVariableValue != null && typeof (IObjRef).IsAssignableFrom(localVariableValue.GetType()))
            return GetDynamicContextByObjRef((IObjRef) localVariableValue);
        }
      }
      else if (variable.Category == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR)
      {
        if (variable.Name.StartsWith("group_"))
          return dynContext;
        object localVariableValue = dynContext.GetLocalVariableValue(variable.Name);
        if (localVariableValue != null)
        {
          if (typeof (IObjRef).IsAssignableFrom(localVariableValue.GetType()))
            return GetDynamicContextByObjRef((IObjRef) localVariableValue);
        }
        else
          Logger.AddError(string.Format("Cannot get value by local variable {0} in {1} at {1}", variable.Name, dynContext.Entity.EditorTemplate.Name, DynamicFSM.CurrentStateInfo));
      }
      Logger.AddError(string.Format("Cannot get dynamic context by variable {0} at {1}", variable.Name, DynamicFSM.CurrentStateInfo));
      return null;
    }

    private IParam GetDynamicContextParamByVariable(
      IDynamicGameObjectContext dynamicContext,
      IVariable variable)
    {
      IParam obj = null;
      if (typeof (IRef).IsAssignableFrom(variable.GetType()))
        obj = dynamicContext.GetContextParam(((IRef) variable).BaseGuid);
      else if (typeof (VMParameter) == variable.GetType())
        obj = dynamicContext.GetContextParam(((VMParameter) variable).BaseGuid);
      return obj ?? dynamicContext.GetContextParam(variable.Name);
    }

    private object GetStaticContextValueByVariable(IContext staticObject, IVariable contextVariable)
    {
      return contextVariable;
    }

    private IDynamicGameObjectContext GetDynamicContextByObjRef(IObjRef objRef)
    {
      VMObjRef vmObjRef = (VMObjRef) objRef;
      return vmObjRef.EngineInstance != null && typeof (VMEntity) == vmObjRef.EngineInstance.GetType() ? ((VMEntity) vmObjRef.EngineInstance).GetFSM() : (IDynamicGameObjectContext) null;
    }

    private VMObjRef GetSelfObjctRef(
      CommonVariable variable,
      IDynamicGameObjectContext activeContext)
    {
      if (variable.Context != null)
      {
        IContext context = variable.Context;
        if (typeof (ILogicObject).IsAssignableFrom(context.GetType()))
        {
          IDynamicGameObjectContext blueprintContext = GetDynamicContextByBlueprintContext((ILogicObject) context, activeContext);
          VMObjRef selfObjctRef = new VMObjRef();
          if (blueprintContext != null)
            selfObjctRef.InitializeInstance(blueprintContext.Entity);
          else
            Logger.AddError(string.Format("Self dynamic context by static object {0} not found at {1}", context.Name, DynamicFSM.CurrentStateInfo));
          return selfObjctRef;
        }
      }
      return (VMObjRef) BaseSerializer.GetDefaultValue(typeof (IObjRef));
    }
  }
}
