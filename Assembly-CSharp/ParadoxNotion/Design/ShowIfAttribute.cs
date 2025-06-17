using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Field)]
  public class ShowIfAttribute(string fieldName, bool show = true) : Attribute 
  {
    public string fieldName = fieldName;
    public bool show = show;
  }
}
