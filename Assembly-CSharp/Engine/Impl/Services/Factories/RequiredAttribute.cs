using System;

namespace Engine.Impl.Services.Factories
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
  public sealed class RequiredAttribute(Type type) : Attribute 
  {
    public Type Type { get; private set; } = type;
  }
}
