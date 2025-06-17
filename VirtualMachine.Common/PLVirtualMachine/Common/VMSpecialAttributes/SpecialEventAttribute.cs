using System;

namespace PLVirtualMachine.Common.VMSpecialAttributes
{
  [AttributeUsage(AttributeTargets.Event)]
  public class SpecialEventAttribute(ESpecialEventName specialName) : Attribute 
  {
    public readonly ESpecialEventName Name = specialName;
  }
}
