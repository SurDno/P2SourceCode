using System;

namespace Engine.Common.Commons
{
  [AttributeUsage(AttributeTargets.Field)]
  public class GroupAttribute : Attribute
  {
    public string Value { get; private set; }

    public GroupAttribute(string value) => Value = value;
  }
}
