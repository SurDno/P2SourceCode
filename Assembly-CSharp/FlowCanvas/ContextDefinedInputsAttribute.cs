using System;

namespace FlowCanvas
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class ContextDefinedInputsAttribute(params Type[] types) : Attribute 
  {
    public Type[] types = types;
  }
}
