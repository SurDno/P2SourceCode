using System.Reflection;

namespace PLVirtualMachine.Base
{
  public class MethodExecuteContext
  {
    public void Initialize(string methodNameKey, MethodInfo methodInfo)
    {
      this.MethodNameKey = methodNameKey;
      this.ExecMethodInfo = methodInfo;
      this.InputParamsInfo = this.ExecMethodInfo.GetParameters();
      this.InputParams = new object[this.InputParamsInfo.Length];
    }

    public string MethodNameKey { get; private set; }

    public MethodInfo ExecMethodInfo { get; private set; }

    public object[] InputParams { get; private set; }

    public ParameterInfo[] InputParamsInfo { get; private set; }
  }
}
