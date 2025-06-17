using System;

namespace SRF.Service
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public sealed class ServiceAttribute(Type serviceType) : Attribute 
  {
    public Type ServiceType { get; private set; } = serviceType;
  }
}
