using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Event, AllowMultiple = false, Inherited = false)]
  public class EventAttribute : Attribute
  {
    public readonly string Name;
    public readonly string InputTypesDesc;
    public readonly bool AtOnce;

    public EventAttribute(string name, string inputTypesDescription)
    {
      this.Name = name;
      this.InputTypesDesc = inputTypesDescription;
      this.AtOnce = false;
    }

    public EventAttribute(string name, string inputTypesDescription, bool bAtOnce)
    {
      this.Name = name;
      this.InputTypesDesc = inputTypesDescription;
      this.AtOnce = bAtOnce;
    }
  }
}
