using System;

namespace PLVirtualMachine.Common.Data
{
  [AttributeUsage(AttributeTargets.Field)]
  public class FieldDataAttribute(string name, DataFieldType dataFieldType = DataFieldType.None)
    : Attribute 
  {
    public readonly string Name = name;
    public readonly DataFieldType DataFieldType = dataFieldType;
  }
}
