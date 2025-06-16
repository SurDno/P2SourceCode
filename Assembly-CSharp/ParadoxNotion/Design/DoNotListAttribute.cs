using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class DoNotListAttribute : Attribute
  {
  }
}
