using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Method, Inherited = false)]
  public class MethodAttribute(string name, string inputSpecialInfo, string outputSpecialInfo)
    : Attribute 
  {
    public readonly string Name = name;
    public readonly string OutputTypesSpecialInfo = outputSpecialInfo;
    public readonly string InputTypesSpecialInfo = inputSpecialInfo;
  }
}
