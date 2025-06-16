using System;

namespace FlowCanvas
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class ContextDefinedInputsAttribute : Attribute
  {
    public Type[] types;

    public ContextDefinedInputsAttribute(params Type[] types) => this.types = types;
  }
}
