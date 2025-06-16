using System;

namespace PLVirtualMachine.Common.Data
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class TypeDataAttribute : Attribute
  {
    public readonly EDataType DataType;

    public TypeDataAttribute(EDataType dataType) => DataType = dataType;
  }
}
