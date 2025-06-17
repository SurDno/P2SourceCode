using System;

namespace Engine.Common.Commons
{
  [AttributeUsage(AttributeTargets.Field)]
  public class GroupAttribute(string value) : Attribute 
  {
    public string Value { get; private set; } = value;
  }
}
