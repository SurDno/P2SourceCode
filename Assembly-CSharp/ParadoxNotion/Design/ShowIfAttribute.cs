using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Field)]
  public class ShowIfAttribute : Attribute
  {
    public string fieldName;
    public bool show;

    public ShowIfAttribute(string fieldName, bool show = true)
    {
      this.fieldName = fieldName;
      this.show = show;
    }
  }
}
