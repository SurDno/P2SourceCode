using System;

namespace Engine.Source.Commons
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class SaveDependAttribute : BaseDependAttribute
  {
    public SaveDependAttribute(Type type)
      : base(type)
    {
    }
  }
}
