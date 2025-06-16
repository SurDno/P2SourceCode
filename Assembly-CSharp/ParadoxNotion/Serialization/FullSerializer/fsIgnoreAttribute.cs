using System;

namespace ParadoxNotion.Serialization.FullSerializer
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class fsIgnoreAttribute : Attribute
  {
  }
}
