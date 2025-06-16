using System;

namespace ParadoxNotion.Serialization.FullSerializer
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class fsPropertyAttribute : Attribute
  {
    public string Name;
    public Type Converter;

    public fsPropertyAttribute()
      : this(string.Empty)
    {
    }

    public fsPropertyAttribute(string name) => this.Name = name;
  }
}
