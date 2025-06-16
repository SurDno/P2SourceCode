using System;

namespace PLVirtualMachine.Common.VMSpecialAttributes
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public class SpecialFunctionAttribute : Attribute
  {
    public readonly ESpecialFunctionName Name;

    public SpecialFunctionAttribute(ESpecialFunctionName specialName) => this.Name = specialName;
  }
}
