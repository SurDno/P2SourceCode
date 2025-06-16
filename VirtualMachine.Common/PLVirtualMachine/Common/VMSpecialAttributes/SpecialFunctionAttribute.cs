using System;

namespace PLVirtualMachine.Common.VMSpecialAttributes
{
  [AttributeUsage(AttributeTargets.Method, Inherited = false)]
  public class SpecialFunctionAttribute : Attribute
  {
    public readonly ESpecialFunctionName Name;

    public SpecialFunctionAttribute(ESpecialFunctionName specialName) => Name = specialName;
  }
}
