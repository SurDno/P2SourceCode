using System;

namespace SRF.Service
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public sealed class ServiceAttribute : Attribute
  {
    public ServiceAttribute(Type serviceType) => ServiceType = serviceType;

    public Type ServiceType { get; private set; }
  }
}
