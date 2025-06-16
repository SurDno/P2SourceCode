using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Otimizations;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NoGCMemoryStrategy))]
  public class NoGCMemoryStrategy_Generated : 
    NoGCMemoryStrategy,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NoGCMemoryStrategy_Generated instance = Activator.CreateInstance<NoGCMemoryStrategy_Generated>();
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
