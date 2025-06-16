using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class CategoryAttribute : Attribute
  {
    public string category;

    public CategoryAttribute(string category) => this.category = category;
  }
}
