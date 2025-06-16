using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public class BaseFunction : IVariable, INamed
  {
    private string name;
    private IFunctionalComponent parentComponent;
    protected string parentComponentName;
    protected List<APIParamInfo> inputParams;
    protected APIParamInfo outputParam;

    public BaseFunction(string name, string componentName)
    {
      this.name = name;
      this.parentComponentName = componentName;
      this.parentComponent = (IFunctionalComponent) null;
    }

    public BaseFunction(BaseFunction func)
    {
      this.name = func.name;
      this.parentComponent = func.ParentComponent;
      this.inputParams = func.inputParams;
      this.outputParam = func.outputParam;
    }

    public BaseFunction(string name, IFunctionalComponent parentComponent)
    {
      this.name = name;
      this.parentComponent = parentComponent;
    }

    public EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION;
    }

    public string Name
    {
      get
      {
        if (this.parentComponent != null)
          return this.parentComponent.Name + "." + this.name;
        return this.parentComponentName == null || "" == this.parentComponentName ? this.name : this.parentComponentName + "." + this.name;
      }
    }

    public string APIName => this.name;

    public IFunctionalComponent ParentComponent => this.parentComponent;

    public VMType Type
    {
      get => this.outputParam != null ? this.outputParam.Type : new VMType(typeof (Nullable));
    }

    public virtual List<APIParamInfo> InputParams => this.inputParams;

    public virtual APIParamInfo OutputParam => this.outputParam;

    public bool HasOutput
    {
      get
      {
        return this.OutputParam != null && this.OutputParam.Type != null && (System.Type) null != this.OutputParam.Type.BaseType && this.OutputParam.Type.BaseType != typeof (void);
      }
    }

    public virtual bool IsEqual(IVariable other)
    {
      if (!typeof (BaseFunction).IsAssignableFrom(other.GetType()))
        return false;
      BaseFunction baseFunction = (BaseFunction) other;
      return (this.parentComponentName == null || !("" != this.parentComponentName) || !(this.parentComponentName != baseFunction.parentComponentName)) && this.Name == baseFunction.Name;
    }

    public void InitParams(List<APIParamInfo> inputParams, APIParamInfo outputParam)
    {
      this.inputParams = inputParams;
      this.outputParam = outputParam;
    }

    public virtual void Clear()
    {
      this.parentComponent = (IFunctionalComponent) null;
      if (this.inputParams != null)
        this.inputParams = (List<APIParamInfo>) null;
      this.outputParam = (APIParamInfo) null;
    }
  }
}
