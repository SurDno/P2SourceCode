using System;

namespace PLVirtualMachine.Common.VMSpecialAttributes
{
  [AttributeUsage(AttributeTargets.Event, AllowMultiple = false, Inherited = false)]
  public class SpecialEventAttribute : Attribute
  {
    public readonly ESpecialEventName Name;

    public SpecialEventAttribute(ESpecialEventName specialName) => this.Name = specialName;
  }
}
