using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class TextAreaFieldAttribute : Attribute
  {
    public float height;

    public TextAreaFieldAttribute(float height) => this.height = height;
  }
}
