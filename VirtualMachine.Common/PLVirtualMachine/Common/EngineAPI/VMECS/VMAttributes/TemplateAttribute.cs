using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
  public class TemplateAttribute : Attribute
  {
  }
}
