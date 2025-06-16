using System;
using System.Collections.Generic;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.GameLogic
{
  public class VMFunction : BaseFunction
  {
    private VMEntity entity;
    private VMLogicObject staticObject;
    private Type parentComponentAPIType;
    private FunctionInfo functionInfo;
    private static Dictionary<string, FunctionInfo> functionsInfoDict = new Dictionary<string, FunctionInfo>();

    public VMFunction(BaseFunction func, DynamicFSM dynFSM)
      : base(func)
    {
      entity = dynFSM.Entity;
      staticObject = dynFSM.FSMStaticObject;
    }

    public VMFunction(
      APIMethodInfo methodInfo,
      string componentName,
      Type componentAPIType,
      DynamicFSM dynFSM)
      : base(methodInfo.MethodName, componentName)
    {
      entity = dynFSM.Entity;
      staticObject = dynFSM.FSMStaticObject;
      parentComponentAPIType = componentAPIType;
      functionInfo = CreateFunctionInfo(componentName, methodInfo);
    }

    public Type ParentComponentAPIType
    {
      get
      {
        return ParentComponent != null ? ((VMFunctionalComponent) ParentComponent).ComponentType : parentComponentAPIType;
      }
    }

    public string ParentComponentAPIName
    {
      get
      {
        return ParentComponent != null ? ((VMBaseObject) ParentComponent).Name : parentComponentName;
      }
    }

    public VMEntity Entity => entity;

    public override List<APIParamInfo> InputParams
    {
      get => functionInfo != null ? functionInfo.Params : base.InputParams;
    }

    public override APIParamInfo OutputParam
    {
      get => functionInfo != null ? functionInfo.OutputParam : base.OutputParam;
    }

    public override void Clear()
    {
      base.Clear();
      entity = null;
      staticObject = null;
      functionInfo = null;
      if (functionsInfoDict == null || functionsInfoDict.Count <= 0)
        return;
      functionsInfoDict.Clear();
    }

    private static FunctionInfo CreateFunctionInfo(string componentName, APIMethodInfo methodInfo)
    {
      string key = componentName + methodInfo.MethodName;
      if (functionsInfoDict.ContainsKey(key))
        return functionsInfoDict[key];
      FunctionInfo functionInfo = new FunctionInfo();
      for (int index = 0; index < methodInfo.InputParams.Count; ++index)
      {
        APIParamInfo inputParam = methodInfo.InputParams[index];
        functionInfo.Params.Add(inputParam);
      }
      if (methodInfo.ReturnParam != null)
        functionInfo.OutputParam = methodInfo.ReturnParam;
      functionsInfoDict.Add(key, functionInfo);
      return functionInfo;
    }
  }
}
