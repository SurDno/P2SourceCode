// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.Serialization.SerializableAttribute
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System;

#nullable disable
namespace PLVirtualMachine.Common.Serialization
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
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
