using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class ColorAttribute(string hexColor) : Attribute 
  {
    public string hexColor = hexColor;
  }
}
