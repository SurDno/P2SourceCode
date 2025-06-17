using System;

namespace PLVirtualMachine.Common.Serialization
{
  [AttributeUsage(AttributeTargets.Field)]
  public class SerializableAttribute(bool isSerializable = true, bool isGuid = false) : Attribute 
  {
    public bool IsGuid = isGuid;
    public bool IsSerializable = isSerializable;
  }
}
