using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class TaskIconAttribute(string iconPath) : Attribute 
  {
    public string IconPath => iconPath;
  }
}
