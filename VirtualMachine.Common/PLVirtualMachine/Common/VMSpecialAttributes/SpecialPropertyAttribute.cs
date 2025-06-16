using System;

namespace PLVirtualMachine.Common.VMSpecialAttributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public class SpecialPropertyAttribute : Attribute
  {
    public readonly ESpecialPropertyName Name;

    public SpecialPropertyAttribute(ESpecialPropertyName specialName) => this.Name = specialName;
  }
}
