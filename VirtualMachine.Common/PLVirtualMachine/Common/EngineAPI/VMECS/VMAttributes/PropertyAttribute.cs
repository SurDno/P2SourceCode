using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class PropertyAttribute(
    string name,
    string specialTypeInfo,
    bool initialInHierarchy,
    object defValue,
    bool isInitial)
    : Attribute 
  {
    public readonly string Name = name;
    public readonly object DefValue = defValue;
    public readonly bool Initial = isInitial;
    public readonly bool InitialInHierarchy = initialInHierarchy;
    public readonly string SpecialTypeInfo = specialTypeInfo;

    public PropertyAttribute(string name, string specialTypeInfo) : this(name, specialTypeInfo, false, null, true) { }

    public PropertyAttribute(string name, string specialTypeInfo, bool initialInHierarchy) : this(name, specialTypeInfo, initialInHierarchy, null, true) { }

    public PropertyAttribute(
      string name,
      string specialTypeInfo,
      bool initialInHierarchy,
      object defValue) : this(name, specialTypeInfo, initialInHierarchy, defValue, true) { }
  }
}
