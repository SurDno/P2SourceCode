using System;

namespace PLVirtualMachine.Common.Serialization
{
  [AttributeUsage(AttributeTargets.Field)]
  public class SerializableAttribute : Attribute
  {
    public bool IsGuid;
    public bool IsSerializable;

    public SerializableAttribute(bool IsSerializable = true, bool IsGuid = false)
    {
      this.IsSerializable = IsSerializable;
      this.IsGuid = IsGuid;
    }
  }
}
