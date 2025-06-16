using System;

namespace Cinemachine
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class SaveDuringPlayAttribute : Attribute
  {
  }
}
