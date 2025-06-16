using System;

namespace Engine.Common.Commons
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class GroupAttribute : Attribute
  {
    public string Value { get; private set; }

    public GroupAttribute(string value) => this.Value = value;
  }
}
