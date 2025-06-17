using System;

namespace UnityEngine.PostProcessing
{
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class MinAttribute(float min) : PropertyAttribute 
  {
    public readonly float min = min;
  }
}
