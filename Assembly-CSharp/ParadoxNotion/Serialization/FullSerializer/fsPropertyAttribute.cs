using System;

namespace ParadoxNotion.Serialization.FullSerializer
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class fsPropertyAttribute(string name) : Attribute 
  {
    public string Name = name;
    public Type Converter;

    public fsPropertyAttribute()
      : this(string.Empty)
    {
    }
  }
}
