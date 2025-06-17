using System;

namespace Engine.Source.Commons
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class DependAttribute(Type type) : BaseDependAttribute(type);
}
