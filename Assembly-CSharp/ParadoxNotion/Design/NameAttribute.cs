using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class NameAttribute(string name) : Attribute 
  {
    public string name = name;
  }
}
