using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class SliderFieldAttribute : Attribute
  {
    public float left;
    public float right;

    public SliderFieldAttribute(float left, float right)
    {
      this.left = left;
      this.right = right;
    }

    public SliderFieldAttribute(int left, int right)
    {
      this.left = (float) left;
      this.right = (float) right;
    }
  }
}
