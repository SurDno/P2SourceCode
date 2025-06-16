using System;
using Cofe.Meta;
using Cofe.Serializations.Data;

namespace Engine.Impl.Services.Factories
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class FactoryAttribute : TypeAttribute
  {
    public Type Type { get; set; }

    public FactoryAttribute()
    {
    }

    public FactoryAttribute(Type type) => Type = type;

    public override void PrepareType(Type type)
    {
      Type factory = type;
      Type face = Type;
      if ((object) face == null)
        face = type;
      Factory.RegisterType(factory, face);
      TypeResolver.AddType(type);
    }
  }
}
