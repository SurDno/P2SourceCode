using System;

namespace UnityEngine.PostProcessing
{
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class GetSetAttribute(string name) : PropertyAttribute 
  {
    public readonly string name = name;
    public bool dirty;
  }
}
