using System;

namespace SRF.Service
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class ServiceAttribute : Attribute
  {
    public ServiceAttribute(Type serviceType) => this.ServiceType = serviceType;

    public Type ServiceType { get; private set; }
  }
}
