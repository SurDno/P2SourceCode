using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class GeneratePartialAttribute : Attribute
  {
    public Type Type { get; set; }

    public TypeEnum Detail { get; set; }

    public GeneratePartialAttribute(TypeEnum detail) => this.Detail = detail;
  }
}
