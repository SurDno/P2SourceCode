using System;

namespace NodeCanvas.Framework
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class BlackboardOnlyAttribute : Attribute
  {
  }
}
