using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class TaskCategoryAttribute(string category) : Attribute 
  {
    public string Category => category;
  }
}
