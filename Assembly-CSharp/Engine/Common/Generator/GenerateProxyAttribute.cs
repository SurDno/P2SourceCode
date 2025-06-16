using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class GenerateProxyAttribute : Attribute
  {
    public Type Type { get; set; }

    public TypeEnum Detail { get; set; }

    public GenerateProxyAttribute(TypeEnum detail) => this.Detail = detail;
  }
}
