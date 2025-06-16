using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (TestComponent))]
  public class TestComponent_Generated : 
    TestComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      TestComponent_Generated instance = Activator.CreateInstance<TestComponent_Generated>();
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
