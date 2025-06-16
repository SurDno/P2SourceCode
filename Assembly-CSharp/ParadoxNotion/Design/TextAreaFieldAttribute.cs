using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Field)]
  public class TextAreaFieldAttribute : Attribute
  {
    public float height;

    public TextAreaFieldAttribute(float height) => this.height = height;
  }
}
