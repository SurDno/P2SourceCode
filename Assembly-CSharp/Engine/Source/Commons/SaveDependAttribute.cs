using System;

namespace Engine.Source.Commons
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class SaveDependAttribute(Type type) : BaseDependAttribute(type);
}
