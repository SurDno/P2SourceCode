using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class TaskDescriptionAttribute : Attribute
  {
    private readonly string mDescription;

    public string Description => this.mDescription;

    public TaskDescriptionAttribute(string description) => this.mDescription = description;
  }
}
