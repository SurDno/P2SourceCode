using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Event)]
  public class EventAttribute(string name, string inputTypesDescription, bool bAtOnce) : Attribute 
  {
    public readonly string Name = name;
    public readonly string InputTypesDesc = inputTypesDescription;
    public readonly bool AtOnce = bAtOnce;

    public EventAttribute(string name, string inputTypesDescription) : this(name, inputTypesDescription, false) { }
  }
}
