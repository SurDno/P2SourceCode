using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class TaskCategoryAttribute : Attribute
  {
    private readonly string mCategory;

    public string Category => this.mCategory;

    public TaskCategoryAttribute(string category) => this.mCategory = category;
  }
}
