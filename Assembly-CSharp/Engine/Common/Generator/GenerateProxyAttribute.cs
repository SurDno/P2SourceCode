using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class GenerateProxyAttribute(TypeEnum detail) : Attribute 
  {
    public Type Type { get; set; }

    public TypeEnum Detail { get; set; } = detail;
  }
}
