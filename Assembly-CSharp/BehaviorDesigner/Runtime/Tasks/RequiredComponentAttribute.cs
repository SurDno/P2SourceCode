using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class RequiredComponentAttribute : Attribute
  {
    private readonly Type mComponentType;

    public Type ComponentType => mComponentType;

    public RequiredComponentAttribute(Type componentType) => mComponentType = componentType;
  }
}
