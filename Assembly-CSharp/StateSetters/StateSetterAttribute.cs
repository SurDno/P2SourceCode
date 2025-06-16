using Cofe.Meta;
using System;

namespace StateSetters
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class StateSetterAttribute : TypeAttribute
  {
    private string id;

    public StateSetterAttribute(string id) => this.id = id;

    public override void ComputeType(Type type)
    {
      StateSetterService.Register(this.id, (IStateSetterItemController) Activator.CreateInstance(type));
    }
  }
}
