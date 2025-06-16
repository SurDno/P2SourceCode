using System;

namespace Engine.Source.Commons
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public abstract class BaseDependAttribute : Attribute
  {
    public Type Type { get; private set; }

    public BaseDependAttribute(Type type) => this.Type = type;
  }
}
