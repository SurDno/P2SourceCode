// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.Data.FieldDataAttribute
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System;

#nullable disable
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
