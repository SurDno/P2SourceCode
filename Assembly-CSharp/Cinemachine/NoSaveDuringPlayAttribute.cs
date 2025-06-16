using System;

namespace Cinemachine
{
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class NoSaveDuringPlayAttribute : PropertyAttribute
  {
  }
}
