using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class TaskIconAttribute : Attribute
  {
    private readonly string mIconPath;

    public string IconPath => this.mIconPath;

    public TaskIconAttribute(string iconPath) => this.mIconPath = iconPath;
  }
}
