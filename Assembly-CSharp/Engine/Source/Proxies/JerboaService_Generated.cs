using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Impl.Services;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (JerboaService))]
  public class JerboaService_Generated : JerboaService, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Amount", Amount);
      DefaultDataWriteUtility.WriteEnum(writer, "Color", Color);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      Amount = DefaultDataReadUtility.Read(reader, "Amount", Amount);
      Color = DefaultDataReadUtility.ReadEnum<JerboaColorEnum>(reader, "Color");
    }
  }
}
