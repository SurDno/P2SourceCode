using System;

namespace Inspectors
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class MultiAttribute : Attribute
  {
  }
}
