using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class TaskNameAttribute(string name) : Attribute 
  {
    public string Name => name;
  }
}
