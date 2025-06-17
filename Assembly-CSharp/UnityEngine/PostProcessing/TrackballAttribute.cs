using System;

namespace UnityEngine.PostProcessing
{
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class TrackballAttribute(string method) : PropertyAttribute 
  {
    public readonly string method = method;
  }
}
