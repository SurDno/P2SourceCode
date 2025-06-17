using System;

namespace Engine.Source.Commons
{
  [AttributeUsage(AttributeTargets.Class)]
  public abstract class BaseDependAttribute(Type type) : Attribute 
  {
    public Type Type { get; private set; } = type;
  }
}
