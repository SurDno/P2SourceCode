using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class ForceObjectFieldAttribute : Attribute
  {
  }
}
