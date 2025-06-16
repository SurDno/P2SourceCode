using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public class MethodAttribute : Attribute
  {
    public readonly string Name;
    public readonly string OutputTypesSpecialInfo;
    public readonly string InputTypesSpecialInfo;

    public MethodAttribute(string name, string inputSpecialInfo, string outputSpecialInfo)
    {
      this.Name = name;
      this.InputTypesSpecialInfo = inputSpecialInfo;
      this.OutputTypesSpecialInfo = outputSpecialInfo;
    }
  }
}
