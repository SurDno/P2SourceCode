using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MessangerComponent))]
  public class MessangerComponent_Generated : 
    MessangerComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      MessangerComponent_Generated instance = Activator.CreateInstance<MessangerComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
    }

    public void DataWrite(IDataWriter writer)
    {
    }

    public void DataRead(IDataReader reader, Type type)
    {
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Registred", this.registred);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.registred = DefaultDataReadUtility.Read(reader, "Registred", this.registred);
    }
  }
}
