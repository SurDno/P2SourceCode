using System;

namespace UnityEngine.PostProcessing
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class GetSetAttribute : PropertyAttribute
  {
    public readonly string name;
    public bool dirty;

    public GetSetAttribute(string name) => this.name = name;
  }
}
