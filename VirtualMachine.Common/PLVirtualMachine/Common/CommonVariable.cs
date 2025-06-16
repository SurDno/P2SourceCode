using Cofe.Loggers;
using Cofe.Serializations.Converters;
using Engine.Common.Types;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;

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
      this.value = (object) localVariable;
      this.commonVariableType = ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE;
      this.Initialize(this.contextData + "%" + this.variableData, ((IVariable) this.value).Type);
      this.variableData = IVariableService.Instance.GetVariableData((IVariable) this.value);
      this.binded = true;
    }

    public override EContextVariableCategory Category
    {
      get
      {
        return this.commonVariableType == ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE ? ((IVariable) this.value).Category : EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_ALL;
      }
    }

    public IContext Context => this.contextObj;

    public object Variable => !this.binded ? (object) this.variableData : this.value;

    public override IEnumerable<IVariable> GetContextVariables(
      EContextVariableCategory contextVarCategory)
    {
      return this.IsSelf ? this.contextObj.GetContextVariables(contextVarCategory) : base.GetContextVariables(contextVarCategory);
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
        this.contextData = "";
        this.variableData = strArray[0];
      }
      else
      {
        this.contextData = strArray[0];
        this.variableData = strArray[1];
        if (strArray.Length <= 2)
          return;
        this.variableData = this.variableData + "%" + strArray[2];
      }
    }

    public bool IsSelf
    {
      get
      {
        if (this.IsNull)
          return false;
        if (!this.selfInited)
          this.CalculateSelf();
        return this.isSelf;
      }
    }

    public bool IsNull
    {
      get
      {
        if (this.IsDataNull(this.contextData))
          return true;
        return this.contextData == "" && this.variableData == "";
      }
    }

    public bool IsBinded => this.binded;

    public ECommonVariableType CommonVariableType => this.commonVariableType;

    public void Bind(
      IContext ownerContext,
      VMType needType = null,
      ILocalContext localContext = null,
      IContextElement contextElement = null)
    {
      try
      {
        if (this.contextData == "" && this.variableData == "" && needType != null && typeof (IRef).IsAssignableFrom(needType.BaseType))
        {
          this.value = BaseSerializer.GetDefaultValue(needType.BaseType);
          this.binded = true;
          this.commonVariableType = ECommonVariableType.CV_TYPE_CONSTANT;
        }
        IGameObjectContext globalContext = (IGameObjectContext) IStaticDataContainer.StaticDataContainer.GameRoot;
        if (ownerContext != null && typeof (IGameObjectContext).IsAssignableFrom(ownerContext.GetType()))
          globalContext = (IGameObjectContext) ownerContext;
        if ("" != this.contextData)
        {
          this.contextObj = IVariableService.Instance.GetContextByData(globalContext, this.contextData, localContext, contextElement);
          if (this.contextObj != null)
          {
            if (this.contextData == this.variableData && typeof (ILogicObject).IsAssignableFrom(this.contextObj.GetType()))
            {
              this.value = (object) ((ILogicObject) this.contextObj).GetSelf();
              this.SetSelf();
              this.commonVariableType = ECommonVariableType.CV_TYPE_CONTEXT_SELF;
              this.binded = true;
            }
            else
            {
              if ("" != this.variableData)
                this.value = (object) IVariableService.Instance.GetContextVariableByData(this.contextObj, this.variableData, localContext, contextElement);
              else if (typeof (ILogicObject).IsAssignableFrom(this.contextObj.GetType()))
                this.value = (object) ((ILogicObject) this.contextObj).GetSelf();
              if (typeof (ILogicObject).IsAssignableFrom(this.contextObj.GetType()) && !((ILogicObject) this.contextObj).Static)
              {
                bool flag = false;
                if (ownerContext != null && ((ILogicObject) ownerContext).Blueprint.IsDerivedFrom(((ILogicObject) this.contextObj).Blueprint.BaseGuid, true))
                  flag = true;
                if (!flag)
                {
                  this.binded = false;
                  Logger.AddError(string.Format("Context {0} must be static at {1}", (object) this.contextObj.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
                  return;
                }
              }
              if (this.variableData == "none" && needType != null && typeof (IRef).IsAssignableFrom(needType.BaseType))
              {
                this.value = BaseSerializer.GetDefaultValue(needType.BaseType);
                this.binded = true;
                this.commonVariableType = ECommonVariableType.CV_TYPE_CONSTANT;
              }
              if (this.value == null && typeof (IParam).IsAssignableFrom(this.contextObj.GetType()) && this.contextData == this.variableData)
              {
                this.value = (object) this.contextObj;
                this.contextObj = (IContext) globalContext;
                this.binded = true;
              }
            }
          }
          else if (this.IsSelf)
          {
            this.commonVariableType = ECommonVariableType.CV_TYPE_CONTEXT_SELF;
            this.binded = false;
            return;
          }
        }
        else if ("" != this.variableData)
        {
          ILogicObject staticContextByData = this.GetStaticContextByData(this.variableData);
          bool flag = false;
          if (staticContextByData != null && staticContextByData.Static)
          {
            this.contextObj = (IContext) staticContextByData;
            this.value = (object) staticContextByData.GetSelf();
            this.SetSelf();
            flag = true;
            this.binded = true;
          }
          if (!flag)
          {
            this.contextObj = ownerContext;
            if (this.variableData.StartsWith("group_"))
            {
              this.value = (object) new LocalVariable();
              ((ContextVariable) this.value).Initialize(this.variableData, new VMType(typeof (IObjRef), this.variableData.Substring("group_".Length)));
              this.commonVariableType = ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE;
              this.binded = true;
            }
            else
              this.value = (object) IVariableService.Instance.GetContextVariableByData(ownerContext, this.variableData, localContext, contextElement);
            if (this.value == null && needType != null)
            {
              if (needType.IsSimple && !this.TryGetConstantValue(needType))
              {
                this.binded = false;
                return;
              }
              if (this.variableData == "none")
              {
                this.value = BaseSerializer.GetDefaultValue(needType.BaseType);
                this.binded = true;
                this.commonVariableType = ECommonVariableType.CV_TYPE_CONSTANT;
              }
            }
            if (this.value == null)
            {
              this.contextObj = (IContext) globalContext;
              IContext contextByData = IVariableService.Instance.GetContextByData(globalContext, this.variableData);
              if (contextByData != null)
              {
                if (typeof (ILogicObject).IsAssignableFrom(contextByData.GetType()))
                {
                  this.contextObj = contextByData;
                  this.value = (object) ((ILogicObject) this.contextObj).GetSelf();
                  this.SetSelf();
                  this.binded = true;
                }
                else if (typeof (IParam).IsAssignableFrom(contextByData.GetType()))
                {
                  if (ownerContext != null && typeof (IParam).IsAssignableFrom(ownerContext.GetType()))
                  {
                    this.contextObj = ownerContext;
                    this.value = (object) contextByData;
                  }
                  if (this.value == null && ((IParam) contextByData).OwnerContext != null)
                  {
                    IGameObjectContext ownerContext1 = ((IParam) contextByData).OwnerContext;
                    if (ownerContext1 != null && typeof (ILogicObject).IsAssignableFrom(ownerContext1.GetType()) && ownerContext != null && typeof (ILogicObject).IsAssignableFrom(ownerContext.GetType()) && ((ILogicObject) ownerContext).Blueprint.IsDerivedFrom(ownerContext1.Blueprint.BaseGuid, true))
                    {
                      this.contextObj = (IContext) ownerContext1;
                      this.value = (object) contextByData;
                    }
                  }
                }
              }
            }
          }
        }
        if ((!this.binded || this.commonVariableType == ECommonVariableType.CV_TYPE_CONSTANT) && this.value == null && needType != null && !CommonVariableUtility.IsLocalVariableData(this.variableData) && !this.TryGetConstantValue(needType))
        {
          this.binded = false;
          return;
        }
        if (this.value != null)
        {
          if (this.variableData != "none")
          {
            if (this.commonVariableType != ECommonVariableType.CV_TYPE_CONSTANT)
              this.commonVariableType = !this.IsSelf ? ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE : ECommonVariableType.CV_TYPE_CONTEXT_SELF;
            VMType secondType = !typeof (IVariable).IsAssignableFrom(this.value.GetType()) ? new VMType(this.value.GetType()) : ((IVariable) this.value).Type;
            if (secondType == null)
            {
              this.binded = false;
              Logger.AddError(string.Format("Variable {0} value type is undefined at {1}", (object) this.ToString(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
            }
            if (needType != null)
            {
              bool isWeak = this.variableData == "none";
              this.binded = VMTypeUtility.IsTypesCompatible(needType, secondType, isWeak);
            }
            else
              this.binded = true;
          }
        }
      }
      catch (Exception ex)
      {
        this.binded = false;
        Logger.AddError(string.Format("Cannot bind common variable, error: {0}", (object) ex.ToString()));
      }
      if (!this.IsBinded)
        return;
      this.OnInit();
    }

    public override IVariable GetContextVariable(string variableName)
    {
      return this.VariableContext != null ? this.VariableContext.GetContextVariable(variableName) : (IVariable) null;
    }

    public IContext VariableContext
    {
      get
      {
        if (this.IsSelf && this.contextObj != null)
          return this.contextObj;
        if (this.variableContext == null)
          this.MakeVariableContext();
        return this.variableContext;
      }
    }

    public override void Clear()
    {
      base.Clear();
      this.contextObj = (IContext) null;
      this.value = (object) null;
      this.variableContext = (IContext) null;
    }

    private bool TryGetConstantValue(VMType needType)
    {
      if (!CommonVariableUtility.CheckParamInfo(this.variableData, needType))
      {
        this.binded = false;
        return true;
      }
      this.value = PLVirtualMachine.Common.Data.StringSerializer.ReadValue(this.variableData, needType.BaseType);
      if (PLVirtualMachine.Common.Data.StringSerializer.LastError != "")
        return false;
      if (this.value != null)
      {
        if (typeof (IRef).IsAssignableFrom(this.value.GetType()))
        {
          this.binded = ((IRef) this.value).StaticInstance != null;
          if (!this.binded)
            return false;
        }
        else
          this.binded = true;
      }
      this.commonVariableType = ECommonVariableType.CV_TYPE_CONSTANT;
      return true;
    }

    private ILogicObject GetStaticContextByData(string data = "")
    {
      if (data == "")
        data = this.contextData;
      EGuidFormat guidFormat = GuidUtility.GetGuidFormat(data);
      if (guidFormat == EGuidFormat.GT_BASE)
        return this.GetTemplateByGuid(DefaultConverter.ParseUlong(data));
      return EGuidFormat.GT_HIERARCHY == guidFormat ? (ILogicObject) IStaticDataContainer.StaticDataContainer.GameRoot.GetWorldHierarhyObjectByGuid(new HierarchyGuid(data)) : (ILogicObject) null;
    }

    private ILogicObject GetTemplateByGuid(ulong baseGuid)
    {
      if (baseGuid != 0UL)
      {
        IObject objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(baseGuid);
        if (objectByGuid != null && typeof (ILogicObject).IsAssignableFrom(objectByGuid.GetType()))
          return (ILogicObject) objectByGuid;
      }
      return (ILogicObject) null;
    }

    private void CalculateSelf()
    {
      this.isSelf = false;
      if (!this.binded)
      {
        if (this.contextData == this.variableData)
        {
          this.isSelf = true;
          this.selfInited = true;
        }
        else if (this.variableData == "" || this.variableData.EndsWith("_Self"))
        {
          this.isSelf = true;
          this.selfInited = true;
        }
      }
      if (!this.binded || this.isSelf)
        return;
      if (this.value != null && this.contextObj != null && typeof (IGameObjectContext).IsAssignableFrom(this.contextObj.GetType()))
      {
        if (typeof (IRef).IsAssignableFrom(this.value.GetType()))
        {
          if (((IRef) this.value).StaticInstance != null && (long) ((IRef) this.value).StaticInstance.BaseGuid == (long) ((IEditorBaseTemplate) this.contextObj).BaseGuid)
            this.isSelf = true;
        }
        else if (typeof (IParam).IsAssignableFrom(this.value.GetType()) && ((INamed) this.value).Name.EndsWith("_Self"))
          this.isSelf = true;
      }
      this.selfInited = true;
    }

    private void MakeVariableContext()
    {
      if (this.value == null)
        return;
      if (typeof (IContext).IsAssignableFrom(this.value.GetType()))
      {
        this.variableContext = (IContext) this.value;
      }
      else
      {
        if (!typeof (IRef).IsAssignableFrom(this.value.GetType()))
          return;
        IObject staticInstance = ((IRef) this.value).StaticInstance;
        if (staticInstance == null || !typeof (IContext).IsAssignableFrom(staticInstance.GetType()))
          return;
        this.variableContext = (IContext) staticInstance;
      }
    }

    private void OnInit()
    {
      VMType type = (VMType) null;
      if (this.value == null)
      {
        if (this.contextObj != null)
        {
          if (!typeof (IGameRoot).IsAssignableFrom(this.contextObj.GetType()))
          {
            this.commonVariableType = ECommonVariableType.CV_TYPE_CONTEXT_SELF;
            type = new VMType(typeof (IObjRef));
          }
          else
            this.binded = false;
        }
        else
          Logger.AddError(string.Format("Invalid common variable creation: data are null"));
      }
      else if (typeof (IVariable).IsAssignableFrom(this.value.GetType()))
      {
        this.commonVariableType = ECommonVariableType.CV_TYPE_CONTEXT_VARIABLE;
        type = ((IVariable) this.value).Type;
      }
      else
      {
        this.commonVariableType = ECommonVariableType.CV_TYPE_CONSTANT;
        type = new VMType(this.value.GetType());
      }
      if (this.IsSelf)
      {
        if (this.contextData == "" && this.variableData != "")
        {
          this.Initialize(this.variableData, type);
        }
        else
        {
          if (!(this.contextData != "") || !(this.variableData == ""))
            return;
          this.Initialize(this.contextData, type);
        }
      }
      else
        this.Initialize(this.contextData + "%" + this.variableData, type);
    }

    private bool IsDataNull(string data) => data == "0" || this.variableData == "none";

    private void SetSelf()
    {
      this.isSelf = true;
      this.selfInited = true;
    }
  }
}
