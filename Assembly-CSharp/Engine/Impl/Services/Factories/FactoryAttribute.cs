// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.Factories.FactoryAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Cofe.Serializations.Data;
using System;

#nullable disable
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
