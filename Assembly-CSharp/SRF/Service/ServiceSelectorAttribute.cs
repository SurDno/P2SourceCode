using System;

namespace SRF.Service
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public sealed class ServiceSelectorAttribute : Attribute
  {
    public ServiceSelectorAttribute(Type serviceType) => this.ServiceType = serviceType;

    public Type ServiceType { get; private set; }
  }
}
