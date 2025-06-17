using System;

namespace PLVirtualMachine.Common.VMSpecialAttributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class SpecialPropertyAttribute(ESpecialPropertyName specialName) : Attribute 
  {
    public readonly ESpecialPropertyName Name = specialName;
  }
}
