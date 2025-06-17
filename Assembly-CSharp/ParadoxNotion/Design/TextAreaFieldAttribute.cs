using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Field)]
  public class TextAreaFieldAttribute(float height) : Attribute 
  {
    public float height = height;
  }
}
