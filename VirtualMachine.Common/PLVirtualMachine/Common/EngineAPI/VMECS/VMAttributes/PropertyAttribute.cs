using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class PropertyAttribute : Attribute
  {
    public readonly string Name;
    public readonly object DefValue;
    public readonly bool Initial;
    public readonly bool InitialInHierarchy;
    public readonly string SpecialTypeInfo;

    public PropertyAttribute(string name, string specialTypeInfo)
    {
      Name = name;
      Initial = true;
      InitialInHierarchy = false;
      DefValue = null;
      SpecialTypeInfo = specialTypeInfo;
    }

    public PropertyAttribute(string name, string specialTypeInfo, bool initialInHierarchy)
    {
      Name = name;
      Initial = true;
      InitialInHierarchy = initialInHierarchy;
      DefValue = null;
      SpecialTypeInfo = specialTypeInfo;
    }

    public PropertyAttribute(
      string name,
      string specialTypeInfo,
      bool initialInHierarchy,
      object defValue)
    {
      Name = name;
      Initial = true;
      InitialInHierarchy = initialInHierarchy;
      DefValue = defValue;
      SpecialTypeInfo = specialTypeInfo;
    }

    public PropertyAttribute(
      string name,
      string specialTypeInfo,
      bool initialInHierarchy,
      object defValue,
      bool isInitial)
    {
      Name = name;
      Initial = isInitial;
      InitialInHierarchy = initialInHierarchy;
      DefValue = defValue;
      SpecialTypeInfo = specialTypeInfo;
    }
  }
}
