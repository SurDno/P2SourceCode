using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class CategoryAttribute(string category) : Attribute 
  {
    public string category = category;
  }
}
