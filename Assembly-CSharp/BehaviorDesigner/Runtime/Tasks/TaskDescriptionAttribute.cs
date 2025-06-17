using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class TaskDescriptionAttribute(string description) : Attribute 
  {
    public string Description => description;
  }
}
