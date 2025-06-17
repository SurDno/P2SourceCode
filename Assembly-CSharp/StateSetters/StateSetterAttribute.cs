using System;
using Cofe.Meta;

namespace StateSetters
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class StateSetterAttribute(string id) : TypeAttribute 
  {
    public override void ComputeType(Type type)
    {
      StateSetterService.Register(id, (IStateSetterItemController) Activator.CreateInstance(type));
    }
  }
}
