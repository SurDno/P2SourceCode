using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class DescriptionAttribute : Attribute
  {
    public string description;

    public DescriptionAttribute(string description) => this.description = description;
  }
}
