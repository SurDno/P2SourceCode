// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes.PropertyAttribute
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public class PropertyAttribute : Attribute
  {
    public readonly string Name;
    public readonly object DefValue;
    public readonly bool Initial;
    public readonly bool InitialInHierarchy;
    public readonly string SpecialTypeInfo;

    public PropertyAttribute(string name, string specialTypeInfo)
    {
      this.Name = name;
      this.Initial = true;
      this.InitialInHierarchy = false;
      this.DefValue = (object) null;
      this.SpecialTypeInfo = specialTypeInfo;
    }

    public PropertyAttribute(string name, string specialTypeInfo, bool initialInHierarchy)
    {
      this.Name = name;
      this.Initial = true;
      this.InitialInHierarchy = initialInHierarchy;
      this.DefValue = (object) null;
      this.SpecialTypeInfo = specialTypeInfo;
    }

    public PropertyAttribute(
      string name,
      string specialTypeInfo,
      bool initialInHierarchy,
      object defValue)
    {
      this.Name = name;
      this.Initial = true;
      this.InitialInHierarchy = initialInHierarchy;
      this.DefValue = defValue;
      this.SpecialTypeInfo = specialTypeInfo;
    }

    public PropertyAttribute(
      string name,
      string specialTypeInfo,
      bool initialInHierarchy,
      object defValue,
      bool isInitial)
    {
      this.Name = name;
      this.Initial = isInitial;
      this.InitialInHierarchy = initialInHierarchy;
      this.DefValue = defValue;
      this.SpecialTypeInfo = specialTypeInfo;
    }
  }
}
