using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Field)]
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
      this.left = left;
      this.right = right;
    }
  }
}
