using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NpcControllerComponent))]
  public class NpcControllerComponent_Generated : 
    NpcControllerComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NpcControllerComponent_Generated instance = Activator.CreateInstance<NpcControllerComponent_Generated>();
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
  }
}
