using System.Reflection;

namespace PLVirtualMachine.Base
{
  public class MethodExecuteContext
  {
    public void Initialize(string methodNameKey, MethodInfo methodInfo)
    {
      MethodNameKey = methodNameKey;
      ExecMethodInfo = methodInfo;
      InputParamsInfo = ExecMethodInfo.GetParameters();
      InputParams = new object[InputParamsInfo.Length];
    }

    public string MethodNameKey { get; private set; }

    public MethodInfo ExecMethodInfo { get; private set; }

    public object[] InputParams { get; private set; }

    public ParameterInfo[] InputParamsInfo { get; private set; }
  }
}
