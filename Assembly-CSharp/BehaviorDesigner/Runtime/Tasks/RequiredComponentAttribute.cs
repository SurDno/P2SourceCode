using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class RequiredComponentAttribute : Attribute
  {
    private readonly Type mComponentType;

    public Type ComponentType => this.mComponentType;

    public RequiredComponentAttribute(Type componentType) => this.mComponentType = componentType;
  }
}
