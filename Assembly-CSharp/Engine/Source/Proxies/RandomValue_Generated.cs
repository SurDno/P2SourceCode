using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Scripts.Expressions.Commons;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (RandomValue))]
  public class RandomValue_Generated : 
    RandomValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RandomValue_Generated instance = Activator.CreateInstance<RandomValue_Generated>();
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
