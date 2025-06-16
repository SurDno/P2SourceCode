using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Impl.Services;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (JerboaService))]
  public class JerboaService_Generated : JerboaService, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Amount", this.Amount);
      DefaultDataWriteUtility.WriteEnum<JerboaColorEnum>(writer, "Color", this.Color);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Amount = DefaultDataReadUtility.Read(reader, "Amount", this.Amount);
      this.Color = DefaultDataReadUtility.ReadEnum<JerboaColorEnum>(reader, "Color");
    }
  }
}
