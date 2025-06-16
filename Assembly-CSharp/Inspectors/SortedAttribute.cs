using System;

namespace Inspectors
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class SortedAttribute : Attribute
  {
  }
}
