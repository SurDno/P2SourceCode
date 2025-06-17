using System;

namespace ParadoxNotion.FlowCanvas.Module
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class ContextDefinedOutputsAttribute(params Type[] types) : Attribute 
  {
    public Type[] types = types;
  }
}
