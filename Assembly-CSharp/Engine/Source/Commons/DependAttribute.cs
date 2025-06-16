using System;

namespace Engine.Source.Commons
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public class DependAttribute : BaseDependAttribute
  {
    public DependAttribute(Type type)
      : base(type)
    {
    }
  }
}
