using System;

namespace PLVirtualMachine.Common.Data
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class FieldDataAttribute : Attribute
  {
    public readonly string Name;
    public readonly DataFieldType DataFieldType;

    public FieldDataAttribute(string name, DataFieldType dataFieldType = DataFieldType.None)
    {
      this.Name = name;
      this.DataFieldType = dataFieldType;
    }
  }
}
