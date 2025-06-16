// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Base.MethodExecuteContext
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using System.Reflection;

#nullable disable
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
