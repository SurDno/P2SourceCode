using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Cofe.Serializations.Converters;
using Engine.Common.Types;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common
{
  public class CommonVariable : ContextVariable, IVMStringSerializable
  {
    private string contextData = "";
    private string variableData = "";
    private IContext contextObj;
    private object value;
    private bool isSelf;
    private bool binded;
    private bool selfInited;
    private IContext variableContext;
    private ECommonVariableType commonVariableType;

    public void Initialise(string contextData, string variableData)
    {
      this.contextData = contextData;
      this.variableData = variableData;
    }

    public void InitialiseFromLocalVariable(string name, VMType type)
    {
      LocalVariable localVariable = new LocalVariable();
      localVariable.Initialize(name, type);
      value = localVariable;
      commonVariableType = ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE;
      Initialize(contextData + "%" + variableData, ((IVariable) value).Type);
      variableData = IVariableService.Instance.GetVariableData((IVariable) value);
      binded = true;
    }

    public override EContextVariableCategory Category
    {
      get
      {
        return commonVariableType == ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE ? ((IVariable) value).Category : EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_ALL;
      }
    }

    public IContext Context => contextObj;

    public object Variable => !binded ? variableData : value;

    public override IEnumerable<IVariable> GetContextVariables(
      EContextVariableCategory contextVarCategory)
    {
      return IsSelf ? contextObj.GetContextVariables(contextVarCategory) : base.GetContextVariables(contextVarCategory);
    }

    public string Write()
    {
      Logger.AddError("Invalid common variable writing!");
      return "";
    }

    public void Read(string data)
    {
      string[] strArray = data.Split('%');
      if (strArray.Length < 1)
        return;
      if (strArray.Length == 1)
      {
        contextData = "";
        variableData = strArray[0];
      }
      else
      {
        contextData = strArray[0];
        variableData = strArray[1];
        if (strArray.Length <= 2)
          return;
        variableData = variableData + "%" + strArray[2];
      }
    }

    public bool IsSelf
    {
      get
      {
        if (IsNull)
          return false;
        if (!selfInited)
          CalculateSelf();
        return isSelf;
      }
    }

    public bool IsNull
    {
      get
      {
        if (IsDataNull(contextData))
          return true;
        return contextData == "" && variableData == "";
      }
    }

    public bool IsBinded => binded;

    public ECommonVariableType CommonVariableType => commonVariableType;

    public void Bind(
      IContext ownerContext,
      VMType needType = null,
      ILocalContext localContext = null,
      IContextElement contextElement = null)
    {
      try
      {
        if (contextData == "" && variableData == "" && needType != null && typeof (IRef).IsAssignableFrom(needType.BaseType))
        {
          value = BaseSerializer.GetDefaultValue(needType.BaseType);
          binded = true;
          commonVariableType = ECommonVariableType.CV_TYPE_CONSTANT;
        }
        IGameObjectContext globalContext = (IGameObjectContext) IStaticDataContainer.StaticDataContainer.GameRoot;
        if (ownerContext != null && typeof (IGameObjectContext).IsAssignableFrom(ownerContext.GetType()))
          globalContext = (IGameObjectContext) ownerContext;
        if ("" != contextData)
        {
          contextObj = IVariableService.Instance.GetContextByData(globalContext, contextData, localContext, contextElement);
          if (contextObj != null)
          {
            if (contextData == variableData && typeof (ILogicObject).IsAssignableFrom(contextObj.GetType()))
            {
              value = ((ILogicObject) contextObj).GetSelf();
              SetSelf();
              commonVariableType = ECommonVariableType.CV_TYPE_CONTEXT_SELF;
              binded = true;
            }
            else
            {
              if ("" != variableData)
                value = IVariableService.Instance.GetContextVariableByData(contextObj, variableData, localContext, contextElement);
              else if (typeof (ILogicObject).IsAssignableFrom(contextObj.GetType()))
                value = ((ILogicObject) contextObj).GetSelf();
              if (typeof (ILogicObject).IsAssignableFrom(contextObj.GetType()) && !((ILogicObject) contextObj).Static)
              {
                bool flag = false;
                if (ownerContext != null && ((ILogicObject) ownerContext).Blueprint.IsDerivedFrom(((ILogicObject) contextObj).Blueprint.BaseGuid, true))
                  flag = true;
                if (!flag)
                {
                  binded = false;
                  Logger.AddError(string.Format("Context {0} must be static at {1}", contextObj.Name, EngineAPIManager.Instance.CurrentFSMStateInfo));
                  return;
                }
              }
              if (variableData == "none" && needType != null && typeof (IRef).IsAssignableFrom(needType.BaseType))
              {
                value = BaseSerializer.GetDefaultValue(needType.BaseType);
                binded = true;
                commonVariableType = ECommonVariableType.CV_TYPE_CONSTANT;
              }
              if (value == null && typeof (IParam).IsAssignableFrom(contextObj.GetType()) && contextData == variableData)
              {
                value = contextObj;
                contextObj = globalContext;
                binded = true;
              }
            }
          }
          else if (IsSelf)
          {
            commonVariableType = ECommonVariableType.CV_TYPE_CONTEXT_SELF;
            binded = false;
            return;
          }
        }
        else if ("" != variableData)
        {
          ILogicObject staticContextByData = GetStaticContextByData(variableData);
          bool flag = false;
          if (staticContextByData != null && staticContextByData.Static)
          {
            contextObj = staticContextByData;
            value = staticContextByData.GetSelf();
            SetSelf();
            flag = true;
            binded = true;
          }
          if (!flag)
          {
            contextObj = ownerContext;
            if (variableData.StartsWith("group_"))
            {
              value = new LocalVariable();
              ((ContextVariable) value).Initialize(variableData, new VMType(typeof (IObjRef), variableData.Substring("group_".Length)));
              commonVariableType = ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE;
              binded = true;
            }
            else
              value = IVariableService.Instance.GetContextVariableByData(ownerContext, variableData, localContext, contextElement);
            if (value == null && needType != null)
            {
              if (needType.IsSimple && !TryGetConstantValue(needType))
              {
                binded = false;
                return;
              }
              if (variableData == "none")
              {
                value = BaseSerializer.GetDefaultValue(needType.BaseType);
                binded = true;
                commonVariableType = ECommonVariableType.CV_TYPE_CONSTANT;
              }
            }
            if (value == null)
            {
              contextObj = globalContext;
              IContext contextByData = IVariableService.Instance.GetContextByData(globalContext, variableData);
              if (contextByData != null)
              {
                if (typeof (ILogicObject).IsAssignableFrom(contextByData.GetType()))
                {
                  contextObj = contextByData;
                  value = ((ILogicObject) contextObj).GetSelf();
                  SetSelf();
                  binded = true;
                }
                else if (typeof (IParam).IsAssignableFrom(contextByData.GetType()))
                {
                  if (ownerContext != null && typeof (IParam).IsAssignableFrom(ownerContext.GetType()))
                  {
                    contextObj = ownerContext;
                    value = contextByData;
                  }
                  if (value == null && ((IParam) contextByData).OwnerContext != null)
                  {
                    IGameObjectContext ownerContext1 = ((IParam) contextByData).OwnerContext;
                    if (ownerContext1 != null && typeof (ILogicObject).IsAssignableFrom(ownerContext1.GetType()) && ownerContext != null && typeof (ILogicObject).IsAssignableFrom(ownerContext.GetType()) && ((ILogicObject) ownerContext).Blueprint.IsDerivedFrom(ownerContext1.Blueprint.BaseGuid, true))
                    {
                      contextObj = ownerContext1;
                      value = contextByData;
                    }
                  }
                }
              }
            }
          }
        }
        if ((!binded || commonVariableType == ECommonVariableType.CV_TYPE_CONSTANT) && value == null && needType != null && !CommonVariableUtility.IsLocalVariableData(variableData) && !TryGetConstantValue(needType))
        {
          binded = false;
          return;
        }
        if (value != null)
        {
          if (variableData != "none")
          {
            if (commonVariableType != ECommonVariableType.CV_TYPE_CONSTANT)
              commonVariableType = !IsSelf ? ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE : ECommonVariableType.CV_TYPE_CONTEXT_SELF;
            VMType secondType = !typeof (IVariable).IsAssignableFrom(value.GetType()) ? new VMType(value.GetType()) : ((IVariable) value).Type;
            if (secondType == null)
            {
              binded = false;
              Logger.AddError(string.Format("Variable {0} value type is undefined at {1}", ToString(), EngineAPIManager.Instance.CurrentFSMStateInfo));
            }
            if (needType != null)
            {
              bool isWeak = variableData == "none";
              binded = VMTypeUtility.IsTypesCompatible(needType, secondType, isWeak);
            }
            else
              binded = true;
          }
        }
      }
      catch (Exception ex)
      {
        binded = false;
        Logger.AddError(string.Format("Cannot bind common variable, error: {0}", ex));
      }
      if (!IsBinded)
        return;
      OnInit();
    }

    public override IVariable GetContextVariable(string variableName)
    {
      return VariableContext != null ? VariableContext.GetContextVariable(variableName) : null;
    }

    public IContext VariableContext
    {
      get
      {
        if (IsSelf && contextObj != null)
          return contextObj;
        if (variableContext == null)
          MakeVariableContext();
        return variableContext;
      }
    }

    public override void Clear()
    {
      base.Clear();
      contextObj = null;
      value = null;
      variableContext = null;
    }

    private bool TryGetConstantValue(VMType needType)
    {
      if (!CommonVariableUtility.CheckParamInfo(variableData, needType))
      {
        binded = false;
        return true;
      }
      value = StringSerializer.ReadValue(variableData, needType.BaseType);
      if (StringSerializer.LastError != "")
        return false;
      if (value != null)
      {
        if (typeof (IRef).IsAssignableFrom(value.GetType()))
        {
          binded = ((IRef) value).StaticInstance != null;
          if (!binded)
            return false;
        }
        else
          binded = true;
      }
      commonVariableType = ECommonVariableType.CV_TYPE_CONSTANT;
      return true;
    }

    private ILogicObject GetStaticContextByData(string data = "")
    {
      if (data == "")
        data = contextData;
      EGuidFormat guidFormat = GuidUtility.GetGuidFormat(data);
      if (guidFormat == EGuidFormat.GT_BASE)
        return GetTemplateByGuid(DefaultConverter.ParseUlong(data));
      return EGuidFormat.GT_HIERARCHY == guidFormat ? IStaticDataContainer.StaticDataContainer.GameRoot.GetWorldHierarhyObjectByGuid(new HierarchyGuid(data)) : (ILogicObject) null;
    }

    private ILogicObject GetTemplateByGuid(ulong baseGuid)
    {
      if (baseGuid != 0UL)
      {
        IObject objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(baseGuid);
        if (objectByGuid != null && typeof (ILogicObject).IsAssignableFrom(objectByGuid.GetType()))
          return (ILogicObject) objectByGuid;
      }
      return null;
    }

    private void CalculateSelf()
    {
      isSelf = false;
      if (!binded)
      {
        if (contextData == variableData)
        {
          isSelf = true;
          selfInited = true;
        }
        else if (variableData == "" || variableData.EndsWith("_Self"))
        {
          isSelf = true;
          selfInited = true;
        }
      }
      if (!binded || isSelf)
        return;
      if (value != null && contextObj != null && typeof (IGameObjectContext).IsAssignableFrom(contextObj.GetType()))
      {
        if (typeof (IRef).IsAssignableFrom(value.GetType()))
        {
          if (((IRef) value).StaticInstance != null && (long) ((IRef) value).StaticInstance.BaseGuid == (long) ((IEditorBaseTemplate) contextObj).BaseGuid)
            isSelf = true;
        }
        else if (typeof (IParam).IsAssignableFrom(value.GetType()) && ((INamed) value).Name.EndsWith("_Self"))
          isSelf = true;
      }
      selfInited = true;
    }

    private void MakeVariableContext()
    {
      if (value == null)
        return;
      if (typeof (IContext).IsAssignableFrom(value.GetType()))
      {
        variableContext = (IContext) value;
      }
      else
      {
        if (!typeof (IRef).IsAssignableFrom(value.GetType()))
          return;
        IObject staticInstance = ((IRef) value).StaticInstance;
        if (staticInstance == null || !typeof (IContext).IsAssignableFrom(staticInstance.GetType()))
          return;
        variableContext = (IContext) staticInstance;
      }
    }

    private void OnInit()
    {
      VMType type = null;
      if (value == null)
      {
        if (contextObj != null)
        {
          if (!typeof (IGameRoot).IsAssignableFrom(contextObj.GetType()))
          {
            commonVariableType = ECommonVariableType.CV_TYPE_CONTEXT_SELF;
            type = new VMType(typeof (IObjRef));
          }
          else
            binded = false;
        }
        else
          Logger.AddError("Invalid common variable creation: data are null");
      }
      else if (typeof (IVariable).IsAssignableFrom(value.GetType()))
      {
        commonVariableType = ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE;
        type = ((IVariable) value).Type;
      }
      else
      {
        commonVariableType = ECommonVariableType.CV_TYPE_CONSTANT;
        type = new VMType(value.GetType());
      }
      if (IsSelf)
      {
        if (contextData == "" && variableData != "")
        {
          Initialize(variableData, type);
        }
        else
        {
          if (!(contextData != "") || !(variableData == ""))
            return;
          Initialize(contextData, type);
        }
      }
      else
        Initialize(contextData + "%" + variableData, type);
    }

    private bool IsDataNull(string data) => data == "0" || variableData == "none";

    private void SetSelf()
    {
      isSelf = true;
      selfInited = true;
    }
  }
}
