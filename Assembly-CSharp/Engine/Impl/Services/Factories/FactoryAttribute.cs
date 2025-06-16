using Cofe.Meta;
using Cofe.Serializations.Data;
using System;

namespace Engine.Impl.Services.Factories
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class FactoryAttribute : TypeAttribute
  {
    public Type Type { get; set; }

    public FactoryAttribute()
    {
    }

    public FactoryAttribute(Type type) => this.Type = type;

    public override void PrepareType(Type type)
    {
      Type factory = type;
      Type face = this.Type;
      if ((object) face == null)
        face = type;
      Factory.RegisterType(factory, face);
      TypeResolver.AddType(type);
    }
  }
}
