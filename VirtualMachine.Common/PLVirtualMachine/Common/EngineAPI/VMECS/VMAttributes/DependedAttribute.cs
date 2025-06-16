using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
  public class DependedAttribute : Attribute
  {
    public string Name;

    public DependedAttribute(string Name) => this.Name = Name;
  }
}
