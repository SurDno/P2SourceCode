using System;

namespace UnityEngine.PostProcessing
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class MinAttribute : PropertyAttribute
  {
    public readonly float min;

    public MinAttribute(float min) => this.min = min;
  }
}
