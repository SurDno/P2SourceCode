using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class RequiredComponentAttribute(Type componentType) : Attribute 
  {
    public Type ComponentType => componentType;
  }
}
