using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (RegisterComponent))]
  public class RegisterComponent_Generated : 
    RegisterComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RegisterComponent_Generated instance = Activator.CreateInstance<RegisterComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((RegisterComponent) target2).tag = this.tag;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Tag", this.tag);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.tag = DefaultDataReadUtility.Read(reader, "Tag", this.tag);
    }
  }
}
