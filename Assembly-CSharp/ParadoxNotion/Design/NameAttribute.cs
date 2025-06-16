using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class NameAttribute : Attribute
  {
    public string name;

    public NameAttribute(string name) => this.name = name;
  }
}
