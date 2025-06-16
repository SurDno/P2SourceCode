using System;
using Cofe.Meta;

namespace StateSetters
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class StateSetterAttribute : TypeAttribute
  {
    private string id;

    public StateSetterAttribute(string id) => this.id = id;

    public override void ComputeType(Type type)
    {
      StateSetterService.Register(id, (IStateSetterItemController) Activator.CreateInstance(type));
    }
  }
}
