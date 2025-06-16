using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Event)]
  public class EventAttribute : Attribute
  {
    public readonly string Name;
    public readonly string InputTypesDesc;
    public readonly bool AtOnce;

    public EventAttribute(string name, string inputTypesDescription)
    {
      Name = name;
      InputTypesDesc = inputTypesDescription;
      AtOnce = false;
    }

    public EventAttribute(string name, string inputTypesDescription, bool bAtOnce)
    {
      Name = name;
      InputTypesDesc = inputTypesDescription;
      AtOnce = bAtOnce;
    }
  }
}
