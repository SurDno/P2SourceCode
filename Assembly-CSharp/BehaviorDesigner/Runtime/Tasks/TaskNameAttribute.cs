using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class TaskNameAttribute : Attribute
  {
    private readonly string mName;

    public string Name => this.mName;

    public TaskNameAttribute(string name) => this.mName = name;
  }
}
