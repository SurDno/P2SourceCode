using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public abstract class ObjectDrawerAttribute : Attribute
  {
  }
}
