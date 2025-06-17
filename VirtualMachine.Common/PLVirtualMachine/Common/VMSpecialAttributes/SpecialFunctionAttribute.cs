using System;

namespace PLVirtualMachine.Common.VMSpecialAttributes
{
  [AttributeUsage(AttributeTargets.Method, Inherited = false)]
  public class SpecialFunctionAttribute(ESpecialFunctionName specialName) : Attribute 
  {
    public readonly ESpecialFunctionName Name = specialName;
  }
}
