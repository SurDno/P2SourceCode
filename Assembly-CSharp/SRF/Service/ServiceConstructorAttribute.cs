using System;

namespace SRF.Service
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public sealed class ServiceConstructorAttribute : Attribute
  {
    public ServiceConstructorAttribute(Type serviceType) => this.ServiceType = serviceType;

    public Type ServiceType { get; private set; }
  }
}
