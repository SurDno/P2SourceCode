using System;
using System.Collections.Generic;
using PLVirtualMachine.Common.EngineAPI;

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
      parentComponentName = componentName;
      parentComponent = null;
    }

    public BaseFunction(BaseFunction func)
    {
      name = func.name;
      parentComponent = func.ParentComponent;
      inputParams = func.inputParams;
      outputParam = func.outputParam;
    }

    public BaseFunction(string name, IFunctionalComponent parentComponent)
    {
      this.name = name;
      this.parentComponent = parentComponent;
    }

    public EContextVariableCategory Category => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION;

    public string Name
    {
      get
      {
        if (parentComponent != null)
          return parentComponent.Name + "." + name;
        return parentComponentName == null || "" == parentComponentName ? name : parentComponentName + "." + name;
      }
    }

    public string APIName => name;

    public IFunctionalComponent ParentComponent => parentComponent;

    public VMType Type => outputParam != null ? outputParam.Type : new VMType(typeof (Nullable));

    public virtual List<APIParamInfo> InputParams => inputParams;

    public virtual APIParamInfo OutputParam => outputParam;

    public bool HasOutput => OutputParam != null && OutputParam.Type != null && null != OutputParam.Type.BaseType && OutputParam.Type.BaseType != typeof (void);

    public virtual bool IsEqual(IVariable other)
    {
      if (!typeof (BaseFunction).IsAssignableFrom(other.GetType()))
        return false;
      BaseFunction baseFunction = (BaseFunction) other;
      return (parentComponentName == null || !("" != parentComponentName) || !(parentComponentName != baseFunction.parentComponentName)) && Name == baseFunction.Name;
    }

    public void InitParams(List<APIParamInfo> inputParams, APIParamInfo outputParam)
    {
      this.inputParams = inputParams;
      this.outputParam = outputParam;
    }

    public virtual void Clear()
    {
      parentComponent = null;
      if (inputParams != null)
        inputParams = null;
      outputParam = null;
    }
  }
}
