using System;

namespace SRF.Service
{
  [AttributeUsage(AttributeTargets.Method, Inherited = false)]
  public sealed class ServiceSelectorAttribute(Type serviceType) : Attribute 
  {
    public Type ServiceType { get; private set; } = serviceType;
  }
}
