using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.Objects;
using System.Collections.Generic;

namespace PLVirtualMachine.GameLogic
{
  public class VMFunction : BaseFunction
  {
    private VMEntity entity;
    private VMLogicObject staticObject;
    private System.Type parentComponentAPIType;
    private FunctionInfo functionInfo;
    private static Dictionary<string, FunctionInfo> functionsInfoDict = new Dictionary<string, FunctionInfo>();

    public VMFunction(BaseFunction func, DynamicFSM dynFSM)
      : base(func)
    {
      this.entity = dynFSM.Entity;
      this.staticObject = dynFSM.FSMStaticObject;
    }

    public VMFunction(
      APIMethodInfo methodInfo,
      string componentName,
      System.Type componentAPIType,
      DynamicFSM dynFSM)
      : base(methodInfo.MethodName, componentName)
    {
      this.entity = dynFSM.Entity;
      this.staticObject = dynFSM.FSMStaticObject;
      this.parentComponentAPIType = componentAPIType;
      this.functionInfo = VMFunction.CreateFunctionInfo(componentName, methodInfo);
    }

    public System.Type ParentComponentAPIType
    {
      get
      {
        return this.ParentComponent != null ? ((VMFunctionalComponent) this.ParentComponent).ComponentType : this.parentComponentAPIType;
      }
    }

    public string ParentComponentAPIName
    {
      get
      {
        return this.ParentComponent != null ? ((VMBaseObject) this.ParentComponent).Name : this.parentComponentName;
      }
    }

    public VMEntity Entity => this.entity;

    public override List<APIParamInfo> InputParams
    {
      get => this.functionInfo != null ? this.functionInfo.Params : base.InputParams;
    }

    public override APIParamInfo OutputParam
    {
      get => this.functionInfo != null ? this.functionInfo.OutputParam : base.OutputParam;
    }

    public override void Clear()
    {
      base.Clear();
      this.entity = (VMEntity) null;
      this.staticObject = (VMLogicObject) null;
      this.functionInfo = (FunctionInfo) null;
      if (VMFunction.functionsInfoDict == null || VMFunction.functionsInfoDict.Count <= 0)
        return;
      VMFunction.functionsInfoDict.Clear();
    }

    private static FunctionInfo CreateFunctionInfo(string componentName, APIMethodInfo methodInfo)
    {
      string key = componentName + methodInfo.MethodName;
      if (VMFunction.functionsInfoDict.ContainsKey(key))
        return VMFunction.functionsInfoDict[key];
      FunctionInfo functionInfo = new FunctionInfo();
      for (int index = 0; index < methodInfo.InputParams.Count; ++index)
      {
        APIParamInfo inputParam = methodInfo.InputParams[index];
        functionInfo.Params.Add(inputParam);
      }
      if (methodInfo.ReturnParam != null)
        functionInfo.OutputParam = methodInfo.ReturnParam;
      VMFunction.functionsInfoDict.Add(key, functionInfo);
      return functionInfo;
    }
  }
}
