using System;

namespace Engine.Source.Commons
{
  [AttributeUsage(AttributeTargets.Class)]
  public abstract class BaseDependAttribute : Attribute
  {
    public Type Type { get; private set; }

    public BaseDependAttribute(Type type) => Type = type;
  }
}
