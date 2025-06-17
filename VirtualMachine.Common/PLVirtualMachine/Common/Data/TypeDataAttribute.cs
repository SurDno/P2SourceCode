using System;

namespace PLVirtualMachine.Common.Data
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class TypeDataAttribute(EDataType dataType) : Attribute 
  {
    public readonly EDataType DataType = dataType;
  }
}
