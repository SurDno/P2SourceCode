using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class DescriptionAttribute(string description) : Attribute 
  {
    public string description = description;
  }
}
