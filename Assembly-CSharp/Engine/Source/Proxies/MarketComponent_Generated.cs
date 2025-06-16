using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MarketComponent))]
  public class MarketComponent_Generated : 
    MarketComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      MarketComponent_Generated instance = Activator.CreateInstance<MarketComponent_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((MarketComponent_Generated) target2).isEnabled = isEnabled;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
    }
  }
}
