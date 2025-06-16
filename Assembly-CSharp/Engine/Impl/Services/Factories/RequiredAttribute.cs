using System;

namespace Engine.Impl.Services.Factories
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
  public sealed class RequiredAttribute : Attribute
  {
    public Type Type { get; private set; }

    public RequiredAttribute(Type Type) => this.Type = Type;
  }
}
